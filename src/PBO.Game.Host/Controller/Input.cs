using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal class InputController : ControllerComponent
    {
        public InputController(Controller controller)
          : base(controller)
        {
            _requirements = new InputRequest[2, 2];
        }

        public bool NeedInput
        { get { return !(_requirements[0, 0] == null && _requirements[0, 1] == null && _requirements[1, 0] == null && _requirements[1, 1] == null); } }
        private InputRequest[,] _requirements;
        public InputRequest[,] InputRequirements
        { get { return _requirements; } }

        public bool CheckInputSucceed(Player player)
        {
            if (!player.GiveUp)
                foreach (Tile t in player.Tiles)
                    if (t.Pokemon != null)
                    {
                        if (t.Pokemon.Action == PokemonAction.WaitingForInput) return false;
                    }
                    else
                    {
                        if (t.WillSendOutPokemonIndex < GameSettings.Mode.OnboardPokemonsPerPlayer() && Controller.CanSendOut(t)) return false;
                    }
            Controller.Timer.Pause(player);
            _requirements[player.TeamId, player.TeamIndex] = null;
            return true;
        }
        private PmInputRequest NewInputRequest(PokemonProxy pm)
        {
            var pir = new PmInputRequest();
            {
                int i = 0;
                bool struggle = true;
                foreach (var move in pm.Moves)
                {
                    if (move.PP != 0)
                    {
                        var f = move.IfSelected();
                        if (f == null)
                        {
                            struggle = false;
                            if (move.MoveE.Id == Ms.CURSE && move.MoveE.GetRange(pm) == MoveRange.SelectedTarget) pir.Curse = true;
                        }
                        else
                        {
                            if (pir.Block == null) pir.Block = new string[pm.Moves.Count()];
                            if (f.Move == move.MoveE.Id) pir.Block[i] = f.Key;
                            else if (pir.Only == null)
                            {
                                pir.Only = f.Key;
                                pir.OnlyMove = f.Move;
                            }
                        }
                    }
                    i++;
                }
                if (struggle)
                {
                    pir.Block = null;
                    pir.Only = null;
                    pir.OnlyMove = Ms.STRUGGLE;
                }
                else
                {
                    pir.CanMega = pm.CanMega;
                    pir.CanZmove = pm.CanZmove;
                }
            }
            {
                pir.CantWithdraw = !pm.CanSelectWithdraw;
            }
            var o = pm.OnboardPokemon.GetCondition(Cs.Encore);
            if (o != null) pir.IsEncore = true;
                return pir;
        }
        
        int[,] time = new int[2, 2];
        PmInputRequest[,][] pirs = new PmInputRequest[2, 2][];
        public bool PauseForTurnInput()
        {
            if (!Controller.CanContinue) return false;
            var xn = GameSettings.Mode.XBound();
            pirs[0, 0] = pirs[0, 1] = pirs[1, 0] = pirs[1, 1] = null;
            foreach (var p in Controller.Board.Pokemons)
                if (p.Action == PokemonAction.WaitingForInput)
                {
                    var player = p.Pokemon.Owner;
                    var id = player.TeamId;
                    var index = player.TeamIndex;
                    if (pirs[id, index] == null)
                    {
                        pirs[id, index] = new PmInputRequest[xn];
                        time[id, index] = player.SpentTime;
                    }
                    pirs[id, index][GameSettings.Mode.GetPokemonIndex(p.OnboardPokemon.X)] = NewInputRequest(p);
                }
            if (pirs[0, 0] != null) _requirements[0, 0] = new InputRequest(time[0, 0], pirs[0, 0],0);
            if (pirs[0, 1] != null) _requirements[0, 1] = new InputRequest(time[0, 1], pirs[0, 1],1);
            if (pirs[1, 0] != null) _requirements[1, 0] = new InputRequest(time[1, 0], pirs[1, 0],0);
            if (pirs[1, 1] != null) _requirements[1, 1] = new InputRequest(time[1, 1], pirs[1, 1],1);
            return true;
        }
        public bool PauseForEndTurnInput()
        {
            if (!Controller.CanContinue) return false;
            foreach (var t in Controller.Board.Tiles)
                if (Controller.CanSendOut(t))
                {
                    var player = Controller.GetPlayer(t);
                    if (_requirements[player.TeamId, player.TeamIndex] == null) _requirements[player.TeamId, player.TeamIndex] = new InputRequest(player.SpentTime,player.TeamIndex);
                }
            return true;
        }
        public bool PauseForSendOutInput(Tile tile)
        {
            if (!Controller.CanContinue) return false;
            if (Controller.CanSendOut(tile))
            {
                var player = Controller.GetPlayer(tile);
                _requirements[player.TeamId, player.TeamIndex] = new InputRequest(player.SpentTime, Controller.GameSettings.Mode.GetPokemonIndex(tile.X),player.TeamIndex);
            }
            return true;
        }
        private bool Switch(PokemonProxy withdraw, int sendoutIndex)
        {
            return withdraw.CanInput && withdraw.InputSwitch(sendoutIndex);
        }
        public bool SendOut(Tile tile, int sendoutIndex)
        {
            if (tile.Pokemon == null && Controller.CanSendOut(tile) && Controller.CanSendOut(Controller.GetPlayer(tile).GetPokemon(sendoutIndex)))
            {
                tile.WillSendOutPokemonIndex = sendoutIndex;
                return true;
            }
            else return Switch(tile.Pokemon, sendoutIndex);
        }
        public bool SelectMove(MoveProxy move, Tile target, bool mega, bool zmove)
        {
            return move.Owner.CanInput && move.Owner.SelectMove(move, target, mega, zmove);
        }
        public bool Struggle(PokemonProxy pokemon)
        {
            return pokemon.CanInput && pokemon.SelectMove(pokemon.StruggleMove, null, false, false);
        }
        public void GiveUp(int teamId, int teamIndex)
        {
            Controller.Teams[teamId].GetPlayer(teamIndex).GiveUp = true;
        }
    }
}
