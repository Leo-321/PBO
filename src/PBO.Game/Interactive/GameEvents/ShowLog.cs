using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
    [DataContract(Name = "e", Namespace = PBOMarks.JSON)]
    public class ShowLog : GameEvent
    {
        private static LogStyle GetStyle(string key)
        {
            switch (key)
            {
                case Ls.PLAYER_DISCONNECT:
                case Ls.INVALID_INPUT:
                case Ls.NO_KEY:
                case Ls.SERROR:
                case "SuperHurt0":
                case "SuperHurt1":
                case "SuperHurt2":
                case "SuperHurt3":
                case "WeakHurt0":
                case "WeakHurt1":
                case "WeakHurt2":
                case "WeakHurt3":
                case "CT0":
                case "CT1":
                case "CT2":
                case "CT3":
                case "Fail":
                case "Fail0":
                case "NoEffect":
                case "Leo":
                case "Ksam":
                case "PurpleWind":
                    return LogStyle.SYS;
                case Ls.PSN:
                case Ls.BRN:
                case Ls.Nightmare:
                case Ls.TrapHurt:
                case Ls.ReHurt:
                case Ls.HpRecover:
                case Ls.SandstormHurt:
                case Ls.HailstormHurt:
                case Ls.SetAbility:
                case Ls.RockyHelmet:
                case Ls.PoisonHeal:
                case Ls.BadDreams:
                case Ls.AquaRing:
                case Ls.LeechSeed:
                case Ls.BellyDrum:
                case Ls.Rest:
                case Ls.Wish:
                case Ls.Ingrain:
                case Ls.EnSubstitute:
                case Ls.Spikes:
                case Ls.StealthRock:
                case Ls.HpAbsorb:
                case Ls.FailSelfHurt:
                case Ls.ItemHpRecover:
                case Ls.ItemHpRecover2:
                case Ls.LifeOrb:
                case Ls.ReHurtItem:
                case Ls.ItemHurt:
                case Ls.EnCurse:
                case Ls.Curse:
                case Ls.FlameBurst:
                case Ls.FireSea:
                case Ls.ConfuseWork:
                case Ls.Hurt:
                case Ls.Powder:
                case Ls.Ability:
                    return LogStyle.NoBr;
                case Ls.br:
                case Ls.setAbility:
                case Ls.skillSwap:
                case Ls.forceWithdraw:
                case Ls.painSplit:
                    return LogStyle.Detail;
                case Ls.php:
                case Ls.nhp:
                    throw new Exception("Invalid key");
                default:
                    return LogStyle.Default;
            }
        }

        [DataMember(Name = "a")]
        protected string Key;

        [DataMember(Name = "b", EmitDefaultValue = false)]
        protected int I0;

        [DataMember(Name = "c", EmitDefaultValue = false)]
        protected int I1;

        [DataMember(Name = "d", EmitDefaultValue = false)]
        protected int I2;

        /// <param name="args">string and int is fine</param>
        public ShowLog(string gameLogKey, int arg0 = 0, int arg1 = 0, int arg2 = 0)
        {
            Key = gameLogKey;
            I0 = arg0;
            I1 = arg1;
            I2 = arg2;
        }
        protected override void Update()
        {
            if (Key == Ls.SendOut1 || Key == Ls.SendOut2 || Key == Ls.SendOut22 || Key == Ls.SendOut3)
            {
                AppendGameLog(Key + "_r", LogStyle.HiddenAfterBattle, I0, I1, I2);
                AppendGameLog(Key + "_f", LogStyle.HiddenInBattle, I0, I1, I2);
            }
            else AppendGameLog(Key, GetStyle(Key), I0, I1, I2);
        }
    }
}