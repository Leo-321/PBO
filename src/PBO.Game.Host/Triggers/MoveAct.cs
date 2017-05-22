using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class MoveAct
    {
        public static void Execute(AtkContext atk)
        {
            var aer = atk.Attacker;
            var move = atk.Move;

            #region pre
            switch (move.Id)
            {
                case Ms.FLING:
                    aer.ShowLogPm("Fling", aer.Pokemon.Item);
                    break;
                case Ms.UPROAR:
                    if (atk.GetCondition(Cs.MultiTurn).Turn == 3)
                    {
                        atk.Attacker.ShowLogPm("EnUproar");
                        foreach (var p in atk.Controller.Board.Pokemons)
                            if (p.State == PokemonState.SLP) p.DeAbnormalState("UproarDeSLP");
                    }
                    break;
            }
            #endregion
            #region main
            switch (move.Id)
            {
                case Ms.MIST: //54
                    AddTeamCondition(atk, Cs.Mist);
                    break;
                case Ms.LIGHT_SCREEN: //113
                    AddTeamCondition(atk, Cs.LightScreen, aer.ItemE(Is.LIGHT_CLAY) ? 8 : 5);
                    break;
                case Ms.REFLECT: //115
                    AddTeamCondition(atk, Cs.Reflect, aer.ItemE(Is.LIGHT_CLAY) ? 8 : 5);
                    break;
                case Ms.Aurora_Veil:
                    if (aer.Controller.Weather == Weather.Hailstorm)
                        AddTeamCondition(atk, Cs.Aurora_Veil, aer.ItemE(Is.LIGHT_CLAY) ? 8 : 5);
                    else atk.FailAll();
                    break;
                case Ms.SAFEGUARD: //219
                    AddTeamCondition(atk, Cs.Safeguard);
                    break;
                case Ms.TAILWIND: //366
                    AddTeamCondition(atk, Cs.Tailwind, 4);
                    break;
                case Ms.LUCKY_CHANT: //381
                    AddTeamCondition(atk, Cs.LuckyChant);
                    break;
                case Ms.GROWTH: //74
                    {
                        var c = aer.Controller.Weather == Weather.IntenseSunlight ? 2 : 1;
                        aer.ChangeLv7D(aer, StatType.Atk, c, true);
                        aer.ChangeLv7D(aer, StatType.SpAtk, c, true);
                    }
                    break;
                case Ms.MIMIC: //102
                    Mimic(atk);
                    break;
                case Ms.FOCUS_ENERGY: //116
                    AddCondition(atk, Cs.FocusEnergy);
                    break;
                case Ms.IMPRISON: //286
                    AddCondition(atk, Cs.Imprison);
                    break;
                case Ms.AQUA_RING: //392
                    AddCondition(atk, Cs.AquaRing);
                    break;
                case Ms.HAZE: //114
                    foreach (var pm in atk.Controller.Board.Pokemons) pm.OnboardPokemon.SetLv7D(0, 0, 0, 0, 0, 0, 0);
                    aer.Controller.ReportBuilder.ShowLog("Haze");
                    break;
                case Ms.BIDE: //117
                    {
                        var turn = atk.GetCondition(Cs.MultiTurn).Turn;
                        if (turn == 1)
                        {
                            atk.Attacker.ShowLogPm("DeBide");
                            AttackMove(atk);
                            atk.RemoveCondition(Cs.Bide);
                        }
                        else if (turn == 2) atk.Attacker.ShowLogPm("Bide");
                    }
                    break;
                case Ms.TRANSFORM: //144
                    if (aer.CanTransform(atk.Target.Defender)) aer.Transform(atk.Target.Defender);
                    else atk.FailAll();
                    break;
                case Ms.SPLASH: //150
                    aer.Controller.ReportBuilder.ShowLog("Splash");
                    break;
                case Ms.REST: //156
                    Rest(atk);
                    break;
                case Ms.CONVERSION: //160
                    Conversion(atk);
                    break;
                case Ms.SUBSTITUTE: //164
                    Substitute(atk);
                    break;
                case Ms.SPIDER_WEB: //169
                case Ms.MEAN_LOOK: //212
                case Ms.BLOCK: //335
                    CantSelectWithdraw(atk);
                    break;
                case Ms.MIND_READER: //170
                case Ms.LOCKON: //199
                    LockOn(atk);
                    break;
                case Ms.CURSE: //174
                    Curse(atk);
                    break;
                case Ms.CONVERSION_2: //176
                    Conversion2(atk);
                    break;
                case Ms.SPITE: //180
                    Spite(atk);
                    break;
                case Ms.PROTECT: //182
                case Ms.DETECT: //197
                    SelfProtect(atk, Cs.Protect, "EnProtect");
                    break;
                case Ms.ENDURE: //203
                    SelfProtect(atk, Cs.Endure, "EnEndure");
                    break;
                case Ms.BELLY_DRUM: //187
                    BellyDrum(atk);
                    break;
                case Ms.SPIKES: //191
                    EntryHazards(atk, "EnSpikes");
                    break;
                case Ms.TOXIC_SPIKES: //390
                    EntryHazards(atk, "EnToxicSpikes");
                    break;
                case Ms.STEALTH_ROCK: //446
                    EntryHazards(atk, "EnStealthRock");
                    break;
                case Ms.STICKY_WEB:
                    EntryHazards(atk, "EnStickyWeb");
                    break;
                case Ms.DESTINY_BOND: //194
                    KOedCondition(atk, Cs.DestinyBond);
                    break;
                case Ms.GRUDGE: //288
                    KOedCondition(atk, Cs.Grudge);
                    break;
                case Ms.SANDSTORM: //201
                    WeatherMove(atk, Weather.Sandstorm);
                    break;
                case Ms.RAIN_DANCE: //240
                    WeatherMove(atk, Weather.Rain);
                    break;
                case Ms.SUNNY_DAY: //241
                    WeatherMove(atk, Weather.IntenseSunlight);
                    break;
                case Ms.HAIL: //258
                    WeatherMove(atk, Weather.Hailstorm);
                    break;
                case Ms.HEAL_BELL: //215
                    HealBell(atk, "HealBell");
                    break;
                case Ms.AROMATHERAPY: //312
                    HealBell(atk, "Aromatherapy");
                    break;
                case Ms.PRESENT: //217
                    if (atk.GetCondition<int>(Cs.Present) == 0) atk.Target.Defender.HpRecoverByOneNth(4, true);
                    else AttackMove(atk);
                    break;
                case Ms.Pollen_Puff:
                    if (atk.Attacker.Pokemon.TeamId == atk.Target.Defender.Pokemon.TeamId) atk.Target.Defender.HpRecover(atk.Target.Defender.Pokemon.MaxHp >> 1);
                    else AttackMove(atk);
                    break;
                case Ms.PAIN_SPLIT: //220
                    PainSplit(atk);
                    break;
                case Ms.BATON_PASS: //226
                    BatonPass(atk);
                    break;
                case Ms.ENCORE: //227
                    Encore(atk);
                    break;
                case Ms.MORNING_SUN: //234
                case Ms.SYNTHESIS: //235
                case Ms.MOONLIGHT: //236
                case Ms.Shore_Up:
                    Moonlight(atk);
                    break;
                case Ms.Floral_Healing:
                    {
                        int hp=atk.Target.Defender.Pokemon.MaxHp;
                        if (atk.Move.Id == Ms.Floral_Healing && aer.Controller.Board.HasCondition(Cs.GrassyTerrain)) hp = hp * 2 / 3;
                        else hp >>= 1;
                        atk.Target.Defender.HpRecover(hp);
                        break;
                    }
                case Ms.PSYCH_UP: //244
                    {
                        var lv5d = atk.Target.Defender.OnboardPokemon.Lv5D;
                        aer.OnboardPokemon.SetLv7D(lv5d.Atk, lv5d.Def, lv5d.SpAtk, lv5d.SpDef, lv5d.Speed, atk.Target.Defender.OnboardPokemon.AccuracyLv, atk.Target.Defender.OnboardPokemon.EvasionLv);
                        aer.ShowLogPm("PsychUp", atk.Target.Defender.Id);
                    }
                    break;
                case Ms.Spectral_Thief:
                    Spectral_Thief(atk);
                    goto default;
                case Ms.FUTURE_SIGHT: //248
                case Ms.DOOM_DESIRE: //353
                    FSDD(atk);
                    break;
                case Ms.BEAT_UP: //251
                    BeatUp(atk);
                    break;
                case Ms.STOCKPILE://255
                    StockPile(atk);
                    break;
                case Ms.SWALLOW: //256
                    {
                        var i = aer.OnboardPokemon.GetCondition<int>(Cs.Stockpile);
                        if (i == 0) atk.FailAll();
                        else aer.HpRecoverByOneNth(8 >> i, true);
                    }
                    break;
                case Ms.MEMENTO: //262
                    aer.Faint();
                    atk.Target.Defender.ChangeLv7D(aer, true, false, -2, 0, -2);
                    break;
                case Ms.FOLLOW_ME: //266
                case Ms.RAGE_POWDER:
                    if (aer.OnboardPokemon.AddTurnCondition(Cs.FollowMe, move.Id)) aer.ShowLogPm(Ls.EnFollowMe);
                    else atk.FailAll();
                    break;
                case Ms.Spotlight:
                    if (atk.Target.Defender.OnboardPokemon.AddTurnCondition(Cs.FollowMe, move.Id)) atk.Target.Defender.ShowLogPm(Ls.EnFollowMe);
                    else atk.FailAll();
                    break;
                case Ms.TAUNT: //269
                    {
                        var der = atk.Target.Defender;
                        if (der.OnboardPokemon.AddCondition(Cs.Taunt, 3)) der.ShowLogPm("EnTaunt");
                        else atk.FailAll();
                    }
                    break;
                case Ms.HELPING_HAND: //270
                    {
                        var der = atk.Target.Defender;
                        der.OnboardPokemon.SetTurnCondition(Cs.HelpingHand);
                        aer.ShowLogPm(Ls.HelpingHand, der.Id);
                    }
                    break;
                case Ms.TRICK: //271
                case Ms.SWITCHEROO: //415
                    Trick(atk);
                    break;
                case Ms.ROLE_PLAY: //272
                    RolePlay(atk);
                    break;
                case Ms.WISH: //273
                    if (!aer.Tile.AddCondition(Cs.Wish, new Condition() { Turn = aer.Controller.TurnNumber + 1, Int = aer.Pokemon.MaxHp >> 1 })) atk.FailAll();
                    break;
                case Ms.MAGIC_COAT: //277
                    aer.OnboardPokemon.SetTurnCondition(Cs.MagicCoat);
                    aer.ShowLogPm("EnMagicCoat");
                    break;
                case Ms.RECYCLE: //278
                    Recycle(atk);
                    break;
                case Ms.SKILL_SWAP: //285
                    SkillSwap(atk);
                    break;
                case Ms.REFRESH: //287
                    if (aer.State == PokemonState.Normal) atk.FailAll();
                    else aer.DeAbnormalState();
                    break;
                case Ms.CAMOUFLAGE: //293
                    Camouflage(atk);
                    break;
                case Ms.MUD_SPORT: //300
                    Sport(atk, Cs.MudSport);
                    break;
                case Ms.WATER_SPORT: //346
                    Sport(atk, Cs.WaterSport);
                    break;
                case Ms.GRAVITY: //356
                    Gravity(atk);
                    break;
                case Ms.HEALING_WISH: //361
                    HealingWish(atk, Cs.HealingWish);
                    break;
                case Ms.LUNAR_DANCE: //461
                    HealingWish(atk, Cs.LunarDance);
                    break;
                case Ms.ACUPRESSURE: //367
                    {
                        var ss = new List<StatType>();
                        foreach (var s in GameHelper.SEVEN_D)
                            if (atk.Target.Defender.CanChangeLv7D(aer, s, 2, false) != 0) ss.Add(s);
                        if (ss.Count == 0) atk.FailAll();
                        else atk.Target.Defender.ChangeLv7D(aer, ss[aer.Controller.GetRandomInt(0, ss.Count - 1)], 2, true);
                    }
                    break;
                case Ms.PSYCHO_SHIFT: //375
                    PsychoShift(atk);
                    break;
                case Ms.GASTRO_ACID: //380
                    GastroAcid(atk);
                    break;
                case Ms.POWER_TRICK: //379
                    PowerTrick(atk);
                    break;
                case Ms.Speed_Swap:
                    SpeedSwap(atk);
                    break;
                case Ms.POWER_SWAP: //384
                    SwapLv7D(atk, "PowerSwap", POWER_STATS);
                    break;
                case Ms.GUARD_SWAP: //385
                    SwapLv7D(atk, "GuardSwap", GUARD_STATS);
                    break;
                case Ms.HEART_SWAP: //391
                    SwapLv7D(atk, "HeartSwap", ALL_STATS);
                    break;
                case Ms.WORRY_SEED: //388
                    SetAbility(atk, As.INSOMNIA);
                    break;
                case Ms.SIMPLE_BEAM: //493
                    SetAbility(atk, As.SIMPLE);
                    break;
                case Ms.MAGNET_RISE: //393
                    if (!aer.OnboardPokemon.HasCondition(Cs.Ingrain) && aer.OnboardPokemon.AddCondition(Cs.MagnetRise, aer.Controller.TurnNumber + 5)) aer.ShowLogPm("EnMagnetRise");
                    else atk.FailAll();
                    break;
                case Ms.DEFOG: //432
                    Defog(atk);
                    break;
                case Ms.TRICK_ROOM: //443
                    Room(atk, Cs.TrickRoom);
                    break;
                case Ms.MAGIC_ROOM: //478
                    Room(atk, Cs.MagicRoom);
                    break;
                case Ms.WIDE_GUARD: //469
                    TeamProtect(atk, Cs.WideGuard);
                    break;
                case Ms.QUICK_GUARD: //501
                    TeamProtect(atk, Cs.QuickGuard);
                    break;
                case Ms.GUARD_SPLIT: //470
                    Split5D(atk, "GuardSplit", GUARD_STATS);
                    break;
                case Ms.POWER_SPLIT: //471
                    Split5D(atk, "PowerSplit", POWER_STATS);
                    break;
                case Ms.WONDER_ROOM: //472
                    WonderRoom(atk);
                    break;
                case Ms.TELEKINESIS: //477
                    Telekinesis(atk);
                    break;
                case Ms.SOAK: //487
                    Soak(atk);
                    break;
                case Ms.ENTRAINMENT: //494
                    Entrainment(atk);
                    break;
                case Ms.AFTER_YOU: //495
                    if (atk.Target.Defender.CanMove)
                    {
                        aer.Controller.Board.SetTurnCondition(Cs.NextActingPokemon, atk.Target.Defender);
                        atk.Target.Defender.ShowLogPm(Ls.AfterYou);
                    }
                    else atk.FailAll();
                    break;
                case Ms.SHELL_SMASH: //504
                    aer.ChangeLv7D(aer, true, false, 2, -1, 2, -1, 2);
                    break;
                case Ms.QUASH: //511
                    if (atk.Target.Defender.CanMove && atk.Target.Defender.OnboardPokemon.AddTurnCondition(Cs.Quash)) atk.Target.Defender.ShowLogPm(Ls.Quash);
                    else atk.FailAll();
                    break;
                case Ms.REFLECT_TYPE: //513
                    ReflectType(atk);
                    break;
                case Ms.BESTOW: //516
                    Bestow(atk);
                    break;
                case Ms.HEAL_PULSE:
                    HealPulse(atk);
                    break;
                case Ms.MAT_BLOCK:
                    if (aer.Field.AddCondition(Cs.MatBlock)) aer.ShowLogPm("EnMatBlock");
                    else atk.FailAll();
                    break;
                case Ms.TRICKORTREAT:
                    AddType(atk, BattleType.Ghost);
                    break;
                case Ms.FORESTS_CURSE:
                    AddType(atk, BattleType.Grass);
                    break;
                case Ms.GRASSY_TERRAIN:
                    Terrain(atk, Cs.GrassyTerrain);
                    break;
                case Ms.MISTY_TERRAIN:
                    Terrain(atk, Cs.MistyTerrain);
                    break;
                case Ms.ELECTRIC_TERRAIN:
                    Terrain(atk, Cs.ElectricTerrain);
                    break;
                case Ms.Psychic_Terrain:
                    Terrain(atk, Cs.PsychicTerrain);
                    break;
                case Ms.FAIRY_LOCK:
                    aer.Controller.Board.SetCondition(Cs.FairyLock, aer.Controller.TurnNumber);
                    aer.Controller.ReportBuilder.ShowLog("EnFairyLock");
                    break;
                case Ms.TOPSYTURVY:
                    TorsyTurvy(atk);
                    break;
                case Ms.SPIKY_SHIELD:
                    SelfProtect(atk, Cs.SpikyShield, "EnProtect");
                    break;
                case Ms.KINGS_SHIELD:
                    SelfProtect(atk, Cs.KingsShield, "EnProtect");
                    break;
                case Ms.Baneful_Bunker:
                    SelfProtect(atk, Cs.Baneful_Bunker, "EnProtect");
                    break;
                case Ms.POWDER:
                    AddTurnCondition(atk, Cs.Powder);
                    break;
                case Ms.CELEBRATE:
                    aer.ShowLogPm("Celebrate");
                    break;
                case Ms.ION_DELUGE:
                    if (aer.Controller.Board.AddTurnCondition(Cs.IonDeluge)) aer.Controller.ReportBuilder.ShowLog("EnIonDeluge");
                    else atk.FailAll();
                    break;
                case Ms.ELECTRIFY:
                    AddTurnCondition(atk, Cs.Electrify);
                    break;
                //gen7:
                case Ms.Toxic_Thread:
                    atk.Target.Defender.AddState(aer,AttachedState.PSN,true);
                    atk.Target.Defender.ChangeLv7D(aer, StatType.Speed, -1, true);
                    break;
                case Ms.Strength_Sap:
                    {
                        var der = atk.Target.Defender;
                        aer.HpRecover(OnboardPokemon.Get5D(der.OnboardPokemon.FiveD.Atk,der.OnboardPokemon.Lv5D.Atk));
                        der.ChangeLv7D(aer, StatType.Atk, -1, true);
                    }
                    break;
                case Ms.Purify:
                    {
                        var der = atk.Target.Defender;
                        if (der.DeAbnormalState())
                            aer.HpRecover(aer.Pokemon.MaxHp * 1 / 2, true);
                        else aer.ShowLogPm("fail");
                    }
                    break;
                case Ms.Spirit_Shackle:
                case Ms.Anchor_Shot:
                case Ms.THOUSAND_WAVES:
                    CantSelectWithdraw(atk);
                    goto default;
                //---
                default:
                    if (move.Move.Category == MoveCategory.Status) StatusMove(atk);
                    else AttackMove(atk);
                    break;
            }
            #endregion
            #region post
            switch (move.Id)
            {
                case Ms.MINIMIZE: //107
                    aer.OnboardPokemon.SetCondition(Cs.Minimize);
                    break;
                case Ms.DEFENSE_CURL: //111
                    aer.OnboardPokemon.SetCondition(Cs.DefenseCurl);
                    break;
                case Ms.CHARGE: //268
                    aer.OnboardPokemon.SetCondition(Cs.Charge, atk.Controller.TurnNumber + 1);
                    aer.ShowLogPm("Charge");
                    break;
                case Ms.Laser_Focus:
                    aer.OnboardPokemon.SetCondition(Cs.Laser_Focus, atk.Controller.TurnNumber + 1);
                    aer.ShowLogPm("Laser_Focus");
                    break;
                case Ms.ROOST: //355
                    aer.OnboardPokemon.SetTurnCondition(Cs.Roost);
                    break;
                case Ms.AUTOTOMIZE: //475
                    if (!atk.Fail && atk.Attacker.OnboardPokemon.Weight > 0.1)
                    {
                        aer.OnboardPokemon.Weight -= 100;
                        aer.ShowLogPm("Autotomize");
                    }
                    break;
                case Ms.Genesis_Supernova:
                    Terrain(atk, Cs.PsychicTerrain);
                    break;
            }
            #endregion
        }

        private static readonly int[] TIMES25 = new int[8] { 2, 2, 2, 3, 3, 3, 4, 5 };
        private static void AttackMove(AtkContext atk)
        {
            var aer = atk.Attacker;
            var move = atk.Move;
            var aa = aer.Ability;

            //生成攻击次数
            if (move.MaxTimes == 0)
                if (!atk.MultiTargets && aa == As.PARENTAL_BOND)
                {
                    atk.Hits = 2;
                    atk.AddCondition(Cs.ParentalBond);
                }
                else atk.Hits = 1;
            else if (move.MinTimes == move.MaxTimes || aa == As.SKILL_LINK) atk.Hits = move.MaxTimes;
            else atk.Hits = TIMES25[atk.Controller.GetRandomInt(0, 7)];

            int atkTeam = aer.Pokemon.TeamId;
            atk.Hit = 0;
            do
            {
                atk.Hit++;
                CalculateDamages.Execute(atk);
                if (atk.Target.Damage == 0 && atk.Hit == 2 && atk.HasCondition(Cs.ParentalBond)) break;
                if (atk.Targets.Count() == 1)
                {
                    Implement(atk.Targets);
                    if (atk.Move.Id == Ms.TRI_ATTACK) MoveE.FilterDefContext(atk);
                }
                else
                {
                    List<DefContext> od = null, fd = null;
                    foreach (var d in atk.Targets)
                        if (d.Defender.Pokemon.TeamId == atkTeam)
                        {
                            if (od == null) od = new List<DefContext>();
                            od.Add(d);
                        }
                        else
                        {
                            if (fd == null) fd = new List<DefContext>();
                            fd.Add(d);
                        }
                    if (od != null) Implement(od);
                    if (fd != null) Implement(fd);
                }
            }
            while (atk.Hit < atk.Hits && atk.Target.Defender.Hp != 0 && aer.Hp != 0 && aer.State != PokemonState.FRZ && aer.State != PokemonState.SLP);

            if (atk.Hits != 1)
            {
                if (atk.Target.EffectRevise > 0) aer.Controller.ReportBuilder.ShowLog("SuperHurt0");
                else if (atk.Target.EffectRevise < 0) aer.Controller.ReportBuilder.ShowLog("WeakHurt0");
                aer.Controller.ReportBuilder.ShowLog("Hits", atk.Hit);
            }
            if (atk.Type == BattleType.Fire)
                foreach (DefContext d in atk.Targets)
                    if (d.Defender.State == PokemonState.FRZ) d.Defender.DeAbnormalState();

            FinalEffect(atk);
        }
        private static bool SetHurt(GameEvents.MoveHurt e, IEnumerable<DefContext> defs, bool effects) //auto delay
        {
            List<int> pms = new List<int>();
            List<int> damages = new List<int>();
            List<int> sh = new List<int>();
            List<int> wh = new List<int>();
            List<int> ct = new List<int>();
            foreach (DefContext d in defs)
                if (!d.HitSubstitute)
                {
                    int id = d.Defender.Id;
                    pms.Add(id);
                    damages.Add(d.Damage);
                    if (d.EffectRevise > 0) sh.Add(id);
                    else if (d.EffectRevise < 0) wh.Add(id);
                    if (d.IsCt) ct.Add(id);
                }
            if (pms.Any()) e.Pms = pms.ToArray();
            else return false;
            if (damages.Any()) e.Damages = damages.ToArray();
            if (effects)
            {
                if (sh.Any()) e.SH = sh.ToArray();
                if (wh.Any()) e.WH = wh.ToArray();
            }
            if (ct.Any()) e.CT = ct.ToArray();
            return true;
        }
        private static void Implement(IEnumerable<DefContext> defs)
        {
            DefContext def = defs.FirstOrDefault();

            if (def == null) return;
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var move = def.AtkContext.Move;

            if (move.Class == MoveClass.OHKO)
            {
                if (!SubstituteTriggers.OHKO(def))
                {
                    def.Defender.Controller.ReportBuilder.ShowLog("OHKO");
                    def.Damage = def.Defender.Hp;
                    var e = new GameEvents.MoveHurt();
                    def.Defender.Controller.ReportBuilder.Add(e);
                    def.MoveHurt();
                    SetHurt(e, defs, true);
                    PassiveEffect(def);
                }
            }
            else
            {
                bool allSub;
                if (atk.IgnoreSubstitute()) allSub = false;
                else
                {
                    allSub = true;
                    foreach (DefContext d in defs) allSub &= SubstituteTriggers.Hurt(d);
                }
                if (!allSub)
                {
                    foreach (DefContext d in defs)
                        if (!d.HitSubstitute)
                            if (d.RemoveCondition(Cs.Antiberry))
                            {
                                d.Defender.ShowLogPm("Antiberry", d.Defender.Pokemon.Item);
                                d.Defender.ConsumeItem();
                            }
                    var e = new GameEvents.MoveHurt();
                    aer.Controller.ReportBuilder.Add(e);
                    foreach (DefContext d in defs)
                        if (!d.HitSubstitute)
                        {
                            d.MoveHurt();
                            atk.TotalDamage += d.Damage;
                        }
                    SetHurt(e, defs, atk.Hits == 1);
                }

                if (move.HurtPercentage > 0)
                    foreach (var d in defs)
                    {
                        int v = d.Damage * move.HurtPercentage / 100;
                        if (aer.ItemE(Is.BIG_ROOT)) v *= (Modifier)0x14cc;
                        if (!d.Defender.AbilityE(As.LIQUID_OOZE)) aer.HpRecover(v, false);
                        else if (aer.CanEffectHurt)
                        {
                            d.Defender.RaiseAbility();
                            aer.EffectHurtImplement(v);
                        }
                    }
                if (move.Class == MoveClass.AttackWithSelfLv7DChange && atk.RandomHappen(move.Lv7DChanges.Probability)) aer.ChangeLv7D(atk.Attacker, move);

                if (move.Id == Ms.FLAME_BURST) //未测试，暂时在这
                {
                    var x = def.Defender.OnboardPokemon.X;
                    var f = atk.Controller.Board[def.Defender.Pokemon.TeamId];
                    var t = f[x - 1];
                    if (t != null && t.Pokemon != null && t.Pokemon.EffectHurtByOneNth(16, Ls.FlameBurst)) t.Pokemon.CheckFaint();
                    t = f[x + 1];
                    if (t != null && t.Pokemon != null && t.Pokemon.EffectHurtByOneNth(16, Ls.FlameBurst)) t.Pokemon.CheckFaint();
                }

                foreach (DefContext d in defs)
                    if (!d.HitSubstitute)
                    {
                        MoveImplementEffect.Execute(d);
                        PassiveEffect(d);
                    }

                if (aer.Hp != 0) MovePostEffect.Execute(def);
                aer.CheckFaint();
            }// OHKO else
        }
        private static void PassiveEffect(DefContext def)
        {
            var der = def.Defender;
            Attacked.Execute(def);
            def.AtkContext.Attacker.CheckFaint();
            var op = der.OnboardPokemon; //the state before withdraw
            if (der.CheckFaint()) STs.KOed(def, op);
            else if (def.AtkContext.Move.MaxTimes > 1) HpChanged.Execute(der);
        }
        private static void FinalEffect(AtkContext atk)
        {
            if (!(atk.Move.HasProbabilitiedAdditonalEffects && atk.Attacker.AbilityE(As.SHEER_FORCE)))
            {
                foreach (DefContext d in atk.Targets) ATs.ColorChange(d);
                ITs.AttackPostEffect(atk);
            }
        }

        private static void StatusMove(AtkContext atk)
        {
            bool notAllFail = false;
            var move = atk.Move;
            var def = atk.Target;
            switch (move.Class)
            {
                case MoveClass.AddState:
                    foreach (var d in atk.Targets) notAllFail |= d.Defender.AddState(d);
                    if (move.AttachedState == AttachedState.PerishSong)
                        if (notAllFail) atk.Controller.ReportBuilder.ShowLog("EnPerishSong");
                        else atk.FailAll();
                    break;
                case MoveClass.Lv7DChange:
                    foreach (var d in atk.Targets) notAllFail |= d.Defender.ChangeLv7D(d);
                    atk.Fail = !notAllFail;
                    break;
                case MoveClass.HpRecover:
                    foreach (var d in atk.Targets)
                        d.Defender.HpRecover(d.Defender.Pokemon.MaxHp * move.MaxHpPercentage / 100, true);
                    break;
                case MoveClass.ConfusionWithLv7DChange:
                    def.Defender.AddState(def);
                    def.Defender.ChangeLv7D(def);
                    break;
                case MoveClass.ForceToSwitch:
                    int aLv = atk.Attacker.Pokemon.Lv, dLv = def.Defender.Pokemon.Lv;
                    if ((aLv < dLv && (aLv + dLv) * atk.Controller.GetRandomInt(0, 255) < dLv >> 2) || !def.Defender.Controller.CanWithdraw(def.Defender)) atk.FailAll();
                    else MoveE.ForceSwitchImplement(def.Defender, atk.DefenderAbilityAvailable());
                    break;
            }
        }

        #region Gen5
        private static void Curse(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.OnboardPokemon.HasType(BattleType.Ghost))
            {
                if (atk.Target.Defender.OnboardPokemon.AddCondition(Cs.Curse))
                {
                    aer.ShowLogPm(Ls.EnCurse, atk.Target.Defender.Id);
                    aer.Hp -= (aer.Pokemon.MaxHp >> 1);
                    aer.CheckFaint();
                }
                else atk.FailAll();
            }
            else aer.ChangeLv7D(aer, true, false, 1, 1, 0, 0, -1);
        }
        private static void StockPile(AtkContext atk)
        {
            var aer = atk.Attacker;
            int i = aer.OnboardPokemon.GetCondition<int>(Cs.Stockpile) + 1;
            aer.OnboardPokemon.SetCondition(Cs.Stockpile, i);
            aer.ShowLogPm("EnStockpile", i);
            aer.ChangeLv7D(aer, false, false, 0, 1, 0, 1);
        }
        private static void CantSelectWithdraw(AtkContext atk)
        {
            if (atk.Target.Defender.OnboardPokemon.AddCondition(Cs.CantSelectWithdraw, atk.Attacker)) atk.Target.Defender.ShowLogPm("EnCantSelectWithdraw");
            else atk.FailAll();
        }
        private static void AddCondition(AtkContext atk, Cs condition)
        {
            if (atk.Target.Defender.OnboardPokemon.AddCondition(condition)) atk.Target.Defender.ShowLogPm("En" + condition);
            else atk.FailAll();
        }
        private static void Telekinesis(AtkContext atk)
        {
            var der = atk.Target.Defender.OnboardPokemon;
            if (der.Form.Species.Number == 50 || der.Form.Species.Number == 51) atk.Target.Defender.NoEffect();
            else if (der.AddCondition(Cs.Telekinesis, atk.Controller.TurnNumber + 2)) atk.Target.Defender.ShowLogPm("EnTelekinesis");
            else atk.FailAll();
        }
        private static void Sport(AtkContext atk, Cs condition)
        {
            var c = atk.Controller;
            if (c.Board.AddCondition(condition, c.TurnNumber + 4)) c.ReportBuilder.ShowLog("En" + condition);
            else atk.FailAll();
        }
        private static void Room(AtkContext atk, Cs condition)
        {
            var c = atk.Controller;
            if (c.Board.AddCondition(condition, c.TurnNumber + 4)) c.ReportBuilder.ShowLog("En" + condition, atk.Attacker.Id);
            else
            {
                c.Board.RemoveCondition(condition);
                c.ReportBuilder.ShowLog("De" + condition);
            }
        }
        private static void AddTeamCondition(AtkContext atk, Cs condition, int turn = 5)
        {
            var team = atk.Attacker.Pokemon.TeamId;
            if (atk.Controller.Board[team].AddCondition(condition, atk.Controller.TurnNumber + turn - 1)) atk.Controller.ReportBuilder.ShowLog("En" + condition, team);
            else atk.FailAll();
        }
        private static void EntryHazards(AtkContext atk, string log)
        {
            var team = 1 - atk.Attacker.Pokemon.TeamId;
            if (EHTs.En(atk.Controller.Board[team], atk.Move)) atk.Controller.ReportBuilder.ShowLog(log, team);
            else atk.FailAll();
        }
        private static void LockOn(AtkContext atk)
        {
            var der = atk.Target.Defender;
            var c = new Condition() { By = atk.Attacker, Turn = atk.Controller.TurnNumber + 1 };
            if (der.OnboardPokemon.AddCondition(Cs.NoGuard, c)) atk.Attacker.ShowLogPm("LockOn", der.Id);
            else atk.FailAll();
        }
        private static void BellyDrum(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.OnboardPokemon.Lv5D.Atk != 6 && aer.Hp > aer.Pokemon.MaxHp >> 1)
            {
                aer.OnboardPokemon.ChangeLv7D(StatType.Atk, 12);
                aer.ShowLogPm(Ls.BellyDrum);
                aer.Hp -= (aer.Pokemon.MaxHp >> 1);
            }
            else atk.FailAll();
        }
        private static void KOedCondition(AtkContext atk, Cs condition)
        {
            atk.Attacker.OnboardPokemon.SetCondition(condition);
            atk.Attacker.ShowLogPm("En" + condition);
        }
        private static void Camouflage(AtkContext atk)
        {
            var aer = atk.Attacker;
            var t = aer.Controller.GameSettings.Terrain.GetBattleType();
            if (aer.OnboardPokemon.SetTypes(t)) aer.ShowLogPm("TypeChange", (int)t);
            else atk.FailAll();
        }
        private static void HealingWish(AtkContext atk, Cs condition)
        {
            var aer = atk.Attacker;
            if (aer.Pokemon.Owner.PmsAlive > aer.Controller.GameSettings.Mode.OnboardPokemonsPerPlayer())
            {
                aer.Tile.SetTurnCondition(condition);
                aer.Faint();
            }
            else atk.FailAll();
        }
        private static void PsychoShift(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.State == PokemonState.Normal) atk.FailAll();
            else
            {
                AttachedState a;
                int t = 0;
                switch (aer.State)
                {
                    case PokemonState.BRN:
                        a = AttachedState.BRN;
                        break;
                    case PokemonState.FRZ:
                        a = AttachedState.FRZ;
                        break;
                    case PokemonState.PAR:
                        a = AttachedState.PAR;
                        break;
                    case PokemonState.PSN:
                        a = AttachedState.PSN;
                        break;
                    case PokemonState.SLP:
                        a = AttachedState.SLP;
                        break;
                    default: //BadlyPSN
                        a = AttachedState.PSN;
                        t = 15;
                        break;
                }
                if (atk.Target.Defender.AddState(aer, a, true, t)) atk.Attacker.DeAbnormalState();
            }
        }
        private static void PowerTrick(AtkContext atk)
        {
            var s = atk.Attacker.OnboardPokemon.FiveD;
            var a = s.Atk;
            s.Atk = s.Def;
            s.Def = a;
            atk.Attacker.ShowLogPm("PowerTrick");
        }
        private static void SpeedSwap(AtkContext atk)
        {
            var s = atk.Attacker.OnboardPokemon.FiveD.Speed;
            atk.Attacker.OnboardPokemon.FiveD.Speed = atk.Target.Defender.OnboardPokemon.FiveD.Speed;
            atk.Target.Defender.OnboardPokemon.FiveD.Speed = s;
            atk.Attacker.ShowLogPm("SpeedSwap",atk.Target.Defender.Id);
        }
        private static void GastroAcid(AtkContext atk)
        {
            var der = atk.Target.Defender;
            var ab = der.Ability;
            if (ab != As.MULTITYPE && ab != As.Comatose && ab!=As.RKS_System && der.OnboardPokemon.AddCondition(Cs.GastroAcid))
            {
                der.ShowLogPm("EnGastroAcid");
                AbilityDetach.Execute(der, ab);
            }
            else atk.FailAll();
        }
        private static readonly IEnumerable<StatType> POWER_STATS = new StatType[] { StatType.Atk, StatType.SpAtk };
        private static readonly IEnumerable<StatType> GUARD_STATS = new StatType[] { StatType.Def, StatType.SpDef };
        private static readonly IEnumerable<StatType> ALL_STATS = new StatType[] { StatType.Atk, StatType.Def, StatType.SpAtk, StatType.SpDef, StatType.Speed, StatType.Accuracy, StatType.Evasion };
        private static void SwapLv7D(AtkContext atk, string log, IEnumerable<StatType> stats)
        {
            var aer = atk.Attacker;
            var der = atk.Target.Defender;
            foreach (var s in stats)
            {
                var t = der.OnboardPokemon.GetLv7D(s);
                der.OnboardPokemon.SetLv7D(s, aer.OnboardPokemon.GetLv7D(s));
                aer.OnboardPokemon.SetLv7D(s, t);
            }
            aer.Controller.ReportBuilder.ShowLog(log, aer.Id, der.Id);
        }
        private static void Split5D(AtkContext atk, string log, IEnumerable<StatType> stats)
        {
            var aer = atk.Attacker;
            var der = atk.Target.Defender;
            foreach (var s in stats)
            {
                var v = (aer.OnboardPokemon.FiveD.GetStat(s) + der.OnboardPokemon.FiveD.GetStat(s)) >> 1;
                aer.OnboardPokemon.FiveD.SetStat(s, v);
                der.OnboardPokemon.FiveD.SetStat(s, v);
            }
            aer.Controller.ReportBuilder.ShowLog(log, aer.Id, der.Id);
        }
        private static void Defog(AtkContext atk)
        {
            var r = atk.Controller.ReportBuilder;
            atk.Target.Defender.ChangeLv7D(atk.Attacker, StatType.Evasion, -1, true);
            var f0 = atk.Controller.Board[0];
            var f1 = atk.Controller.Board[1];
            EHTs.De(r, f0);
            if (f0.RemoveCondition(Cs.Reflect)) r.ShowLog("DeReflect", 0);
            if (f0.RemoveCondition(Cs.LightScreen)) r.ShowLog("DeLightScreen", 0);
            if (f0.RemoveCondition(Cs.Aurora_Veil)) r.ShowLog("DeAurora_Veil", 0);
            if (f0.RemoveCondition(Cs.Mist)) r.ShowLog("DeMist", 0);
            if (f0.RemoveCondition(Cs.Safeguard)) r.ShowLog("DeSafeguard", 0);
            EHTs.De(r, f1);
            if (f1.RemoveCondition(Cs.Reflect)) r.ShowLog("DeReflect", 1);
            if (f1.RemoveCondition(Cs.LightScreen)) r.ShowLog("DeLightScreen", 1);
            if (f1.RemoveCondition(Cs.Aurora_Veil)) r.ShowLog("DeAurora_Veil", 1);
            if (f1.RemoveCondition(Cs.Mist)) r.ShowLog("DeMist", 1);
            if (f1.RemoveCondition(Cs.Safeguard)) r.ShowLog("DeSafeguard", 1);
        }
        private static void Soak(AtkContext atk)
        {
            var der = atk.Target.Defender;
            if (!ITs.PlatedArceus(der.Pokemon) && der.OnboardPokemon.SetTypes(BattleType.Water)) der.ShowLogPm("TypeChange", (int)BattleType.Water);
            else atk.FailAll();
        }
        private static void ReflectType(AtkContext atk)
        {
            var ao = atk.Attacker.OnboardPokemon;
            var types = atk.Target.Defender.OnboardPokemon.Types.ToArray();
            if (ao.SetTypes(types[0], types.ValueOrDefault(1))) atk.Attacker.ShowLogPm("ReflectType", atk.Target.Defender.Id);
            else atk.FailAll();
        }
        private static void Bestow(AtkContext atk)
        {
            var aer = atk.Attacker;
            var der = atk.Target.Defender;
            if (der.Pokemon.Item == 0)
            {
                var i = aer.Pokemon.Item;
                aer.RemoveItem();
                der.SetItem(i);
                der.ShowLogPm("Bestow", i, aer.Id);
            }
            else atk.FailAll();
        }
        private static void Moonlight(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.CanHpRecover(true))
            {
                var hp = aer.Pokemon.MaxHp;
                var w = aer.Controller.Weather;
                if (w == Weather.Sandstorm && atk.Move.Id == Ms.Shore_Up) hp = hp * 2 / 3;
                else if (w == Weather.IntenseSunlight) hp = hp * 2 / 3;
                else if (w == Weather.Normal) hp >>= 1;
                else hp >>= 2;
                aer.HpRecover(hp);
            }
            else atk.Fail = true;
        }
        private static void Substitute(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (atk.Attacker.Hp > 1 && atk.Attacker.Hp > atk.Attacker.Pokemon.MaxHp / 4) 
            {
                if (aer.OnboardPokemon.HasCondition(Cs.Substitute)) aer.ShowLogPm("HasSubstitute");
                else
                {
                    int hp = aer.Pokemon.MaxHp / 4;
                    aer.OnboardPokemon.SetCondition(Cs.Substitute, hp);
                    aer.Controller.ReportBuilder.EnSubstitute(aer);
                    aer.Pokemon.Hp -= hp;
                }
            }
            else atk.FailAll();
        }
        private static void Trick(AtkContext atk)
        {
            var aer = atk.Attacker;
            var der = atk.Target.Defender;
            var ai = aer.Pokemon.Item;
            var di = der.Pokemon.Item;
            if (di == 0 && ai == 0 || ai != 0 && ITs.NeverLostItem(aer.Pokemon) || di != 0 && !ITs.CanLostItem(der)) atk.FailAll();
            else
            {
                aer.ShowLogPm("Trick");
                if (ai != 0) aer.RemoveItem();
                if (di != 0) der.RemoveItem();
                if (ai != 0)
                {
                    der.SetItem(ai);
                    der.ShowLogPm("GetItem", ai);
                }
                if (di != 0)
                {
                    aer.SetItem(di);
                    aer.ShowLogPm("GetItem", di);
                }
            }
        }
        private static void WonderRoom(AtkContext atk)
        {
            var c = atk.Controller;
            foreach (var pm in c.Board.Pokemons)
            {
                var stats = pm.OnboardPokemon.FiveD;
                var d = stats.Def;
                stats.Def = stats.SpDef;
                stats.SpDef = d;
            }
            if (c.Board.AddCondition(Cs.WonderRoom, c.TurnNumber + 4)) c.ReportBuilder.ShowLog("EnWonderRoom");
            else
            {
                c.Board.RemoveCondition(Cs.WonderRoom);
                c.ReportBuilder.ShowLog("DeWonderRoom");
            }
        }
        private static void Rest(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.Hp == aer.Pokemon.MaxHp) atk.FailAll();
            else if (PTs.CanAddXXX(aer, aer, true, AttachedState.SLP, true))
            {
                aer.ShowLogPm(Ls.Rest);
                aer.Hp = aer.Pokemon.MaxHp;
                aer.Pokemon.State = PokemonState.SLP;
                aer.Pokemon.SLPTurn = 3;
            }
        }
        private static void Recycle(AtkContext atk)
        {
            var aer = atk.Attacker;
            if (aer.Pokemon.Item == 0)
            {
                if (aer.Pokemon.UsedItem == 0) atk.FailAll();
                else
                {
                    aer.SetItem(aer.Pokemon.UsedItem);
                    aer.ShowLogPm("Recycle", aer.Pokemon.UsedItem);
                    aer.Pokemon.UsedItem = 0;
                }
            }
            else atk.FailAll();
        }
        private static void PainSplit(AtkContext atk)
        {
            var aer = atk.Attacker;
            var der = atk.Target.Defender;
            int hp = (aer.Hp + der.Hp) >> 1;
            aer.Pokemon.Hp = hp;
            der.Pokemon.Hp = hp;
            atk.Controller.ReportBuilder.ShowLog("PainSplit", aer.Id, der.Id);
            atk.Controller.ReportBuilder.ShowLog(Ls.painSplit, hp);
        }
        private static void HealBell(AtkContext atk, string log)
        {
            var aer = atk.Attacker;
            foreach (var pm in aer.Field.Pokemons) pm.Pokemon.State = PokemonState.Normal;
            foreach (var owner in aer.Controller.Teams[aer.Pokemon.TeamId].Players)
                foreach (var pm in owner.Pokemons)
                    if (pm.Hp > 0 && pm.State != PokemonState.Normal) pm.Pokemon.State = PokemonState.Normal;
            aer.Controller.ReportBuilder.ShowLog(log);
        }
        private static void Gravity(AtkContext atk)
        {
            var c = atk.Controller;
            if (c.Board.HasCondition(Cs.Gravity)) atk.FailAll();
            else
            {
                c.ReportBuilder.ShowLog("EnGravity");
                foreach (var pm in c.Board.Pokemons)
                    if (pm.CoordY == CoordY.Air)
                    {
                        pm.CoordY = CoordY.Plate;
                        pm.Action = PokemonAction.Done;
                        pm.OnboardPokemon.RemoveCondition(Cs.SkyDrop);
                        c.ReportBuilder.SetY(pm);
                        pm.ShowLogPm("Gravity");
                    }
                    else if (!HasEffect.IsGroundAffectable(pm, true, false)) pm.ShowLogPm("Gravity");
                c.Board.SetCondition(Cs.Gravity, c.TurnNumber + 4);
            }
        }
        private static void Conversion(AtkContext atk)
        {
            var type = atk.Attacker.Moves.First().MoveE.Move.Type;
            atk.Attacker.OnboardPokemon.SetTypes(type);
            atk.Attacker.ShowLogPm("TypeChange", (int)type);
        }
        private static void BatonPass(AtkContext atk)
        {
            var aer = atk.Attacker;
            var t = aer.Tile;
            var o = aer.OnboardPokemon;
            if (aer.Controller.Withdraw(aer, "SelfWithdraw", 0, false))
            {
                t.SetCondition(Cs.BatonPass, o);
                aer.Controller.PauseForSendOutInput(t);
            }
            else atk.FailAll();
        }
        private static void SkillSwap(AtkContext atk)
        {
            int a = atk.Attacker.OnboardPokemon.Ability;
            int d = atk.Target.Defender.OnboardPokemon.Ability;
            if
              (
              a == d ||
              a == As.WONDER_GUARD || a == As.ILLUSION || a == As.MULTITYPE || a==As.RKS_System || a==As.Comatose ||
              d == As.WONDER_GUARD || d == As.ILLUSION || d == As.MULTITYPE || d==As.RKS_System || d==As.Comatose
              )
                atk.FailAll();
            else
            {
                var aer = atk.Attacker;
                var der = atk.Target.Defender;
                aer.Controller.ReportBuilder.ShowLog("SkillSwap", aer.Id, der.Id);
                if (aer.Pokemon.TeamId != der.Pokemon.TeamId)
                {
                    aer.ShowLogPm("skillSwap", d);
                    der.ShowLogPm("skillSwap", a);
                }
                AbilityDetach.Execute(aer);
                AbilityDetach.Execute(der);
                aer.OnboardPokemon.Ability = d;
                der.OnboardPokemon.Ability = a;
                AbilityAttach.Execute(aer);
                AbilityAttach.Execute(der);
            }
        }
        private static void SetAbility(AtkContext atk, int ability)
        {
            var fa = atk.Target.Defender.Ability;
            if (fa == As.TRUANT || fa == As.MULTITYPE || fa == As.RKS_System || fa==As.Comatose) atk.FailAll();
            else
            {
                atk.Target.Defender.ChangeAbility(ability);
                atk.Target.Defender.ShowLogPm(Ls.SetAbility, ability);
                atk.Controller.ReportBuilder.ShowLog(Ls.setAbility, fa);
            }
        }
        private static void RolePlay(AtkContext atk)
        {
            int a = atk.Attacker.OnboardPokemon.Ability;
            int d = atk.Target.Defender.OnboardPokemon.Ability;
            if
              (
              a == d ||
              a == As.ILLUSION || a == As.MULTITYPE || a==As.RKS_System || a==As.Comatose ||
              d == As.WONDER_GUARD || d == As.FORECAST || d == As.MULTITYPE || d == As.ILLUSION || d == As.ZEN_MODE || d==As.Shields_Down || d==As.Schooling || d==As.Power_Construct || d==As.RKS_System || d==As.Comatose
              )
                atk.FailAll();
            else
            {
                atk.Attacker.ChangeAbility(d);
                atk.Attacker.ShowLogPm(Ls.SetAbility, d);
                atk.Controller.ReportBuilder.ShowLog(Ls.setAbility, a);
            }
        }
        private static void Entrainment(AtkContext atk)
        {
            int a = atk.Attacker.OnboardPokemon.Ability;
            int d = atk.Target.Defender.OnboardPokemon.Ability;
            if
              (
              a == d ||
              a == As.FORECAST || a == As.ILLUSION || a == As.ZEN_MODE || a == As.FLOWER_GIFT || a==As.Shields_Down || a==As.Schooling || a==As.Battle_Bond ||
              d == As.TRUANT || d == As.MULTITYPE || d == As.FLOWER_GIFT || d==As.RKS_System || d==As.Comatose
              )
                atk.FailAll();
            else
            {
                atk.Target.Defender.ChangeAbility(a);
                atk.Target.Defender.ShowLogPm(Ls.SetAbility, a);
                atk.Controller.ReportBuilder.ShowLog(Ls.setAbility, d);
            }
        }
        private static void Conversion2(AtkContext atk)
        {
            if (atk.Target.Defender.AtkContext == null) atk.FailAll();
            else
            {
                BattleType a = atk.Target.Defender.AtkContext.Type;
                if (a == BattleType.Invalid) a = BattleType.Normal;
                var rawtypes = atk.Attacker.OnboardPokemon.Types; //避免反复调用HasType性能
                var types = (from t in (BattleType[])Enum.GetValues(typeof(BattleType))
                             where !rawtypes.Contains(t) && (a.EffectRevise(t) < 0 || a.NoEffect(t)) //自动排除Invalid
                             select t).ToArray();
                var n = types.Length;
                if (n != 0)
                {
                    var type = types[atk.Controller.GetRandomInt(0, n - 1)];
                    atk.Attacker.OnboardPokemon.SetTypes(type);
                    atk.Attacker.ShowLogPm("TypeChange", (int)type);
                }
            }
        }
        private static void Encore(AtkContext atk)
        {
            var der = atk.Target.Defender;
            var last = der.LastMove;
            if (last != null && last.Id != 102 && last.Id != 144 && last.Id != 227)
                foreach (var m in der.Moves)
                    if (m.MoveE == last)
                    {
                        var c = new Condition() { Turn = 3, Move = last };
                        if (m.PP != 0 && der.OnboardPokemon.AddCondition(Cs.Encore, c))
                        {
                            der.ShowLogPm("EnEncore");
                            return;
                        }
                        break;
                    }
            atk.FailAll();
        }
        private static void Mimic(AtkContext atk)
        {
            var aer = atk.Attacker;
            var last = atk.Target.Defender.LastMove;
            if (last == null || aer.Moves.Any((m) => m.MoveE == last)) atk.FailAll();
            else
            {
                foreach (var m in aer.Moves)
                    if (m.MoveE == last)
                    {
                        atk.FailAll();
                        return;
                    }
                aer.ChangeMove(atk.Move, last);
                aer.Controller.ReportBuilder.Mimic(aer, last);
            }
        }
        private static void Spite(AtkContext atk)
        {
            var der = atk.Target.Defender;
            var move = der.LastMove;
            foreach (var m in der.Moves)
                if (m.MoveE == move)
                {
                    if (m.PP != 0)
                    {
                        var fp = m.PP;
                        m.PP -= 4;
                        der.ShowLogPm("Spite", m.MoveE.Id, fp - m.PP);
                        return;
                    }
                    break;
                }
            atk.FailAll();
        }

        private static void FSDD(AtkContext atk)
        {
            if (atk.HasCondition(Cs.FSDD)) AttackMove(atk);
            else
            {
                var tile = atk.Attacker.SelectedTarget;
                //使用猫手发动预知未来，不会选择空场地。
                if (tile == null) tile = MoveE.GetRangeTiles(atk, MoveRange.SelectedTarget, null).FirstOrDefault(); 
                if (tile == null || tile.HasCondition(Cs.FSDD)) atk.FailAll();
                else
                {
                    atk.Attacker.ShowLogPm("EnFSDD" + atk.Move.Id);
                    var c = new Condition();
                    c.Turn = atk.Controller.TurnNumber + 2;
                    c.Atk = new AtkContext(atk.Attacker) { IgnoreSwitchItem = true };
                    c.Atk.SetCondition(Cs.FSDD, atk.Move);
                    tile.AddCondition(Cs.FSDD, c);
                }
            }
        }
        private static void BeatUp(AtkContext atk)
        {
            PokemonProxy aer = atk.Attacker;

            int hits = 0;
            foreach (var owner in aer.Controller.Teams[aer.Pokemon.TeamId].Players)
                foreach (var pm in owner.Pokemons)
                    if (pm == aer || pm.State == PokemonState.Normal)
                    {
                        hits++;
                        atk.SetCondition(Cs.BeatUp, pm.Pokemon.Form.Data.Base.Atk);
                        CalculateDamages.Execute(atk);
                        Implement(atk.Targets);
                        if (atk.Target.Defender.Hp == 0 || aer.Hp == 0 || aer.State == PokemonState.FRZ || aer.State == PokemonState.SLP) goto Loopend;
                    }
            Loopend:
            atk.Controller.ReportBuilder.ShowLog("Hits", hits);

            FinalEffect(atk);
        }
        #endregion

        private static void HealPulse(AtkContext atk)
        {
            atk.Target.Defender.HpRecover(atk.Target.Defender.Pokemon.MaxHp * (atk.Attacker.AbilityE(As.MEGA_LAUNCHER) ? 75 : 50) / 100, true);
        }
        private static void AddType(AtkContext atk, BattleType type)
        {
            var der = atk.Target.Defender;
            if (der.OnboardPokemon.HasType(type)) atk.FailAll();
            else
            {
                der.OnboardPokemon.SetCondition(Cs.Type3, type);
                der.ShowLogPm("AddType", (int)type);
            }
        }
        private static void Terrain(AtkContext atk, Cs terrain)
        {
            var c = atk.Controller;
            var b = c.Board;
            int x = atk.Attacker.Item == Is.Terrain_Extender ? 7 : 4;
            if (b.AddCondition(terrain, c.TurnNumber + x))
            {
                if (!(terrain != Cs.GrassyTerrain && b.RemoveCondition(Cs.GrassyTerrain)|| terrain != Cs.ElectricTerrain && b.RemoveCondition(Cs.ElectricTerrain)) && terrain != Cs.MistyTerrain) b.RemoveCondition(Cs.MistyTerrain);
                if (terrain != Cs.PsychicTerrain) b.RemoveCondition(Cs.PsychicTerrain);
                c.ReportBuilder.ShowLog("En" + terrain);
            }
            else atk.FailAll();
        }
        private static void TorsyTurvy(AtkContext atk)
        {
            var op = atk.Target.Defender.OnboardPokemon;
            op.SetLv7D(0 - op.Lv5D.Atk, 0 - op.Lv5D.SpAtk, 0 - op.Lv5D.Def, 0 - op.Lv5D.SpDef, 0 - op.Lv5D.Speed, 0 - op.AccuracyLv, 0 - op.EvasionLv);
            atk.Target.Defender.ShowLogPm("TorsyTurvy");
        }
        private static void SelfProtect(AtkContext atk, Cs condition, string log)
        {
            atk.Attacker.OnboardPokemon.SetTurnCondition(condition);
            atk.Attacker.ShowLogPm(log);
        }
        private static void TeamProtect(AtkContext atk, Cs condition)
        {
            var team = atk.Attacker.Pokemon.TeamId;
            atk.Controller.Board[team].SetTurnCondition(condition);
            atk.Controller.ReportBuilder.ShowLog("En" + condition, team);
        }
        private static void WeatherMove(AtkContext atk, Weather weather)
        {
            var c = atk.Controller;
            if (!STs.SetWeather(atk.Attacker, weather, false, true)) atk.FailAll(null);
        }
        private static void AddTurnCondition(AtkContext atk, Cs condition)
        {
            if (atk.Target.Defender.OnboardPokemon.AddTurnCondition(condition)) atk.Target.Defender.ShowLogPm("En" + condition);
            else atk.FailAll();
        }
        private static void Spectral_Thief(AtkContext atk)
        {
            var aer = atk.Attacker;
            var der = atk.Target.Defender;
            int Datk = der.OnboardPokemon.Lv5D.Atk;
            int Ddef = der.OnboardPokemon.Lv5D.Def;
            int Dsa = der.OnboardPokemon.Lv5D.SpAtk;
            int Dsd = der.OnboardPokemon.Lv5D.SpDef;
            int Dsp = der.OnboardPokemon.Lv5D.Speed;
            int Dac = der.OnboardPokemon.AccuracyLv;
            int Dev = der.OnboardPokemon.EvasionLv;
            aer.OnboardPokemon.SetLv7D(Datk > 0 ? Datk : 0, Ddef > 0 ? Ddef : 0,Dsa > 0 ? Dsa : 0,Dsd > 0 ? Dsd : 0,Dsp > 0 ? Dsp : 0,Dac > 0 ? Dac : 0,Dev > 0 ? Dev : 0);
            der.OnboardPokemon.SetLv7D(Datk < 0 ? Datk : 0, Ddef < 0 ? Ddef : 0, Dsa < 0 ? Dsa : 0, Dsd < 0 ? Dsd : 0, Dsp < 0 ? Dsp : 0, Dac < 0 ? Dac : 0, Dev < 0 ? Dev : 0);
            aer.ShowLogPm("Spectral_Thief", atk.Target.Defender.Id);
        }
    }
}
