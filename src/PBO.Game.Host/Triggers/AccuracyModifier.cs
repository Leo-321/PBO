using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class AccuracyModifier
  {
    public static Modifier Execute(DefContext def)
    {
      var der = def.Defender;
      var atk = def.AtkContext;
      var aer = atk.Attacker;
      
      Modifier m = 0x1000;
      switch (def.Defender.Ability)
      {
        case As.WONDER_SKIN:
          if (atk.Move.Move.Category == MoveCategory.Status) m *= 0x999;
          break;
        case As.SAND_VEIL:
          if (aer.Controller.Weather == Weather.Sandstorm) m *= 0xccc;
          break;
        case As.SNOW_CLOAK:
          if (aer.Controller.Weather == Weather.Hailstorm) m *= 0xccc;
          break;
        case As.TANGLED_FEET:
          if (der.OnboardPokemon.HasCondition(Cs.Confuse)) m *= 0xccc;
          break;
      }
      
      m *= atk.AccuracyModifier;

      {
        int i = der.Item;
        if (i == Is.BRIGHT_POWDER) m *= 0xE66;
        else if (i == Is.LAX_INCENSE) m *= 0xF34;
      }

      if (aer.ItemE(Is.ZOOM_LENS) && aer.LastMoveTurn == der.LastMoveTurn) m *= 0x1333;
      
      if (der.Controller.Board.HasCondition(Cs.Gravity)) m *= 0x1AAA;//如果场上存在重力，命中×5/3。
      
      return m;
    }
  }
}
