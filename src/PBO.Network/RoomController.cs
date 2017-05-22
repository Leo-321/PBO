using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network.Commands;

namespace PokemonBattleOnline.Network
{
    public class RoomController : ObservableObject
    {
        public static event Action<string, User> RoomChat;
        internal static void OnRoomChat(string chat, User user)
        {
            UIDispatcher.Invoke(RoomChat, chat, user);
        }
        public static event Action<GameStopReason, User> GameStop;
        public static event Action<User[]> TimeReminder;
        internal static void OnTimeReminder(User[] waitForWhom)
        {
            UIDispatcher.Invoke(TimeReminder, new object[] { waitForWhom });
        }
        public static event Action<KeyValuePair<User, int>[]> TimeUp;
        internal static void OnTimeUp(KeyValuePair<User, int>[] remainingTime)
        {
            UIDispatcher.Invoke(TimeUp, new object[] { remainingTime });
        }
        public static event Action Entered;
        internal static void OnEntered()
        {
            UIDispatcher.Invoke(Entered);
        }
        public static event Action Quited;
        public static event Action GameInited;

        internal readonly Client _Client;

        internal RoomController(Client client)
        {
            _Client = client;
        }

        public ClientController Client
        { get { return _Client.Controller; } }
        public User User
        { get { return _Client.Controller.User; } }
        public Room Room
        { get { return _Client.Controller.User.Room; } }
        private GameOutward _game;
        public GameOutward Game
        {
            get { return _game; }
            private set
            {
#if DEBUG
                if (value == null) System.Diagnostics.Debugger.Break();
#endif
                if (_game != value)
                {
                    _game = value;
                    _game.GameEnd += Game_GameEnd;
                    OnPropertyChanged("Game");
                    UIDispatcher.Invoke(GameInited);
                }
            }
        }
        private void Game_GameEnd()
        {
            OnGameStop(GameStopReason.GameEnd, null);
        }

        internal void OnGameStop(GameStopReason reason, User player)
        {
            _game = null;
            _playerController = null;
            OnPropertyChanged();
            UIDispatcher.Invoke(GameStop, reason, player);
        }
        private PlayerController _playerController;
        public PlayerController PlayerController
        {
            get { return _playerController; }
            private set
            {
                if (_playerController != value)
                {
                    _playerController = value;
                    OnPropertyChanged("PlayerController");
                }
            }
        }
        private bool _prepare00;
        public bool Prepare00
        {
            get { return _prepare00; }
            internal set
            {
                if (_prepare00 != value)
                {
                    _prepare00 = value;
                    OnPropertyChanged("Prepare00");
                }
            }
        }
        private bool _prepare01;
        public bool Prepare01
        {
            get { return _prepare01; }
            internal set
            {
                if (_prepare01 != value)
                {
                    _prepare01 = value;
                    OnPropertyChanged("Prepare01");
                }
            }
        }
        private bool _prepare10;
        public bool Prepare10
        {
            get { return _prepare10; }
            internal set
            {
                if (_prepare10 != value)
                {
                    _prepare10 = value;
                    OnPropertyChanged("Prepare10");
                }
            }
        }
        private bool _prepare11;
        public bool Prepare11
        {
            get { return _prepare11; }
            internal set
            {
                if (_prepare11 != value)
                {
                    _prepare11 = value;
                    OnPropertyChanged("Prepare11");
                }
            }
        }

        public void ChangeSeat(Seat seat)
        {
            if (Game == null && Room.IsValidSeat(seat) && User.Seat != seat && Room[seat] == null && !(seat == Seat.Spectator && Room.Players.Count() == 1))
                _Client.Send(SetSeatC2S.ChangeSeat(Room.Id, seat));
        }
        public void Chat(string chat)
        {
            _Client.Send(ChatC2S.RoomChat(chat));
        }
        public void Quit()
        {
            _Client.Send(SetSeatC2S.LeaveRoom());
        }

        private PokemonData[] Self;
        public void GamePrepare(PokemonData[] team)
        {
            if (team.Length != 0 && User.Seat != Seat.Spectator && Room != null && !Room.Battling)
            {
                if (team.Length > Room.Settings.Mode.PokemonsPerPlayer()) team = team.SubArray(0, Room.Settings.Mode.PokemonsPerPlayer());
                Self = team;
                _Client.Send(PrepareC2S.Prepare(team));
            }
        }
        public void GameUnPrepare()
        {
            if (User.Seat != Seat.Spectator && !Room.Battling) _Client.Send(PrepareC2S.UnPrepare());
        }

        internal void Reset()
        {
            _game = null;
            _playerController = null;
            InputRequest = null;
            _prepare00 = false;
            _prepare01 = false;
            _prepare10 = false;
            _prepare11 = false;
            OnPropertyChanged();
        }

        internal void OnQuited()
        {
            Reset();
            UIDispatcher.BeginInvoke(Quited);
        }

        internal PokemonData[] Partner;
        internal void GameStart(ReportFragment gf)
        {
            Dictionary<int, string> ps = new Dictionary<int, string>();
            var mi = Room.Settings.Mode.PlayersPerTeam();
            string[,] players = new string[2, mi];
            for (int t = 0; t < 2; ++t)
                for (int i = 0; i < mi; ++i) players[t, i] = Room[t, i].Name;
            if (User.Seat != Seat.Spectator)
            {
                PlayerController = new PlayerController(this, Self, Partner);
                Partner = null;
            }
            Game = new GameOutward(Room.Settings, players, gf);
            Game.Start();
        }

        internal InputRequest InputRequest;

        private IAsyncResult lastAsyncResult;
        internal void Update(GameEvent[] es)
        {
            var d = new Action<IAsyncResult, GameEvent[]>(UpdateImplement);
            lastAsyncResult = d.BeginInvoke(lastAsyncResult, es, UpdateImplementCallback, null);
        }
        private void UpdateImplement(IAsyncResult lastAsyncResult, GameEvent[] es)
        {
            if (lastAsyncResult != null) lastAsyncResult.AsyncWaitHandle.WaitOne();
            Game.Update(es);
            if (PlayerController != null)
            {
                PlayerController.Game.Update(es);
                if (InputRequest != null)
                {
                    PlayerController.OnRequireInput(InputRequest);
                    InputRequest = null;
                }
            }
        }
        private static void UpdateImplementCallback(IAsyncResult ar)
        {
            ((Action<IAsyncResult, GameEvent[]>)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
        }
    }
}
