using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
    /// <summary>
    /// because damage can be zero, and also the flash animation, this cannot be replaced by several ShowHp
    /// </summary>
    [DataContract(Name = "ed", Namespace = PBOMarks.JSON)]
    public class MoveHurt : GameEvent
    {
        [DataMember(Name = "a")]
        public int[] Pms;
        [DataMember(Name = "b", EmitDefaultValue = false)]
        public int[] Damages;
        [DataMember(Name = "e", EmitDefaultValue = false)]
        public int[] CT;
        [DataMember(Name = "c", EmitDefaultValue = false)]
        public int[] SH; //效果拔群
        [DataMember(Name = "d", EmitDefaultValue = false)]
        public int[] WH; //没有什么效果

        protected override void Update()
        {
            int max = 0;
            for (int i = 0; i < Pms.Length; ++i)
            {
                PokemonOutward p = GetPokemon(Pms[i]);
                p.Hurt(Damages[i]);
                AppendGameLog(Ls.Hurt, LogStyle.Detail | LogStyle.NoBr | LogStyle.HiddenInBattle, Pms[i]);
                AppendGameLog(Ls.nhp, LogStyle.Detail | LogStyle.NoBr | LogStyle.HiddenInBattle, -Damages[i]);
                AppendGameLog(Ls.br, LogStyle.Detail | LogStyle.HiddenInBattle);
                if (Damages[i] > max) max = Damages[i];
            }
            Sleep = 11 * max;
            if (SH != null) AppendGameLog("SuperHurt" + SH.Length, LogStyle.SYS, SH.ValueOrDefault(0), SH.ValueOrDefault(1), SH.ValueOrDefault(2));
            if (WH != null) AppendGameLog("WeakHurt" + WH.Length,LogStyle.SYS, WH.ValueOrDefault(0), WH.ValueOrDefault(1), WH.ValueOrDefault(2));
            if (CT != null) AppendGameLog("CT" + CT.Length, LogStyle.SYS, CT.ValueOrDefault(0), CT.ValueOrDefault(1), CT.ValueOrDefault(2));
        }
        public override void Update(SimGame game)
        {
            for (int i = 0; i < Pms.Length; ++i)
            {
                var pm = GetPokemon(game, Pms[i]);
                if (pm != null) pm.SetHp(pm.Hp.Value - Damages[i]);
            }
        }
    }
}
