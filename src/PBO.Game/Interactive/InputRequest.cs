using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
    public class SelectMoveFail
    {
        public readonly string Key;
        public readonly int Move; //区分Block和Only

        public SelectMoveFail(string key, int move)
        {
            Key = key;
            Move = move;
        }
    }
    [DataContract(Name = "pir", Namespace = PBOMarks.JSON)]
    public sealed class PmInputRequest
    {
        [DataMember(EmitDefaultValue = false)]
        public int OnlyMove;

        [DataMember(EmitDefaultValue = false)]
        public string Only; //choice/encore

        [DataMember(EmitDefaultValue = false)]
        public string[] Block; //封印挑拨寻衅回复封印残废

        [DataMember(EmitDefaultValue = false)]
        public bool CantWithdraw;

        [DataMember(EmitDefaultValue = false)]
        public bool CanMega;

        //gen7
        [DataMember(EmitDefaultValue = false)]
        public bool CanZmove;

        [DataMember(EmitDefaultValue = false)]
        public bool IsEncore;

        /// <summary>
        /// 持有诅咒技能且可选目标
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public bool Curse;

        #region Client
        private SimGame Game;
        private SimOnboardPokemon Pm;
        private string error;
        public void Init(SimGame game, SimOnboardPokemon pm)
        {
            Game = game;
            Pm = pm;
        }
        public string GetErrorMessage()
        {
            string r = error;
            error = null;
            return r;
        }
        private void SetErrorMessage(string key, string arg1, string arg2)
        {
            var text = GameString.Current.BattleLog("subtitle_" + key);
            error = string.Format(text, Pm.Pokemon.Name, arg1, arg2);
        }
        public bool Fight()
        {
            return OnlyMove == Ms.STRUGGLE;
        }
        /// <summary>
        /// 不判断PP数及技能是否存在
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        public bool Move(SimMove move)
        {
            if (OnlyMove != 0 && OnlyMove != move.Type.Id) SetErrorMessage(Only, GameString.Current.Move(RomData.GetMove(OnlyMove).Id), GameString.Current.Item(Pm.Pokemon.Item));
            else
              if (Block != null)
                for (int i = 0; i < Pm.Moves.Length; ++i)
                    if (move == Pm.Moves[i])
                    {
                        if (Block[i] != null) SetErrorMessage(Block[i], GameString.Current.Move(move.Type.Id), null);
                        break;
                    }
            return error == null;
        }
        /// <summary>
        /// 判断pokemon是否在场和Hp
        /// </summary>
        /// <param name="pokemon"></param>
        /// <returns></returns>
        public bool Pokemon(SimPokemon pokemon)
        {
            if (pokemon.Owner != Game.Player)
            {
                error = GameString.Current.BattleLog("PokemonIsPartner");
                return false;
            }
            if (pokemon.Hp.Value == 0)
            {
                error = string.Format(GameString.Current.BattleLog("PokemonFainted"), pokemon.Name);
                return false;
            }
            if (pokemon.IndexInOwner < Game.Settings.Mode.OnboardPokemonsPerPlayer())
            {
                error = string.Format(GameString.Current.BattleLog("PokemonFighting"), pokemon.Name);
                return false;
            }
            if (CantWithdraw)
            {
                error = string.Format(GameString.Current.BattleLog("PokemonCannotWithdraw"), Pm.Pokemon.Name);
                return false;
            }
            return true;
        }
        #endregion
    }

    [DataContract(Name = "ir", Namespace = PBOMarks.JSON)]
    public class InputRequest
    {
        [DataMember(Name = "a")]
        public readonly int Time;

        [DataMember(EmitDefaultValue = false)]
        public readonly PmInputRequest[] Pms;

        [DataMember(EmitDefaultValue = false)]
        private readonly int i;
        public int? Index
        { get { return i == 0 ? null : (int?)(i - 1); } }

        public int playerIndex;

        /// <summary>
        /// 回合末选择精灵登场
        /// </summary>
        public InputRequest(int time,int index)
        {
            Time = time;
            playerIndex = index;
        }
        /// <summary>
        /// 逃生按钮 追击死亡
        /// </summary>
        /// <param name="time"></param>
        /// <param name="i">被换上的精灵将在队伍中的位置</param>
        public InputRequest(int time, int i,int index)
        {
            playerIndex = index;
            Time = time;
            this.i = i + 1;
        }
        /// <summary>
        /// 回合前选择精灵出招、换精灵，pms的顺序为精灵在队伍中的位置顺序，可能null，最多GameMode.PokemonsPerPlayer
        /// </summary>
        public InputRequest(int time, PmInputRequest[] pms,int index)
        {
            Time = time;
            Pms = pms;
            playerIndex = index;
        }
        protected InputRequest(InputRequest ir)
        {
            Pms = ir.Pms;
            i = ir.i;
            Time = ir.Time;
            playerIndex = ir.playerIndex;
        }

        #region PlayerClient
        public event Action<ActionInput> InputFinished;
        private ActionInput input;
        private SimGame game;
        private string error;

        public bool IsSendOut
        { get { return Pms == null; } }
        public int CurrentI
        { get; private set; }
        public bool CanMega
        { get { return Pms != null && Pms[CurrentI].CanMega; } }
        public bool CanZmove
        { get { return Pms != null && Pms[CurrentI].CanZmove; } }
        public bool NeedTarget
        { get { return game.Settings.Mode.NeedTarget(); } }
        public MoveRange MoveRange
        { get; private set; }

        private void NextPm()
        {
            while (++CurrentI < Pms.Length)
                if (Pms[CurrentI] != null) break;
            if (CurrentI == Pms.Length) InputFinished(input);
        }
        public void Init(SimGame game)
        {
            this.game = game;
            CurrentI = -1;
            if (Pms != null)
            {
                for (int i = 0; i < Pms.Length; ++i)
                    if (Pms[i] != null)
                    {
                        if(game.OnboardPokemons[playerIndex]!=null)
                        Pms[i].Init(game, game.OnboardPokemons[playerIndex]);
                        else
                        Pms[i].Init(game, game.OnboardPokemons[i]);
                        if (CurrentI == -1) CurrentI = i;
                    }
            }
            input = new ActionInput(game.Settings.Mode.OnboardPokemonsPerPlayer());
        }
        public string GetErrorMessage()
        {
            return error;
        }
        public bool Fight()
        {
            if (Pms[CurrentI].Fight())
            {
                input.Struggle(CurrentI);
                NextPm();
                return true;
            }
            error = Pms[CurrentI].GetErrorMessage();
            return false;
        }

        private SimMove move;
        private bool mega;
        private bool zmove;
        public bool Move(SimMove move, bool mega, bool zmove)
        {
            if (Pms[CurrentI].Move(move))
            {
                if (NeedTarget)
                {
                    MoveRange = move.Type.Id == Ms.CURSE && Pms[CurrentI].Curse ? MoveRange.SelectedTarget : move.Type.Range;
                    this.move = move;
                    this.mega = mega;
                    this.zmove = zmove;
                }
                else
                {
                    input.UseMove(CurrentI, move, mega, zmove);
                    NextPm();
                }
                return true;
            }
            error = Pms[CurrentI].GetErrorMessage();
            return false;
        }
        public void Target(int team, int x)
        {
            if (move != null)
            {
                input.UseMove(CurrentI, move, mega, zmove, team, x);
                move = null;
                mega = false;
                zmove = false;
                NextPm();
            }
        }
        public void Target()
        {
            if (move != null)
            {
                input.UseMove(CurrentI, move, mega ,zmove);
                move = null;
                mega = false;
                zmove = false;
                NextPm();
            }
        }
        public bool Pokemon(SimPokemon pokemon, int index)
        {
            if (pokemon.Owner != game.Player)
            {
                error = GameString.Current.BattleLog("PokemonIsPartner");
                return false;
            }
            if (pokemon.Hp.Value == 0)
            {
                error = string.Format(GameString.Current.BattleLog("PokemonFainted"), pokemon.Name);
                return false;
            }
            if (pokemon.IndexInOwner < game.Settings.Mode.OnboardPokemonsPerPlayer())
            {
                error = string.Format(GameString.Current.BattleLog("PokemonFighting"), pokemon.Name);
                return false;
            }
            input.SendOut(index, pokemon);
            //多打中有多只精灵倒下，要把哪只精灵收回来
            //单打和合作没有这种需要
            InputFinished(input);
            return true;
        }
        public bool Pokemon(SimPokemon pokemon)
        {
            if (Pms[CurrentI].Pokemon(pokemon))
            {
                input.Switch(CurrentI, pokemon);
                NextPm();
                return true;
            }
            error = Pms[CurrentI].GetErrorMessage();
            return false;
        }
        public void GiveUp()
        {
            input.GiveUp = true;
            InputFinished(input);
        }
        //public void Undo();
        //public void TurnLeft();
        //public void TurnRight();
        //public void MoveToCenter();
        #endregion
    }
}
