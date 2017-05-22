using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class MoveImplementEffect
    {
        public static void Execute(DefContext def)
        {
            var der = def.Defender;
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var move = atk.Move;

            switch (move.Id)
            {
                case Ms.TRI_ATTACK: //161
                    TriAttack(def);
                    break;
                case Ms.THIEF: //168
                case Ms.COVET: //343
                    Thief(def);
                    break;
                case Ms.KNOCK_OFF: //282
                    RemoveItem(def, false, "KnockOff");
                    break;
                case Ms.INCINERATE: //510
                    RemoveItem(def, true, "Incinerate");
                    break;
                case Ms.SECRET_POWER: //290
                    SecretPower(def);
                    break;
                case Ms.NATURAL_GIFT: //363
                    def.AtkContext.Attacker.ConsumeItem(false);
                    break;
                case Ms.PLUCK: //365
                case Ms.BUG_BITE: //450
                    EatDefenderBerry(def);
                    break;
                case Ms.FLING: //374
                    {
                        var i = aer.Pokemon.Item;
                        aer.ConsumeItem(false);
                        ITs.RaiseItemByMove(def.Defender, i, aer);
                    }
                    break;
                case Ms.SHADOW_FORCE: //467
                case Ms.PHANTOM_FORCE:
                case Ms.HYPERSPACE_HOLE:
                case Ms.HYPERSPACE_FURY:
                    if (def.Defender.OnboardPokemon.RemoveCondition(Cs.Protect)) def.Defender.ShowLogPm("DeProtect");
                    break;
                case Ms.CLEAR_SMOG: //499
                    der.OnboardPokemon.SetLv7D(0, 0, 0, 0, 0, 0, 0);
                    der.ShowLogPm("7DReset");
                    break;
                case Ms.Core_Enforcer:
                    if (der.LastMoveTurn == der.Controller.TurnNumber && !GameHelper.CantLoseAbility(der.Ability) && der.OnboardPokemon.AddCondition(Cs.GastroAcid))
                        der.ShowLogPm("LoseAbility");
                    break;
                default:
                    if (der.Hp > 0)
                    {
                        if (move.Class == MoveClass.AttackWithTargetLv7DChange) der.ChangeLv7D(def);
                        else if (move.Class == MoveClass.AttackWithState) der.AddState(def);
                        if (!def.AbilityE(As.INNER_FOCUS) && (move.FlinchProbability != 0 && def.RandomHappen(move.FlinchProbability) || ATs.Stench(def) || ITs.CanAttackFlinch(def))) der.OnboardPokemon.SetTurnCondition(Cs.Flinch);
                    }
                    break;
            }
            switch (move.Id)
            {
                case Ms.RELIC_SONG:
                    if (aer.CanChangeForm(648)) aer.ChangeForm(1 - aer.OnboardPokemon.Form.Index);
                    break;
            }
        }

        private static void TriAttack(DefContext def)
        {
            if (def.RandomHappen(20))
            {
                int i = def.Defender.Controller.GetRandomInt(0, 2);
                AttachedState s = i == 0 ? AttachedState.PAR : i == 1 ? AttachedState.BRN : AttachedState.FRZ;
                def.Defender.AddState(def.AtkContext.Attacker, s, false);
            }
        }

        private static void Thief(DefContext def)
        {
            var aer = def.AtkContext.Attacker;
            if (def.AtkContext.HasCondition(Cs.Thief) && ITs.CanLostItem(def.Defender))
            {
                var i = def.Defender.Pokemon.Item;
                def.Defender.RemoveItem();
                aer.SetItem(i); //先铁棘再果子
                aer.ShowLogPm("Thief", i, def.Defender.Id);
            }
        }

        private static void SecretPower(DefContext def)
        {
            if (def.RandomHappen(30))
                switch (def.Defender.Controller.GameSettings.Terrain)
                {
                    case Terrain.Path:
                        def.Defender.AddState(def.AtkContext.Attacker, AttachedState.PAR, false);
                        break;
                }
        }

        private static void EatDefenderBerry(DefContext def)
        {
            if (ITs.CanLostItem(def.Defender))
            {
                var i = def.Defender.Pokemon.Item;
                if (ITs.Berry(i)) def.SetCondition(Cs.EatenBerry, i);
            }
        }

        private static void RemoveItem(DefContext def, bool sp, string log)
        {
            var der = def.Defender;
            if (ITs.CanLostItem(der))
            {
                var i = der.Pokemon.Item;
                if (!sp || ITs.Berry(i) || ITs.Gem(i))
                {
                    der.RemoveItem();
                    der.ShowLogPm(log, i, sp ? 0 : def.AtkContext.Attacker.Id);
                }
            }
        }
    }
}
