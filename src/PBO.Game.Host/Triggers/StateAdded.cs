using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class StateAdded
  {
    public static void Execute(PokemonProxy pm)
    {
      switch (pm.Item)
      {
        case Is.MENTAL_HERB:
          MentalHerb(pm);
          break;
        case Is.CHERI_BERRY:
          DeStateBerry(pm, PokemonState.PAR);
          break;
        case Is.CHESTO_BERRY:
          DeStateBerry(pm, PokemonState.SLP);
          break;
        case Is.RAWST_BERRY:
          DeStateBerry(pm, PokemonState.BRN);
          break;
        case Is.ASPEAR_BERRY:
          DeStateBerry(pm, PokemonState.FRZ);
          break;
        case Is.PECHA_BERRY:
          if (pm.State == PokemonState.PSN || pm.State == PokemonState.BadlyPSN)
          {
            var s = pm.State.ToString();
            pm.DeAbnormalState("ItemDePSN", Is.PECHA_BERRY);
            pm.ConsumeItem();
          }
          break;
        case Is.PERSIM_BERRY:
          if (pm.OnboardPokemon.RemoveCondition(Cs.Confuse))
          {
            pm.ShowLogPm("ItemDeConfuse", Is.PERSIM_BERRY);
            pm.ConsumeItem();
          }
          break;
        case Is.LUM_BERRY:
          if (pm.State != PokemonState.Normal)
          {
            pm.DeAbnormalState("ItemDe" + pm.State.ToString(), Is.LUM_BERRY);
            pm.ConsumeItem();
          }
          break;
      }
    }

    private static void DeStateBerry(PokemonProxy pm, PokemonState state)
    {
      if (pm.State == state)
      {
        pm.DeAbnormalState("ItemDe" + state.ToString(), pm.Pokemon.Item);
        pm.ConsumeItem();
      }
    }

    private static bool MentalHerb(PokemonProxy pm, Cs condition)
    {
      if (pm.OnboardPokemon.RemoveCondition(condition))
      {
        pm.ShowLogPm("De" + condition);
        return true;
      }
      return false;
    }
    private static void MentalHerb(PokemonProxy pm)
    {
      var a = pm.OnboardPokemon.RemoveCondition(Cs.Attract);
      if (a) pm.ShowLogPm("ItemDeAttract", pm.Pokemon.Item);
      if (a | MentalHerb(pm, Cs.Encore) | MentalHerb(pm, Cs.Taunt) | MentalHerb(pm, Cs.Torment) | MentalHerb(pm, Cs.Disable)) pm.ConsumeItem();
    }
  }
}
