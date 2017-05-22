using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class CanImplement
  {
    public static bool Execute(DefContext def)
    {
      var der = def.Defender;
      var move = def.AtkContext.Move;

      switch (def.Defender.Ability)
      {
        case As.VOLT_ABSORB: //10
          if (!NoEffectWithAbsorb(def, BattleType.Electric)) return false;
          break;
        case As.WATER_ABSORB: //11
        case As.DRY_SKIN: //87
          if (!NoEffectWithAbsorb(def, BattleType.Water)) return false;
          break;
        case As.OBLIVIOUS: //12
          if (move.Id == Ms.CAPTIVATE || move.Id == Ms.TAUNT)//顺序已测
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.FLASH_FIRE: //18
          if (def.AtkContext.Type == BattleType.Fire)
          {
            der.OnboardPokemon.SetCondition(Cs.FlashFire);
            der.RaiseAbility();
            der.ShowLogPm("FlashFire");
            return false;
          }
          break;
        case As.WONDER_GUARD: //25
          if ((move.Move.Category != MoveCategory.Status || move.Id == Ms.THUNDER_WAVE) && def.AtkContext.Type.EffectRevise(der.OnboardPokemon.Types) <= 0)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.LIGHTNINGROD: //31
          if (!NoEffectWithLv7DUp(def, BattleType.Electric, StatType.SpAtk)) return false;
          break;
        case As.MOTOR_DRIVE: //78
          if (!NoEffectWithLv7DUp(def, BattleType.Electric, StatType.Speed)) return false;
          break;
        case As.STORM_DRAIN: //114
          if (!NoEffectWithLv7DUp(def, BattleType.Water, StatType.SpAtk)) return false;
          break;
        case As.SAP_SIPPER: //157
          if (!NoEffectWithLv7DUp(def, BattleType.Grass, StatType.Atk)) return false;
          break;
        case As.SOUNDPROOF: //43
          if (move.Sound)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.STICKY_HOLD: //60
          var m = move.Id;
          if (m == Ms.THIEF || m == Ms.TRICK || m == Ms.COVET || m == Ms.SWITCHEROO)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.TELEPATHY: //140
          if ((move.Move.Category != MoveCategory.Status || move.Id == Ms.THUNDER_WAVE) && def.AtkContext.Attacker.Pokemon.TeamId == der.Pokemon.TeamId)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.BULLETPROOF:
          if (move.Bullet)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.Queenly_Majesty:
          if (def.AtkContext.Attacker.Priority>0)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
        case As.OVERCOAT:
          if (move.Powder)
          {
            der.RaiseAbility();
            der.NoEffect();
            return false;
          }
          break;
      }
      if (move.Mental)
        foreach(var p in der.Field.Pokemons)
          if (p.RaiseAbility(As.AROMA_VEIL))
          {
            def.Defender.NoEffect();
            return false;
          }
      if (move.Powder && der.ItemE(Is.SAFETY_GOGGLES))
      {
        def.Defender.ShowLogPm("SafetyGoggles");
        return false;
      }
      //gen7
      if (def.AtkContext.Attacker.Priority > 0&&def.AtkContext.Attacker.Pokemon.TeamId!=def.Defender.Pokemon.TeamId)
        foreach(var p in der.Field.Pokemons)
          if (p.RaiseAbility(As.Dazzling))
          {
            def.Defender.NoEffect();
            return false;
          }
      if (def.AtkContext.Attacker.Priority > 0 && def.AtkContext.Controller.Board.HasCondition(Cs.PsychicTerrain) && HasEffect.IsGroundAffectable(der, true, false) && der!=def.AtkContext.Attacker)
       {
          def.Defender.NoEffect();
          return false;
       }      
            
      return true;
    }

    private static bool NoEffectWithAbsorb(DefContext def, BattleType type)
    {
      if ((def.AtkContext.Move.Move.Category != MoveCategory.Status || def.AtkContext.Move.Id == Ms.THUNDER_WAVE) && def.AtkContext.Type == type)
      {
        var der = def.Defender;
        der.RaiseAbility();
        if (der.Hp == der.Pokemon.MaxHp) der.NoEffect();
        else der.HpRecoverByOneNth(4);
        return false;
      }
      return true;
    }
    private static bool NoEffectWithLv7DUp(DefContext def, BattleType type, StatType stat)
    {
      if (def.AtkContext.Type == type)
      {
        var der = def.Defender;
        der.RaiseAbility();
        if (!der.ChangeLv7D(der, stat, 1, false)) der.NoEffect();
        return false;
      }
      return true;
    }
  }
}
