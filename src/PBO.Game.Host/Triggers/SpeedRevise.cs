using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class SpeedRevise
  {
    public static int Execute(PokemonProxy pm, int speed)
    {
      if (pm.State == PokemonState.PAR) speed >>= 1;
      switch (pm.Ability)
      {
        case As.CHLOROPHYLL:
          if (pm.Controller.Weather == Weather.IntenseSunlight) speed <<= 1;
          break;
        case As.SAND_RUSH:
          if (pm.Controller.Weather == Weather.Sandstorm) speed <<= 1;
          break;
        case As.SWIFT_SWIM:
          if (pm.Controller.Weather == Weather.Rain) speed <<= 1;
          break;
        case As.Slush_Rush:
          if (pm.Controller.Weather == Weather.Hailstorm) speed <<= 1;
          break;
        case As.Surge_Surfer:
          if (pm.Controller.Board.HasCondition(Cs.ElectricTerrain)) speed <<= 1;
          break;
        case As.UNBURDEN:
          if (pm.Pokemon.Item == 0 && pm.OnboardPokemon.HasCondition(Cs.HadItem)) speed <<= 1;
          break;
        case As.QUICK_FEET:
          if (pm.State != PokemonState.Normal) speed += speed >> 1;
          break;
      }
      switch (pm.Item)
      {
        case Is.MACHO_BRACE:
        case Is.IRON_BALL:
        case Is.POWER_BRACER:
        case Is.POWER_BELT:
        case Is.POWER_LENS:
        case Is.POWER_BAND:
        case Is.POWER_ANKLET:
        case Is.POWER_WEIGHT:
          speed >>= 1;
          break;
        case Is.QUICK_POWDER:
          if (pm.Pokemon.Form.Species.Number == 132 && !pm.OnboardPokemon.HasCondition(Cs.Transform)) speed <<= 1;
          break;
        case Is.CHOICE_SCARF:
          speed += speed >> 1;
          break;
      }
      if (pm.Field.HasCondition(Cs.Tailwind)) speed <<= 1;
      if (pm.Field.HasCondition(Cs.Swamp)) speed = (speed + 1) >> 2; //小数点是0.5以下就舍去，如果是0.75就四舍五入
      return speed;
    }
  }
}
