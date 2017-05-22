using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class Attacked
  {
    public static void Execute(DefContext def)
    {
      var der = def.Defender;
      var atk = def.AtkContext;
      var aer = atk.Attacker;
      var touch = atk.touch;
      var realHurt = def.Damage != 0;

      if (touch && aer.AbilityE(As.POISON_TOUCH) && der.Controller.RandomHappen(30) && der.CanAddState(aer, AttachedState.PSN, false))
      {
        aer.RaiseAbility();
        der.AddState(aer, AttachedState.PSN, false);
      }
      if (der.AtkContext != null && der.AtkContext.HasCondition(Cs.Bide))
      {
        var o = der.AtkContext.GetCondition(Cs.Bide);
        o.By = aer;
        o.Damage += def.Damage;
      }
      switch (der.OnboardPokemon.Ability) //此时破格不能无视
      {
        case As.ILLUSION:
          ATs.DeIllusion(def.Defender);
          break;
        case As.STATIC:
          if (touch) AddState(def, AttachedState.PAR);
          break;
        case As.POISON_POINT:
          if (touch) AddState(def, AttachedState.PSN);
          break;
        case As.FLAME_BODY:
          if (touch) AddState(def, AttachedState.BRN);
          break;
        case As.CUTE_CHARM:
          if (touch) AddState(def, AttachedState.Attract);
          break;
        case As.ROUGH_SKIN:
        case As.IRON_BARBS:
          if (touch) RoughSkin(def);
          break;
        case As.EFFECT_SPORE:
          if (touch && realHurt) EffectSpore(def);
          break;
        case As.ANGER_POINT:
          if (def.IsCt) der.ChangeLv7D(der, StatType.Atk, 12, false, true, "AngerPoint");
          break;
        case As.AFTERMATH:
          if (touch && der.Hp == 0 && aer.CanEffectHurt && aer.Controller.Board.Pokemons.HasAbility(As.DAMP) == null)
          {
            der.RaiseAbility();
            aer.EffectHurtByOneNthImplement(4);
          }
          break;
        case As.Innards_Out:
          if(der.Hp==0)
          {
            der.RaiseAbility();
            aer.EffectHurt(def.Damage);
          }
          break;
        case As.PICKPOCKET:
          if (touch) Pickpocket(def);
          break;
        case As.CURSED_BODY:
          if (atk.Controller.RandomHappen(30) && aer.CanAddState(der, AttachedState.Disable, false))
          {
            der.RaiseAbility();
            aer.AddState(der, AttachedState.Disable, false);
          }
          break;
        case As.WEAK_ARMOR:
          if (atk.Move.Move.Category == MoveCategory.Physical) der.ChangeLv7D(der, false, true, 0, -1, 0, 0, 1);
          break;
        case As.MUMMY:
          var aa = aer.Ability;
          if (touch && aa != As.MULTITYPE && aa!=As.RKS_System && aa!=As.Comatose && aa != As.MUMMY)
          {
            der.RaiseAbility();
            var fa = aer.OnboardPokemon.Ability;
            aer.ChangeAbility(As.MUMMY);
            aer.ShowLogPm(Ls.SetAbility, As.MUMMY);
            aer.Controller.ReportBuilder.ShowLog(Ls.setAbility, fa);
          }
          break;
        case As.JUSTIFIED:
          if (atk.Type == BattleType.Dark) der.ChangeLv7D(der, StatType.Atk, 1, false, true);
          break;
        case As.RATTLED:
          if (atk.Type == BattleType.Dark || atk.Type == BattleType.Ghost || atk.Type == BattleType.Bug) der.ChangeLv7D(der, StatType.Speed, 1, false, true);
          break;
        case As.GOOEY:
          if (touch && aer.CanChangeLv7D(der, StatType.Speed, -1, false) != 0)
          {
            der.RaiseAbility();
            aer.ChangeLv7D(der, StatType.Speed, -1, false);
          }
          break;
        //gen7
        case As.Stamina:
            der.ChangeLv7D(der, StatType.Def, 1, false, true);
          break;
        case As.Tangling_Hair:
            if(touch) aer.ChangeLv7D(der, StatType.Speed, -1, false);
            break;
        case As.Water_Compaction:
            if (atk.Type == BattleType.Water) der.ChangeLv7D(der, StatType.Def, 2, false, true);
          break;
        case As.Berserk:
            if (der.Hp < der.Pokemon.MaxHp / 2 && der.Hp + atk.TotalDamage >= der.Pokemon.MaxHp / 2) der.ChangeLv7D(der, StatType.SpAtk, 1, false, true);
          break;
      }

            var itm = def.Defender.Item;
            if (def.Defender.Pokemon.Item == Is.ROCKY_HELMET)
                itm = Is.ROCKY_HELMET;

      switch (itm)
      {
        case Is.STICKY_BARB: //65
          if (touch && aer.Pokemon.Item == 0 && der.Controller.RandomHappen(10))
          {
            der.RemoveItem();
            aer.SetItem(Is.STICKY_BARB);
          }
          break;
        case Is.ROCKY_HELMET: //104
          if (touch) aer.EffectHurtByOneNth(6, Ls.RockyHelmet, 0, 0);
          break;
        case Is.AIR_BALLOON: //105
          ITs.AirBalloon(def);
          break;
        case Is.ABSORB_BULB: //109
          AttackedUpItem(def, BattleType.Water, StatType.SpAtk);
          break;
        case Is.CELL_BATTERY: //110
          AttackedUpItem(def, BattleType.Electric, StatType.Atk);
          break;
        case Is.ENIGMA_BERRY: //188
          if (def.EffectRevise > 0) der.HpRecoverByOneNth(4, false, Ls.ItemHpRecover, Is.ENIGMA_BERRY, true);
          break;
        case Is.JABOCA_BERRY: //191
          ReHurtBerry(def, MoveCategory.Physical);
          break;
        case Is.ROWAP_BERRY: //192
          ReHurtBerry(def, MoveCategory.Special);
          break;
        case Is.LUMINOUS_MOSS:
          AttackedUpItem(def, BattleType.Water, StatType.SpDef);
          break;
        case Is.SNOWBALL:
          AttackedUpItem(def, BattleType.Ice, StatType.Atk);
          break;
        case Is.WEAKNESS_POLICY:
          if (def.EffectRevise > 0 && (der.CanChangeLv7D(der, StatType.Atk, 2, false) != 0 || der.CanChangeLv7D(der, StatType.SpAtk, 2, false) != 0))
          {
            der.ShowLogPm("WeaknessPolicy");
            der.ChangeLv7D(der, false, false, 2, 0, 2);
            der.ConsumeItem();
          }
          break;
        case Is.KEE_BERRY:
          AttackedUpItem(def, MoveCategory.Physical, StatType.Def);
          break;
        case Is.MARANGA_BERRY:
          AttackedUpItem(def, MoveCategory.Special, StatType.SpDef);
          break;
      }
      if (der.OnboardPokemon.HasCondition(Cs.Rage)) der.ChangeLv7D(der, StatType.Atk, 1, false, false, "Rage");
      if (aer.Pokemon.Item == 0 && ITs.CanLostItem(der) && aer.RaiseAbility(As.MAGICIAN))
      {
        var i = der.Pokemon.Item;
        aer.SetItem(i);
        der.RemoveItem();
        der.ShowLogPm("Magician", i);
      }
      if (der.OnboardPokemon.HasCondition(Cs.Beak_Blast) && atk.Move.NeedTouch) aer.AddState(der, AttachedState.BRN, false);
    }
    #region Gen5
    private static void AddState(DefContext def, AttachedState state)
    {
      if (def.AtkContext.Attacker.CanAddState(def.Defender, state, false) && def.AtkContext.Controller.RandomHappen(30))
      {
        def.Defender.RaiseAbility();
        def.AtkContext.Attacker.AddState(def.Defender, state, false, 0);
      }
    }
    private static void RoughSkin(DefContext def)
    {
      if (def.AtkContext.Attacker.CanEffectHurt)
      {
        def.Defender.RaiseAbility();
        def.AtkContext.Attacker.EffectHurtByOneNthImplement(8);
      }
    }
    private static void EffectSpore(DefContext d)
    {
      var a = d.AtkContext;
      if (a.Controller.RandomHappen(10))
      {
        var i = a.Controller.GetRandomInt(0, 2);
        var state = i == 0 ? AttachedState.PAR : i == 1 ? AttachedState.SLP : AttachedState.PSN;
        if (a.Attacker.CanAddState(d.Defender, state, false))
        {
          d.Defender.RaiseAbility();
          a.Attacker.AddState(d.Defender, state, false);
        }
      }
    }
    private static void Pickpocket(DefContext d)
    {
      var der = d.Defender;
      var aer = d.AtkContext.Attacker;
      if (der.Pokemon.Item == 0 && ITs.CanLostItem(aer))
      {
        var i = aer.Pokemon.Item;
        aer.RemoveItem();
        der.RaiseAbility();
        der.SetItem(i);
        der.ShowLogPm("Pickpocket", i);
      }
    }
    private static void ReHurtBerry(DefContext def, MoveCategory category)
    {
      var aer = def.AtkContext.Attacker;
      if (def.AtkContext.Move.Move.Category == category && aer.CanEffectHurt)
      {
        int hp = aer.Pokemon.MaxHp >> 3;
        if (hp == 0) hp = 1;
        aer.ShowLogPm("ReHurtItem", def.Defender.Id, def.Defender.Pokemon.Item);
        aer.Hp -= hp;
        def.Defender.ConsumeItem();
      }
    }
    #endregion
    private static void AttackedUpItem(DefContext def, BattleType type, StatType stat)
    {
      if (def.AtkContext.Type == type) ITs.ChangeLv5D(def.Defender, stat, 1);
    }
    private static void AttackedUpItem(DefContext def, MoveCategory cat, StatType stat)
    {
      if (def.AtkContext.Move.Move.Category == cat) ITs.ChangeLv5D(def.Defender, stat, 1);
    }
  }
}
