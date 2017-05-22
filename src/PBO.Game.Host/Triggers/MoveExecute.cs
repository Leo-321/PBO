using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class MoveExecute
    {
        public static void Execute(AtkContext atk,Tile selectTile = null)
        {
            var move = atk.Move.Id;
            var aer = atk.Attacker;

            if (MoveNotFail.Execute(atk))
                switch (move)
                {
                    case Ms.METRONOME: //118
                        Metronome(atk);
                        break;
                    case Ms.MIRROR_MOVE: //119
                        MirrorMove(atk);
                        break;
                    case Ms.SLEEP_TALK: //214
                        SleepTalk(atk);
                        break;
                    case Ms.NATURE_POWER: //267
                        NaturePower(atk,selectTile);
                        break;
                    case Ms.ASSIST: //274
                        Assist(atk);
                        break;
                    case Ms.SNATCH: //289
                        Snatch(atk);
                        break;
                    case Ms.ME_FIRST: //382
                        MeFirst(atk);
                        break;
                    case Ms.COPYCAT: //383
                        Copycat(atk);
                        break;
                    case Ms.Instruct:
                        Instruct(atk);
                        goto default;
                    default:
                        atk.ImplementPressure();
                        Generic(atk);
                        if (atk.Fail)
                            switch (move)
                            {
                                case Ms.JUMP_KICK:
                                case Ms.HIGH_JUMP_KICK:
                                    if (aer.EffectHurtByOneNth(2, Ls.FailSelfHurt)) aer.CheckFaint();
                                    break;
                                case Ms.SELFDESTRUCT:
                                case Ms.EXPLOSION:
                                    aer.Faint();
                                    break;
                                case Ms.NATURAL_GIFT: //363
                                case Ms.FLING:
                                    aer.ConsumeItem();
                                    break;
                            }
                        break;
                }
            else atk.ImplementPressure();
            if (atk.Move.Id == Ms.RAGE && aer != null) aer.OnboardPokemon.SetCondition(Cs.Rage);
        }

        private static void Generic(AtkContext atk)
        {
            var move = atk.Move;
            var aer = atk.Attacker;

            if (move.PrepareOneTurn && PrepareOneTurn(atk)) return;

            if (aer.Controller.GameSettings.Mode.XBound() != 1 && (move.Id == Ms.WATER_PLEDGE || move.Id == Ms.FIRE_PLEDGE || move.Id == Ms.GRASS_PLEDGE))
                if (aer.Field.AddTurnCondition(Cs.Plege, move.Id))
                {
                    foreach (var pm in aer.Controller.ActingPokemons)
                        if (pm.Pokemon.TeamId == aer.Pokemon.TeamId && pm.SelectedMove != null &&
                            pm.SelectedMove.MoveE.Id != move.Id && (pm.SelectedMove.MoveE.Id == Ms.WATER_PLEDGE || pm.SelectedMove.MoveE.Id == Ms.FIRE_PLEDGE || pm.SelectedMove.MoveE.Id == Ms.GRASS_PLEDGE) &&
                            pm.CanMove)
                        {
                            aer.ShowLogPm(Ls.Pledge, pm.Id);
                            aer.Controller.Board.SetTurnCondition(Cs.NextActingPokemon, pm);
                            return;
                        }
                }

            if (move.Snatchable)
                foreach (var pm in atk.Controller.OnboardPokemons)
                    if (pm.OnboardPokemon.HasCondition(Cs.Snatch))
                    {
                        pm.OnboardPokemon.RemoveCondition(Cs.Snatch);
                        pm.ShowLogPm("Snatch", aer.Id);
                        var s = new AtkContext(pm) { Move = move };
                        InitAtkContext.Execute(s);
                        MoveE.BuildDefContext(s, null);
                        if (MoveNotFail.Execute(s)) MoveAct.Execute(s);
                        else s.FailAll();
                        atk.SetAttackerAction(PokemonAction.Done);
                        return;
                    }

            if (move.MagicCoat && atk.Targets == null && !atk.HasCondition(Cs.IgnoreMagicCoat))
                foreach (var p in aer.Controller.GetOnboardPokemons(1 - aer.Pokemon.TeamId))
                    if (STs.MagicCoat(atk, p))
                    {
                        atk.SetCondition(Cs.MagicCoat, new List<PokemonProxy>() { p });
                        atk.FailAll(null);
                        MoveE.MagicCoat(atk);
                        return;
                    }

            CalculateType.Execute(atk);

            if (atk.Type == BattleType.Fire && aer.OnboardPokemon.HasCondition(Cs.Powder))
            {
                aer.EffectHurtByOneNth(4, Ls.Powder);
                atk.FailAll(null);
                return;
            }
            if (atk.Move.Move.Category != MoveCategory.Status)
            {
                if (atk.Type == BattleType.Fire && aer.Controller.Board.GetCondition<int>(Cs.SpWeather) == As.PRIMORDIAL_SEA)
                {
                    atk.FailAll(Ls.HeavyRain);
                    return;
                }
                if (atk.Type == BattleType.Water && aer.Controller.Board.GetCondition<int>(Cs.SpWeather) == As.DESOLATE_LAND)
                {
                    atk.FailAll(Ls.HarshSunlight);
                    return;
                }
            }
            if (aer.AbilityE(As.PROTEAN) && aer.OnboardPokemon.SetTypes(atk.Type))
            {
                aer.RaiseAbility();
                aer.ShowLogPm("TypeChange", (int)atk.Type);
            }

            MoveE.FilterDefContext(atk);
            if (atk.Targets != null && atk.Target == null) atk.FailAll(null);
            else
            {
                MoveAct.Execute(atk);
                MoveE.MoveEnding(atk);
            }

            if (move.MagicCoat && atk.Targets != null) MoveE.MagicCoat(atk);
        }

        private static void Snatch(AtkContext atk)
        {
            atk.Attacker.OnboardPokemon.SetTurnCondition(Cs.Snatch);
            atk.Attacker.ShowLogPm("EnSnatch");
            atk.SetAttackerAction(PokemonAction.Done);
        }

        private static readonly int[] ASSIST_BLOCK = { 166, 274, 118, 165, 271, 415, 214, 382, 448, 68, 243, 194, 182, 197, 203, 364, 264, 266, 476, 270, 383, 119, 289, 525, 509, 144, Ms.NATURE_POWER, Ms.THIEF, Ms.COVET, Ms.MIMIC };
        private static void Assist(AtkContext atk)
        {
            var aer = atk.Attacker;
            var moves = new List<MoveTypeE>();
            foreach (var pm in aer.Pokemon.Owner.Pokemons)
                foreach (var m in pm.Pokemon.Moves)
                    if (pm != aer && !(ASSIST_BLOCK.Contains(m.Type.Id))) moves.Add(MoveTypeE.Get(m.Type));
            if (moves.Count == 0) atk.FailAll();
            else atk.StartExecute(moves[aer.Controller.GetRandomInt(0, moves.Count - 1)]);
        }

        private static void NaturePower(AtkContext atk,Tile selectTile = null)
        {
            MoveTypeE m;
            switch (atk.Controller.GameSettings.Terrain)
            {
                case Terrain.Path:
                    m = MoveTypeE.Get(Ms.TRI_ATTACK);
                    break;
                default:
                    atk.Controller.ReportBuilder.ShowLog("error");
                    return;
            }
            atk.StartExecute(m, selectTile, "NaturePower");
        }

        private static readonly int[] SLEEPTALK_BLOCK = new int[] { 214, 117, 448, 253, 383, 119, 382, 264 };
        private static void SleepTalk(AtkContext atk)
        {
            var aer = atk.Attacker;
            var moves = new List<MoveTypeE>();
            foreach (var m in aer.Moves)
                if (!(m.MoveE.PrepareOneTurn || SLEEPTALK_BLOCK.Contains(m.MoveE.Id))) moves.Add(m.MoveE);
            var n = moves.Count;
            if (n == 0) atk.FailAll();
            else atk.StartExecute(moves[aer.Controller.GetRandomInt(0, moves.Count - 1)]);
        }

        private static readonly int[] METRONOME_BLOCK = new int[] { 68, 102, 118, 119, 144, 165, 166, 168, 173, 182, 194, 197, 203, 214, 243, 264, 266, 267, 270, 271, 274, 289, 343, 364, 382, 383, 415, 448, 469, 476, 495, 501, 511, Ms.BESTOW, Ms.TECHNO_BLAST, Ms.RELIC_SONG, Ms.SECRET_SWORD, Ms.FREEZE_SHOCK, Ms.ICE_BURN, Ms.VCREATE, Ms.CELEBRATE, Ms.DIAMOND_STORM, Ms.HAPPY_HOUR, Ms.HOLD_HANDS, Ms.HYPERSPACE_HOLE, Ms.STEAM_ERUPTION, Ms.THOUSAND_ARROWS, Ms.THOUSAND_WAVES, Ms.LANDS_WRATH, Ms.LIGHT_OF_RUIN, Ms.ORIGIN_PULSE, Ms.PRECIPICE_BLADES, Ms.DRAGON_ASCENT, Ms.HYPERSPACE_FURY };
        private static void Metronome(AtkContext atk)
        {
            LOOP:
            var m = MoveTypeE.Get(atk.Controller.GetRandomInt(1, RomData.MOVES));
            if (METRONOME_BLOCK.Contains(m.Id) || m.Zmove ) goto LOOP;
            atk.StartExecute(m, null, "Metronome");
        }

        private static readonly int[] COPYCAT_BLOCK = new int[] { 182, 197, 383, 102, 166, 271, 415, 509, 525, 194, 364, 264, 165 };
        private static void Copycat(AtkContext atk)
        {
            var o = atk.Controller.Board.GetCondition(Cs.LastMove);
            if (o == null || COPYCAT_BLOCK.Contains(o.Move.Id)) atk.FailAll();
            else atk.StartExecute(o.Move);
        }

        private static void MeFirst(AtkContext atk)
        {
            var der = atk.Target.Defender;
            var m = der.SelectedMove.MoveE;
            if (der.OnboardPokemon.HasCondition(Cs.SkyDrop) || m.Id == Ms.STRUGGLE || m.Id == Ms.FOCUS_PUNCH || m.Id == Ms.Shell_Trap || m.Zmove) atk.FailAll();
            else
            {
                atk.SetTurnCondition(Cs.MeFirst);
                atk.StartExecute(m, der.Tile);
            }
        }

        private static void MirrorMove(AtkContext atk)
        {
            var last = atk.Target.Defender.AtkContext;
            if(last!=null&&last.Move!=null&& last.Move.Mirrorable)
                atk.StartExecute(last.Move, atk.Target.Defender.Tile);
            atk.FailAll();
        }

        private static void Instruct(AtkContext atk)
        {
            var last = atk.Target.Defender.AtkContext;
            if(last!=null&&last.Move!=null&&last.Move.Mirrorable)
                last.StartExecute(last.Move, last.Attacker.SelectedTarget, "UseMove", false);
            else atk.FailAll();
        }

        private static bool PrepareOneTurn(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.Action == PokemonAction.MoveAttached)
            {
                var m = atk.Move.Id;
                switch (m)
                {
                    case Ms.FLY: //19
                    case Ms.BOUNCE: //340
                        aer.CoordY = CoordY.Air;
                        break;
                    case Ms.DIG: //91
                        aer.CoordY = CoordY.Underground;
                        break;
                    case Ms.DIVE: //291
                        aer.CoordY = CoordY.Water;
                        break;
                    case Ms.SHADOW_FORCE: //467
                    case Ms.PHANTOM_FORCE:
                        aer.CoordY = CoordY.Another;
                        break;
                    case Ms.SKY_DROP:
                        MoveE.FilterDefContext(atk);
                        if (atk.Target == null) atk.FailAll(null);
                        else
                        {
                            aer.CoordY = CoordY.Air;
                            atk.Target.Defender.CoordY = CoordY.Air;
                            atk.Target.Defender.OnboardPokemon.SetCondition(Cs.SkyDrop);
                            aer.ShowLogPm(Ls.EnSkyDrop, atk.Target.Defender.Id);
                            atk.SetAttackerAction(PokemonAction.Moving);
                        }
                        return true;
                }
                aer.ShowLogPm("Prepare" + m.ToString());
                if (m == Ms.SKULL_BASH) aer.ChangeLv7D(atk.Attacker, StatType.Def, 1, false);
                atk.SetAttackerAction(PokemonAction.Moving);
                return !((m == Ms.SOLAR_BEAM || m == Ms.Solar_Blade) && aer.Controller.Weather == Weather.IntenseSunlight || ITs.PowerHerb(aer));
            }
            return false;
        }
    }
}
