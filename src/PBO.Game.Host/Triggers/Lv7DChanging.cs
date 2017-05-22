using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class Lv7DChanging
  {
    public static int Execute(PokemonProxy pm, PokemonProxy by, StatType stat, int change, bool showFail)
    {
      var a = pm.Ability;

      if (change < 0 && by != pm)
      {
        if (pm.Field.HasCondition(Cs.Mist)) //根据百科非技能似乎不该发动，但排除了一下这样写肯定是对的
        {
          if (showFail) pm.ShowLogPm("Mist");
          return 0;
        }
        if (by == null || pm.Pokemon.TeamId != by.Pokemon.TeamId)
        {
          switch (a)
          {
            case As.CLEAR_BODY:
            case As.WHITE_SMOKE:
            case As.Full_Metal_Body:
              if (showFail)
              {
                pm.RaiseAbility();
                pm.ShowLogPm("7DLockAll");
              }
              return 0;
            case As.KEEN_EYE:
              if (CantLvDown(pm, by, stat, change, showFail, StatType.Accuracy)) return 0;
              break;
            case As.HYPER_CUTTER:
              if (CantLvDown(pm, by, stat, change, showFail, StatType.Atk)) return 0;
              break;
            case As.BIG_PECKS:
              if (CantLvDown(pm, by, stat, change, showFail, StatType.Def)) return 0;
              break;
          }
        }
        if (pm.OnboardPokemon.HasType(BattleType.Grass))
          foreach(var p in pm.Field.Pokemons)
            if (p.AbilityE(As.FLOWER_VEIL))
            {
              if (showFail)
              {
                p.RaiseAbility();
                pm.ShowLogPm("7DLock", (int)stat);
              }
              return 0;
            }
      }

      switch (a)
      {
        case As.SIMPLE: //86
          change <<= 1;
          break;
        case As.CONTRARY: //126
          change = 0 - change;
          break;
      }

      return change;
    }

    private static bool CantLvDown(PokemonProxy pm, PokemonProxy by, StatType stat, int change, bool showFail, StatType s0)
    {
      if (stat == s0)
      {
        if (showFail)
        {
          pm.RaiseAbility();
          pm.ShowLogPm("7DLock", (int)stat);
        }
        return true;
      }
      return false;
    }
  }
}
