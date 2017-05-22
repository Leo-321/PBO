using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class DamageModifier
  {
    public static Modifier Execute(DefContext def)
    {
      var der = def.Defender;
      var atk = def.AtkContext;
      var move = atk.Move;
      var aer = atk.Attacker;

      //If the target's side is affected by Reflect, the move used was physical, the user's ability isn't Infiltrator and the critical hit flag isn't set. 
      //The value of the modificator is 0xA8F if there is more than one Pokemon per side of the field and 0x800 otherwise.
      //Same as above with Light Screen and special moves.
      Modifier m = (Modifier)(!def.IsCt && (der.Pokemon.TeamId == aer.Pokemon.TeamId || !aer.AbilityE(As.INFILTRATOR)) &&
        (move.Move.Category == MoveCategory.Physical && der.Field.HasCondition(Cs.Reflect) || move.Move.Category == MoveCategory.Special && der.Field.HasCondition(Cs.LightScreen)) ?
        atk.MultiTargets ? 0xA8F : 0x800 : 0x1000);
      m*= (Modifier)(!def.IsCt && (der.Pokemon.TeamId == aer.Pokemon.TeamId || !aer.AbilityE(As.INFILTRATOR)) &&
        (move.Move.Category == MoveCategory.Physical && der.Field.HasCondition(Cs.Aurora_Veil) || move.Move.Category == MoveCategory.Special && der.Field.HasCondition(Cs.Aurora_Veil)) ?
        atk.MultiTargets ? 0xA8F : 0x800 : 0x1000);

            {
        //If the target's ability is Multiscale and the target is at full health.
        m *= (Modifier)((def.AbilityE(As.MULTISCALE) || def.AbilityE(As.Shadow_Shield)) && der.Hp == der.Pokemon.MaxHp ? 0x800 : 0x1000);
        //If the user's ability is Tinted Lens and the move wasn't very effective.
        if (def.EffectRevise < 0 && aer.AbilityE(As.TINTED_LENS)) m *= 0x2000;
        //If one of the target's allies' ability is Friend Guard.
        foreach (PokemonProxy pm in der.Controller.GetOnboardPokemons(der.Pokemon.TeamId))
          if (pm != der && pm.AbilityE(As.FRIEND_GUARD)) m *= 0xC00;
        //If user has ability Sniper and move was a critical hit.
        if (def.IsCt && aer.AbilityE(As.SNIPER)) m *= 0x1800;
        //If the target's ability is Solid Rock or Filter and the move was super effective.
        if (def.EffectRevise > 0 && (def.AbilityE(As.FILTER) || def.AbilityE(As.SOLID_ROCK) || def.AbilityE(As.Prism_Armor))) m *= 0xC00;
        if (atk.Hit == 2 && atk.HasCondition(Cs.ParentalBond)) m *= 0x400;
      }

      switch (aer.Item)
      {
        //If the user is holding an expert belt and the move was super effective.
        case Is.EXPERT_BELT:
          if (def.EffectRevise > 0) m *= 0x1333;
          break;
        //If the user is holding a Life Orb.
        case Is.LIFE_ORB:
          m *= 0x14cc;
          break;
        //If the user is holding the item Metronome. If n is the number of time the current move was used successfully and successively, the value of the modifier is 0x1000+n*0x333 if n≤4 and 0x2000 otherwise.
        case Is.METRONOME:
          var c = aer.OnboardPokemon.GetCondition(Cs.LastMove);
          if (c != null && move == c.Move)
          {
            if (c.Int < 5) m *= (ushort)(0x1000 + c.Int * 0x333);
            else m *= 0x2000;
          }
          break;
      }

      //If the target is holding a damage lowering berry of the attack's type.
      {
        var item = der.Item;
        if (
          item == Is.CHILAN_BERRY && atk.Type == BattleType.Normal ||
          atk.Type == AntiBerry(item) && def.EffectRevise > 0
          )
        {
          def.SetCondition(Cs.Antiberry);
          m *= 0x800;
        }
      }

      switch (move.Id)
      {
        case Ms.STOMP: //23
        case Ms.DRAGON_RUSH:
        case Ms.STEAMROLLER: //537
        case Ms.PHANTOM_FORCE:
        case Ms.FLYING_PRESS:
          if (der.OnboardPokemon.HasCondition(Cs.Minimize)) m *= 0x2000;
          break;
        case Ms.SURF:
        case Ms.WHIRLPOOL:
          if (der.CoordY == CoordY.Water) m *= 0x2000;
          break;
        case Ms.EARTH_POWER: //89
        case Ms.MAGNITUDE: //222
          if (der.CoordY == CoordY.Underground) m *= 0x2000;
          break;
        case Ms.KNOCK_OFF:
          if (der.Pokemon.Item != 0 && !ITs.NeverLostItem(der.Pokemon)) m *= 0x1800;
          break;
      }
      return m;
    }

    private static BattleType AntiBerry(int item)
    {
      switch (item)
      {
        case Is.OCCA_BERRY:
          return BattleType.Fire;
        case Is.PASSHO_BERRY:
          return BattleType.Water;
        case Is.WACAN_BERRY:
          return BattleType.Electric;
        case Is.RINDO_BERRY:
          return BattleType.Grass;
        case Is.YACHE_BERRY:
          return BattleType.Ice;
        case Is.CHOPLE_BERRY:
          return BattleType.Fighting;
        case Is.KEBIA_BERRY:
          return BattleType.Poison;
        case Is.SHUCA_BERRY:
          return BattleType.Ground;
        case Is.COBA_BERRY:
          return BattleType.Flying;
        case Is.PAYAPA_BERRY:
          return BattleType.Psychic;
        case Is.TANGA_BERRY:
          return BattleType.Bug;
        case Is.CHARTI_BERRY:
          return BattleType.Rock;
        case Is.KASIB_BERRY:
          return BattleType.Ghost;
        case Is.HABAN_BERRY:
          return BattleType.Dragon;
        case Is.COLBUR_BERRY:
          return BattleType.Dark;
        case Is.BABIRI_BERRY:
          return BattleType.Steel;
        case Is.ROSELI_BERRY:
          return BattleType.Fairy;
      }
      return BattleType.Invalid;
    }
  }
}
