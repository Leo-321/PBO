using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonBattleOnline.Game.Host
{
    internal enum MoveClass
    {
        Attack,
        AddState,
        Lv7DChange,
        HpRecover,
        AttackWithState,
        ConfusionWithLv7DChange,
        AttackWithTargetLv7DChange,
        AttackWithSelfLv7DChange,
        AttackAndAbsorb,
        OHKO,
        AddMapState,
        AddTeamState,
        ForceToSwitch,
        Special
    }
    internal enum AttachedState : sbyte
    {
        None = 0,
        PAR = 0x01,
        SLP = 0x02,
        FRZ = 0x03,
        BRN = 0x04,
        /// <summary>
        /// 毒和剧毒都是，按持续时间分别
        /// </summary>
        PSN = 0x05,
        Confuse = 0x06,
        /// <summary>
        /// //着迷
        /// </summary>
        Attract = 0x07,
        Trap = 0x08,
        Nightmare = 0x09,
        /// <summary>
        /// 寻衅
        /// </summary>
        Torment = 0x0C,
        Disable = 0x0D,
        Yawn = 0x0E,
        HealBlock = 0x0F,
        /// <summary>
        /// 嗅觉，奇迹之眼
        /// </summary>
        CanAttack = 0x11,
        LeechSeed = 0x12,
        /// <summary>
        /// 扣押，5回合内不得使用道具
        /// </summary>
        Embargo = 0x13,
        PerishSong = 0x14,
        /// <summary>
        /// 扎根
        /// </summary>
        Ingrain = 0x15,
        SoundBlock = 0x16
    }
    internal class MoveLv7DChanges
    {
        public readonly StatType Type1;
        public readonly StatType Type2;
        public readonly StatType Type3;
        public readonly int Change1;
        public readonly int Change2;
        public readonly int Change3;
        public readonly int Probability;
        public MoveLv7DChanges(StatType type1, StatType type2, StatType type3, int change1, int change2, int change3, int probability)
        {
            Type1 = type1;
            Type2 = type2;
            Type3 = type3;
            Change1 = change1;
            Change2 = change2;
            Change3 = change3;
            Probability = probability;
        }
    }
    internal class MoveTypeE
    {
        private static readonly MoveTypeE[] Moves;
        static MoveTypeE()
        {
            using (var sr = new System.IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PokemonBattleOnline.Game.Host.dat.Moves.txt")))
            {
                Moves = new MoveTypeE[RomData.MOVES + 1];
                for (int i = 1; i <= RomData.MOVES; ++i) Moves[i] = new MoveTypeE(RomData.GetMove(i), sr.ReadLine());
            }
        }
        public static MoveTypeE Get(MoveType move)
        {
            return Get(move.Id);
        }
        public static MoveTypeE Get(int id)
        {
            return Moves[id];
        }

        public readonly MoveType Move;
        public readonly MoveClass Class;
        public readonly AttachedState AttachedState;
        public readonly int AttachedProbability;
        public readonly MoveLv7DChanges Lv7DChanges;
        public readonly bool NeedTouch;
        public readonly bool Protectable;
        public readonly bool MagicCoat;
        public readonly bool Snatchable;
        public readonly bool Mirrorable;
        public readonly bool IsRemote;
        public readonly bool IgnoreSubstitute;

        private MoveTypeE(MoveType move, string str)
        {
            Move = move;
            var s = str.Split('\t');
            //Lv7DChange			SpAtk	SpDef	Speed	2	2	2	0	0	0	0	0	0	0	0
            //AttackWithState	PAR	100								1	1	0	0	1	0	0
            Class = (MoveClass)Enum.Parse(typeof(MoveClass), s[0]);
            if (!string.IsNullOrEmpty(s[1]))
            {
                AttachedState = (AttachedState)Enum.Parse(typeof(AttachedState), s[1]);
                AttachedProbability = int.Parse(s[2]);
            }
            if (!string.IsNullOrEmpty(s[3]))
            {
                var t1 = (StatType)Enum.Parse(typeof(StatType), s[3]);
                var t2 = string.IsNullOrEmpty(s[4]) ? StatType.Invalid : (StatType)Enum.Parse(typeof(StatType), s[4]);
                var t3 = string.IsNullOrEmpty(s[5]) ? StatType.Invalid : (StatType)Enum.Parse(typeof(StatType), s[5]);
                Lv7DChanges = new MoveLv7DChanges(t1, t2, t3, int.Parse(s[6]), string.IsNullOrEmpty(s[7]) ? 0 : int.Parse(s[7]), string.IsNullOrEmpty(s[8]) ? 0 : int.Parse(s[8]), int.Parse(s[9]));
            }
            NeedTouch = s[10][0] == '1';
            Protectable = s[11][0] == '1';
            MagicCoat = s[12][0] == '1';
            Snatchable = s[13][0] == '1';
            Mirrorable = s[14][0] == '1';
            IsRemote = s[15][0] == '1';
            IgnoreSubstitute = s[16][0] == '1';
        }

        public int Id
        { get { return Move.Id; } }

        public int Priority
        {
            get
            {
                switch (Id)
                {
                    case Ms.TRICK_ROOM:
                        return -7;
                    case Ms.WHIRLWIND:
                    case Ms.ROAR:
                    case Ms.CIRCLE_THROW:
                    case Ms.DRAGON_TAIL:
                        return -6;
                    case Ms.COUNTER:
                    case Ms.MIRROR_COAT:
                        return -5;
                    case Ms.REVENGE:
                    case Ms.AVALANCHE:
                        return -4;
                    case Ms.FOCUS_PUNCH:
                    case Ms.Shell_Trap:
                    case Ms.Beak_Blast:
                        return -3;
                    case Ms.VITAL_THROW:
                        return -1;
                    case Ms.QUICK_ATTACK:
                    case Ms.BIDE:
                    case Ms.MACH_PUNCH:
                    case Ms.SUCKER_PUNCH:
                    case Ms.VACUUM_WAVE:
                    case Ms.BULLET_PUNCH:
                    case Ms.ICE_SHARD:
                    case Ms.SHADOW_SNEAK:
                    case Ms.AQUA_JET:
                    case Ms.ION_DELUGE:
                    case Ms.WATER_SHURIKEN:
                    case Ms.POWDER:
                    case Ms.BABYDOLL_EYES:
                    case Ms.Accelerock:
                        return 1;
                    case Ms.ALLY_SWITCH:
                    case Ms.EXTREME_SPEED:
                    case Ms.FOLLOW_ME:
                    case Ms.FEINT:
                    case Ms.RAGE_POWDER:
                    case Ms.First_Impression:
                    case Ms.Spotlight:
                        return 2;
                    case Ms.FAKE_OUT:
                    case Ms.WIDE_GUARD:
                    case Ms.QUICK_GUARD:
                    case Ms.CRAFTY_SHIELD:
                        return 3;
                    case Ms.PROTECT:
                    case Ms.DETECT:
                    case Ms.ENDURE:
                    case Ms.MAGIC_COAT:
                    case Ms.SNATCH:
                    case Ms.KINGS_SHIELD:
                    case Ms.SPIKY_SHIELD:
                    case Ms.Baneful_Bunker:
                        return 4;
                    case Ms.HELPING_HAND:
                        return 5;
                    default:
                        return 0;
                }
            }
        }

        public int FlinchProbability
        {
            get
            {
                switch (Id)
                {
                    case Ms.BONE_CLUB:
                    case Ms.HYPER_FANG:
                    case Ms.EXTRASENSORY:
                    case Ms.THUNDER_FANG:
                    case Ms.ICE_FANG:
                    case Ms.FIRE_FANG:
                        return 10;
                    case Ms.WATERFALL:
                    case Ms.TWISTER:
                    case Ms.DARK_PULSE:
                    case Ms.DRAGON_RUSH:
                    case Ms.ZEN_HEADBUTT:
                        return 20;
                    case Ms.STOMP:
                    case Ms.ROLLING_KICK:
                    case Ms.HEADBUTT:
                    case Ms.BITE:
                    case Ms.SKY_ATTACK:
                    case Ms.ROCK_SLIDE:
                    case Ms.SNORE:
                    case Ms.NEEDLE_ARM:
                    case Ms.ASTONISH:
                    case Ms.AIR_SLASH:
                    case Ms.IRON_HEAD:
                    case Ms.HEART_STAMP:
                    case Ms.STEAMROLLER:
                    case Ms.ICICLE_CRASH:
                    case Ms.Zing_Zap:
                        return 30;
                    case Ms.FAKE_OUT:
                        return 100;
                    default:
                        return 0;
                }
        }
        }

        public int HurtPercentage
        {
            get
            {
                switch(Id)
                {
                    case Ms.HEAD_SMASH:
                        return -50;
                    case Ms.DOUBLEEDGE:
                    case Ms.VOLT_TACKLE:
                    case Ms.FLARE_BLITZ:
                    case Ms.BRAVE_BIRD:
                    case Ms.WOOD_HAMMER:
                        return -33;
                    case Ms.TAKE_DOWN:
                    case Ms.SUBMISSION:
                    case Ms.WILD_CHARGE:
                    case Ms.HEAD_CHARGE:
                        return -25;
                    case Ms.ABSORB:
                    case Ms.MEGA_DRAIN:
                    case Ms.DREAM_EATER:
                    case Ms.LEECH_LIFE:
                    case Ms.GIGA_DRAIN:
                    case Ms.DRAIN_PUNCH:
                    case Ms.HORN_LEECH:
                    case Ms.PARABOLIC_CHARGE:
                        return 50;
                    case Ms.DRAINING_KISS:
                    case Ms.OBLIVION_WING:
                        return 75;
                    default:
                        return 0;
                }
            }
        }
        public int MaxHpPercentage
        {
            get
            {
                switch (Id)
                {
                    case Ms.STRUGGLE:
                        return -25;
                    case Ms.SWALLOW: //useless
                        return 25;
                    case Ms.RECOVER:
                    case Ms.SOFTBOILED:
                    case Ms.MILK_DRINK:
                    case Ms.MORNING_SUN:
                    case Ms.SYNTHESIS:
                    case Ms.MOONLIGHT:
                    case Ms.SLACK_OFF:
                    case Ms.ROOST:
                    case Ms.HEAL_ORDER:
                    case Ms.HEAL_PULSE:
                    case Ms.Shore_Up:
                    case Ms.Floral_Healing:
                        return 50;
                    default:
                        return 0;
                }
            }
        }

        public int MinTimes
        {
            get
            {
                switch (Id)
                {
                    case Ms.DOUBLE_KICK:
                    case Ms.TWINEEDLE:
                    case Ms.BONEMERANG:
                    case Ms.DOUBLE_HIT:
                    case Ms.DUAL_CHOP:
                    case Ms.GEAR_GRIND:
                    case Ms.DOUBLE_SLAP:
                    case Ms.COMET_PUNCH:
                    case Ms.FURY_ATTACK:
                    case Ms.PIN_MISSILE:
                    case Ms.SPIKE_CANNON:
                    case Ms.BARRAGE:
                    case Ms.FURY_SWIPES:
                    case Ms.BONE_RUSH:
                    case Ms.ARM_THRUST:
                    case Ms.BULLET_SEED:
                    case Ms.ICICLE_SPEAR:
                    case Ms.ROCK_BLAST:
                    case Ms.TAIL_SLAP:
                    case Ms.WATER_SHURIKEN:
                        return 2;
                    case Ms.TRIPLE_KICK:
                        return 3;
                    case Ms.BEAT_UP:
                        return 12;
                    default:
                        return 0;
                }
            }
        }
        public int MaxTimes
        {
            get
            {
                switch (Id)
                {
                    case Ms.DOUBLE_KICK:
                    case Ms.TWINEEDLE:
                    case Ms.BONEMERANG:
                    case Ms.DOUBLE_HIT:
                    case Ms.DUAL_CHOP:
                    case Ms.GEAR_GRIND:
                        return 2;
                    case Ms.TRIPLE_KICK:
                        return 3;
                    case Ms.DOUBLE_SLAP:
                    case Ms.COMET_PUNCH:
                    case Ms.FURY_ATTACK:
                    case Ms.PIN_MISSILE:
                    case Ms.SPIKE_CANNON:
                    case Ms.BARRAGE:
                    case Ms.FURY_SWIPES:
                    case Ms.BONE_RUSH:
                    case Ms.ARM_THRUST:
                    case Ms.BULLET_SEED:
                    case Ms.ICICLE_SPEAR:
                    case Ms.ROCK_BLAST:
                    case Ms.TAIL_SLAP:
                    case Ms.WATER_SHURIKEN:
                        return 5;
                    case Ms.BEAT_UP:
                        return 12;
                    default:
                        return 0;
                }
            }
        }

        public bool AvailableEvenSleeping
        { get { return Id == Ms.SLEEP_TALK || Id == Ms.SNORE; } }

        public bool SkipSleepMTA
        { get { return Id == Ms.THRASH || Id == Ms.PETAL_DANCE || Id == Ms.OUTRAGE || Id == Ms.ROLLOUT || Id == Ms.ICE_BALL; } }

        public bool Switch
        { get { return Id == Ms.UTURN || Id == Ms.VOLT_SWITCH || Id == Ms.PARTING_SHOT; } }

        public bool IgnoreDefenderLv7D
        { get { return Id == Ms.CHIP_AWAY || Id == Ms.SACRED_SWORD || Id == Ms.Darkest_Lariat; } }

        public bool UsePhysicalDef
        { get { return Id == Ms.PSYSHOCK || Id == Ms.PSYSTRIKE || Id == Ms.SECRET_SWORD; } }

        public MoveRange GetRange(PokemonProxy pm)
        {
            return Id == Ms.CURSE ? pm.OnboardPokemon.HasType(BattleType.Ghost) ? MoveRange.SelectedTarget : MoveRange.Self : Move.Range;
        }

        public bool HasProbabilitiedAdditonalEffects
        {
            get
            {//
                return
                  Class == MoveClass.AttackWithState ||
                  Class == MoveClass.AttackWithTargetLv7DChange ||
                  FlinchProbability > 0 ||
                  (AttachedState != AttachedState.None && AttachedProbability > 0) ||
                  (Class == MoveClass.AttackWithSelfLv7DChange && Lv7DChanges.Change1 > 0);
            }
        }

        private static int[] MENTAL = new int[] { Ms.DISABLE, Ms.ATTRACT, Ms.ENCORE, Ms.TORMENT, Ms.TAUNT, Ms.HEAL_BLOCK };
        public bool Mental
        { get { return MENTAL.Contains(Id); } }

        private static int[] TEETH = new int[] { Ms.BITE, Ms.CRUNCH, Ms.FIRE_FANG, Ms.HYPER_FANG, Ms.HYPER_FANG, Ms.ICE_FANG, Ms.POISON_FANG, Ms.THUNDER_FANG };
        public bool Teeth
        { get { return TEETH.Contains(Id); } }

        private static int[] BULLET = new int[] { Ms.ACID_SPRAY, Ms.AURA_SPHERE, Ms.BARRAGE, Ms.BULLET_SEED, Ms.EGG_BOMB, Ms.ELECTRO_BALL, Ms.ENERGY_BALL, Ms.FOCUS_BLAST, Ms.GYRO_BALL, Ms.ICE_BALL, Ms.MAGNET_BOMB, Ms.MIST_BALL, Ms.MUD_BOMB, Ms.OCTAZOOKA, Ms.ROCK_WRECKER, Ms.SEARING_SHOT, Ms.SEED_BOMB, Ms.SHADOW_BALL, Ms.SLUDGE_BOMB, Ms.WEATHER_BALL, Ms.ZAP_CANNON };
        public bool Bullet
        { get { return BULLET.Contains(Id); } }

        private static int[] PULSE = new int[] { Ms.AURA_SPHERE, Ms.DARK_PULSE, Ms.DRAGON_PULSE, Ms.WATER_PULSE };
        /// <summary>
        /// does not contains heal pulse
        /// </summary>
        public bool Pulse
        { get { return PULSE.Contains(Id); } }

        private static readonly int[] POWDER = new int[] { Ms.COTTON_SPORE, Ms.POISON_POWDER, Ms.POWDER, Ms.SLEEP_POWDER, Ms.SPORE, Ms.STUN_SPORE };
        public bool Powder
        { get { return POWDER.Contains(Id); } }

        public bool Spore
        { get { return Id == Ms.SPORE || Id == Ms.COTTON_SPORE || Id == Ms.STUN_SPORE; } }

        private static readonly int[] SOUND = new int[] { Ms.GROWL, Ms.ROAR, Ms.SING, Ms.SUPERSONIC, Ms.SCREECH, Ms.SNORE, Ms.PERISH_SONG, Ms.HEAL_BELL, Ms.UPROAR, Ms.HYPER_VOICE, Ms.METAL_SOUND, Ms.GRASS_WHISTLE, Ms.BUG_BUZZ, Ms.CHATTER, Ms.ROUND, Ms.ECHOED_VOICE, Ms.RELIC_SONG, Ms.SNARL, Ms.CONFIDE, Ms.DISARMING_VOICE, Ms.BOOMBURST, Ms.Sparkling_Aria, Ms.Clanging_Scales };
        public bool Sound
        { get { return SOUND.Contains(Id); } }

        private static readonly int[] FIST = new int[] { Ms.BULLET_PUNCH, Ms.COMET_PUNCH, Ms.DIZZY_PUNCH, Ms.DRAIN_PUNCH, Ms.DYNAMIC_PUNCH, Ms.FIRE_PUNCH, Ms.FOCUS_PUNCH, Ms.HAMMER_ARM, Ms.ICE_PUNCH, Ms.MACH_PUNCH, Ms.MEGA_PUNCH, Ms.METEOR_MASH, Ms.POWERUP_PUNCH, Ms.SHADOW_PUNCH, Ms.SKY_UPPERCUT, Ms.THUNDER_PUNCH };
        public bool Fist
        { get { return FIST.Contains(Id); } }

        private static readonly int[] HEAL = new int[] { Ms.RECOVER, Ms.SOFTBOILED, Ms.REST, Ms.MILK_DRINK, Ms.MORNING_SUN, Ms.SYNTHESIS, Ms.MOONLIGHT, Ms.SWALLOW, Ms.WISH, Ms.SLACK_OFF, Ms.ROOST, Ms.HEALING_WISH, Ms.HEAL_ORDER, Ms.LUNAR_DANCE, Ms.HEAL_PULSE, Ms.Purify, Ms.Strength_Sap, Ms.Floral_Healing, Ms.Shore_Up };
        public bool Heal
        { get { return HEAL.Contains(Id); } }

        private static readonly int[] NOGRAVITY = new int[] { Ms.FLY, Ms.JUMP_KICK, Ms.HIGH_JUMP_KICK, Ms.SPLASH, Ms.BOUNCE, Ms.MAGNET_RISE, Ms.TELEKINESIS, Ms.SKY_DROP };
        public bool UnavailableWithGravity
        { get { return NOGRAVITY.Contains(Id); } }

        public bool SelfDeFrozen
        { get { return Id == Ms.FLAME_WHEEL || Id == Ms.SACRED_FIRE || Id == Ms.FLARE_BLITZ || Id == Ms.SCALD || Id == Ms.FUSION_FLARE; } }

        private static readonly int[] STIFF = new int[] { Ms.HYPER_BEAM, Ms.BLAST_BURN, Ms.HYDRO_CANNON, Ms.FRENZY_PLANT, Ms.GIGA_IMPACT, Ms.ROCK_WRECKER, Ms.ROAR_OF_TIME };
        public bool StiffOneTurn
        { get { return STIFF.Contains(Id); } }

        private static readonly int[] PREPARE = new int[] { Ms.RAZOR_WIND, Ms.FLY, Ms.SOLAR_BEAM, Ms.DIG, Ms.SKULL_BASH, Ms.SKY_ATTACK, Ms.DIVE, Ms.BOUNCE, Ms.SHADOW_FORCE, Ms.SKY_DROP, Ms.FREEZE_SHOCK, Ms.ICE_BURN, Ms.PHANTOM_FORCE, Ms.GEOMANCY, Ms.Solar_Blade };
        public bool PrepareOneTurn
        { get { return PREPARE.Contains(Id); } }

        private static int[] CONTINUOUS_USE = new int[] { Ms.PROTECT, Ms.DETECT, Ms.ENDURE, Ms.CRAFTY_SHIELD, Ms.SPIKY_SHIELD, Ms.KINGS_SHIELD, Ms.Baneful_Bunker };
        public bool HardToUseContinuously
        { get { return CONTINUOUS_USE.Contains(Id); } }

        private static readonly int[] CT1 = new int[] { Ms.KARATE_CHOP, Ms.RAZOR_WIND, Ms.RAZOR_LEAF, Ms.SKY_ATTACK, Ms.CRABHAMMER, Ms.SLASH, Ms.AEROBLAST, Ms.CROSS_CHOP, Ms.BLAZE_KICK, Ms.AIR_CUTTER, Ms.POISON_TAIL, Ms.LEAF_BLADE, Ms.NIGHT_SLASH, Ms.SHADOW_CLAW, Ms.PSYCHO_CUT, Ms.CROSS_POISON, Ms.STONE_EDGE, Ms.ATTACK_ORDER, Ms.SPACIAL_REND, Ms.DRILL_RUN };
        public bool Ct1
        { get { return CT1.Contains(Id); } }

        public bool MustCt
        { get { return Id == Ms.STORM_THROW || Id == Ms.FROST_BREATH; } }

        private static readonly int[] dance = new int[] {Ms.DRAGON_DANCE,Ms.FEATHER_DANCE,Ms.FIERY_DANCE,Ms.LUNAR_DANCE,Ms.PETAL_DANCE,Ms.QUIVER_DANCE,Ms.SWORDS_DANCE,Ms.TEETER_DANCE};
        public bool Dance
        { get { return dance.Contains(Id); } }

        public bool Zmove
        { get { return Id <= 719 && Id >= 673; } }

        public bool CommonZmove
        { get { return Id <= 708 && Id >= 673; } }
    }
}
