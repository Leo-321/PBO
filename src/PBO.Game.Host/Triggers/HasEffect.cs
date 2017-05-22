using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class HasEffect
  {
    public static bool Execute(DefContext def)
    {
      var der = def.Defender;
      var atk = def.AtkContext;
      var move = atk.Move;

      switch (move.Id)
      {
        case Ms.DREAM_EATER:
          return der.State == PokemonState.SLP && !der.OnboardPokemon.HasType(BattleType.Dark);
        case Ms.SYNCHRONOISE:
          return Sychronoise(def);
        case Ms.CAPTIVATE:
          return Captivate(def);
        case Ms.VENOM_DRENCH:
          return der.State == PokemonState.PSN || der.State == PokemonState.BadlyPSN;
        case Ms.FLOWER_SHIELD:
        case Ms.ROTOTILLER:
          return der.OnboardPokemon.HasType(BattleType.Grass);
        case Ms.MAGNETIC_FLUX:
          {
            var ab = der.Ability;
            return ab == As.PLUS || ab == As.MINUS;
          }
        case Ms.Gear_Up:
          {
            var ab = der.Ability;
            return ab == As.PLUS || ab == As.MINUS;
          }
        case Ms.SPIDER_WEB: //169
        case Ms.MEAN_LOOK: //212
        case Ms.BLOCK: //335
          return !der.OnboardPokemon.HasType(BattleType.Ghost);
      }

      if ((move.Powder || move.Spore) && der.OnboardPokemon.HasType(BattleType.Grass)) return false;

      if (move.Move.Category == MoveCategory.Status && move.Id != Ms.THUNDER_WAVE) return true;
      if (move.Class == MoveClass.OHKO && (der.Pokemon.Lv > atk.Attacker.Pokemon.Lv || der.RaiseAbility(As.STURDY))) return false;
      BattleType canAtk;
      {
        var o = der.OnboardPokemon.GetCondition(Cs.CanAttack);
        canAtk = o == null ? BattleType.Invalid : o.BattleType;
      }
      return (canAtk != BattleType.Invalid && der.OnboardPokemon.HasType(canAtk)
              || der.ItemE(Is.RING_TARGET)
              || (atk.Type == BattleType.Ground ? IsGroundAffectable(der, atk.DefenderAbilityAvailable(), true) : NonGround(def)));
    }
    public static bool IsGroundAffectable(PokemonProxy pm, bool abilityAvailable, bool raiseAbility)
    {
      var o = pm.OnboardPokemon;
      return
        (o.HasCondition(Cs.SmackDown) || o.HasCondition(Cs.Ingrain) || pm.Controller.Board.HasCondition(Cs.Gravity)) || pm.ItemE(Is.IRON_BALL)
        || !(o.HasType(BattleType.Flying)
             || o.HasCondition(Cs.MagnetRise) || o.HasCondition(Cs.Telekinesis)
             || pm.ItemE(Is.AIR_BALLOON)
             || (abilityAvailable && (raiseAbility ? pm.RaiseAbility(As.LEVITATE) : pm.AbilityE(As.LEVITATE))));
    }
    private static bool NonGround(DefContext def)
    {
      var type = def.AtkContext.Type.NoEffect();
      return type == BattleType.Invalid || type == BattleType.Ghost && def.AtkContext.Attacker.AbilityE(As.SCRAPPY) || !def.Defender.OnboardPokemon.HasType(type);
    }
    private static bool Sychronoise(DefContext def)
    {
      var types = def.AtkContext.Attacker.OnboardPokemon.Types;
      foreach (var t in types)
        if (def.Defender.OnboardPokemon.HasType(t)) return true;
      return false;
    }
    private static bool Captivate(DefContext def)
    {
      var dg = def.Defender.OnboardPokemon.Gender;
      var ag = def.AtkContext.Attacker.OnboardPokemon.Gender;
      return dg == PokemonGender.Male && ag == PokemonGender.Female || dg == PokemonGender.Female && ag == PokemonGender.Male;
    }
  }
}
