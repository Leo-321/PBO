using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class HpChanged
    {
        public static void Execute(PokemonProxy pm)
        {
            switch (pm.Item)
            {
                case Is.ORAN_BERRY: //135
                    RecoverBerry(pm, 10);
                    break;
                case Is.BERRY_JUICE: //194
                    RecoverBerry(pm, 20);
                    break;
                case Is.SITRUS_BERRY: //138
                    if (pm.Hp << 1 < pm.Pokemon.MaxHp) pm.HpRecoverByOneNth(4, false, Ls.ItemHpRecover, Is.SITRUS_BERRY, true);
                    break;
                case Is.FIGY_BERRY: //139
                case Is.WIKI_BERRY: //140
                case Is.MAGO_BERRY: //141
                case Is.AGUAV_BERRY: //142
                case Is.IAPAPA_BERRY: //143
                    TastyBerry(pm);
                    break;
                case Is.LIECHI_BERRY: //181
                    Up1Berry(pm, StatType.Atk);
                    break;
                case Is.GANLON_BERRY:
                    Up1Berry(pm, StatType.Def);
                    break;
                case Is.SALAC_BERRY:
                    Up1Berry(pm, StatType.Speed);
                    break;
                case Is.PETAYA_BERRY:
                    Up1Berry(pm, StatType.SpAtk);
                    break;
                case Is.APICOT_BERRY: //185
                    Up1Berry(pm, StatType.SpDef);
                    break;
                case Is.LANSAT_BERRY: //186
                    if (ATs.Gluttony(pm) && pm.OnboardPokemon.AddCondition(Cs.FocusEnergy))
                    {
                        pm.ConsumeItem();
                        pm.ShowLogPm("ItemEnFocusEnergy", Is.LANSAT_BERRY);
                    }
                    break;
                case Is.STARF_BERRY: //187
                    StarfBerry(pm);
                    break;
                case Is.MICLE_BERRY: //189
                    if (ATs.Gluttony(pm) && pm.OnboardPokemon.AddCondition(Cs.MicleBerry))
                    {
                        pm.ConsumeItem();
                        pm.ShowLogPm("MicleBerry", Is.MICLE_BERRY);
                    }
                    break;
            }
        }
        private static void Up1Berry(PokemonProxy pm, StatType stat)
        {
            if (ATs.Gluttony(pm)) ITs.ChangeLv5D(pm, stat, 1);
        }
        private static void StarfBerry(PokemonProxy pm)
        {
            if (ATs.Gluttony(pm))
            {
                var ss = new List<StatType>(5);
                if (pm.CanChangeLv7D(pm, StatType.Atk, 2, false) != 0) ss.Add(StatType.Atk);
                if (pm.CanChangeLv7D(pm, StatType.Def, 2, false) != 0) ss.Add(StatType.Def);
                if (pm.CanChangeLv7D(pm, StatType.SpAtk, 2, false) != 0) ss.Add(StatType.SpAtk);
                if (pm.CanChangeLv7D(pm, StatType.SpDef, 2, false) != 0) ss.Add(StatType.SpDef);
                if (pm.CanChangeLv7D(pm, StatType.Speed, 2, false) != 0) ss.Add(StatType.Speed);
                var n = ss.Count;
                if (n != 0) ITs.ChangeLv5D(pm, ss[pm.Controller.GetRandomInt(0, n - 1)], 2);
            }
        }
        private static void RecoverBerry(PokemonProxy pm, int hp)
        {
            if (pm.Hp << 1 <= pm.Pokemon.MaxHp) pm.HpRecover(hp, false, Ls.ItemHpRecover, pm.Pokemon.Item, true);
        }
        private static void TastyBerry(PokemonProxy pm)
        {
            if (ATs.Gluttony(pm) && pm.CanHpRecover(false))
            {
                pm.HpRecoverByOneNth(2, false, Ls.ItemHpRecover, pm.Pokemon.Item, true);
                if (pm.Pokemon.Nature.DislikeTaste(ITs.GetTaste(pm.Pokemon.Item))) pm.AddState(pm, AttachedState.Confuse, false);
            }
        }
    }
}
