using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    internal static class ATs
    {
        public static void RaiseAbility(this PokemonProxy pm)
        {
            pm.ShowLogPm(Ls.Ability, pm.OnboardPokemon.Ability);
        }
        public static bool RaiseAbility(this PokemonProxy pm, int abilityId)
        {
            if (pm.AbilityE(abilityId))
            {
                RaiseAbility(pm);
                return true;
            }
            return false;
        }
        public static bool RaiseAbility(this DefContext def, int abilityId)
        {
            if (def.AbilityE(abilityId))
            {
                RaiseAbility(def.Defender);
                return true;
            }
            return false;
        }

        public static bool DefenderAbilityAvailable(this AtkContext atk)
        {
            var pm = atk.Attacker;
            return !(pm.AbilityE(As.MOLD_BREAKER) || pm.AbilityE(As.TURBOBLAZE) || pm.AbilityE(As.TERAVOLT) || atk.Move.Move.Id == Ms.Moongeist_Beam || atk.Move.Move.Id == Ms.Sunsteel_Strike );
        }
        public static bool CannotBeCted(this DefContext def)
        {
            return def.AbilityE(As.BATTLE_ARMOR) || def.AbilityE(As.SHELL_ARMOR);
        }

        public static bool IgnoreWeather(Controller c)
        {
            foreach (PokemonProxy p in c.OnboardPokemons)
                if (p.AbilityE(As.AIR_LOCK) || p.AbilityE(As.CLOUD_NINE)) return true;
            return false;
        }
        public static void Pressure(AtkContext atk, MoveRange range)
        {
            var ts =
              atk.Move.Move.Range == MoveRange.Board || atk.Move.Move.Range == MoveRange.OpponentField ?
              atk.Attacker.Controller.Board[1 - atk.Attacker.Pokemon.TeamId].Pokemons :
              atk.Targets == null ?
              Enumerable.Empty<PokemonProxy>() : null;
            if (ts == null)
            {
                foreach (var d in atk.Targets)
                    if (d.Defender.Pokemon.TeamId != atk.Attacker.Pokemon.TeamId && d.Defender.AbilityE(As.PRESSURE)) atk.Pressure++;
            }
            else
                foreach (var d in ts)
                    if (d.AbilityE(As.PRESSURE)) atk.Pressure++;
        }
        public static void Withdrawn(PokemonProxy pm, int ability)
        {
            switch (ability)
            {
                case As.REGENERATOR:
                    if (pm.Hp != 0) pm.Pokemon.Hp += pm.Pokemon.MaxHp / 3;
                    break;
                case As.NATURAL_CURE:
                    if (pm.Hp != 0) pm.Pokemon.State = PokemonState.Normal;
                    break;
                case As.UNNERVE:
                    foreach (var p in pm.Controller.GetOnboardPokemons(1 - pm.Pokemon.TeamId)) ITs.Attach(p);
                    break;
                case As.PRIMORDIAL_SEA:
                    DeSpWeather(pm, ability, Ls.DeHeavyRain);
                    break;
                case As.DESOLATE_LAND:
                    DeSpWeather(pm, ability, Ls.DeHarshSunlight);
                    break;
                case As.DELTA_STREAM:
                    DeSpWeather(pm, ability, Ls.DeMysteriousAirCurrent);
                    break;
            }
        }
        public static void Synchronize(PokemonProxy pm, PokemonProxy by, AttachedState state, int turn)
        {
            if (pm != by && pm.RaiseAbility(As.SYNCHRONIZE)) by.AddState(pm, state, true, turn);
        }
        public static void Illusion(PokemonProxy pm)
        {
            if (pm.AbilityE(As.ILLUSION))
                foreach (var p in pm.Pokemon.Owner.Pokemons.Reverse())
                    if (p != pm && p.Hp > 0)
                    {
                        pm.OnboardPokemon.SetCondition(Cs.Illusion, p.Pokemon);
                        break;
                    }
        }
        public static void ColorChange(DefContext def)
        {
            var type = def.AtkContext.Type;
            if (type == BattleType.Invalid) type = BattleType.Normal;
            if (!def.HitSubstitute) // performance
            {
                var dt = def.Defender.OnboardPokemon.Types;
                if (!(dt.First() == type && dt.Last() == type) && def.Defender.RaiseAbility(As.COLOR_CHANGE))
                {
                    def.Defender.OnboardPokemon.SetTypes(type);
                    def.Defender.ShowLogPm("TypeChange", (int)type);
                }
            }
        }
        public static double WeightModifier(PokemonProxy pm)
        {
            double m;
            int id = pm.Ability;
            if (id == As.HEAVY_METAL) m = 2d;
            else if (id == As.LIGHT_METAL) m = 0.5d;
            else m = 1d;
            return m;
        }
        public static bool Stench(DefContext def)
        {
            return def.AtkContext.Attacker.AbilityE(As.STENCH) && def.Defender.Controller.RandomHappen(10);
        }
        public static bool Trace(int abilityId)
        {
            return !(abilityId == As.FORECAST || abilityId == As.ILLUSION || abilityId == As.ZEN_MODE || abilityId == As.Shields_Down || abilityId == As.MULTITYPE || abilityId == As.TRACE || abilityId == As.Schooling || abilityId == As.Power_Construct || abilityId==As.Battle_Bond || abilityId==As.RKS_System || abilityId==As.Comatose );
        }
        public static void Trace(PokemonProxy sendout)
        {
            int ab = sendout.OnboardPokemon.Ability;
            if (Trace(ab))
                foreach (var pm in sendout.Controller.Board[1 - sendout.Pokemon.TeamId].GetAdjacentPokemonsByOppositeX(sendout.OnboardPokemon.X))
                    if (pm.RaiseAbility(As.TRACE))
                    {
                        pm.ChangeAbility(sendout.OnboardPokemon.Ability);
                        pm.Controller.ReportBuilder.ShowLog("Trace", sendout.Id, sendout.OnboardPokemon.Ability);
                    }
        }
        public static bool Gluttony(PokemonProxy pm)
        {
            return pm.Hp << 2 <= pm.Pokemon.MaxHp || pm.AbilityE(As.GLUTTONY) && pm.Hp << 1 <= pm.Pokemon.MaxHp;
        }

        internal static void SlowStart(Controller Controller)
        {
            foreach (var pm in Controller.OnboardPokemons)
                if (pm.AbilityE(As.SLOW_START))
                {
                    int turn = pm.OnboardPokemon.GetCondition<int>(Cs.SlowStart);
                    if (turn == Controller.TurnNumber)
                    {
                        pm.OnboardPokemon.RemoveCondition(Cs.SlowStart);
                        pm.ShowLogPm("DeSlowStart");
                    }
                }
        }
        internal static void RecoverAfterMoldBreaker(PokemonProxy pm)
        {
            int id = pm.Ability;
            if (id == As.LIMBER || id == As.OBLIVIOUS || id == As.IMMUNITY || id == As.INSOMNIA || id == As.OWN_TEMPO || id == As.MAGMA_ARMOR || id == As.WATER_VEIL || id == As.VITAL_SPIRIT || id == As.Water_Bubble)
                AbilityAttach.Execute(pm);
        }
        internal static bool Unnerve(PokemonProxy pm)
        {
            return pm.OnboardPokemon.HasCondition(Cs.Unnerve) && pm.AbilityE(As.UNNERVE);
        }
        internal static void AttachUnnerve(Controller c)
        {
            foreach (var pm in c.OnboardPokemons)
                if (!pm.OnboardPokemon.HasCondition(Cs.Unnerve) && pm.AbilityE(As.UNNERVE))
                {
                    pm.OnboardPokemon.SetCondition(Cs.Unnerve);
                    pm.RaiseAbility();
                    pm.Controller.ReportBuilder.ShowLog("Unnerve", 1 - pm.Pokemon.TeamId);
                }
        }
        internal static void AttachWeatherObserver(PokemonProxy pm)
        {
            var a = pm.OnboardPokemon.Ability;
            if (a == As.FORECAST || a == As.FLOWER_GIFT) pm.OnboardPokemon.SetCondition(Cs.ObserveWeather);
        }

        public static void WeatherChanged(Controller c)
        {
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.HasCondition(Cs.ObserveWeather))
                {
                    var ab = pm.Ability;
                    if (ab == As.FORECAST || ab == As.FLOWER_GIFT) AbilityAttach.Execute(pm);
                }
        }
        public static void DeIllusion(PokemonProxy pm)
        {
            if (pm.OnboardPokemon.RemoveCondition(Cs.Illusion))
            {
                pm.Controller.ReportBuilder.DeIllusion(pm);
                pm.ShowLogPm("DeIllusion");
            }
        }
        public static void StanceChange(PokemonProxy pm)
        {
            if (pm.SelectedMove.MoveE.Id == Ms.KINGS_SHIELD)
            {
                if (pm.CanChangeForm(681, 0) && RaiseAbility(pm, As.STANCE_CHANGE)) pm.ChangeForm(0, false, "StanceChangeShield");
            }
            else if (pm.SelectedMove.MoveE.Move.Category != MoveCategory.Status && pm.CanChangeForm(681, 1) && RaiseAbility(pm, As.STANCE_CHANGE)) pm.ChangeForm(1, false, "StanceChangeSword");
        }
        public static void DeSpWeather(PokemonProxy pm, int ability, string log)
        {
            var c = pm.Controller;
            if (c.Board.GetCondition<int>(Cs.SpWeather) == ability && c.Board.Pokemons.HasAbilityExcept(ability,pm) == null)
            {
                c.ReportBuilder.ShowLog(log);
                c.Board.RemoveCondition(Cs.SpWeather);
                c.Weather = Weather.Normal;
            }
        }
    }
}
