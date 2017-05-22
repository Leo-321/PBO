using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class STs
    {
        public static PokemonProxy HasAbility(this IEnumerable<PokemonProxy> pms, int ability)
        {
            foreach (var p in pms)
                if (p.AbilityE(ability)) return p;
            return null;
        }
        public static PokemonProxy HasAbilityExcept(this IEnumerable<PokemonProxy> pms, int ability, PokemonProxy except)
        {
            foreach (var p in pms)
                if (p.AbilityE(ability)&&p!=except) return p;
            return null;
        }
        public static PokemonProxy RaiseAbility(this IEnumerable<PokemonProxy> pms, int ability)
        {
            foreach (var p in pms)
                if (p.RaiseAbility(ability)) return p;
            return null;
        }
        #region gen5
        public static void KOed(DefContext def, OnboardPokemon o)
        {
            var der = def.Defender;
            var aer = def.AtkContext.Attacker;
            if (o.HasCondition(Cs.DestinyBond))
            {
                aer.ShowLogPm("DestinyBond"); //战报顺序已测
                aer.Faint();
            }
            var mp = def.AtkContext.MoveProxy;
            if (o.HasCondition(Cs.Grudge) && mp != null && mp.PP != 0)
            {
                mp.PP = 0;
                aer.ShowLogPm("Grudge");
            }
            if (def.AtkContext.Move.Id == Ms.FELL_STINGER) aer.ChangeLv7D(aer, StatType.Atk, 2, false);
            if (aer.AbilityE(As.MOXIE)) aer.ChangeLv7D(aer, StatType.Atk, 1, false, true);
            if (aer.AbilityE(As.Beast_Boost)) aer.ChangeLv7D(aer, aer.MaxStat, 1, false, true);
            if (aer.AbilityE(As.Battle_Bond) && aer.CanChangeForm(658, 1))
            {
                aer.RaiseAbility();
                aer.ChangeForm(1, true);
            }
        }
        public static void WillAct(PokemonProxy pm)
        {
            pm.OnboardPokemon.RemoveCondition(Cs.DestinyBond);
            pm.OnboardPokemon.RemoveCondition(Cs.Grudge);
            pm.OnboardPokemon.RemoveCondition(Cs.Rage);
            var i = pm.OnboardPokemon.GetCondition<int>(Cs.Taunt);
            if (i != 0) pm.OnboardPokemon.SetCondition(Cs.Taunt, i - 1);
            var o = pm.OnboardPokemon.GetCondition(Cs.Encore);
            if (o != null) o.Turn--;
        }
        public static void SendingOut(PokemonProxy pm)
        {
            pm.Reset();
            var o = pm.OnboardPokemon;
            if (pm.State == PokemonState.BadlyPSN) o.SetCondition(Cs.BadlyPSN, pm.Controller.TurnNumber);
            var pass = pm.Tile.GetCondition<OnboardPokemon>(Cs.BatonPass);
            if (pass != null)
            {
                o.SetLv7D(pass.Lv5D.Atk, pass.Lv5D.Def, pass.Lv5D.SpAtk, pass.Lv5D.SpDef, pass.Lv5D.Speed, pass.AccuracyLv, pass.EvasionLv);
                pm.Tile.RemoveCondition(Cs.BatonPass);
                object c;
                //混乱状态 
                c = pass.GetCondition<object>(Cs.Confuse);
                if (c != null) o.SetCondition(Cs.Confuse, c);
                //寄生种子状态 
                c = pass.GetCondition<object>(Cs.LeechSeed);
                if (c != null) o.SetCondition(Cs.LeechSeed, c);
                //扣押状态
                c = pass.GetCondition<object>(Cs.Embargo);
                if (c != null) o.SetCondition(Cs.Embargo, c);
                //回复封印状态 
                c = pass.GetCondition<object>(Cs.HealBlock);
                if (c != null) o.SetCondition(Cs.HealBlock, c);
                //地狱突刺状态
                c = pass.GetCondition<object>(Cs.SoundBlock);
                if (c != null) o.SetCondition(Cs.SoundBlock, c);
                //念动力状态
                c = pass.GetCondition<object>(Cs.Telekinesis);
                if (c != null) o.SetCondition(Cs.Telekinesis, c);
                //胃液状态
                if (pass.HasCondition(Cs.GastroAcid)) o.SetCondition(Cs.GastroAcid);
                //扎根状态
                if (pass.HasCondition(Cs.Ingrain)) o.SetCondition(Cs.Ingrain);
                //液态圈状态
                if (pass.HasCondition(Cs.AquaRing)) o.SetCondition(Cs.AquaRing);
                //蓄气状态 
                if (pass.HasCondition(Cs.FocusEnergy)) o.SetCondition(Cs.FocusEnergy);
                //替身状态
                c = pass.GetCondition<object>(Cs.Substitute);
                if (c != null) o.SetCondition(Cs.Substitute, c);
                //电磁浮游状态
                c = pass.GetCondition<object>(Cs.MagnetRise);
                if (c != null) o.SetCondition(Cs.MagnetRise, c);
                //灭亡之歌状态
                c = pass.GetCondition<object>(Cs.PerishSong);
                if (c != null) o.SetCondition(Cs.PerishSong, c);
            }
            ATs.Illusion(pm);//幻影特性以交换前的队伍顺序决定
        }
        public static void Withdrawing(PokemonProxy pm, bool canPursuit)
        {
            if (canPursuit && pm.Hp != 0)
            {
                foreach (var p in pm.Controller.Board[1 - pm.Pokemon.TeamId].GetAdjacentPokemonsByOppositeX(pm.OnboardPokemon.X).ToArray())
                    if (p.SelectedMove != null && p.SelectedMove.MoveE.Id == Ms.PURSUIT && p.CanMove)
                    {
                        p.OnboardPokemon.SetCondition(Cs.Pursuiting);
                        p.Move();
                        p.OnboardPokemon.RemoveCondition(Cs.Pursuiting);
                        if (pm.Hp == 0) return;
                    }
            }
            foreach (var p in pm.Controller.Board.Pokemons)
                if (p != pm)
                {
                    var op = p.OnboardPokemon;
                    {
                        var o = op.GetCondition(Cs.CanAttack);
                        if (o != null && o.By == pm) op.RemoveCondition(Cs.CanAttack);
                    }
                    {
                        if (op.GetCondition<PokemonProxy>(Cs.CantSelectWithdraw) == pm) op.RemoveCondition(Cs.CantSelectWithdraw);
                    }
                    {
                        var o = op.GetCondition(Cs.HealBlock);
                        if (o != null && o.By == pm) op.RemoveCondition(Cs.HealBlock);
                    }
                    /*{
                        var o = op.GetCondition(Cs.SoundBlock);
                        if (o != null && o.By == pm) op.RemoveCondition(Cs.SoundBlock);
                    }*/
                    {
                        if (op.GetCondition<PokemonProxy>(Cs.Attract) == pm) op.RemoveCondition(Cs.Attract);
                    }
                    {
                        if (op.GetCondition<PokemonProxy>(Cs.Torment) == pm) op.RemoveCondition(Cs.Torment);
                    }
                    {
                        var o = op.GetCondition(Cs.Trap);
                        if (o != null && o.By == pm) op.RemoveCondition(Cs.Trap);
                    }
                }
        }
        public static bool SetWeather(PokemonProxy pm, Weather weather, bool ability, bool showFail)
        {
            var c = pm.Controller;
            if (c.Board.Weather == weather)
            {
                if (showFail) c.ReportBuilder.ShowLog(Ls.Fail0);
                return false;
            }
            if (ability) pm.RaiseAbility();
            switch (c.Board.GetCondition<int>(Cs.SpWeather))
            {
                case As.PRIMORDIAL_SEA:
                    c.ReportBuilder.ShowLog(Ls.HeavyRain2);
                    return false;
                case As.DESOLATE_LAND:
                    c.ReportBuilder.ShowLog(Ls.HarshSunlight2);
                    return false;
                case As.DELTA_STREAM:
                    c.ReportBuilder.ShowLog(Ls.MysteriousAirCurrent2);
                    return false;
                default:
                    c.ReportBuilder.ShowLog(weather == Weather.IntenseSunlight ? Ls.EnIntenseSunlight : weather == Weather.Rain ? Ls.EnRain : weather == Weather.Hailstorm ? Ls.EnHailstorm : Ls.EnSandstorm);
                    c.Weather = weather;
                    c.Board.SetCondition(Cs.Weather, (c.TurnNumber == 0 ? 1 : c.TurnNumber) + (pm.ItemE(weather.Item()) ? 7 : 4));
                    return true;
            }
        }
        public static bool Remaining1HP(PokemonProxy pm, bool ability)
        {
            if (pm.OnboardPokemon.HasCondition(Cs.Endure))
            {
                pm.ShowLogPm("Endure");
                return true;
            }
            if (ability && pm.Hp == pm.Pokemon.MaxHp && pm.RaiseAbility(As.STURDY))
            {
                pm.ShowLogPm("Endure");
                return true;
            }
            if (pm.ItemE(Is.FOCUS_BAND) && pm.Controller.OneNth(10) || pm.ItemE(Is.FOCUS_SASH) && pm.Hp == pm.Pokemon.MaxHp)
            {
                pm.ShowLogPm("FocusItem", pm.Pokemon.Item);
                if (pm.Pokemon.Item == Is.FOCUS_SASH) pm.ConsumeItem();
                return true;
            }
            return false;
        }
        public static void FocusPunch(Controller c)
        {
            if(c.Board.AddTurnCondition(Cs.FocusPunch))
            {
                foreach (PokemonProxy p in c.OnboardPokemons)
                    if (p.Action == PokemonAction.MoveAttached && p.SelectedMove.MoveE.Id == Ms.FOCUS_PUNCH) p.ShowLogPm("EnFocusPunch");
                foreach (PokemonProxy p in c.OnboardPokemons)
                    if (p.Action == PokemonAction.MoveAttached && p.SelectedMove.MoveE.Id == Ms.Shell_Trap) p.ShowLogPm("EnShellTrap");
                foreach (PokemonProxy p in c.OnboardPokemons)
                    if (p.Action == PokemonAction.MoveAttached && p.SelectedMove.MoveE.Id == Ms.Beak_Blast) p.ShowLogPm("EnBeak_Blast");
            }
            foreach (PokemonProxy p in c.OnboardPokemons)
                if(p.Action == PokemonAction.MoveAttached && p.SelectedMove.MoveE.Id == Ms.Beak_Blast)
                    p.OnboardPokemon.AddTurnCondition(Cs.Beak_Blast);
        }
        public static bool MagicCoat(AtkContext atk, PokemonProxy der)
        {
            //atk.Move.AdvancedFlags.MagicCoat is already checked
            if (der.OnboardPokemon.HasCondition(Cs.MagicCoat) || atk.DefenderAbilityAvailable() && der.AbilityE(As.MAGIC_BOUNCE))
            {
                var o = atk.GetCondition<List<PokemonProxy>>(Cs.MagicCoat);
                if (o == null)
                {
                    o = new List<PokemonProxy>();
                    atk.SetCondition(Cs.MagicCoat, o);
                }
                o.Add(der);
                return true;
            }
            return false;
        }
        public static bool CanExecuteMove(PokemonProxy pm, MoveTypeE move)
        {
            //重力
            if (move.UnavailableWithGravity && pm.Controller.Board.HasCondition(Cs.Gravity))
            {
                pm.ShowLogPm("GravityCantUseMove", move.Id);
                return false;
            }
            //回复封印
            if (move.Heal && pm.OnboardPokemon.HasCondition(Cs.HealBlock))
            {
                pm.ShowLogPm("HealBlockCantUseMove", move.Id);
                return false;
            }
            //地狱突刺
            if(move.Sound&&pm.OnboardPokemon.HasCondition(Cs.SoundBlock))
            {
                pm.ShowLogPm("SoundBlockCantUseMove", move.Id);
                return false;
            }
            return true;
        }
        public static Modifier AccuracyModifier(AtkContext atk)
        {
            var aer = atk.Attacker;
            int ab = aer.Ability;
            Modifier m = (Modifier)(ab == As.COMPOUNDEYES ? 0x14cc : ab == As.HUSTLE && atk.Move.Move.Category == MoveCategory.Physical ? 0xccc : 0x1000);
            foreach (PokemonProxy pm in atk.Controller.GetOnboardPokemons(aer.Pokemon.TeamId))
                if (pm.AbilityE(As.VICTORY_STAR)) m *= 0x1199;
            if (aer.OnboardPokemon.RemoveCondition(Cs.MicleBerry)) m *= 0x1199;
            if (aer.ItemE(Is.WIDE_LENS)) m *= 0x1199;
            return m;
        }
        #endregion
        public static void Lv7DDown(PokemonProxy pm)
        {
            switch (pm.Ability)
            {
                case As.DEFIANT:
                    pm.ChangeLv7D(pm, StatType.Atk, 2, false, true);
                    break;
                case As.COMPETITIVE:
                    pm.ChangeLv7D(pm, StatType.SpAtk, 2, false, true);
                    break;
            }
        }
    }
    internal static class SubstituteTriggers
    {
        public static bool IgnoreSubstitute(this AtkContext atk)
        {
            return atk.Move.IgnoreSubstitute || atk.Attacker.AbilityE(As.INFILTRATOR);
        }
        private static int Generic(DefContext def)
        {
            int hp = def.Defender.OnboardPokemon.GetCondition<int>(Cs.Substitute);
            def.HitSubstitute = hp > 0;
            return hp;
        }
        private static void Disappear(PokemonProxy pm)
        {
            pm.Controller.ReportBuilder.DeSubstitute(pm);
            pm.OnboardPokemon.RemoveCondition(Cs.Substitute);
        }
        public static bool Hurt(DefContext def)
        {
            int hp = Generic(def);
            if (hp > 0)
            {
                Controller c = def.Defender.Controller;
                if (def.Damage > hp) def.Damage = hp;
                hp -= def.Damage;
                def.Defender.ShowLogPm("HurtSubstitute");
                if (def.EffectRevise > 0) c.ReportBuilder.ShowLog("SuperHurt0");
                else if (def.EffectRevise < 0) c.ReportBuilder.ShowLog("WeakHurt0");
                if (def.IsCt) c.ReportBuilder.ShowLog("CT0");
                if (def.Defender.ItemE(Is.AIR_BALLOON)) ITs.AirBalloon(def);
                if (hp == 0) Disappear(def.Defender);
                else def.Defender.OnboardPokemon.SetCondition(Cs.Substitute, hp);
                return true;
            }
            return false;
        }
        public static bool OHKO(DefContext def)
        {
            int hp = Generic(def);
            if (hp > 0)
            {
                def.Damage = hp;
                Disappear(def.Defender);
                return true;
            }
            return false;
        }
    }
    internal static class EHTs
    {
        private static List<Condition> GetHazards(Field field)
        {
            return field.GetCondition<List<Condition>>(Cs.Hazards);
        }
        public static bool En(Field field, MoveTypeE move)
        {
            var hazards = GetHazards(field);
            if (hazards == null)
            {
                hazards = new List<Condition>();
                field.AddCondition(Cs.Hazards, hazards);
            }
            foreach (var eh in hazards)
                if (eh.Move == move) return En(eh);
            var newh = new Condition() { Move = move };
            if (move.Id == Ms.SPIKES) newh.Int = 8;
            hazards.Add(newh);
            return true;
        }
        private static bool En(Condition hazard)
        {
            switch (hazard.Move.Id)
            {
                case Ms.SPIKES:
                    if (hazard.Int == 4) return false;
                    hazard.Int = hazard.Int == 8 ? 6 : 4;
                    return true;
                case Ms.TOXIC_SPIKES:
                    if (hazard.Bool) return false;
                    hazard.Bool = true;
                    return true;
                default:
                    return false;
            }
        }
        public static void De(ReportBuilder report, Field field)
        {
            var hazards = GetHazards(field);
            if (hazards != null)
            {
                foreach (var eh in hazards.ToArray()) DeReport(report, eh, field);
                hazards.Clear();
            }
        }
        public static void De(ReportBuilder report, Field field, MoveTypeE move)
        {
            var hazards = GetHazards(field);
            if (hazards != null)
                foreach (var eh in hazards)
                    if (eh.Move == move)
                    {
                        hazards.Remove(eh);
                        DeReport(report, eh, field);
                        break;
                    }
        }
        private static void DeReport(ReportBuilder report, Condition hazard, Field field)
        {
            var m = hazard.Move.Id;
            report.ShowLog(m == Ms.SPIKES ? "DeSpikes" : m == Ms.TOXIC_SPIKES ? "DeToxicSpikes" : m == Ms.STEALTH_ROCK ? "DeStealthRock" : "DeStickyWeb", field.Team);
        }
        public static void Debut(PokemonProxy pm) //欢迎登场，口耐的精灵们（笑
        {
            var hazards = GetHazards(pm.Field);
            if (hazards != null)
                foreach (var eh in hazards.ToArray())
                {
                    Debut(eh, pm);
                    if (pm.CheckFaint()) return;
                }
        }
        private static void Debut(Condition hazard, PokemonProxy pm)
        {
            switch (hazard.Move.Id)
            {
                case Ms.SPIKES:
                    if (HasEffect.IsGroundAffectable(pm, true, false)) pm.EffectHurtByOneNth(hazard.Int, Ls.Spikes);
                    break;
                case Ms.TOXIC_SPIKES:
                    if (HasEffect.IsGroundAffectable(pm, true, false))
                        if (pm.OnboardPokemon.HasType(BattleType.Poison)) De(pm.Controller.ReportBuilder, pm.Field, hazard.Move);
                        else if (pm.CanAddState(pm, AttachedState.PSN, false)) pm.AddState(pm, AttachedState.PSN, false, hazard.Bool ? 15 : 0);
                    break;
                case Ms.STEALTH_ROCK:
                    int revise = BattleType.Rock.EffectRevise(pm.OnboardPokemon.Types);//羽栖有效无效都无所谓
                    int hp = (revise > 0 ? pm.Pokemon.MaxHp << revise : pm.Pokemon.MaxHp >> -revise) >> 3;
                    pm.EffectHurt(hp, Ls.StealthRock);
                    break;
                case Ms.STICKY_WEB:
                    if (HasEffect.IsGroundAffectable(pm, true, false))
                    {
                        pm.ShowLogPm("StickyWeb");
                        pm.ChangeLv7D(null, StatType.Speed, -1, false, false);
                    }
                    break;
            }
        }
    }
}
