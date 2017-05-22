using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class CalculateDamages
    {
        public static void Execute(AtkContext atk)
        {
            var move = atk.Move;
            var def = atk.Target;
            var der = def.Defender;
            var aer = atk.Attacker;

            bool ls = false, r = false, av= false, feint = false;
            switch (move.Id)
            {
                case Ms.BRICK_BREAK: //280
                case Ms.Psychic_Fangs:
                    if (der.Pokemon.TeamId != aer.Pokemon.TeamId)
                    {
                        ls = der.Field.RemoveCondition(Cs.LightScreen);
                        r = der.Field.RemoveCondition(Cs.Reflect);
                        av = der.Field.RemoveCondition(Cs.Aurora_Veil);
                    }
                    break;
                case Ms.FEINT: //364
                    feint = der.OnboardPokemon.RemoveCondition(Cs.Protect) | (der.Pokemon.TeamId != atk.Attacker.Pokemon.TeamId && (der.Field.RemoveCondition(Cs.QuickGuar) | der.Field.RemoveCondition(Cs.WideGuard)));
                    break;
            }
            switch (move.Id)
            {
                case Ms.COUNTER:
                    Counter(atk, Cs.PhysicalDamage, 0x2000);
                    break;
                case Ms.MIRROR_COAT:
                    Counter(atk, Cs.SpecialDamage, 0x2000);
                    break;
                case Ms.METAL_BURST:
                    Counter(atk, Cs.Damage, 0x1800);
                    break;
                case Ms.SONIC_BOOM: //49
                    def.Damage = 20;
                    break;
                case Ms.DRAGON_RAGE: //82
                    def.Damage = 40;
                    break;
                case Ms.SEISMIC_TOSS: //69
                case Ms.NIGHT_SHADE: //101
                    def.Damage = aer.Pokemon.Lv;
                    break;
                case Ms.BIDE: //117
                    def.Damage = atk.GetCondition(Cs.Bide).Damage << 1;
                    break;
                case Ms.PSYWAVE: //149
                    def.Damage = der.Controller.GetRandomInt(50, 150) * aer.Pokemon.Lv / 100;
                    if (def.Damage == 0) def.Damage = 1;
                    break;
                case Ms.SUPER_FANG: //162
                case Ms.Natures_Madness:
                    def.Damage = der.Hp >> 1;
                    if (def.Damage == 0) def.Damage = 1;
                    break;
                case Ms.Guardian_of_Alola:
                    def.Damage = der.Hp * 3 / 4;
                    if (def.Damage == 0) def.Damage = 1;
                    break;
                case Ms.ENDEAVOR: //283
                    def.Damage = der.Hp - aer.Hp;
                    if (def.Damage < 0) def.Damage = 0;
                    break;
                case Ms.FINAL_GAMBIT: //515
                    def.Damage = atk.Attacker.Hp;
                    break;
                default:
                    if (move.Class != MoveClass.OHKO)
                    {
                        ITs.CheckGem(atk);
                        foreach (DefContext d in atk.Targets) CalculateDamage(d);
                    }
                    break;
            }
            switch (move.Id)
            {
                case Ms.FALSE_SWIPE:
                case Ms.HOLD_BACK:
                    if (def.Damage >= def.Defender.Hp) def.Damage = def.Defender.Hp - 1;
                    break;
                case Ms.BEAT_UP:
                    {//无视属性相克修正，但依然会显示“没有什么效果”“效果拔群”的战报。
                        BattleType a = def.AtkContext.Type;
                        def.EffectRevise = a == BattleType.Ground && der.ItemE(Is.IRON_BALL) && der.OnboardPokemon.HasType(BattleType.Flying) ? 0 : a.EffectRevise(der.OnboardPokemon.Types);
                    }
                    break;
                case Ms.SELFDESTRUCT: //120
                case Ms.EXPLOSION: //153
                case Ms.FINAL_GAMBIT: //515
                    atk.Attacker.Faint();
                    break;
                case Ms.BRICK_BREAK: //280
                case Ms.Psychic_Fangs:
                    if (ls) atk.Controller.ReportBuilder.ShowLog("DeLightScreen", der.Field.Team);
                    if (r) atk.Controller.ReportBuilder.ShowLog("DeReflect", der.Field.Team);
                    if (av) atk.Controller.ReportBuilder.ShowLog("DeAurora_Veil", der.Field.Team);
                    break;
                case Ms.FEINT: //364
                    if (feint) der.ShowLogPm("Feint");
                    break;
            }
            bool Zprotect = false;
            if (move.Zmove)
                Zprotect = der.OnboardPokemon.RemoveCondition(Cs.Protect);
            if (Zprotect)
            {
                def.Damage >>= 2;
                der.ShowLogPm("Zprotect");
            }
            if (atk.DefenderAbilityAvailable() && def.Defender.CanChangeForm(778, 1) && def.Damage>0 && def.Defender.RaiseAbility(As.Disguise))
            {
                def.Defender.ChangeForm(1, true);
                def.Damage = 0;
            }
        }
        private static void CalculateEffectRevise(DefContext def)
        {
            var atk = def.AtkContext;
            var der = def.Defender;
            var types = def.Defender.OnboardPokemon.Types;
            BattleType a = atk.Type;
            def.EffectRevise = a == BattleType.Ground && der.ItemE(Is.IRON_BALL) && der.OnboardPokemon.HasType(BattleType.Flying) ? 0 : a.EffectRevise(types);
            if (atk.Move.Id == Ms.FLYING_PRESS) def.EffectRevise += BattleType.Flying.EffectRevise(types);
            else if (atk.Move.Id == Ms.FREEZEDRY && a == BattleType.Ice && types.Contains(BattleType.Water)) def.EffectRevise += 2;
        }
        private static readonly int[] LV_CT = { 16, 8, 2 };
        private static void Ct(DefContext def)
        {
            if (!(def.Defender.Field.HasCondition(Cs.LuckyChant) || def.CannotBeCted()))
            {
                var atk = def.AtkContext;
                if (atk.Attacker.OnboardPokemon.HasCondition(Cs.Laser_Focus) || atk.Move.MustCt || (atk.Attacker.Ability == As.Merciless && (def.Defender.State == PokemonState.BadlyPSN || def.Defender.State == PokemonState.PSN))) def.IsCt = true;
                else
                {
                    var pm = atk.Attacker;
                    var ct = atk.Move.Ct1 ? 1 : 0;
                    if (atk.Move.Move.Id == Ms.tenkk_Volt) ct = 2;
                    if (pm.OnboardPokemon.HasCondition(Cs.FocusEnergy)) ct += 2;
                    if (pm.AbilityE(As.SUPER_LUCK)) ct++;
                    switch (pm.Item)
                    {
                        case Is.SCOPE_LENS:
                        case Is.RAZOR_CLAW:
                            ct++;
                            break;
                        case Is.LUCKY_PUNCH:
                            if (pm.Pokemon.Form.Species.Number == 113) ct += 2;
                            break;
                        case Is.STICK:
                            if (pm.Pokemon.Form.Species.Number == 83) ct += 2;
                            break;
                    }
                    def.IsCt = ct > 2 || pm.Controller.OneNth(LV_CT[ct]);
                }
            }
        }
        private static void CalculateDamage(DefContext def)
        {
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var c = aer.Controller;
            var move = atk.Move;

            Ct(def);

            def.Damage = aer.Pokemon.Lv * 2 / 5 + 2;
            {
                MoveBasePower.Execute(def);
                def.Damage *= def.BasePower * PowerModifier.Execute(def);
            }
            {
                int a;
                {
                    OnboardPokemon p;
                    if (move.Id == Ms.FOUL_PLAY) p = def.Defender.OnboardPokemon;
                    else p = atk.Attacker.OnboardPokemon;
                    StatType st = move.Move.Category == MoveCategory.Physical ? StatType.Atk : StatType.SpAtk;
                    a = p.FiveD.GetStat(st);
                    if (!def.AbilityE(As.UNAWARE))
                    {
                        int atkLv = p.Lv5D.GetStat(st);
                        if (!(def.IsCt && atkLv < 0)) a = OnboardPokemon.Get5D(a, atkLv);
                    }
                }
                a *= AModifier.Hustle(atk);
                def.Damage *= a * AModifier.Execute(def);
            }
            {
                StatType st = move.Move.Category == MoveCategory.Physical || move.UsePhysicalDef ? StatType.Def : StatType.SpDef;
                int d = def.Defender.OnboardPokemon.FiveD.GetStat(st);
                int defLv;
                if (aer.AbilityE(As.UNAWARE) || move.IgnoreDefenderLv7D) defLv = 0;
                else
                {
                    defLv = def.Defender.OnboardPokemon.Lv5D.GetStat(st);
                    if (!(def.IsCt && defLv > 0)) d = OnboardPokemon.Get5D(d, defLv);
                }
                d *= DModifier.Sandstorm(def);
                def.Damage /= d * DModifier.Execute(def);
            }
            def.Damage /= 50;
            def.Damage += 2;
            //1.Apply the multi-target modifier
            if (atk.MultiTargets) def.ModifyDamage(0xC00);
            //2.Apply the weather modifier
            {
                Weather w = c.Weather;
                BattleType type = atk.Type;
                if (w == Weather.IntenseSunlight)
                {
                    if (type == BattleType.Water) def.ModifyDamage(0x800);
                    else if (type == BattleType.Fire) def.ModifyDamage(0x1800);
                }
                else if (w == Weather.Rain)
                {
                    if (type == BattleType.Water) def.ModifyDamage(0x1800);
                    else if (type == BattleType.Fire) def.ModifyDamage(0x800);
                }
            }
            //3.In case of a critical hit, double the value
            if (def.IsCt) def.ModifyDamage(0x1800);
            //4.Alter with a random factor
            def.Damage *= aer.Controller.GetRandomInt(85, 100);
            def.Damage /= 100;
            //5.Apply STAB modifier
            if (atk.Attacker.OnboardPokemon.HasType(atk.Type))
                def.ModifyDamage((Modifier)(atk.Attacker.AbilityE(As.ADAPTABILITY) ? 0x2000 : 0x1800));
            //6.Alter with type effectiveness
            CalculateEffectRevise(def);
            if (def.EffectRevise > 0)
            {
                if (atk.Type.EffectRevise(BattleType.Flying) > 0 && def.Defender.OnboardPokemon.HasType(BattleType.Flying) && aer.Controller.Board.GetCondition<int>(Cs.SpWeather) == As.DELTA_STREAM)
                {
                    c.ReportBuilder.ShowLog(Ls.MysteriousAirCurrent);
                    def.ModifyDamage(0x800);
                }
                def.Damage <<= def.EffectRevise;
            }
            else if (def.EffectRevise < 0) def.Damage >>= -def.EffectRevise;
            //7.Alter with user's burn
            if (move.Move.Category == MoveCategory.Physical && aer.State == PokemonState.BRN && !aer.AbilityE(As.GUTS) && move.Id!=Ms.FACADE ) def.Damage >>= 1;
            //8.Make sure damage is at least 1
            if (def.Damage < 1) def.Damage = 1;
            //9.Apply the final modifier
            def.Damage *= DamageModifier.Execute(def);
        }

        private static void Counter(AtkContext atk, Cs condition, Modifier modifier)
        {
            ITs.CheckGem(atk);
            atk.Target.Damage = atk.Attacker.OnboardPokemon.GetCondition(condition).Damage;
            atk.Target.ModifyDamage(modifier);
        }
    }
}
