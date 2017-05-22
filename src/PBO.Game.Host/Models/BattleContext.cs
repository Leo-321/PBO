using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    internal class AtkContext : ConditionalObject
    {
        public readonly PokemonProxy Attacker;
        public readonly MoveProxy MoveProxy; //压力、诅咒身躯，针对一开始选的技能
        public BattleType Type;
        public int Pressure;
        public Modifier AccuracyModifier;
        public int TotalDamage;
        public bool Fail;
        public bool IgnoreSwitchItem;
        public MoveTypeE Move;
        public bool MultiTargets;
        public int Hits;
        /// <summary>
        /// 1, 2, 3...
        /// </summary>
        public int Hit;
        public bool touch
        { get { return Move.NeedTouch && Attacker.Ability != As.Long_Reach && !Attacker.ItemE(Is.Protective_Pads); } }

        public AtkContext(MoveProxy mp)
        {
            MoveProxy = mp;
            Attacker = mp.Owner;
        }
        public AtkContext(PokemonProxy pm)
        {
            Attacker = pm;
        }

        public Controller Controller
        { get { return Attacker.Controller; } }
        /// <summary>
        /// null: 不需要目标的技能 empty: 技能需要目标但是没有目标
        /// </summary>
        public IEnumerable<DefContext> Targets
        { get; private set; }
        public DefContext Target
        { get; private set; }

        public void SetAttackerAction(PokemonAction action)
        {
            //预知未来、破灭之愿的攻击方已下场，不应改变Action
            if (Attacker.AtkContext == this) Attacker.Action = action;
        }

        private MoveTypeE transmove;
        private bool trans;
        public MoveType BaseOfZmove
        { get { if (trans) return transmove.Move; else return MoveProxy.Move.Type; } }

        public void StartExecute(MoveTypeE move, Tile selectTile = null, string log = "UseMove",bool canChangeZmove=true)
        {
            Move = move;
            if (Attacker.SelectZmove && canChangeZmove && !move.Zmove && move.Move.Category != MoveCategory.Status)
            {
                Move = MoveTypeE.Get(GameHelper.CommonZmove(Move.Move));
                trans = true;
                transmove = move;
            }
            else
                trans = false;
            if (STs.CanExecuteMove(Attacker, Move))
            {
                InitAtkContext.Execute(this);
                MoveE.BuildDefContext(this, selectTile); //蓄力技如果选择的是压力特性的精灵，在第一回合就会扣取2点PP，即使最后攻击到的不是压力特性的精灵。
                if (MoveProxy != null) ATs.Pressure(this, Move.GetRange(Attacker));
                if (log == "FSDD")
                    Target.Defender.ShowLogPm(log, Move.Id);
                else if (log != null) Attacker.ShowLogPm(log, Move.Id);
                MoveExecute.Execute(this,selectTile);
            }
            else FailAll(null);
        }
        public void ContinueExecute(Tile selectTile)
        {
            TotalDamage = 0;
            if (Move.Id != Ms.SKY_DROP) MoveE.BuildDefContext(this, selectTile); //如果被带入空中的精灵被打死
            MoveExecute.Execute(this);
        }
        public void SetTargets(IEnumerable<DefContext> targets)
        {
            Targets = targets;
            Target = targets.FirstOrDefault();
        }
        public void ImplementPressure()
        {
            if (MoveProxy != null)
            {
                MoveProxy.PP -= Pressure;
                Pressure = 0;
            }
        }
        public bool RandomHappen(int percentage)
        {
            if (percentage == 0) return true;
            var a = Attacker.Ability;
            return !(Attacker.AbilityE(As.SHEER_FORCE)&& Move.HasProbabilitiedAdditonalEffects) && Controller.RandomHappen(Attacker.AbilityE(As.SERENE_GRACE) ? percentage * 3 : Attacker.Field.HasCondition(Cs.Rainbow) ? percentage * 2 : percentage);
        }
        public void FailAll(string log = Ls.Fail0, int arg0 = 0, int arg1 = 0)
        {
            Fail = true;
            SetAttackerAction(PokemonAction.Done);
            if (log != null) Controller.ReportBuilder.ShowLog(log, arg0, arg1);
        }
    }
    internal class DefContext : ConditionalObject
    {
        public readonly AtkContext AtkContext;
        public readonly PokemonProxy Defender;
        public int Damage;
        public int BasePower;
        public bool IsCt;
        public bool HitSubstitute;
        /// <summary>
        /// bit operation, -2, -1, 0, 1, 2
        /// </summary>
        public int EffectRevise;

        public DefContext(AtkContext a, PokemonProxy pm)
        {
            AtkContext = a;
            Defender = pm;
        }

        /// <summary>
        /// 无防御、心眼、锁定
        /// </summary>
        public bool NoGuard
        {
            get
            {
                if (AtkContext.Attacker.AbilityE(As.NO_GUARD) || Defender.AbilityE(As.NO_GUARD)) return true;
                Condition c = Defender.OnboardPokemon.GetCondition(Cs.NoGuard);
                return c != null && c.By == AtkContext.Attacker && c.Turn == Defender.Controller.TurnNumber;
            }
        }

        public int Ability
        { get { return (AtkContext.DefenderAbilityAvailable()|| Defender.AbilityE(As.Full_Metal_Body) || Defender.AbilityE(As.Shadow_Shield) || Defender.AbilityE(As.Prism_Armor)) ? Defender.Ability : 0; } }
        public bool AbilityE(int ability)
        {
            return (Defender.AbilityE(ability) && (AtkContext.DefenderAbilityAvailable() || Defender.AbilityE(As.Full_Metal_Body) || Defender.AbilityE(As.Shadow_Shield) || Defender.AbilityE(As.Prism_Armor)));
        }

        public bool RandomHappen(int percentage)
        {
            return percentage == 0 || !AbilityE(As.SHIELD_DUST) && AtkContext.RandomHappen(percentage);
        }

        public void MoveHurt()
        {
            Damage = Defender.MoveHurt(Damage, AtkContext.DefenderAbilityAvailable());
            {
                var o = new Condition();
                o.Damage = Damage;
                o.By = AtkContext.Attacker;
                o.ByTile = AtkContext.Attacker.Tile;
                var c = AtkContext.Move.Move.Category == MoveCategory.Physical ? Cs.PhysicalDamage : Cs.SpecialDamage;
                Defender.OnboardPokemon.SetTurnCondition(c, o);
                Defender.OnboardPokemon.SetTurnCondition(Cs.Damage, o);
            }
        }
        public void ModifyDamage(Modifier modifier)
        {
            Damage *= modifier;
        }
        public void Fail()
        {
            Defender.ShowLogPm("Fail");
        }
    }
}
