using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Game.GameEvents;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class CanExecute
  {
    public static bool Execute(PokemonProxy pm)
    {
      return
        Sleeping(pm) &&
        Frozen(pm) &&
        Disable(pm) &&
        Truant(pm) &&
        Imprison(pm) &&
        HealBlock(pm) &&
        Confuse(pm) &&
        Flinch(pm) &&
        Taunt(pm) && 
        Gravity(pm) &&  
        Attract(pm) &&
        Paralyzed(pm) &&
        SoundBlock(pm) &&
        Shell_Trap(pm) &&
        FocusPunch(pm);
    }
    private static bool Sleeping(PokemonProxy pm)
    {
      if (pm.State == PokemonState.SLP)
      {
        pm.Pokemon.SLPTurn -= pm.AbilityE(As.EARLY_BIRD) ? 2 : 1;
        if (pm.Pokemon.SLPTurn <= 0 && !pm.AbilityE(As.Comatose)) pm.DeAbnormalState();
        else
        {
          pm.ShowLogPm("SLP");
          return pm.SelectedMove.MoveE.AvailableEvenSleeping || pm.AbilityE(As.Comatose);
        }
      }
      return true;
    }
    private static bool Frozen(PokemonProxy p)
    {
      if (p.State == PokemonState.FRZ)
      {
        if (p.SelectedMove.MoveE.SelfDeFrozen) p.DeAbnormalState("DeFRZ2", p.SelectedMove.MoveE.Id);
        else if (p.Controller.GetRandomInt(0, 3) == 0) p.DeAbnormalState();
        else
        {
          p.ShowLogPm("FRZ");
          return false;
        }
      }
      return true;
    }
    private static bool Disable(PokemonProxy p)
    {
      var c = p.OnboardPokemon.GetCondition(Cs.Disable);
      if (c != null && p.SelectedMove.MoveE == c.Move) 
      {
        p.ShowLogPm("Disable", p.SelectedMove.MoveE.Id);
        return false;
      }
      return true;
    }
    private static bool Truant(PokemonProxy p)
    {
      if (p.AbilityE(As.TRUANT))
      {
        if (p.OnboardPokemon.GetCondition<int>(Cs.Truant) == p.Controller.TurnNumber)
        {
          p.RaiseAbility();
          p.ShowLogPm("Truant");
          return false;
        }
        p.OnboardPokemon.SetCondition(Cs.Truant, p.Controller.TurnNumber + 1);
      }
      return true;
    }
    private static bool Imprison(PokemonProxy p)
    {
      var move = p.SelectedMove.MoveE;
      foreach (PokemonProxy pm in p.Controller.Board[1 - p.Pokemon.TeamId].GetAdjacentPokemonsByOppositeX(p.OnboardPokemon.X))
        if (pm.OnboardPokemon.HasCondition(Cs.Imprison))
          foreach (MoveProxy m in pm.Moves)
            if (m.MoveE == move)
            {
              p.ShowLogPm("Imprison", move.Id);
              return false;
            }
      return true;
    }
    private static bool HealBlock(PokemonProxy pm)
    {
      if (pm.SelectedMove.MoveE.Heal && pm.OnboardPokemon.HasCondition(Cs.HealBlock))
      {
        pm.ShowLogPm("HealBlockCantUseMove", pm.SelectedMove.MoveE.Move.Id);
        return false;
      }
      return true;
    }
    private static bool SoundBlock(PokemonProxy pm)
    {
      if (pm.SelectedMove.MoveE.Sound && pm.OnboardPokemon.HasCondition(Cs.SoundBlock))
       {
         pm.ShowLogPm("SoundBlockCantUseMove", pm.SelectedMove.MoveE.Move.Id);
         return false;
       }
       return true;
    }
    private static bool Confuse(PokemonProxy pm)
    {
      int count = pm.OnboardPokemon.GetCondition<int>(Cs.Confuse);
      if (count != 0)
        if (--count > 0)
        {
          pm.ShowLogPm("Confuse");
          pm.OnboardPokemon.SetCondition(Cs.Confuse, count);
          if (pm.Controller.OneNth(3))
          {
            pm.ShowLogPm(Ls.ConfuseWork);
            var e = new GameEvents.ShowHp() { Pm = pm.Id };
            pm.Controller.ReportBuilder.Add(e);
            pm.MoveHurt((pm.Pokemon.Lv * 2 / 5 + 2) * 40 * OnboardPokemon.Get5D(pm.OnboardPokemon.FiveD.Atk, pm.OnboardPokemon.Lv5D.Atk) / OnboardPokemon.Get5D(pm.OnboardPokemon.FiveD.Def, pm.OnboardPokemon.Lv5D.Def) / 50 + 2, true);
            e.Hp = pm.Hp;
            pm.CheckFaint();
            //if (!pm.CheckFaint()) pm.Item.HpChanged(pm); //◇硝子玩偶◇ 22:21:00 你知道混乱打自己的时候不触发加HP的果子么
            return false;
          }
        }
        else
        {
          pm.OnboardPokemon.RemoveCondition(Cs.Confuse);
          pm.ShowLogPm("DeConfuse");
        }
      return true;
    }
    private static bool Flinch(PokemonProxy pm)
    {
      if (pm.OnboardPokemon.HasCondition(Cs.Flinch))
      {
        pm.ShowLogPm("Flinch");
        if (pm.RaiseAbility(As.STEADFAST)) pm.ChangeLv7D(pm, StatType.Speed, 1, false);
        return false;
      }
      return true;
    }
    private static bool Taunt(PokemonProxy p)
    {
      if (p.SelectedMove.MoveE.Move.Category == MoveCategory.Status && p.OnboardPokemon.HasCondition(Cs.Taunt))
      {
        p.ShowLogPm("Taunt", p.SelectedMove.MoveE.Id);
        return false;
      }
      return true;
    }
    private static bool Gravity(PokemonProxy p)
    {
      if (p.SelectedMove.MoveE.UnavailableWithGravity && p.Controller.Board.HasCondition(Cs.Gravity))
      {
        p.ShowLogPm("GravityCantUseMove", p.SelectedMove.MoveE.Id);
        return false;
      }
      return true;
    }
    private static bool Attract(PokemonProxy p)
    {
      var pm = p.OnboardPokemon.GetCondition<PokemonProxy>(Cs.Attract);
      if (pm != null)
      {
        p.ShowLogPm("Attract", pm.Id);
        if (p.Controller.RandomHappen(50))
        {
          p.ShowLogPm("AttractWork");
          return false;
        }
      }
      return true;
    }
    private static bool Paralyzed(PokemonProxy p)
    {
      if (p.State == PokemonState.PAR && p.Controller.OneNth(4))
      {
        p.ShowLogPm("PARWork");
        return false;
      }
      return true;
    }
    private static bool FocusPunch(PokemonProxy p)
    {
      if (p.SelectedMove.MoveE.Id == Ms.FOCUS_PUNCH && p.OnboardPokemon.HasCondition(Cs.Damage))
      {
        p.ShowLogPm("DeFocusPunch");
        return false;
      }
      return true;
    }
    private static bool Shell_Trap(PokemonProxy p)
    {
      if(p.SelectedMove.MoveE.Id==Ms.Shell_Trap)
        if(p.OnboardPokemon.HasCondition(Cs.PhysicalDamage))
            return true;
        else
        {
            p.ShowLogPm("DeShell_Trap");
            return false;
        }
      else
      return true;
    }
  }
}
