using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
    [DataContract(Name = "ii", Namespace = PBOMarks.JSON)]
    public sealed class IInput
    {
        public static IInput UseMove(SimMove move, bool mega, bool zmove, int targetTeam, int targetX)
        {
            return new IInput(move.Type.Id, mega, zmove, targetTeam + 1, targetX + 1, 0);
        }
        public static IInput UseMove(SimMove move, bool mega, bool zmove)
        {
            return new IInput(move.Type.Id, mega, zmove, 0, 0, 0);
        }
        public static IInput SendOut(SimPokemon sendout)
        {
            return new IInput(0, false, false, 0, 0, sendout.IndexInOwner);
        }
        public static IInput Struggle()
        {
            return new IInput(0, false, false, 0, 0, 0);
        }

        [DataMember(Name = "a", EmitDefaultValue = false)]
        public readonly int Move;

        [DataMember(Name = "e", EmitDefaultValue = false)]
        public readonly bool Mega;

        [DataMember(Name = "f", EmitDefaultValue = false)]
        public readonly bool Zmove;

        [DataMember(Name = "c", EmitDefaultValue = false)]
        public readonly int TargetTeam;

        [DataMember(Name = "d", EmitDefaultValue = false)]
        public readonly int TargetX;

        [DataMember(Name = "b", EmitDefaultValue = false)]
        public readonly int SendOutIndex;

        private IInput(int move, bool mega, bool zmove, int targetTeam, int targetX, int sendout)
        {
            Move = move;
            Mega = mega;
            Zmove = zmove;
            TargetTeam = targetTeam;
            TargetX = targetX;
            SendOutIndex = sendout;
        }
    }
    [DataContract(Name = "ai", Namespace = PBOMarks.JSON)]
    public class ActionInput
    {
        public ActionInput(int maxI)
        {
        }
        protected ActionInput(ActionInput action)
        {
            I0 = action.I0;
            I1 = action.I1;
            I2 = action.I2;
            GiveUp = action.GiveUp;
        }
        protected ActionInput()
        {
        }

        [DataMember(EmitDefaultValue = false)]
        public bool GiveUp
        { get; internal set; }

        [DataMember(EmitDefaultValue = false)]
        private IInput I0;

        [DataMember(EmitDefaultValue = false)]
        private IInput I1;

        [DataMember(EmitDefaultValue = false)]
        private IInput I2;

        private void Set(int i, IInput input)
        {
            if (i == 0) I0 = input;
            else if (i == 1) I1 = input;
            else I2 = input;
        }
        public IInput Get(int i)
        {
            return i == 0 ? I0 : i == 1 ? I1 : I2;
        }

        public void UseMove(int i, SimMove move, bool mega, bool zmove, int targetTeam, int targetX)
        {
            Set(i, IInput.UseMove(move, mega, zmove, targetTeam, targetX));
        }
        public void UseMove(int i, SimMove move, bool mega ,bool zmove)
        {
            Set(i, IInput.UseMove(move, mega, zmove));
        }
        public void Switch(int i, SimPokemon sendout)
        {
            Set(i, IInput.SendOut(sendout));
        }
        public void SendOut(int i, SimPokemon sendout)
        {
            Set(i, IInput.SendOut(sendout));
        }
        public void Struggle(int i)
        {
            Set(i, IInput.Struggle());
        }
    }
}
