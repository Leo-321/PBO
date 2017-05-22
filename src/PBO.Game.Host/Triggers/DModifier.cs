using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class DModifier
    {
        public static Modifier Execute(DefContext def)
        {
            var der = def.Defender;
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var cat = atk.Move.Move.Category;
            var n = der.Pokemon.Form.Species.Number;

            Modifier m = 0x1000;

            switch (def.Ability)
            {
                case As.MARVEL_SCALE:
                    if (der.State != PokemonState.Normal) m *= 0x1800;
                    break;
                case As.GRASS_PELT:
                    if (der.Controller.Board.HasCondition(Cs.GrassyTerrain)) m *= 0x1800;
                    break;
                case As.FUR_COAT:
                    if (cat == MoveCategory.Physical) m *= 0x2000;
                    break;
            }

            if (cat == MoveCategory.Special && der.Controller.Weather == Weather.IntenseSunlight)
                foreach (PokemonProxy pm in der.Controller.GetOnboardPokemons(der.Pokemon.TeamId))
                    if (pm.Pokemon.Form.Species.Number == 421 && pm.AbilityE(As.FLOWER_GIFT)) m *= 0x1800;

            switch (der.Item)
            {
                case Is.EVIOLITE:
                    foreach (var e in RomData.Evolutions)
                        if (e.From == der.Pokemon.Form.Species.Number)
                        {
                            m = 0x1800;
                            break;
                        }
                    break;
                case Is.SOUL_DEW:
                    if (cat == MoveCategory.Special && (n == 380 || n == 381)) m *= 0x1800;
                    break;
                case Is.DEEPSEASCALE:
                    if (cat == MoveCategory.Special && n == 366) m *= 0x2000;
                    break;
                case Is.METAL_POWDER:
                    if (cat == MoveCategory.Physical && n == 132 && !der.OnboardPokemon.HasCondition(Cs.Transform)) m *= 0x2000;
                    break;
                case Is.ASSAULT_VEST:
                    if (cat == MoveCategory.Special) m *= 0x1800;
                    break;
            }
            return m;
        }

        public static Modifier Sandstorm(DefContext def)
        {
            return (Modifier)(def.AtkContext.Move.Move.Category == MoveCategory.Special && def.Defender.Controller.Weather == Weather.Sandstorm && def.Defender.OnboardPokemon.HasType(BattleType.Rock) ? 0x1800 : 0x1000);
        }
    }
}
