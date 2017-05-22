using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Game.Host;
using PokemonBattleOnline.Network.Commands;

namespace PokemonBattleOnline.Network
{
    internal class RoomHost : IDisposable
    {
        public readonly Server Server;
        public readonly Room Room;
        private readonly Dictionary<int, ServerUser> Users;
        private InitingGame initingGame;
        private GameContext game;

        public RoomHost(Server server, int id, GameSettings settings)
        {
            Server = server;
            Room = new Room(id, settings);
            Users = new Dictionary<int, ServerUser>();
        }

        public void Send(IS2C s2c)
        {
            foreach (var su in Users.Values) su.Send(s2c);
        }

        private InitingGame rig;
        private void TryStartGame()
        {
            if (initingGame.CanComplete)
            {
                Room.Battling = true;
                Server.Send(RoomS2C.ChangeBattling(Room.Id));
                if (Room.Settings.Mode.PlayersPerTeam() == 2)
                {
                    Server.GetUser(Room[0, 0].Id).Send(new PartnerInfoS2C(initingGame.GetPokemons(0, 1)));
                    Server.GetUser(Room[0, 1].Id).Send(new PartnerInfoS2C(initingGame.GetPokemons(0, 0)));
                    Server.GetUser(Room[1, 0].Id).Send(new PartnerInfoS2C(initingGame.GetPokemons(1, 1)));
                    Server.GetUser(Room[1, 1].Id).Send(new PartnerInfoS2C(initingGame.GetPokemons(1, 0)));
                }
                rig = initingGame;
                game = initingGame.Complete();
                initingGame = null;
                game.GameUpdated += OnGameUpdate;
                game.GameEnd += EndGame;
                game.TimeUp += OnTimeUp;
                game.WaitingNotify += OnWaitingForInput;
                game.Error += OnError;
                Send(new GameStartS2C(game.GetFragment()));
                game.Start();
            }
        }

        private void EndGame()
        {
            Record.Add(Room, rig);
            Record.Add(Room, game);
            game.Dispose();
            game = null;
            Room.Battling = false;
            Server.Send(RoomS2C.ChangeBattling(Room.Id));
        }
        private void OnError()
        {
            Record.Error(Room, game);
            EndGame();
            Server.Send(GameEndS2C.GameStop(0, GameStopReason.Error));
        }
        private void OnGameStop(int userId, GameStopReason reason)
        {
            Record.Add(Room, game, reason, userId);
            EndGame();
            Send(GameEndS2C.GameStop(userId, reason));
        }
        private void OnTimeUp(int[,] time)
        {
            //Record.Add(Room, game, "TimeUp");
            //EndGame();
            //var ps = new List<KeyValuePair<int, int>>(4);
            //foreach (var p in Room.Players) ps.Add(new KeyValuePair<int, int>(p.Id, time[p.Seat.TeamId(), p.Seat.TeamIndex()]));
            //Send(GameEndS2C.TimeUp(ps.ToArray()));
        }
        private void OnWaitingForInput(bool[,] players)
        {
            var ps = new List<int>(4);
            foreach (var p in Room.Players)
                if (players[p.Seat.TeamId(), p.Seat.TeamIndex()]) ps.Add(p.Id);
            Send(new WaitingForInputS2C(ps.ToArray()));
        }
        private void OnGameUpdate(GameEvent[] events, InputRequest[,] requirements)
        {
            if (requirements != null)
            {
                foreach (var p in Room.Players)
                {
                    var r = requirements[p.Seat.TeamId(), p.Seat.TeamIndex()];
                    if (r != null) Server.GetUser(p.Id).Send(new RequireInputS2C(r));
                }
            }
            Send(new GameUpdateS2C(events));
        }

        private bool IsPrepared(Seat seat)
        {
            return initingGame != null && seat != Seat.Spectator && initingGame.GetPokemons(seat.TeamId(), seat.TeamIndex()) != null;
        }
        private int GameId;
        public void Prepare(ServerUser su, IPokemonData[] pokemons)
        {
            if (game == null)
            {
                if (initingGame == null) initingGame = new InitingGame(++GameId, Room.Settings);
                var seat = su.User.Seat;
                if (initingGame.Prepare(seat.TeamId(), seat.TeamIndex(), pokemons))
                {
                    Send(new SetPrepareS2C(seat, true));
                    TryStartGame();
                }
            }
        }
        public void UnPrepare(ServerUser su)
        {
            if (game == null)
            {
                var seat = su.User.Seat;
                if (IsPrepared(seat))
                {
                    initingGame.UnPrepare(seat.TeamId(), su.User.Seat.TeamIndex());
                    Send(new SetPrepareS2C(seat, false));
                }
            }
        }

        public void AddUser(ServerUser su, Seat seat)
        {
            var user = su.User;
            if (user.Room == null && Room.IsValidSeat(seat) && Room[seat] == null)
            {
                if (seat == Seat.Spectator) Room.AddSpectator(user);
                else Room[seat] = user;
                Users.Add(user.Id, su);
                Server.Send(SetSeatS2C.InRoom(user));
                if (game != null) su.Send(new GameStartS2C(game.GetFragment()));
                else if (initingGame != null)
                {
                    if (IsPrepared(Seat.Player00)) su.Send(new SetPrepareS2C(Seat.Player00, true));
                    if (IsPrepared(Seat.Player10)) su.Send(new SetPrepareS2C(Seat.Player10, true));
                    if (Room.Settings.Mode.PlayersPerTeam() == 2)
                    {
                        if (IsPrepared(Seat.Player01)) su.Send(new SetPrepareS2C(Seat.Player01, true));
                        if (IsPrepared(Seat.Player11)) su.Send(new SetPrepareS2C(Seat.Player11, true));
                    }
                }
            }
        }
        public void RemoveUser(ServerUser su)
        {
            var id = su.User.Id;
            var seat = su.User.Seat;
            if (seat == Seat.Spectator)
            {
                Users.Remove(id);
                Room.RemoveSpectator(su.User);
                Server.Send(SetSeatS2C.LeaveRoom(id));
            }
            else if (Room.Players.Count() == 1) Server.RemoveRoom(this);
            else
            {
                if (game != null) OnGameStop(id, GameStopReason.PlayerStop);
                else UnPrepare(su);
                Users.Remove(id);
                Room[seat] = null;
                Server.Send(SetSeatS2C.LeaveRoom(id));
            }
        }
        public void ChangeSeat(ServerUser su, Seat seat)
        {
            var user = su.User;
            if (game == null && Room.IsValidSeat(seat) && user.Seat != seat && Room[seat] == null)
            {
                if (user.Seat == Seat.Spectator) Room[seat] = user;
                else if (seat != Seat.Spectator)
                {
                    UnPrepare(su);
                    Room[seat] = su.User;
                }
                else if (Room.Players.Count() != 1)
                {
                    UnPrepare(su);
                    Room.AddSpectator(user);
                }
                else return;
                Server.Send(SetSeatS2C.InRoom(user));
            }
        }

        public void Input(ServerUser su, ActionInput action)
        {
            var seat = su.User.Seat;
            if (game != null)
                if (game.InputAction(seat.TeamId(), seat.TeamIndex(), action)) game.TryContinue();
                else OnGameStop(su.User.Id, GameStopReason.InvalidInput);
        }

        public void Dispose()
        {
            if (game != null) game.Dispose();
        }
    }
}
