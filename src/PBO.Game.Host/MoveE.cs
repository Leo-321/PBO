using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    internal static class MoveE
    {
        public static void MagicCoat(AtkContext atk)
        {
            var list = atk.GetCondition<List<PokemonProxy>>(Cs.MagicCoat);
            if (list != null)
            {
                atk.RemoveCondition(Cs.MagicCoat);
                foreach (var d in list)
                {
                    var a = new AtkContext(d);
                    a.SetCondition(Cs.IgnoreMagicCoat);
                    a.StartExecute(atk.Move, atk.Attacker.Tile, d.RaiseAbility(As.MAGIC_BOUNCE) ? "MagicBounce" : "MagicCoat");
                    if (atk.Target == null) break;
                }
            }
        }

        public static IEnumerable<Tile> GetRangeTiles(AtkContext atk, MoveRange range, Tile select)
        {
            var aer = atk.Attacker;
            IEnumerable<Tile> targets = null;
            Board b = aer.Controller.Board;
            var remote = atk.Move.IsRemote;
            var team = aer.Pokemon.TeamId;
            var oTeam = 1 - team;
            var x = aer.OnboardPokemon.X;
            var ox = aer.Controller.GameSettings.Mode.XBound() - 1 - x;
            switch (range)
            {
                case MoveRange.TeamField: //do nothing
                case MoveRange.OpponentField: //do nothing
                case MoveRange.Board: //do nothing
                case MoveRange.TeamPokemons: //防音防不住治愈铃铛，所以这只是个摆设
                    break;
                case MoveRange.Adjacent:
                    {
                        var ts = new List<Tile>();
                        Tile t;
                        t = b[team][x - 1]; if (t != null) ts.Add(t);
                        t = b[team][x + 1]; if (t != null) ts.Add(t);
                        t = b[oTeam][ox - 1]; if (t != null) ts.Add(t);
                        ts.Add(b[oTeam][ox]);
                        t = b[oTeam][ox + 1]; if (t != null) ts.Add(t);
                        targets = ts;
                    }
                    break;
                case MoveRange.OpponentPokemons:
                    {
                        var ts = new List<Tile>();
                        Tile t;
                        t = b[oTeam][ox - 1]; if (t != null) ts.Add(t);
                        ts.Add(b[oTeam][ox]);
                        t = b[oTeam][ox + 1]; if (t != null) ts.Add(t);
                        targets = ts;
                    }
                    break;
                case MoveRange.All:
                    targets = b.Tiles;
                    break;
                case MoveRange.SelectedTeammate:
                    if (select == null)
                    {
                        var pms = new List<PokemonProxy>();
                        foreach (var pm in remote ? b[team].Pokemons : b[team].GetAdjacentPokemonsByX(x))
                            if (pm != aer) pms.Add(pm);
                        targets = pms.Any() ? new Tile[] { pms[aer.Controller.GetRandomInt(0, pms.Count - 1)].Tile } : Enumerable.Empty<Tile>();
                    }
                    else targets = new Tile[] { select };
                    break;
                case MoveRange.RandomOpponentPokemon:
                    {
                        var pms = (List<PokemonProxy>)(remote? b[oTeam].Pokemons : b[oTeam].GetAdjacentPokemonsByX(ox));
                        targets = pms.Any() ? new Tile[] { pms[aer.Controller.GetRandomInt(0, pms.Count - 1)].Tile } : Enumerable.Empty<Tile>();
                    }
                    break;
                case MoveRange.SelectedTarget:
                    if (select == null || select.Field.Team == oTeam) goto case MoveRange.SelectedOpponent;
                    targets = new Tile[] { select };
                    break;
                case MoveRange.SelectedOpponent:
                    //非鬼系选诅咒后变诅咒随机对方一个精灵
                    if (select == null) goto case MoveRange.RandomOpponentPokemon;
                    //因为移动或交换位置造成距离不足，技能失败
                    if (!(remote || ox - 1 <= select.X && select.X <= ox + 1)) targets = Enumerable.Empty<Tile>();
                    if (select.Pokemon != null) targets = new Tile[] { select };
                    else if (b[oTeam][ox].Pokemon != null) targets = new Tile[] { b[oTeam][ox] }; //据说正对面精灵优先
                    else goto case MoveRange.RandomOpponentPokemon;
                    break;
                case MoveRange.Self: //done?
                    targets = new Tile[] { aer.Tile };
                    break;
                case MoveRange.RandomTeamPokemon:
                    {
                        var pms = (List<PokemonProxy>)(remote ? b[team].Pokemons : b[team].GetAdjacentPokemonsByX(x));
                        targets = new Tile[] { pms[aer.Controller.GetRandomInt(0, pms.Count - 1)].Tile };
                    }
                    break;
            }
            return targets;
        }
        /// <summary>
        /// check canwithdraw first, null log to show nothing
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="ability"></param>
        /// <param name="log"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public static bool ForceSwitchImplement(PokemonProxy pm, bool ability)
        {
            if (ability && pm.RaiseAbility(As.SUCTION_CUPS))
            {
                pm.ShowLogPm("SuctionCups"); 
                return false;
            }
            if (pm.OnboardPokemon.HasCondition(Cs.Ingrain))
            {
                pm.ShowLogPm("IngrainCantMove");
                return false;
            }
            var c = pm.Controller;
            var sendouts = new List<int>();
            {
                var pms = pm.Pokemon.Owner.Pokemons.ToArray();
                for (int i = pm.Controller.GameSettings.Mode.OnboardPokemonsPerPlayer(); i < pms.Length; ++i)
                    if (c.CanSendOut(pms[i])) sendouts.Add(i);
            }
            var tile = pm.Tile;
            ITs.Attach(pm);
            c.Withdraw(pm, Ls.forceWithdraw, 0, false);
            tile.WillSendOutPokemonIndex = sendouts[c.GetRandomInt(0, sendouts.Count - 1)];
            c.SendOut(tile, true, "ForceSendOut");
            return true;
        }

        public static void BuildDefContext(AtkContext atk, Tile select)
        {
            switch (atk.Move.Id)
            {
                case Ms.COUNTER: //68
                    Counter(atk, Cs.PhysicalDamage);
                    break;
                case Ms.MIRROR_COAT: //243
                    Counter(atk, Cs.SpecialDamage);
                    break;
                case Ms.METAL_BURST: //368
                    Counter(atk, Cs.Damage);
                    break;
                case Ms.BIDE:
                    if (atk.GetCondition(Cs.MultiTurn).Turn == 1)
                    {
                        var o = atk.GetCondition(Cs.Bide);
                        var targets = new List<DefContext>();
                        if (o.By != null)
                        {
                            var t = GetRangeTiles(atk, MoveRange.SelectedTarget, o.By.Tile).FirstOrDefault();
                            if (t != null && t.Pokemon != null) targets.Add(new DefContext(atk, t.Pokemon));
                        }
                        if (!targets.Any()) atk.Attacker.ShowLogPm("UseMove", Ms.BIDE); //奇葩的战报
                        atk.SetTargets(targets);
                    }
                    break;
                default:
                    IEnumerable<Tile> ts = GetRangeTiles(atk, atk.Move.GetRange(atk.Attacker), select);
                    if (ts != null)
                    {
                        var targets = new List<DefContext>();
                        foreach (Tile t in ts)
                            if (t.Pokemon != null) targets.Add(new DefContext(atk, t.Pokemon));
                        atk.SetTargets(targets);
                    }
                    break;
            }
        }
        private static void Counter(AtkContext atk, Cs condition)
        {
            var o = atk.Attacker.OnboardPokemon.GetCondition(condition);
            if (o != null)
            {
                var pmTile = o.ByTile;
                if (pmTile != null && pmTile.Pokemon.Pokemon.TeamId != atk.Attacker.Pokemon.TeamId)
                {
                    atk.SetTargets(new DefContext[] { new DefContext(atk, pmTile.Pokemon) });
                    return;
                }
            }
            atk.SetTargets(new DefContext[0]);
        }

        #region CalculateTargets
        public static void FilterDefContext(AtkContext atk)
        {
            if ((atk.Move.Id == Ms.FUTURE_SIGHT || atk.Move.Id == Ms.DOOM_DESIRE) && !atk.HasCondition(Cs.FSDD)) return;
            if (atk.Targets == null) return;
            var move = atk.Move;
            var aer = atk.Attacker;

            if (move.GetRange(aer) == MoveRange.SelectedTarget)
            {
                var all = atk.Move.IsRemote || aer.Controller.GameSettings.Mode != GameMode.Triple;
                PokemonProxy retarget = null;
                var rp = !(aer.OnboardPokemon.HasType(BattleType.Grass) || aer.AbilityE(As.OVERCOAT) || aer.ItemE(Is.SAFETY_GOGGLES));
                foreach (var pm in atk.Controller.OnboardPokemons)
                    if (pm.Pokemon.TeamId != aer.Pokemon.TeamId && pm != atk.Target.Defender && (all || aer.OnboardPokemon.X == 1 || aer.OnboardPokemon.X != pm.OnboardPokemon.X))
                    {
                        var fm = pm.OnboardPokemon.GetCondition<int>(Cs.FollowMe);
                        if (fm != 0 && (rp || fm != Ms.RAGE_POWDER))
                        {
                            retarget = pm;
                            break;
                        }
                    }
                if (retarget == null)
                {
                    int ab = 0;
                    if (atk.Type == BattleType.Electric) ab = As.LIGHTNINGROD;
                    else if (atk.Type == BattleType.Water) ab = As.STORM_DRAIN;
                    if (ab != 0)
                        foreach (var pm in atk.Controller.Board.Pokemons)
                            if (pm != aer && pm != atk.Target.Defender && (all || aer.OnboardPokemon.X == 1 || pm.OnboardPokemon.X == 1 || aer.Pokemon.TeamId != pm.Pokemon.TeamId && aer.OnboardPokemon.X != pm.OnboardPokemon.X) && pm.RaiseAbility(ab))
                            {
                                retarget = pm;
                                break;
                            }
                }
                if (retarget != null)
                {
                    retarget.ShowLogPm("ReTarget");
                    atk.SetTargets(new DefContext[] { new DefContext(atk, retarget) });
                }
            }

            List<DefContext> targets = atk.Targets.ToList();

            #region Check CoordY
            {
                var count = 0;
                bool allhit = true;
                foreach (DefContext def in targets.ToArray())
                {
                    ++count;
                    if (!(def.Defender.CoordY == CoordY.Plate || def.NoGuard || IsYInRange(def)))
                    {
                        def.Defender.ShowLogPm(Ls.Miss);
                        targets.Remove(def);
                        allhit = false;
                    }
                }
                atk.Attacker.LastMiss = !allhit;
                if (count > 1) atk.MultiTargets = true;
            }
            #endregion
            #region Attack Move and Thunder Wave: Check for Immunity (or Levitate) on the Ally side, position 1, then position 3. Then check Opponent side, position 1, then 2, then 3,
            foreach (DefContext def in targets.ToArray())
                if (!HasEffect.Execute(def))
                {
                    targets.Remove(def);
                    def.Defender.NoEffect();
                }
            #endregion
            #region WideGuard QuickGuard CraftyShield MatBlock
            if (move.Move.Category != MoveCategory.Status && move.Move.Range != MoveRange.SelectedTarget)
                foreach (var def in targets.ToArray())
                    if (def.Defender.Field.HasCondition(Cs.WideGuard))
                    {
                        def.Defender.ShowLogPm("WideGuard");
                        targets.Remove(def);
                    }
            if (aer.Priority > 0 && move.Id != Ms.FEINT)
                foreach (var def in targets.ToArray())
                    if (def.Defender.Field.HasCondition(Cs.QuickGuard))
                    {
                        def.Defender.ShowLogPm("QuickGuard");
                        targets.Remove(def);
                    }
            if (move.Move.Category == MoveCategory.Status)
            {
                foreach (var def in targets.ToArray())
                    if (def.Defender.Field.HasCondition(Cs.CraftyShield))
                    {
                        def.Defender.ShowLogPm("CraftyShield");
                        targets.Remove(def);
                    }
            }
            else
            {
                var d0 = targets.FirstOrDefault();
                if (d0 != null && d0.Defender.Field.HasCondition(Cs.MatBlock))
                {
                    d0.Defender.Controller.ReportBuilder.ShowLog("MatBlock", move.Id);
                    var td = d0.Defender.Pokemon.TeamId;
                    foreach (var d in targets.ToArray())
                        if (d.Defender.Pokemon.TeamId == td) targets.Remove(d);
                    d0 = targets.FirstOrDefault();
                    if (d0 != null && d0.Defender.Field.HasCondition(Cs.MatBlock)) targets.Clear();
                }
            }
            #endregion
            #region Protect KingsShield SpikyShield
            if (move.Protectable)
            {
                foreach (DefContext d in targets.ToArray())
                    if (d.Defender.OnboardPokemon.HasCondition(Cs.Protect))
                    {
                        d.Defender.ShowLogPm("Protect");
                        targets.Remove(d);
                    }
                foreach (var d in targets.ToArray())
                    if (d.Defender.OnboardPokemon.HasCondition(Cs.SpikyShield))
                    {
                        d.Defender.ShowLogPm("Protect");
                        if (atk.touch) aer.EffectHurtByOneNth(8);
                        targets.Remove(d);
                    }
                foreach (var d in targets.ToArray())
                    if (d.Defender.OnboardPokemon.HasCondition(Cs.Baneful_Bunker))
                    {
                        d.Defender.ShowLogPm("Protect");
                        if (atk.touch) aer.AddState(d.Defender, AttachedState.PSN, true);
                        targets.Remove(d);
                    }
            }
            if (move.Protectable && move.Move.Category != MoveCategory.Status)
            {
                foreach (var d in targets.ToArray())
                    if (d.Defender.OnboardPokemon.HasCondition(Cs.KingsShield))
                    {
                        d.Defender.ShowLogPm("Protect");
                        if (atk.touch) aer.ChangeLv7D(d.Defender, StatType.Atk, -2, false);
                        targets.Remove(d);
                    }
            }
            #endregion
            #region Check for Telepathy (and possibly other abilities)
            {
                var mc = move.MagicCoat && !atk.HasCondition(Cs.IgnoreMagicCoat);
                var ab = atk.DefenderAbilityAvailable();
                foreach (DefContext def in targets.ToArray())
                    if (def.Defender != atk.Attacker && (mc && STs.MagicCoat(atk, def.Defender) || ab && !CanImplement.Execute(def))) targets.Remove(def);
            }
            #endregion
            if (move.Move.Category == MoveCategory.Status && !atk.IgnoreSubstitute())
                foreach (DefContext d in targets.ToArray())
                    if (d.Defender != aer && d.Defender.OnboardPokemon.HasCondition(Cs.Substitute))
                    {
                        d.Fail();
                        targets.Remove(d);
                    }
            if (move.Id == Ms.SKY_DROP)
                foreach (var d in targets.ToArray())
                    if (d.Defender.OnboardPokemon.Weight >= 200)
                    {
                        d.Fail();
                        targets.Remove(d);
                    }
            #region Check for misses
            if (!MustHit(atk))
            {
                if (move.Class != MoveClass.OHKO) atk.AccuracyModifier = STs.AccuracyModifier(atk);
                bool allhit = true;
                foreach (DefContext def in targets.ToArray())
                    if (!(MustHit(def) || CanHit(def)))
                    {
                        targets.Remove(def);
                        def.Defender.ShowLogPm(Ls.Miss);
                        allhit = false;
                    }
                atk.Attacker.LastMiss = allhit;
            }
            #endregion
            atk.SetTargets(targets);
        }
        private static bool IsYInRange(DefContext def)
        {
            var y = def.Defender.CoordY;
            var m = def.AtkContext.Move.Id;
            return
              y == CoordY.Plate ||
              y == CoordY.Water && (m == Ms.SURF || m == Ms.WHIRLPOOL) ||
              y == CoordY.Underground && (m == Ms.EARTHQUAKE || m == Ms.FISSURE) ||
              y == CoordY.Air && (m == Ms.GUST || m == Ms.TWISTER || m == Ms.THUNDER || m == Ms.HURRICANE || m == Ms.SKY_UPPERCUT);
        }
        public static bool CanHit(DefContext def)
        {
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var c = atk.Controller;
            var move = atk.Move;
            int acc;
            if (move.Class == MoveClass.OHKO)
            {
                acc = move.Move.Accuracy + aer.Pokemon.Lv - def.Defender.Pokemon.Lv;
                if (move.Id == Ms.SHEER_COLD && !aer.OnboardPokemon.Types.Contains(BattleType.Ice)) acc = acc - 10;
            }
            else
            {
                int lv;
                if (def.AbilityE(As.UNAWARE)) lv = 0;
                else lv = aer.OnboardPokemon.AccuracyLv;
                //如果攻击方是天然特性，防御方的回避等级按0计算。 
                //循序渐进无视防御方回避等级。
                //将攻击方的命中等级减去防御方的回避等级。 
                if (!(move.IgnoreDefenderLv7D || aer.AbilityE(As.UNAWARE) || aer.AbilityE(As.KEEN_EYE))) lv -= def.Defender.OnboardPokemon.EvasionLv;
                if (lv < -6) lv = -6;
                else if (lv > 6) lv = 6;
                //用技能基础命中乘以命中等级修正，向下取整。
                int numerator = 3, denominator = 3;
                if (lv > 0) numerator += lv;
                else denominator -= lv;
                acc = (c.Weather == Weather.IntenseSunlight && (move.Id == Ms.THUNDER || move.Id == Ms.HURRICANE) ? 50 : atk.Move.Move.Accuracy) * numerator / denominator;
                acc *= AccuracyModifier.Execute(def);
            }
            //产生1～100的随机数，如果小于等于命中，判定为命中，否则判定为失误。
            return c.RandomHappen(acc);
        }
        public static bool MustHit(AtkContext atk)
        {
            var m = atk.Move.Id;
            return
              atk.Move.Move.Accuracy == 0
              || atk.Attacker.AbilityE(As.NO_GUARD)
              || (m == Ms.THUNDER || m == Ms.HURRICANE) && atk.Controller.Weather == Weather.Rain
              || m == Ms.BLIZZARD && atk.Controller.Weather == Weather.Hailstorm
              || atk.Attacker.OnboardPokemon.HasType(BattleType.Poison) && m == Ms.TOXIC;
        }
        public static bool MustHit(DefContext def)
        {
            var m = def.AtkContext.Move.Id;
            return
              def.NoGuard
              || (m == Ms.STOMP || m == Ms.DRAGON_RUSH || m == Ms.STEAMROLLER || m == Ms.PHANTOM_FORCE || m == Ms.FLYING_PRESS) && def.Defender.OnboardPokemon.HasCondition(Cs.Minimize);
        }
        #endregion

        public static void MoveEnding(AtkContext atk)
        {
            var aer = atk.Attacker;

            if (atk.Move.Id == Ms.SPIT_UP || atk.Move.Id == Ms.SWALLOW)
            {
                int i = aer.OnboardPokemon.GetCondition<int>(Cs.Stockpile);
                aer.ChangeLv7D(atk.Attacker, false, false, 0, -i, 0, -i);
                aer.OnboardPokemon.RemoveCondition(Cs.Stockpile);
                aer.ShowLogPm("DeStockpile");
            }

            MagicCoat(atk);

            atk.SetAttackerAction(atk.Move.StiffOneTurn ? PokemonAction.Stiff : PokemonAction.Done);
            if (atk.Targets != null)
                foreach (var d in atk.Targets)
                {
                    ITs.Attach(d.Defender);
                    ATs.RecoverAfterMoldBreaker(d.Defender);
                }
            ITs.Attach(atk.Attacker); //先树果汁后PP果

            var c = aer.Controller;
            {
                var o = atk.GetCondition(Cs.MultiTurn);
                if (o != null)
                {
                    o.Turn--;
                    if (o.Turn != 0) atk.SetAttackerAction(PokemonAction.Moving);
                    else if (o.Bool) aer.AddState(aer, AttachedState.Confuse, false, 0, "EnConfuse2");
                }
            }
            {
                var o = atk.GetCondition<Tile>(Cs.EjectButton);
                if (o != null)
                {
                    c.PauseForSendOutInput(o);
                    return;
                }
            }
            {
                var tile = aer.Tile;
                if (atk.Move.Switch && tile != null)
                {
                    c.Withdraw(aer, "SelfWithdraw", 0, true);
                    c.PauseForSendOutInput(tile);
                }
            }
        }
    }
}
