using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class MoveNotFail
    {
        /// <summary>
        /// contains battle log
        /// </summary>
        /// <param name="atk"></param>
        /// <returns></returns>
        public static bool Execute(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (atk.Targets == null || atk.Target != null)
                switch (atk.Move.Id)
                {
                    case Ms.SKETCH: //166
                    case Ms.TELEPORT:
                    case Ms.HAPPY_HOUR:
                        break;
                    case Ms.SELFDESTRUCT: //120
                    case Ms.EXPLOSION:
                        if (aer.Controller.Board.Pokemons.RaiseAbility(As.DAMP) != null)
                        {
                            atk.FailAll("FailSp", atk.Attacker.Id, atk.Move.Id);
                            return false;
                        }
                        return true;
                    case Ms.REST: //156
                        foreach (var p in aer.Controller.OnboardPokemons)
                            if (p.Action == PokemonAction.Moving && p.AtkContext.Move.Id == Ms.UPROAR)
                            {
                                atk.FailAll(null);
                                aer.ShowLogPm("UproarCantSLP");
                                return false;
                            }
                        if (aer.Hp == aer.Pokemon.MaxHp)
                        {
                            atk.FailAll("FullHp", aer.Id);
                            return false;
                        }
                        if (!PTs.CanAddXXX(aer, aer, true, AttachedState.SLP, true))
                        {
                            atk.FailAll(null);
                            return false;
                        }
                        return true;
                    case Ms.SNORE: //173
                    case Ms.SLEEP_TALK:
                        if (aer.State == PokemonState.SLP) return true;
                        break;
                    case Ms.FAKE_OUT: //252
                    case Ms.MAT_BLOCK:
                    case Ms.First_Impression:
                        foreach (var m in aer.Moves)
                            if (m.HasUsed) goto FAIL;
                        return true;
                    case Ms.STOCKPILE: //254
                        if (aer.OnboardPokemon.GetCondition<int>(Cs.Stockpile) != 3) return true;
                        break;
                    case Ms.SPIT_UP: //255
                    case Ms.SWALLOW:
                        if (aer.OnboardPokemon.HasCondition(Cs.Stockpile)) return true;
                        break;
                    case Ms.NATURAL_GIFT: //363
                        if (ITs.CanLostItem(aer) && ITs.CanUseItem(aer) && ITs.Berry(aer.Pokemon.Item)) return true;
                        break;
                    case Ms.FLING: //374
                        if (ITs.CanLostItem(aer) && ITs.CanUseItem(aer) && MoveBasePower.FlingPower(aer.Pokemon.Item) != 0) return true;
                        break;
                    case Ms.ME_FIRST: //382
                    case Ms.SUCKER_PUNCH: //389
                        {
                            var der = atk.Target.Defender;
                            var dm = der.SelectedMove;
                            if (!(der.LastMoveTurn == der.Controller.TurnNumber || dm == null || dm.Move.Type.Category == MoveCategory.Status)) return true;
                        }
                        break;
                    case Ms.LAST_RESORT: //387
                        foreach (var m in aer.Moves)
                            if (!m.HasUsed && m.MoveE.Id != Ms.LAST_RESORT) goto FAIL;
                        return true;
                    case Ms.ALLY_SWITCH:
                        if (aer.Controller.GameSettings.Mode == GameMode.Double || aer.Controller.GameSettings.Mode == GameMode.Triple && aer.OnboardPokemon.X != 1) return true;
                        break;
                    case Ms.BESTOW: //516
                        if (aer.Pokemon.Item == 0 || ITs.NeverLostItem(aer.Pokemon)) return true;
                        break;
                    case Ms.BELCH: //562
                        if (aer.OnboardPokemon.HasCondition(Cs.Belch)) return true;
                        break;
                    case Ms.HYPERSPACE_FURY:
                        if (aer.Pokemon.Form.Species.Number == Ps.HOOPA) return true;
                        break;
                    case Ms.Burn_Up:
                        if (aer.OnboardPokemon.Types.Contains(BattleType.Fire)) return true;
                        return false;
                    default:
                        if ((!atk.Move.HardToUseContinuously || ContinuousUse(atk,Cs.ContinuousUse)) && (!(atk.Move.Move.Id == Ms.DESTINY_BOND) || ContinuousUse(atk, Cs.ContinuousUse2))) return true;
                        break;
                }
            FAIL:
            atk.FailAll();
            return false;
        }
        private static bool ContinuousUse(AtkContext atk, Cs ContinuousUse)
        {
            if (ContinuousUse == Cs.ContinuousUse && atk.Controller.ActingPokemons[atk.Controller.ActingPokemons.Count - 1] == atk.Attacker)
            {
                atk.Attacker.OnboardPokemon.RemoveCondition(ContinuousUse);
                return false;
            }
            var o = atk.Attacker.OnboardPokemon;
            var c = o.GetCondition(Cs.LastMove);
            if (c != null)
            {
                if (ContinuousUse == Cs.ContinuousUse && c.Move.HardToUseContinuously || ContinuousUse == Cs.ContinuousUse2 && c.Move.Move.Id == Ms.DESTINY_BOND)
                {
                    var count = o.GetCondition<int>(ContinuousUse);
                    if (atk.Controller.GetRandomInt(0, 0xffff - 1) < 0xffff >> count)
                    {
                        o.SetCondition(ContinuousUse, count + 1);
                        return true;
                    }
                    atk.Attacker.OnboardPokemon.RemoveCondition(ContinuousUse);
                    return false;
                }
            }
            o.SetCondition(ContinuousUse, 1);
            return true;
        }
    }
}
