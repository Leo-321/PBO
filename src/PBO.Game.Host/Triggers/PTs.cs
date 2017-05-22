using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
  internal static partial class PTs
  {
        public static void ShowLogPm(this PokemonProxy pm, string key, int arg1 = 0, int arg2 = 0)
    {
            pm.Controller.ReportBuilder.ShowLog(key, pm.Id, arg1, arg2);
    }
    public static void NoEffect(this PokemonProxy pm)
    {
      ShowLogPm(pm, "NoEffect");
    }
    public static bool CanHpRecover(this PokemonProxy pm, bool showFail = false)
    {
      if (pm.AliveOnboard)
      {
        if (pm.Hp == pm.Pokemon.MaxHp)
        {
          if (showFail) ShowLogPm(pm, "FullHp");
          return false;
        }
        if (pm.OnboardPokemon.HasCondition(Cs.HealBlock))
        {
          ShowLogPm(pm, "HealBlock");
          return false;
        }
        return true;
      }
      return false;
    }
    public static bool CanChangeForm(this PokemonProxy pm, int number)
    {
      return pm.Pokemon.Form.Species.Number == number && !pm.OnboardPokemon.HasCondition(Cs.Transform);
    }
    public static bool CanChangeForm(this PokemonProxy pm, int number, int form)
    {
      return pm.OnboardPokemon.Form.Index != form && CanChangeForm(pm, number);
    }
        //gen7
    public static bool CanUseZmove(this PokemonProxy pm)
    {
        foreach(var p in pm.Pokemon.Moves)
                if (GameHelper.Zmove(p, pm.Item, pm.DexId, pm.Pokemon.Form.Index) != null)
                    return true;
        return false;
    }
    public static void ChangeForm(this PokemonProxy pm, int form, bool forever = false, string log = "FormChange")
    {
      pm.OnboardPokemon.ChangeForm(pm.OnboardPokemon.Form.Species.GetForm(form));
      if (forever) pm.Pokemon.Form = pm.OnboardPokemon.Form;
      pm.Controller.ReportBuilder.ChangeForm(pm, forever);
      if (log != null) ShowLogPm(pm, log);
      AbilityAttach.Execute(pm);
    }
    /// <summary>
    /// null log to show default log
    /// </summary>
    /// <param name="log"></param>
    /// <param name="arg1"></param>
    public static bool DeAbnormalState(this PokemonProxy pm, string log = null, int arg1 = 0)
    {
      if (pm.State != PokemonState.Normal && pm.Hp > 0)
      {
        ShowLogPm(pm, log ?? "De" + pm.State, arg1);
        pm.Pokemon.State = PokemonState.Normal;
        return true;
      }
      return false;
    }
    public static bool CheckFaint(this PokemonProxy pm)
    {
      if (pm.Hp == 0 && pm.OnboardPokemon != pm.NullOnboardPokemon)
      {
        ATs.Withdrawn(pm, pm.OnboardPokemon.Ability);
        pm.Field.SetCondition(Cs.FaintTurn, pm.Controller.TurnNumber);
        pm.Pokemon.State = PokemonState.Faint;
        pm.Controller.Withdraw(pm, "Faint", 0, false);
        foreach(var p in pm.Controller.OnboardPokemons)
        {
          if(p.AbilityE(As.Soul_Heart))
            p.ChangeLv7D(p,StatType.SpAtk,1,false,true);
        }
        foreach(var p in pm.Field.Pokemons)
        {
          var d = pm.OnboardPokemon.Ability;
          if ((p.AbilityE(As.Receiver) || p.AbilityE(As.Power_of_Alchemy)) && d != As.WONDER_GUARD && d != As.FORECAST && d != As.MULTITYPE && d != As.ILLUSION && d != As.ZEN_MODE && d != As.Shields_Down && d != As.Schooling && d != As.Shields_Down && d != As.Battle_Bond && d!=As.RKS_System && d!=As.Comatose )
          {
                p.RaiseAbility();
                p.ChangeAbility(d);
                p.Controller.ReportBuilder.ShowLog("Receiver", pm.Id, pm.OnboardPokemon.Ability);
          }
        }
        return true;
      }
      return false;
    }
    public static void ConsumeItem(this PokemonProxy pm, bool cheekPouch = true)
    {
      pm.OnboardPokemon.SetTurnCondition(Cs.UsedItem, pm.Pokemon.Item);
      pm.Pokemon.UsedItem = pm.Pokemon.Item;
      if (ITs.Berry(pm.Pokemon.Item))
      {
        pm.OnboardPokemon.SetCondition(Cs.Belch);
        pm.Pokemon.UsedBerry = pm.Pokemon.Item;
        if (CanHpRecover(pm) && ATs.RaiseAbility(pm, As.CHEEK_POUCH)) HpRecoverByOneNth(pm, 3);
      }
      RemoveItem(pm);
    }
    /// <summary>
    /// Item should not be null, or Unburden effect will be wrong
    /// </summary>
    public static void RemoveItem(this PokemonProxy pm)
    {
      pm.Pokemon.Item = 0;
      if (pm.AbilityE(As.UNBURDEN)) pm.OnboardPokemon.SetCondition(Cs.Unburden);
    }
    public static void SetItem(this PokemonProxy pm, int item)
    {
      pm.Pokemon.Item = item;
      pm.OnboardPokemon.RemoveCondition(Cs.Unburden);
      pm.OnboardPokemon.RemoveCondition(Cs.ChoiceItem);
    }
    public static void ChangeAbility(this PokemonProxy pm, int ab)
    {
      AbilityDetach.Execute(pm);
      pm.OnboardPokemon.Ability = ab;
      AbilityAttach.Execute(pm);
    }

    public static void CalculatePriority(this PokemonProxy pm)
    {
      pm.Priority = 0;
      pm.ItemSpeedValue = 0;
      if (pm.Action != PokemonAction.WillSwitch)
      {
        var m = pm.SelectedMove.MoveE;
        pm.Priority = m.Priority;
                if (m.Move.Category == MoveCategory.Status && pm.AbilityE(As.PRANKSTER) || m.Move.Type == BattleType.Flying && pm.AbilityE(As.GALE_WINGS) && pm.Hp == pm.Pokemon.MaxHp || pm.AbilityE(As.Triage) && m.Heal) pm.Priority++;
        switch (pm.Item)
        {
          case Is.LAGGING_TAIL:
          case Is.FULL_INCENSE:
            pm.ItemSpeedValue = -1;
            break;
          case Is.QUICK_CLAW:
            if (pm.Controller.RandomHappen(20))
            {
              ShowLogPm(pm, "QuickItem", Is.QUICK_CLAW);
              pm.ItemSpeedValue = 1;
            }
            break;
          case Is.CUSTAP_BERRY:
            if (ATs.Gluttony(pm))
            {
              ShowLogPm(pm, "QuickItem", Is.CUSTAP_BERRY);
              pm.ConsumeItem();
              pm.ItemSpeedValue = 1;
            }
            break;
        }
      }
    }
  }
}
