using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class InitAtkContext
  {
    public static void Execute(AtkContext atk)
    {
      var aer = atk.Attacker;

      switch (atk.Move.Id)
      {
        case Ms.THRASH: //37
        case Ms.PETAL_DANCE:
        case Ms.OUTRAGE:
          MultiTurn(atk, aer.Controller.GetRandomInt(2, 3), true);
          break;
        case Ms.BIDE: //117
          MultiTurn(atk, 3);
          atk.SetCondition(Cs.Bide, new Condition());
          break;
        case Ms.THIEF: //168
        case Ms.COVET: //343
          if (aer.Pokemon.Item == 0) atk.SetCondition(Cs.Thief);
          break;
        case Ms.ROLLOUT: //205
        case Ms.ICE_BALL:
          MultiTurn(atk, 5);
          break;
        case Ms.PRESENT: //217
          {
            var random = atk.Controller.GetRandomInt(0, 99);
            atk.SetCondition(Cs.Present, random < 20 ? 0 : random < 60 ? 40 : random < 90 ? 80 : 100);
          }
          break;
        case Ms.MAGNITUDE: //222
          Magnitude(atk);
          break;
        case Ms.PURSUIT: //228
          if (aer.OnboardPokemon.HasCondition(Cs.Pursuiting)) atk.IgnoreSwitchItem = true;
          break;
        case Ms.UPROAR: //253
          MultiTurn(atk, 3);
          break;
      }
    }
    private static void Magnitude(AtkContext atk)
    {
      var random = atk.Controller.GetRandomInt(0, 99);
      if (random >= 95)
      {
        atk.SetCondition(Cs.Magnitude, 7);
        atk.Controller.ReportBuilder.ShowLog("Magnitude", 10);
      }
      else
      {
        var a = random < 5 ? 0 : random < 16 ? 1 : random < 35 ? 2 : random < 65 ? 3 : random < 85 ? 4 : 5;
        atk.SetCondition(Cs.Magnitude, a);
        atk.Controller.ReportBuilder.ShowLog("Magnitude", 4 + a);
      }
    }
    private static void MultiTurn(AtkContext atk, int turn, bool isRandom = false)
    {
      atk.SetCondition(Cs.MultiTurn, new Condition() { Turn = turn, Bool = isRandom });
    }
  }
}
