using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class PowerModifier
    {
        public static Modifier Execute(DefContext def)
        {
            var der = def.Defender;
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var c = def.Defender.Controller;

            Modifier m = Abilities(def);

            m *= AttackerItem(atk);

            m *= Move(def);
            //If user used Charge the previous turn and move is Electric type.
            if (atk.Type == BattleType.Electric && atk.Attacker.OnboardPokemon.GetCondition<int>(Cs.Charge) == c.TurnNumber) m *= 0x2000;
            //If user has been the target of Helping Hand this turn.
            var hh = aer.OnboardPokemon.GetCondition<int>(Cs.HelpingHand);
            if (hh > 0) m *= 0x1800;
            if (hh > 1) m *= 0x1800;
            //If Water Sport was used by any Pokémon still on the field and move is Fire type.
            //If Mud Sport was used by any Pokémon still on the field and move is Electric type.
            if ((atk.Type == BattleType.Fire && c.Board.HasCondition(Cs.WaterSport)) ||
              (atk.Type == BattleType.Electric && c.Board.HasCondition(Cs.MudSport)))
                m *= 0x800;

            if ((atk.Type == BattleType.Electric && c.Board.HasCondition(Cs.ElectricTerrain) || atk.Type == BattleType.Grass && c.Board.HasCondition(Cs.GrassyTerrain) || atk.Type==BattleType.Psychic&&c.Board.HasCondition(Cs.PsychicTerrain)) && HasEffect.IsGroundAffectable(aer, true, false)) m *= 0x1800;
            else if (atk.Type == BattleType.Dragon && c.Board.HasCondition(Cs.MistyTerrain) && HasEffect.IsGroundAffectable(der, true, false)) m *= 0x800;

            return m;
        }
        private static Modifier Abilities(DefContext def)
        {
            var der = def.Defender;
            var atk = def.AtkContext;
            var move = atk.Move;
            var aer = atk.Attacker;
            var type = atk.Type;

            Modifier m = 0x1000;

            if (atk.HasCondition(Cs.Sukin)) m *= 0x1333;
            else
                switch (aer.Ability)
                {
                    case As.RIVALRY:
                        {
                            var u = aer.OnboardPokemon.Gender;
                            var t = der.OnboardPokemon.Gender;
                            if (u != PokemonGender.None && t != PokemonGender.None)
                                m *= (Modifier)(u == t ? 0x1400 : 0xc00);
                        }
                        break;
                    case As.IRON_FIST:
                        if (move.Fist) m *= 0x1333;
                        break;
                    case As.TECHNICIAN:
                        if (def.BasePower <= 60) m *= 0x1800;
                        break;
                    case As.RECKLESS:
                        if (move.HurtPercentage < 0 || move.Id == Ms.JUMP_KICK || move.Id == Ms.HIGH_JUMP_KICK) m *= 0x1333;
                        break;
                    case As.TOXIC_BOOST:
                        if ((aer.State == PokemonState.PSN || aer.State == PokemonState.BadlyPSN) && move.Move.Category == MoveCategory.Physical) m *= 0x1800;
                        break;
                    case As.FLARE_BOOST:
                        if (move.Move.Category == MoveCategory.Special && aer.State == PokemonState.BRN) m *= 0x1800;
                        break;
                    case As.ANALYTIC:
                        {
                            var s = true;
                            var turn = aer.LastMoveTurn;
                            foreach (var p in aer.Controller.ActingPokemons)
                                if (p.LastMoveTurn != turn)
                                {
                                    s = false;
                                    break;
                                }
                            if (s) m *= 0x14cd;
                        }
                        break;
                    case As.SHEER_FORCE:
                        if (atk.Move.HasProbabilitiedAdditonalEffects) m *= 0x14cd;
                        break;
                    case As.SAND_FORCE:
                        if ((type == BattleType.Rock || type == BattleType.Ground || type == BattleType.Steel) && aer.Controller.Weather == Weather.Sandstorm) m *= 0x14cd;
                        break;
                    case As.STRONG_JAW:
                        if (move.Teeth) m *= 0x1800;
                        break;
                    case As.TOUGH_CLAWS:
                        if (move.NeedTouch) m *= 0x14cd;
                        break;
                    case As.MEGA_LAUNCHER:
                        if (move.Pulse) m *= 0x1800;
                        break;
                    //gen7
                    case As.Stakeoutt:
                        if (der.OnboardPokemon.HasCondition(Cs.Switched)) m *= 0x2000;
                        break;
                    case As.Water_Bubble:
                        if (type == BattleType.Water) m *= 0x2000;
                        break;
                    case As.steelworker:
                        if (type == BattleType.Steel) m *= 0x1800;
                        break;
                }

            //如果防御方是耐热特性，攻击方火属性技能威力×0.5。
            //如果防御方是干燥肌肤特性，攻击方火属性技能威力×1.25。 
            if (type == BattleType.Fire)
            {
                int id = def.Ability;
                if (id == As.HEATPROOF || id == As.Water_Bubble) m *= 0x800;
                else if (id == As.DRY_SKIN) m *= 0x1800;
                else if (id == As.Fluffy) m *= 0x2000;
            }

            if (atk.touch && der.Ability == As.Fluffy)
                m *= 0x800;

            if (type == BattleType.Dark || type == BattleType.Fairy)
            {
                var a = type == BattleType.Dark ? As.DARK_AURA : As.FAIRY_AURA;
                if (aer.Controller.Board.Pokemons.HasAbility(a) != null) m *= (Modifier)(aer.Controller.Board.Pokemons.HasAbility(As.AURA_BREAK) == null ? 0x1555 : 0xc00);
            }

            return m;
        }

        private static Modifier AttackerItem(AtkContext atk)
        {
            if (atk.HasCondition(Cs.Gem)) return 0x1800;
            switch (atk.Attacker.Item)
            {
                case Is.GRISEOUS_ORB:
                    return Orb(atk, 487, BattleType.Ghost);
                case Is.ADAMANT_ORB:
                    return Orb(atk, 483, BattleType.Steel);
                case Is.LUSTROUS_ORB:
                    return Orb(atk, 484, BattleType.Water);
                case Is.SILVERPOWDER:
                case Is.INSECT_PLATE:
                    return TypeItem(atk, BattleType.Bug);
                case Is.METAL_COAT:
                case Is.IRON_PLATE:
                    return TypeItem(atk, BattleType.Steel);
                case Is.SOFT_SAND:
                case Is.EARTH_PLATE:
                    return TypeItem(atk, BattleType.Ground);
                case Is.HARD_STONE:
                case Is.STONE_PLATE:
                case Is.ROCK_INCENSE:
                    return TypeItem(atk, BattleType.Rock);
                case Is.MIRACLE_SEED:
                case Is.MEADOW_PLATE:
                case Is.ROSE_INCENSE:
                    return TypeItem(atk, BattleType.Grass);
                case Is.BLACKGLASSES:
                case Is.DREAD_PLATE:
                    return TypeItem(atk, BattleType.Dark);
                case Is.BLACK_BELT:
                case Is.FIST_PLATE:
                    return TypeItem(atk, BattleType.Fighting);
                case Is.MAGNET:
                case Is.ZAP_PLATE:
                    return TypeItem(atk, BattleType.Electric);
                case Is.MYSTIC_WATER:
                case Is.SPLASH_PLATE:
                case Is.WAVE_INCENSE:
                case Is.SEA_INCENSE:
                    return TypeItem(atk, BattleType.Water);
                case Is.SHARP_BEAK:
                case Is.SKY_PLATE:
                    return TypeItem(atk, BattleType.Flying);
                case Is.POISON_BARB:
                case Is.TOXIC_PLATE:
                    return TypeItem(atk, BattleType.Poison);
                case Is.NEVERMELTICE:
                case Is.ICICLE_PLATE:
                    return TypeItem(atk, BattleType.Ice);
                case Is.SPELL_TAG:
                case Is.SPOOKY_PLATE:
                    return TypeItem(atk, BattleType.Ghost);
                case Is.TWISTEDSPOON:
                case Is.MIND_PLATE:
                case Is.ODD_INCENSE:
                    return TypeItem(atk, BattleType.Psychic);
                case Is.CHARCOAL:
                case Is.FLAME_PLATE:
                    return TypeItem(atk, BattleType.Fire);
                case Is.DRAGON_FANG:
                case Is.DRACO_PLATE:
                    return TypeItem(atk, BattleType.Dragon);
                case Is.PIXIE_PLATE:
                    return TypeItem(atk, BattleType.Fairy);
                case Is.SILK_SCARF:
                    return TypeItem(atk, BattleType.Normal);
                case Is.MUSCLE_BAND:
                    return Category(atk, MoveCategory.Physical);
                case Is.WISE_GLASSES:
                    return Category(atk, MoveCategory.Special);
                default:
                    return 0x1000;
            }
        }
        private static Modifier TypeItem(AtkContext atk, BattleType type)
        {
            return (Modifier)(atk.Type == type ? 0x1333 : 0x1000);
        }
        private static Modifier Orb(AtkContext atk, int pm, BattleType type)
        {
            return (Modifier)(atk.Attacker.Pokemon.Form.Species.Number == pm && (atk.Type == BattleType.Dragon || atk.Type == type) ? 0x1333 : 0x1000);
        }
        private static Modifier Category(AtkContext atk, MoveCategory category)
        {
            return (Modifier)(atk.Move.Move.Category == category ? 0x1199 : 0x1000);
        }

        private static Modifier Move(DefContext def)
        {
            var der = def.Defender;
            var atk = def.AtkContext;
            var move = atk.Move.Id;
            Modifier m = 0x1000;

            switch (move)
            {
                case Ms.FUSION_FLARE: //558
                    {
                        var b = der.Controller.Board;
                        b.SetTurnCondition(Cs.Fusion);
                        var o = b.GetCondition(Cs.LastMove);
                        if (o != null && o.Move.Id == Ms.FUSION_BOLT && b.HasCondition(Cs.Fusion)) m = 0x2000;
                    }
                    break;
                case Ms.FUSION_BOLT: //559
                    {
                        var b = der.Controller.Board;
                        b.SetTurnCondition(Cs.Fusion);
                        var o = b.GetCondition(Cs.LastMove);
                        if (o != null && o.Move.Id == Ms.FUSION_FLARE && b.HasCondition(Cs.Fusion)) m = 0x2000;
                    }
                    break;
                case Ms.FACADE: //263
                    if (atk.Attacker.State != PokemonState.Normal && atk.Attacker.State != PokemonState.FRZ) m = 0x2000;
                    break;
                case Ms.BRINE: //362
                    if (der.Hp << 1 <= der.Pokemon.MaxHp) m = 0x2000;
                    break;
                case Ms.VENOSHOCK: //474
                    if (atk.Attacker.State == PokemonState.PSN && atk.Attacker.State == PokemonState.BadlyPSN) m = 0x2000;
                    break;
                case Ms.RETALIATE: //514
                    if (atk.Attacker.Field.GetCondition<int>(Cs.FaintTurn) == der.Controller.TurnNumber - 1) m = 0x2000;
                    break;
            }
            if (atk.HasCondition(Cs.MeFirst)) m *= 0x1800;
            switch (move)
            {
                case Ms.SOLAR_BEAM:
                case Ms.Solar_Blade:
                    Weather w = def.Defender.Controller.Weather;
                    if (w != Weather.IntenseSunlight && w != Weather.Normal) m *= 0x800;
                    break;
                case Ms.EARTHQUAKE:
                case Ms.BULLDOZE:
                case Ms.MAGNITUDE:
                    if (der.Controller.Board.HasCondition(Cs.GrassyTerrain)) m *= 0x800;
                    break;
            }

            return m;
        }
    }
}
