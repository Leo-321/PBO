using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    internal static class ITs
    {
        public static void Attach(PokemonProxy pm)
        {
            if (pm.ItemE(Is.LEPPA_BERRY))
            {
                foreach (var m in pm.Moves)
                    if (m.PP == 0)
                    {
                        m.PP += 10;
                        pm.ConsumeItem();
                        pm.ShowLogPm("ItemPPRecover", Is.LEPPA_BERRY, m.MoveE.Id);
                        return;
                    }
            }
            else
            {
                WhiteHerb(pm);
                Seed(pm);
                HpChanged.Execute(pm);
                StateAdded.Execute(pm);
            }
        }

        /// <summary>
        /// auto consume item
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="stat"></param>
        /// <param name="change"></param>
        public static void ChangeLv5D(PokemonProxy pm, StatType stat, int change)
        {
            change = pm.CanChangeLv7D(pm, stat, change, false);
            if (change == 0) return;
            var i = pm.Pokemon.Item;
            string log;
            switch (change)
            {
                case 1:
                    log = "Item7DUp1";
                    break;
                case 2:
                    log = "Item7DUp2";
                    break;
                case -1:
                    log = "7DDown1";
                    break;
                case -2:
                    log = "7DDown2";
                    break;
                default:
                    log = change > 0 ? "Item7DUp3" : "7DDown3";
                    break;
            }
            pm.OnboardPokemon.ChangeLv7D(stat, change);
            pm.ConsumeItem();
            pm.ShowLogPm(log, i, (int)stat);
        }

        public static bool ChoiceItem(int item)
        {
            return item == Is.CHOICE_BAND || item == Is.CHOICE_SCARF || item == Is.CHOICE_SPECS;
        }

        /// <summary>
        /// pm.Item should not be 0
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        public static bool NeverLostItem(Pokemon pm)
        {
            var i = pm.Item;
            return
              i == Is.RSVP_MAIL ||
              pm.Form.Species.Number == MegaNumber(i) ||
              pm.Form.Species.Number == 487 && i == Is.GRISEOUS_ORB || //giratina
              PlatedArceus(pm) ||
              pm.Form.Species.Number == 649 && Is.DOUSE_DRIVE <= i && i <= Is.CHILL_DRIVE || //genesect
              Is.Normalium_Z <= i && i <= Is.Pikashunium_Z; 
        }
        /// <summary>
        /// false if item is zero
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        public static bool CanLostItem(PokemonProxy pm)
        {
            return !(pm.Pokemon.Item == 0 || NeverLostItem(pm.Pokemon) || pm.AbilityE(As.STICKY_HOLD));
        }
        public static bool CanUseItem(PokemonProxy pm)
        { return !(pm.OnboardPokemon.HasCondition(Cs.Embargo) || pm.Controller.Board.HasCondition(Cs.MagicRoom) || pm.AbilityE(As.KLUTZ)); }
        public static bool PlatedArceus(Pokemon pm)
        {
            return pm.Form.Species.Number == 493 && Is.FLAME_PLATE <= pm.Item && pm.Item <= Is.IRON_PLATE;
        }
        public static bool Berry(int id)
        {
            return id / 1000 == 8;
        }
        public static StatType GetTaste(int item)
        {
            switch (item)
            {
                case Is.FIGY_BERRY:
                    return StatType.Atk;
                case Is.WIKI_BERRY:
                    return StatType.SpAtk;
                case Is.MAGO_BERRY:
                    return StatType.Speed;
                case Is.AGUAV_BERRY:
                    return StatType.SpDef;
                case Is.IAPAPA_BERRY:
                    return StatType.Def;
            }
            return StatType.Invalid;
        }
        public static void RaiseItemByMove(PokemonProxy pm, int id, PokemonProxy by)
        {
            var op = pm.OnboardPokemon;
            switch (id)
            {
                case Is.WHITE_HERB:
                    {
                        bool raise = false;
                        var lvs = (Simple6D)op.Lv5D;
                        if (lvs.Atk < 0) { lvs.Atk = 0; raise = true; }
                        if (lvs.Def < 0) { lvs.Def = 0; raise = true; }
                        if (lvs.SpAtk < 0) { lvs.SpAtk = 0; raise = true; }
                        if (lvs.SpDef < 0) { lvs.SpDef = 0; raise = true; }
                        if (lvs.Speed < 0) { lvs.Speed = 0; raise = true; }
                        if (op.AccuracyLv < 0) { op.AccuracyLv = 0; raise = true; }
                        if (op.EvasionLv < 0) { op.EvasionLv = 0; raise = true; }
                        if (raise) pm.ShowLogPm("WhiteHerb");
                    }
                    break;
                case Is.MENTAL_HERB:
                    if (op.RemoveCondition(Cs.Attract)) pm.ShowLogPm("ItemDeAttract", Is.MENTAL_HERB);
                    break;
                case Is.KINGS_ROCK:
                    op.AddTurnCondition(Cs.Flinch);
                    break;
                case Is.LIGHT_BALL:
                    pm.AddState(by, AttachedState.PAR, false);
                    break;
                case Is.POISON_BARB:
                    pm.AddState(by, AttachedState.PSN, false);
                    break;
                case Is.TOXIC_ORB:
                    pm.AddState(by, AttachedState.PSN, false, 15);//战报待验证
                    break;
                case Is.FLAME_ORB:
                    pm.AddState(by, AttachedState.BRN, false);//战报
                    break;
                case Is.RAZOR_FANG:
                    op.AddTurnCondition(Cs.Flinch);
                    break;
                case Is.CHERI_BERRY:
                    if (pm.State == PokemonState.PAR) pm.DeAbnormalState();//战报
                    break;
                case Is.CHESTO_BERRY:
                    if (pm.State == PokemonState.PAR) pm.DeAbnormalState();
                    break;
                case Is.PECHA_BERRY:
                    if (pm.State == PokemonState.PAR) pm.DeAbnormalState();
                    break;
                case Is.RAWST_BERRY:
                    if (pm.State == PokemonState.BRN) pm.DeAbnormalState();
                    break;
                case Is.ASPEAR_BERRY:
                    if (pm.State == PokemonState.FRZ) pm.DeAbnormalState();
                    break;
                case Is.LEPPA_BERRY:
                    foreach (var m in pm.Moves)
                        if (m.PP == 0)
                        {
                            m.PP += 10;
                            pm.ShowLogPm("ItemPPRecover", Is.LEPPA_BERRY, m.MoveE.Id);
                            return;
                        }
                    foreach (var m in pm.Moves)
                        if (m.PP != m.Move.PP.Origin)
                        {
                            m.PP += 10;
                            pm.ShowLogPm("ItemPPRecover", Is.LEPPA_BERRY, m.MoveE.Id);
                            return;
                        }
                    break;
                case Is.ORAN_BERRY:
                    pm.HpRecover(10, false, Ls.ItemHpRecover, Is.ORAN_BERRY);
                    break;
                case Is.PERSIM_BERRY:
                    if (op.RemoveCondition(Cs.Confuse)) pm.ShowLogPm("DeConfuse");
                    break;
                case Is.LUM_BERRY:
                    if (pm.State != PokemonState.Normal) pm.DeAbnormalState();
                    break;
                case Is.SITRUS_BERRY:
                    pm.HpRecoverByOneNth(3, false, Ls.ItemHpRecover, Is.SITRUS_BERRY);
                    break;
                case Is.FIGY_BERRY:
                case Is.WIKI_BERRY:
                case Is.MAGO_BERRY:
                case Is.AGUAV_BERRY:
                case Is.IAPAPA_BERRY:
                    pm.HpRecoverByOneNth(8, false, Ls.ItemHpRecover, id);
                    if (pm.Pokemon.Nature.DislikeTaste(GetTaste(id))) pm.AddState(pm, AttachedState.Confuse, false);
                    break;
                case Is.LIECHI_BERRY:
                    pm.ChangeLv7D(by, StatType.Atk, 1, false);
                    break;
                case Is.GANLON_BERRY:
                    pm.ChangeLv7D(by, StatType.Def, 1, false);
                    break;
                case Is.SALAC_BERRY:
                    pm.ChangeLv7D(by, StatType.Speed, 1, false);
                    break;
                case Is.PETAYA_BERRY:
                    pm.ChangeLv7D(by, StatType.SpAtk, 1, false);
                    break;
                case Is.APICOT_BERRY:
                    pm.ChangeLv7D(by, StatType.SpDef, 1, false);
                    break;
                case Is.LANSAT_BERRY:
                    if (pm.OnboardPokemon.AddCondition(Cs.FocusEnergy)) pm.ShowLogPm("ItemEnFocusEnergy", Is.LANSAT_BERRY);
                    break;
                case Is.STARF_BERRY:
                    {
                        var ss = new List<StatType>();
                        if (pm.CanChangeLv7D(by, StatType.Atk, 2, false) != 0) ss.Add(StatType.Atk);
                        if (pm.CanChangeLv7D(by, StatType.Def, 2, false) != 0) ss.Add(StatType.Def);
                        if (pm.CanChangeLv7D(by, StatType.SpAtk, 2, false) != 0) ss.Add(StatType.SpAtk);
                        if (pm.CanChangeLv7D(by, StatType.SpDef, 2, false) != 0) ss.Add(StatType.SpDef);
                        if (pm.CanChangeLv7D(by, StatType.Speed, 2, false) != 0) ss.Add(StatType.Speed);
                        var n = ss.Count;
                        if (n != 0) pm.ChangeLv7D(by, ss[pm.Controller.GetRandomInt(0, n - 1)], 2, false);
                    }
                    break;
                case Is.MICLE_BERRY:
                    if (pm.OnboardPokemon.AddCondition(Cs.MicleBerry)) pm.ShowLogPm("MicleBerry", Is.MICLE_BERRY);
                    break;
            }
        }

        public static void WhiteHerb(PokemonProxy pm)
        {
            if (pm.ItemE(Is.WHITE_HERB)&& Reset7D(pm))
            {
                pm.ShowLogPm("WhiteHerb", pm.Pokemon.Item);
                pm.ConsumeItem();
            }
        }
        public static bool Reset7D(PokemonProxy pm)
        {
            Simple6D lvs = (Simple6D)pm.OnboardPokemon.Lv5D;
            bool raise = false;
            if (lvs.Atk < 0) { lvs.Atk = 0; raise = true; }
            if (lvs.Def < 0) { lvs.Def = 0; raise = true; }
            if (lvs.SpAtk < 0) { lvs.SpAtk = 0; raise = true; }
            if (lvs.SpDef < 0) { lvs.SpDef = 0; raise = true; }
            if (lvs.Speed < 0) { lvs.Speed = 0; raise = true; }
            if (pm.OnboardPokemon.AccuracyLv < 0) { pm.OnboardPokemon.AccuracyLv = 0; raise = true; }
            if (pm.OnboardPokemon.EvasionLv < 0) { pm.OnboardPokemon.EvasionLv = 0; raise = true; }
            return raise;
        }

        public static void Seed(PokemonProxy pm)
        {
            var b = pm.Controller.Board;
            if (pm.ItemE(Is.Electric_Seed)&&b.HasCondition(Cs.ElectricTerrain) || pm.ItemE(Is.Grassy_Seed)&&b.HasCondition(Cs.GrassyTerrain))
            {
                pm.ShowLogPm("Seed", pm.Pokemon.Item);
                pm.ChangeLv7D(pm, StatType.Def, 1, false);
                pm.ConsumeItem();
            }
            if (pm.ItemE(Is.Misty_Seed) && b.HasCondition(Cs.MistyTerrain) || pm.ItemE(Is.Psychic_Seed) && b.HasCondition(Cs.PsychicTerrain))
            {
                pm.ShowLogPm("Seed", pm.Pokemon.Item);
                pm.ChangeLv7D(pm, StatType.SpDef, 1, false);
                pm.ConsumeItem();
            }
        }
        public static bool AirBalloon(PokemonProxy pm) //气球的提示信息不是Attach而是Debut，是唯一会Debut的道具
        {
            if (pm.ItemE(Is.AIR_BALLOON)) //batonpass embargo
            {
                pm.ShowLogPm("EnBalloon");
                return true;
            }
            return false;
        }
        public static void AirBalloon(DefContext def)
        {
            def.Defender.RemoveItem();
            def.Defender.ShowLogPm("DeBalloon");
        }
        public static void AttackPostEffect(AtkContext atk)
        {
            var aer = atk.Attacker;
            var c = aer.Controller;
            bool e = true;
            if (!atk.IgnoreSwitchItem)
            {
                bool r = aer.Hp != 0 && aer.Controller.CanWithdraw(aer);
                foreach (var d in atk.Targets)
                    if (!d.HitSubstitute && d.Defender.Tile != null)
                    {
                        var der = d.Defender;
                        var i = der.Item;
                        if (e && i == Is.EJECT_BUTTON)
                        {
                            atk.SetCondition(Cs.EjectButton, der.Tile);
                            der.ConsumeItem();
                            c.Withdraw(der, "EjectButton", 0, true);
                            if (r == false) break;
                            e = false;
                        }
                        else if (r && i == Is.RED_CARD)
                        {
                            der.ConsumeItem();
                            der.ShowLogPm("RedCard", aer.Id);
                            r = !MoveE.ForceSwitchImplement(aer, true);
                            if (e == false) return;
                        }
                        else if (e && (der.Ability == As.Emergency_Exit || der.Ability == As.Wimp_Out) && der.Hp <= der.Pokemon.MaxHp / 2 && atk.TotalDamage + der.Hp > der.Pokemon.MaxHp / 2)
                        {
                            atk.SetCondition(Cs.EjectButton, der.Tile);
                            if (der.Ability == As.Emergency_Exit)
                                c.Withdraw(der, "EmergencyExit", 0, true);
                            else if (der.Ability == As.Wimp_Out)
                                c.Withdraw(der, "WimpOut", 0, true);
                            if (r == false) break;
                            e = false;
                        }
                    }
            }
            bool moreThanHalf = aer.Hp > aer.Pokemon.MaxHp / 2;
            if (aer.ItemE(Is.SHELL_BELL))
            {
                if (atk.TotalDamage != 0)
                    aer.HpRecoverByOneNth(atk.TotalDamage, false, Ls.ItemHpRecover, Is.SHELL_BELL, false, true);
            }
            else if (aer.ItemE(Is.LIFE_ORB) && aer.EffectHurtByOneNth(10, Ls.LifeOrb)) aer.CheckFaint();
        }
        public static bool CanAttackFlinch(DefContext def)
        {
            return (def.AtkContext.Attacker.ItemE(Is.KINGS_ROCK) || def.AtkContext.Attacker.ItemE(Is.RAZOR_FANG)) && def.Defender.Controller.RandomHappen(10);
        }
        public static bool PowerHerb(PokemonProxy pm)
        {
            if (pm.ItemE(Is.POWER_HERB))
            {
                pm.ShowLogPm(Ls.PowerHerb, Is.POWER_HERB);
                pm.ConsumeItem();
                pm.CoordY = CoordY.Plate;
                return true;
            }
            return false;
        }
        public static double FloatStone(PokemonProxy pm)
        {
            if (pm.ItemE(Is.FLOAT_STONE)) return 0.5d;
            return 1d;
        }
        public static bool Gem(int item)
        {
            return item / 1000 == 7;
        }
        public static void CheckGem(AtkContext atk)
        {
            int i = atk.Attacker.Item;
            BattleType t;
            switch (i)
            {
                case Is.FIRE_GEM:
                    t = BattleType.Fire;
                    break;
                case Is.WATER_GEM:
                    t = BattleType.Water;
                    break;
                case Is.ELECTRIC_GEM:
                    t = BattleType.Electric;
                    break;
                case Is.GRASS_GEM:
                    t = BattleType.Grass;
                    break;
                case Is.ICE_GEM:
                    t = BattleType.Ice;
                    break;
                case Is.FIGHTING_GEM:
                    t = BattleType.Fighting;
                    break;
                case Is.POISON_GEM:
                    t = BattleType.Poison;
                    break;
                case Is.GROUND_GEM:
                    t = BattleType.Ground;
                    break;
                case Is.FLYING_GEM:
                    t = BattleType.Flying;
                    break;
                case Is.PSYCHIC_GEM:
                    t = BattleType.Psychic;
                    break;
                case Is.BUG_GEM:
                    t = BattleType.Bug;
                    break;
                case Is.ROCK_GEM:
                    t = BattleType.Rock;
                    break;
                case Is.GHOST_GEM:
                    t = BattleType.Ghost;
                    break;
                case Is.DRAGON_GEM:
                    t = BattleType.Dragon;
                    break;
                case Is.DARK_GEM:
                    t = BattleType.Dark;
                    break;
                case Is.STEEL_GEM:
                    t = BattleType.Steel;
                    break;
                case Is.FAIRY_GEM:
                    t = BattleType.Fairy;
                    break;
                case Is.NORMAL_GEM:
                    t = BattleType.Normal;
                    break;
                default:
                    return;
            }
            if (atk.Type == t)
            {
                atk.Controller.ReportBuilder.ShowLog("Gem", i, atk.Move.Id);
                atk.SetTurnCondition(Cs.Gem);
                atk.Attacker.ConsumeItem();
            }
        }
        public static void DestinyKnot(PokemonProxy pm, PokemonProxy by)
        {
            if (pm.ItemE(Is.DESTINY_KNOT)) by.AddState(pm, AttachedState.Attract, false, 0, "ItemEnAttract", Is.DESTINY_KNOT);
        }

        public static int MegaNumber(int item)
        {
            switch (item)
            {
                case Is.VENUSAURITE:
                    return 3;
                case Is.CHARIZARDITE_X:
                case Is.CHARIZARDITE_Y:
                    return 6;
                case Is.BLASTOISINITE:
                    return 9;
                case Is.ALAKAZITE:
                    return 65;
                case Is.GENGARITE:
                    return 94;
                case Is.KANGASKHANITE:
                    return 115;
                case Is.PINSIRITE:
                    return 127;
                case Is.GYARADOSITE:
                    return 130;
                case Is.AERODACTYLITE:
                    return 142;
                case Is.MEWTWONITE_X:
                case Is.MEWTWONITE_Y:
                    return 150;
                case Is.AMPHAROSITE:
                    return 181;
                case Is.SCIZORITE:
                    return 212;
                case Is.HERACRONITE:
                    return 214;
                case Is.HOUNDOOMINITE:
                    return 229;
                case Is.TYRANITARITE:
                    return 248;
                case Is.BLAZIKENITE:
                    return 257;
                case Is.GARDEVOIRITE:
                    return 282;
                case Is.MAWILITE:
                    return 303;
                case Is.AGGRONITE:
                    return 306;
                case Is.MEDICHAMITE:
                    return 308;
                case Is.MANECTITE:
                    return 310;
                case Is.BANETTITE:
                    return 354;
                case Is.ABSOLITE:
                    return 359;
                case Is.GARCHOMPITE:
                    return 445;
                case Is.LUCARIONITE:
                    return 448;
                case Is.ABOMASITE:
                    return 460;
                case Is.BEEDRILLITE:
                    return 15;
                case Is.PIDGEOTITE:
                    return 18;
                case Is.SLOWBRONITE:
                    return 80;
                case Is.STEELIXITE:
                    return 208;
                case Is.SCEPTILITE:
                    return 254;
                case Is.SWAMPERTITE:
                    return 260;
                case Is.SABLENITE:
                    return 302;
                case Is.SHARPEDONITE:
                    return 319;
                case Is.CAMERUPTITE:
                    return 323;
                case Is.ALTARIANITE:
                    return 334;
                case Is.GLALITITE:
                    return 362;
                case Is.SALAMENCITE:
                    return 373;
                case Is.METAGROSSITE:
                    return 376;
                case Is.LATIASITE:
                    return 380;
                case Is.LATIOSITE:
                    return 381;
                case Is.LOPUNNITE:
                    return 428;
                case Is.GALLADITE:
                    return 475;
                case Is.AUDINITE:
                    return 531;
                case Is.DIANCITE:
                    return 719;
                default:
                    return 0;
            }
        }
        public static int MegaForm(int item)
        {
            return item == Is.CHARIZARDITE_Y || item == Is.MEWTWONITE_Y ? 2 : 1;
        }
        /// <summary>
        /// weather should not be Normal
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="weather"></param>
        /// <returns></returns>
        public static int Item(this Weather weather)
        {
            return weather == Weather.Hailstorm ? Is.ICY_ROCK : weather == Weather.Sandstorm ? Is.SMOOTH_ROCK : weather == Weather.IntenseSunlight ? Is.HEAT_ROCK : Is.DAMP_ROCK;
        }
    }
}
