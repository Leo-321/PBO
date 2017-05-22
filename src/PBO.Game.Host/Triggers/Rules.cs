using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
  internal static class Rules
  {
    public static bool CanAddState(PokemonProxy pm, AttachedState state, PokemonProxy by, bool showFail)
    {
      if (state != AttachedState.SLP || !pm.Controller.GameSettings.SleepRule || pm.Pokemon.TeamId == by.Pokemon.TeamId) goto TRUE;
      var p = pm.Field.GetCondition<PokemonProxy>(Cs.RULE_SLP);
      if (p == null || p == pm || p.State != PokemonState.SLP && !p.OnboardPokemon.HasCondition(Cs.Yawn)) goto PREPARE;
      pm.ShowLogPm("RULE_SLP");
      return false;
    PREPARE:
      pm.Field.SetCondition(Cs.RULE_SLP, pm);
    TRUE:
      return true;
    }
  }
}
