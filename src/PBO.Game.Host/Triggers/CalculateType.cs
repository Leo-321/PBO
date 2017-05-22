using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class CalculateType
    {
        public static void Execute(AtkContext atk)
        {
            switch (atk.Move.Id)
            {
                case Ms.STRUGGLE: //165
                    atk.Type = BattleType.Invalid;
                    break;
                case Ms.HIDDEN_POWER: //237
                    HiddenPower(atk);
                    break;
                case Ms.NATURAL_GIFT: //363
                    NatureGift(atk);
                    break;
                case Ms.JUDGMENT: //449
                    Judgement(atk);
                    break;
                case Ms.Multi_Attack:
                    Multi_Attack(atk);
                    break;
                case Ms.WEATHER_BALL:
                    Weather_Ball(atk);
                    break;
                case Ms.TECHNO_BLAST: //546
                    {
                        var i = atk.Attacker.Pokemon.Item;
                        atk.Type = i == Is.DOUSE_DRIVE ? BattleType.Water : i == Is.SHOCK_DRIVE ? BattleType.Electric : i == Is.BURN_DRIVE ? BattleType.Fire : i == Is.CHILL_DRIVE ? BattleType.Ice : BattleType.Normal;
                    }
                    break;
                default:
                    var aer = atk.Attacker;
                    if (aer.OnboardPokemon.HasCondition(Cs.Electrify)) atk.Type = BattleType.Electric;
                    else if (atk.Move.Sound && aer.Ability == As.Liquid_Voice) atk.Type = BattleType.Water;
                    else if (atk.Move.Id == Ms.Revelation_Dance) atk.Type = atk.Attacker.OnboardPokemon.TypeOne;
                    else if (atk.Move.Move.Type == BattleType.Normal || aer.AbilityE(As.NORMALIZE))
                        if (aer.AbilityE(As.AERILATE))
                        {
                            atk.Type = BattleType.Flying;
                            atk.SetCondition(Cs.Sukin);
                        }
                        else if (aer.AbilityE(As.Galvanize))
                        {
                            atk.Type = BattleType.Electric;
                            atk.SetCondition(Cs.Sukin);
                        }
                        else if (aer.AbilityE(As.PIXILATE))
                        {
                            atk.Type = BattleType.Fairy;
                            atk.SetCondition(Cs.Sukin);
                        }
                        else if (aer.AbilityE(As.REFRIGERATE))
                        {
                            atk.Type = BattleType.Ice;
                            atk.SetCondition(Cs.Sukin);
                        }
                        else atk.Type = atk.Controller.Board.HasCondition(Cs.IonDeluge) ? BattleType.Electric : BattleType.Normal;
                    else atk.Type = atk.Move.Move.Type;
                    break;
            }
        }

        private static void HiddenPower(AtkContext atk)
        {
            atk.Type = GameHelper.HiddenPower(atk.Attacker.Pokemon.Iv);
        }

        private static void NatureGift(AtkContext atk)
        {
            atk.Type = GameHelper.NatureGift(atk.Attacker.Pokemon.Item);
        }
        private static void Weather_Ball(AtkContext atk)
        {
            if (atk.Controller.Weather == Weather.Hailstorm)
                atk.Type = BattleType.Ice;
            else if (atk.Controller.Weather == Weather.IntenseSunlight)
                atk.Type = BattleType.Fire;
            else if (atk.Controller.Weather == Weather.Rain)
                atk.Type = BattleType.Water;
            else if (atk.Controller.Weather == Weather.Sandstorm)
                atk.Type = BattleType.Rock;
        }

        private static void Judgement(AtkContext atk)
        {
            switch (atk.Attacker.Pokemon.Item)
            {
                case Is.FLAME_PLATE:
                    atk.Type = BattleType.Fire;
                    break;
                case Is.SPLASH_PLATE:
                    atk.Type = BattleType.Water;
                    break;
                case Is.ZAP_PLATE:
                    atk.Type = BattleType.Electric;
                    break;
                case Is.MEADOW_PLATE:
                    atk.Type = BattleType.Grass;
                    break;
                case Is.ICICLE_PLATE:
                    atk.Type = BattleType.Ice;
                    break;
                case Is.FIST_PLATE:
                    atk.Type = BattleType.Fighting;
                    break;
                case Is.TOXIC_PLATE:
                    atk.Type = BattleType.Poison;
                    break;
                case Is.EARTH_PLATE:
                    atk.Type = BattleType.Ground;
                    break;
                case Is.SKY_PLATE:
                    atk.Type = BattleType.Flying;
                    break;
                case Is.MIND_PLATE:
                    atk.Type = BattleType.Psychic;
                    break;
                case Is.INSECT_PLATE:
                    atk.Type = BattleType.Bug;
                    break;
                case Is.STONE_PLATE:
                    atk.Type = BattleType.Rock;
                    break;
                case Is.SPOOKY_PLATE:
                    atk.Type = BattleType.Ghost;
                    break;
                case Is.DRACO_PLATE:
                    atk.Type = BattleType.Dragon;
                    break;
                case Is.DREAD_PLATE:
                    atk.Type = BattleType.Dark;
                    break;
                case Is.IRON_PLATE:
                    atk.Type = BattleType.Steel;
                    break;
                case Is.PIXIE_PLATE:
                    atk.Type = BattleType.Fairy;
                    break;
                default:
                    atk.Type = BattleType.Normal;
                    break;
            }
        }
        private static void Multi_Attack(AtkContext atk)
        {
            switch (atk.Attacker.Pokemon.Item)
            {
                case Is.Fire_Memory:
                    atk.Type = BattleType.Fire;
                    break;
                case Is.Water_Memory:
                    atk.Type = BattleType.Water;
                    break;
                case Is.Electric_Memory:
                    atk.Type = BattleType.Electric;
                    break;
                case Is.Grass_Memory:
                    atk.Type = BattleType.Grass;
                    break;
                case Is.Ice_Memory:
                    atk.Type = BattleType.Ice;
                    break;
                case Is.Fighting_Memory:
                    atk.Type = BattleType.Fighting;
                    break;
                case Is.Poison_Memory:
                    atk.Type = BattleType.Poison;
                    break;
                case Is.Ground_Memory:
                    atk.Type = BattleType.Ground;
                    break;
                case Is.Flying_Memory:
                    atk.Type = BattleType.Flying;
                    break;
                case Is.Psychic_Memory:
                    atk.Type = BattleType.Psychic;
                    break;
                case Is.Bug_Memory:
                    atk.Type = BattleType.Bug;
                    break;
                case Is.Rock_Memory:
                    atk.Type = BattleType.Rock;
                    break;
                case Is.Ghost_Memory:
                    atk.Type = BattleType.Ghost;
                    break;
                case Is.Dragon_Memory:
                    atk.Type = BattleType.Dragon;
                    break;
                case Is.Dark_Memory:
                    atk.Type = BattleType.Dark;
                    break;
                case Is.Steel_Memory:
                    atk.Type = BattleType.Steel;
                    break;
                case Is.Fairy_Memory:
                    atk.Type = BattleType.Fairy;
                    break;
                default:
                    atk.Type = BattleType.Normal;
                    break;
            }
        }
}
}
