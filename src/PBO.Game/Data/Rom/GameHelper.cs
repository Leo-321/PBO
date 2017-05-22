using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
  public static class GameHelper
  {
    public static readonly StatType[] SEVEN_D = { StatType.Atk, StatType.Def, StatType.SpAtk, StatType.SpDef, StatType.Speed, StatType.Accuracy, StatType.Evasion };
    /// <summary>
    /// [atk, def]
    /// </summary>
    private static readonly int[,] REVISE = new int[19, 19] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, -1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 1, 0, -1, -1, 0, 1, -1, 0, 1, 0, 0, 0, 0, -1, 1, 0, 1, -1 }, { 0, 0, 1, 0, 0, 0, -1, 1, 0, -1, 0, 0, 1, -1, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, -1, -1, -1, 0, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1 }, { 0, 0, 0, 0, 1, 0, 1, -1, 0, 1, 1, 0, -1, 1, 0, 0, 0, 0, 0 }, { 0, 0, -1, 1, 0, -1, 0, 1, 0, -1, 1, 0, 0, 0, 0, 1, 0, 0, 0 }, { 0, 0, -1, -1, -1, 0, 0, 0, -1, -1, -1, 0, 1, 0, 1, 0, 0, 1, -1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0 }, { 0, 0, 0, 0, 0, 0, 1, 0, 0, -1, -1, -1, 0, -1, 0, 1, 0, 0, 1 }, { 0, 0, 0, 0, 0, 0, -1, 1, 0, 1, -1, -1, 1, 0, 0, 1, -1, 0, 0 }, { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, -1, -1, 0, 0, 0, -1, 0, 0 }, { 0, 0, 0, -1, -1, 1, 1, -1, 0, -1, -1, 1, -1, 0, 0, 0, -1, 0, 0 }, { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, -1, -1, 0, 0, -1, 0, 0 }, { 0, 0, 1, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, -1, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0, 1, 0, 0, 0, -1, -1, -1, 1, 0, 0, -1, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, -1 }, { 0, 0, 1, 0, -1, 0, 0, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 1, 0 } };
    /// <summary>
    /// [atk]
    /// </summary>
    private static readonly BattleType[] NO_EFFECT;

    static GameHelper()
    {
      NO_EFFECT = new BattleType[RomData.BATTLETYPES + 1];
      NO_EFFECT[(int)BattleType.Normal] = NO_EFFECT[(int)BattleType.Fighting] = BattleType.Ghost;
      NO_EFFECT[(int)BattleType.Electric] = BattleType.Ground;
      NO_EFFECT[(int)BattleType.Poison] = BattleType.Steel;
      NO_EFFECT[(int)BattleType.Psychic] = BattleType.Dark;
      NO_EFFECT[(int)BattleType.Ghost] = BattleType.Normal;
      NO_EFFECT[(int)BattleType.Ground] = BattleType.Flying;
      NO_EFFECT[(int)BattleType.Dragon] = BattleType.Fairy;
    }
    public static int Get5D(StatType statType, PokemonNature nature, int typeBase, int iv, int ev, int lv)
    {
      return (((typeBase << 1) + iv + (ev >> 2)) * lv / 100 + 5) * nature.StatRevise(statType) / 10;
    }
    public static int GetHp(int typeBase, int iv, int ev, int lv)
    {
      return typeBase == 1 ? 1 : (((typeBase << 1) + iv + (ev >> 2)) * lv / 100 + 10 + lv);
    }

    public static bool NoEffect(this BattleType a, BattleType d)
    {
      return d != BattleType.Invalid && NO_EFFECT[(int)a] == d;
    }
    public static BattleType NoEffect(this BattleType a)
    {
      return NO_EFFECT[(int)a];
    }
    public static int EffectRevise(this BattleType a, BattleType d)
    {
      return REVISE[(int)a, (int)d];
    }
    public static int EffectRevise(this BattleType a, IEnumerable<BattleType> d)
    {
      var r = 0;
      foreach(var t in d) r += EffectRevise(a, t);
      return r;
    }

    public static BattleType HiddenPower(I6D iv)
    {
      int pI = iv.Hp & 1;
      pI |= (iv.Atk & 1) << 1;
      pI |= (iv.Def & 1) << 2;
      pI |= (iv.Speed & 1) << 3;
      pI |= (iv.SpAtk & 1) << 4;
      pI |= (iv.SpDef & 1) << 5;
      return (BattleType)(pI * 15 / 63 + 2);
    }

    public static BattleType NatureGift(int item)
        {
            switch (item)
            {
                case Is.CHERI_BERRY:
                case Is.BLUK_BERRY:
                case Is.WATMEL_BERRY:
                case Is.OCCA_BERRY:
                    return BattleType.Fire;
                case Is.CHESTO_BERRY:
                case Is.NANAB_BERRY:
                case Is.DURIN_BERRY:
                case Is.PASSHO_BERRY:
                    return BattleType.Water;
                case Is.PECHA_BERRY:
                case Is.BELUE_BERRY:
                case Is.WEPEAR_BERRY:
                case Is.WACAN_BERRY:
                    return BattleType.Electric;
                case Is.RAWST_BERRY:
                case Is.PINAP_BERRY:
                case Is.RINDO_BERRY:
                case Is.LIECHI_BERRY:
                    return BattleType.Grass;
                case Is.ASPEAR_BERRY:
                case Is.POMEG_BERRY:
                case Is.YACHE_BERRY:
                case Is.GANLON_BERRY:
                    return BattleType.Ice;
                case Is.LEPPA_BERRY:
                case Is.KELPSY_BERRY:
                case Is.CHOPLE_BERRY:
                case Is.SALAC_BERRY:
                    return BattleType.Fighting;
                case Is.ORAN_BERRY:
                case Is.QUALOT_BERRY:
                case Is.KEBIA_BERRY:
                case Is.PETAYA_BERRY:
                    return BattleType.Poison;
                case Is.PERSIM_BERRY:
                case Is.HONDEW_BERRY:
                case Is.SHUCA_BERRY:
                case Is.APICOT_BERRY:
                    return BattleType.Ground;
                case Is.LUM_BERRY:
                case Is.GREPA_BERRY:
                case Is.COBA_BERRY:
                case Is.LANSAT_BERRY:
                    return BattleType.Flying;
                case Is.SITRUS_BERRY:
                case Is.TAMATO_BERRY:
                case Is.PAYAPA_BERRY:
                case Is.STARF_BERRY:
                    return BattleType.Psychic;
                case Is.FIGY_BERRY:
                case Is.CORNN_BERRY:
                case Is.TANGA_BERRY:
                case Is.ENIGMA_BERRY:
                    return BattleType.Bug;
                case Is.WIKI_BERRY:
                case Is.MAGOST_BERRY:
                case Is.CHARTI_BERRY:
                case Is.MICLE_BERRY:
                    return BattleType.Rock;
                case Is.MAGO_BERRY:
                case Is.RABUTA_BERRY:
                case Is.KASIB_BERRY:
                case Is.CUSTAP_BERRY:
                    return BattleType.Ghost;
                case Is.AGUAV_BERRY:
                case Is.NOMEL_BERRY:
                case Is.HABAN_BERRY:
                case Is.JABOCA_BERRY:
                    return BattleType.Dragon;
                case Is.IAPAPA_BERRY:
                case Is.SPELON_BERRY:
                case Is.COLBUR_BERRY:
                case Is.ROWAP_BERRY:
                    return BattleType.Dark;
                case Is.RAZZ_BERRY:
                case Is.PAMTRE_BERRY:
                case Is.BABIRI_BERRY:
                    return BattleType.Steel;
                case Is.ROSELI_BERRY:
                case Is.KEE_BERRY:
                case Is.MARANGA_BERRY:
                    return BattleType.Fairy;
                case Is.CHILAN_BERRY:
                    return BattleType.Normal;
            }
            return BattleType.Normal;
        }
        public static int NaturalGiftPower(int item)
        {
            switch (item)
            {
                case Is.CHERI_BERRY:
                case Is.CHESTO_BERRY:
                case Is.PECHA_BERRY:
                case Is.RAWST_BERRY:
                case Is.ASPEAR_BERRY:
                case Is.LEPPA_BERRY:
                case Is.ORAN_BERRY:
                case Is.PERSIM_BERRY:
                case Is.LUM_BERRY:
                case Is.SITRUS_BERRY:
                case Is.FIGY_BERRY:
                case Is.WIKI_BERRY:
                case Is.MAGO_BERRY:
                case Is.AGUAV_BERRY:
                case Is.IAPAPA_BERRY:
                case Is.RAZZ_BERRY:
                    return 60;
                case Is.BLUK_BERRY:
                case Is.NANAB_BERRY:
                case Is.WEPEAR_BERRY:
                case Is.PINAP_BERRY:
                case Is.POMEG_BERRY:
                case Is.KELPSY_BERRY:
                case Is.QUALOT_BERRY:
                case Is.HONDEW_BERRY:
                case Is.GREPA_BERRY:
                case Is.TAMATO_BERRY:
                case Is.CORNN_BERRY:
                case Is.MAGOST_BERRY:
                case Is.RABUTA_BERRY:
                case Is.NOMEL_BERRY:
                case Is.SPELON_BERRY:
                case Is.PAMTRE_BERRY:
                    return 70;
                case Is.WATMEL_BERRY:
                case Is.DURIN_BERRY:
                case Is.BELUE_BERRY:
                    return 80;
                case Is.OCCA_BERRY:
                case Is.PASSHO_BERRY:
                case Is.WACAN_BERRY:
                case Is.RINDO_BERRY:
                case Is.YACHE_BERRY:
                case Is.CHOPLE_BERRY:
                case Is.KEBIA_BERRY:
                case Is.SHUCA_BERRY:
                case Is.COBA_BERRY:
                case Is.PAYAPA_BERRY:
                case Is.TANGA_BERRY:
                case Is.CHARTI_BERRY:
                case Is.KASIB_BERRY:
                case Is.HABAN_BERRY:
                case Is.COLBUR_BERRY:
                case Is.BABIRI_BERRY:
                case Is.ROSELI_BERRY:
                case Is.CHILAN_BERRY:
                    return 60;
                case Is.LIECHI_BERRY:
                case Is.GANLON_BERRY:
                case Is.SALAC_BERRY:
                case Is.PETAYA_BERRY:
                case Is.APICOT_BERRY:
                case Is.LANSAT_BERRY:
                case Is.STARF_BERRY:
                case Is.ENIGMA_BERRY:
                case Is.MICLE_BERRY:
                case Is.CUSTAP_BERRY:
                case Is.JABOCA_BERRY:
                case Is.ROWAP_BERRY:
                case Is.KEE_BERRY:
                case Is.MARANGA_BERRY:
                    return 80;
            }
            return 0;
        }
        public static int FlingPower(int item)
        {
            switch (item)
            {
                case Is.IRON_BALL:
                    return 130;
                case Is.RARE_BONE:
                case Is.HARD_STONE:
                    return 100;
                case Is.DEEPSEATOOTH:
                case Is.THICK_CLUB:
                case Is.GRIP_CLAW:
                case Is.FLAME_PLATE:
                case Is.SPLASH_PLATE:
                case Is.ZAP_PLATE:
                case Is.MEADOW_PLATE:
                case Is.ICICLE_PLATE:
                case Is.FIST_PLATE:
                case Is.TOXIC_PLATE:
                case Is.EARTH_PLATE:
                case Is.SKY_PLATE:
                case Is.MIND_PLATE:
                case Is.INSECT_PLATE:
                case Is.STONE_PLATE:
                case Is.SPOOKY_PLATE:
                case Is.DRACO_PLATE:
                case Is.DREAD_PLATE:
                case Is.IRON_PLATE:
                case Is.PIXIE_PLATE:
                    return 90;
                case Is.RAZOR_CLAW:
                case Is.QUICK_CLAW:
                case Is.STICKY_BARB:
                    return 80;
                case Is.POWER_BRACER:
                case Is.POWER_BELT:
                case Is.POWER_LENS:
                case Is.POWER_BAND:
                case Is.POWER_ANKLET:
                case Is.POWER_WEIGHT:
                case Is.DOUSE_DRIVE:
                case Is.SHOCK_DRIVE:
                case Is.BURN_DRIVE:
                case Is.CHILL_DRIVE:
                case Is.POISON_BARB:
                case Is.DRAGON_FANG:
                    return 70;
                case Is.ROCKY_HELMET:
                case Is.MACHO_BRACE:
                case Is.GRISEOUS_ORB:
                case Is.ADAMANT_ORB:
                case Is.LUSTROUS_ORB:
                case Is.STICK:
                case Is.HEAT_ROCK:
                case Is.DAMP_ROCK:
                    return 60;
                case Is.SHARP_BEAK:
                    return 50;
                case Is.EVIOLITE:
                case Is.LUCKY_PUNCH:
                case Is.ICY_ROCK:
                    return 40;
                case Is.BERRY_JUICE:
                case Is.LIFE_ORB:
                case Is.SHELL_BELL:
                case Is.METRONOME:
                case Is.SCOPE_LENS:
                case Is.RAZOR_FANG:
                case Is.KINGS_ROCK:
                case Is.FLOAT_STONE:
                case Is.BLACK_SLUDGE:
                case Is.TOXIC_ORB:
                case Is.FLAME_ORB:
                case Is.EJECT_BUTTON:
                case Is.ABSORB_BULB:
                case Is.CELL_BATTERY:
                case Is.LIGHT_BALL:
                case Is.SOUL_DEW:
                case Is.DEEPSEASCALE:
                case Is.LIGHT_CLAY:
                case Is.BINDING_BAND:
                case Is.METAL_COAT:
                case Is.MIRACLE_SEED:
                case Is.BLACKGLASSES:
                case Is.BLACK_BELT:
                case Is.MAGNET:
                case Is.MYSTIC_WATER:
                case Is.NEVERMELTICE:
                case Is.SPELL_TAG:
                case Is.TWISTEDSPOON:
                case Is.CHARCOAL:
                    return 30;
                case Is.RING_TARGET:
                case Is.BRIGHT_POWDER:
                case Is.WIDE_LENS:
                case Is.ZOOM_LENS:
                case Is.MUSCLE_BAND:
                case Is.WISE_GLASSES:
                case Is.EXPERT_BELT:
                case Is.WHITE_HERB:
                case Is.MENTAL_HERB:
                case Is.DESTINY_KNOT:
                case Is.LAGGING_TAIL:
                case Is.SHED_SHELL:
                case Is.LEFTOVERS:
                case Is.FOCUS_BAND:
                case Is.FOCUS_SASH:
                case Is.AIR_BALLOON:
                case Is.RED_CARD:
                case Is.METAL_POWDER:
                case Is.QUICK_POWDER:
                case Is.POWER_HERB:
                case Is.BIG_ROOT:
                case Is.SMOOTH_ROCK:
                case Is.CHOICE_BAND:
                case Is.CHOICE_SCARF:
                case Is.CHOICE_SPECS:
                case Is.SILVERPOWDER:
                case Is.SOFT_SAND:
                case Is.SILK_SCARF:
                case Is.SEA_INCENSE:
                case Is.LAX_INCENSE:
                case Is.ODD_INCENSE:
                case Is.ROCK_INCENSE:
                case Is.FULL_INCENSE:
                case Is.WAVE_INCENSE:
                case Is.ROSE_INCENSE:
                case Is.CHERI_BERRY:
                case Is.CHESTO_BERRY:
                case Is.PECHA_BERRY:
                case Is.RAWST_BERRY:
                case Is.ASPEAR_BERRY:
                case Is.LEPPA_BERRY:
                case Is.ORAN_BERRY:
                case Is.PERSIM_BERRY:
                case Is.LUM_BERRY:
                case Is.SITRUS_BERRY:
                case Is.FIGY_BERRY:
                case Is.WIKI_BERRY:
                case Is.MAGO_BERRY:
                case Is.AGUAV_BERRY:
                case Is.IAPAPA_BERRY:
                case Is.RAZZ_BERRY:
                case Is.BLUK_BERRY:
                case Is.NANAB_BERRY:
                case Is.WEPEAR_BERRY:
                case Is.PINAP_BERRY:
                case Is.POMEG_BERRY:
                case Is.KELPSY_BERRY:
                case Is.QUALOT_BERRY:
                case Is.HONDEW_BERRY:
                case Is.GREPA_BERRY:
                case Is.TAMATO_BERRY:
                case Is.CORNN_BERRY:
                case Is.MAGOST_BERRY:
                case Is.RABUTA_BERRY:
                case Is.NOMEL_BERRY:
                case Is.SPELON_BERRY:
                case Is.PAMTRE_BERRY:
                case Is.WATMEL_BERRY:
                case Is.DURIN_BERRY:
                case Is.BELUE_BERRY:
                case Is.OCCA_BERRY:
                case Is.PASSHO_BERRY:
                case Is.WACAN_BERRY:
                case Is.RINDO_BERRY:
                case Is.YACHE_BERRY:
                case Is.CHOPLE_BERRY:
                case Is.KEBIA_BERRY:
                case Is.SHUCA_BERRY:
                case Is.COBA_BERRY:
                case Is.PAYAPA_BERRY:
                case Is.TANGA_BERRY:
                case Is.CHARTI_BERRY:
                case Is.KASIB_BERRY:
                case Is.HABAN_BERRY:
                case Is.COLBUR_BERRY:
                case Is.BABIRI_BERRY:
                case Is.ROSELI_BERRY:
                case Is.CHILAN_BERRY:
                case Is.LIECHI_BERRY:
                case Is.GANLON_BERRY:
                case Is.SALAC_BERRY:
                case Is.PETAYA_BERRY:
                case Is.APICOT_BERRY:
                case Is.LANSAT_BERRY:
                case Is.STARF_BERRY:
                case Is.ENIGMA_BERRY:
                case Is.MICLE_BERRY:
                case Is.CUSTAP_BERRY:
                case Is.JABOCA_BERRY:
                case Is.ROWAP_BERRY:
                case Is.KEE_BERRY:
                case Is.MARANGA_BERRY:
                    return 10;
                default:
                    return 0;
            }
        }

        //gen7
        public static Move Zmove(Move move, int item, int pokemon, int form)
        {
            var x = Zmove(move.Type, item, pokemon, form);
            if (x == null)
                return null;
            return new Move(x, move.PP.Value);
        }
        public static MoveType Zmove(MoveType Type, int item, int pokemon, int form)
        {
            if (form == 0 && item == Is.Snorlium_Z && pokemon == 143 && Type.Id == Ms.GIGA_IMPACT)
                return RomData.Moves.ElementAtOrDefault(Ms.Pulverizing_Pancake-1);
            if (form == 1 && item == Is.Aloraichium_Z && pokemon == 26 && Type.Id == Ms.THUNDERBOLT)
                return RomData.Moves.ElementAtOrDefault(Ms.Stoked_Sparksurfer-1);
            if (form == 0 && item == Is.Pikanium_Z && pokemon == 25 && Type.Id == Ms.VOLT_TACKLE)
                return RomData.Moves.ElementAtOrDefault(Ms.Castatropika-1);
            if (form == 0 && item == Is.Pikashunium_Z && pokemon == 25 && Type.Id == Ms.THUNDERBOLT)
                return RomData.Moves.ElementAtOrDefault(Ms.tenkk_Volt-1);
            if (form == 0 && item == Is.Eevium_Z && pokemon == 133 && Type.Id == Ms.LAST_RESORT)
                return RomData.Moves.ElementAtOrDefault(Ms.Extreme_Evoboost-1);
            if (form == 0 && item == Is.Tapunium_Z && pokemon>=785 && pokemon<=788 && Type.Id == Ms.Natures_Madness)
                return RomData.Moves.ElementAtOrDefault(Ms.Guardian_of_Alola-1);
            if (form == 0 && item == Is.Mewnium_Z && pokemon == 151 && Type.Id == Ms.PSYCHIC)
                return RomData.Moves.ElementAtOrDefault(Ms.Genesis_Supernova-1);
            if (form == 0 && item == Is.Decidium_Z && pokemon == 724 && Type.Id == Ms.Spirit_Shackle)
                return RomData.Moves.ElementAtOrDefault(Ms.Sinister_Arrow_Raid-1);
            if (form == 0 && item == Is.Incinium_Z && pokemon == 727 && Type.Id == Ms.Darkest_Lariat)
                return RomData.Moves.ElementAtOrDefault(Ms.Malicious_Moonsault-1);
            if (form == 0 && item == Is.Primarium_Z && pokemon == 730 && Type.Id == Ms.Sparkling_Aria)
                return RomData.Moves.ElementAtOrDefault(Ms.Oceanic_Operetta-1);
            if (form == 0 && item == Is.Marshadium_Z && pokemon == 802 && Type.Id == Ms.Spectral_Thief)
                return RomData.Moves.ElementAtOrDefault(Ms.Stars_Strike-1);

            if (Zstone(item) != Type.Type && (Type.Id != Ms.HIDDEN_POWER || item != Is.Normalium_Z))
                return null;
            if (Type.Category == MoveCategory.Status && Type.Id != Ms.HIDDEN_POWER)
                return Type;
            return CommonZmove(Type);

        }
        public static MoveType CommonZmove(MoveType Type)
        {
            var ty = Type.Type;
            if (Type.Id == Ms.HIDDEN_POWER)
                ty = BattleType.Normal;
            int x = (int)ty;
            if (Type.Category == MoveCategory.Special)
                x += 18;
            return new MoveType(672 + x, ty, Type.Category, CommonZmovePower(Type), 0, 1, MoveRange.SelectedOpponent);
        }
        public static int CommonZmovePower(MoveType Type)
        {
            int power=1;
            int op = Type.Power;
            if (op < 60)
                power = 100;
            else if (op < 70)
                power = 120;
            else if (op < 80)
                power = 140;
            else if (op < 90)
                power = 160;
            else if (op < 100)
                power = 175;
            else if (op < 110)
                power = 180;
            else if (op < 120)
                power = 185;
            else if (op < 130)
                power = 190;
            else if (op < 140)
                power = 195;
            else
                power = 200;

            if (Type.Id == Ms.VCREATE)
                power = 220;
            else if (Type.Id == Ms.STRUGGLE)
                power = 1;
            return power;
        }
        public static BattleType Zstone(int item)
        {
            switch (item)
            {
                case Is.Normalium_Z:
                    return BattleType.Normal;
                case Is.Fightinium_Z:
                    return BattleType.Fighting;
                case Is.Flyinium_Z:
                    return BattleType.Flying;
                case Is.Poisonium_Z:
                    return BattleType.Poison;
                case Is.Groundium_Z:
                    return BattleType.Ground;
                case Is.Rockium_Z:
                    return BattleType.Rock;
                case Is.Buginium_Z:
                    return BattleType.Bug;
                case Is.Ghostium_Z:
                    return BattleType.Ghost;
                case Is.Steelium_Z:
                    return BattleType.Steel;
                case Is.Firium_Z:
                    return BattleType.Fire;
                case Is.Waterium_Z:
                    return BattleType.Water;
                case Is.Grassium_Z:
                    return BattleType.Grass;
                case Is.Electrium_Z:
                    return BattleType.Electric;
                case Is.Psychium_Z:
                    return BattleType.Psychic;
                case Is.Icium_Z:
                    return BattleType.Ice;
                case Is.Dragonium_Z:
                    return BattleType.Dragon;
                case Is.Darkinium_Z:
                    return BattleType.Dark;
                case Is.Fairium_Z:
                    return BattleType.Fairy;
                default:
                    return BattleType.Invalid;
            }
        }

        public static readonly int[] attack1 = new int[] { 339, 468, 336, 652, 43, 96, 316, 379, 563, 103, 159, 39, 269, 576, 261, 526 };
        public static readonly int[] attack2 = new int[] { 119 };
        public static readonly int[] attack3 = new int[] { 150 };
        public static readonly int[] def1 = new int[] { 392, 608, 651, 335, 204, 455, 587, 297, 579, 580, 45, 106, 561, 568, 220, 589, 139, 77, 501, 115, 46, 169, 191, 596, 446, 628, 644, 321, 259, 92, 390, 599, 469, 110 };
        public static readonly int[] spatk1 = new int[] { 109,582,373,313,663,356,74,627,569,319,170,357,171,654,513,493,487,186,298,477 };
        public static readonly int[] spatk2 = new int[] { 377,375 };
        public static readonly int[] spdef1 = new int[] { 268, 590, 322, 578, 598, 494, 260, 137, 275, 113, 478, 602, 212, 581, 300, 656, 78, 86, 346, 18, 273, 472 };
        public static readonly int[] spdef2 = new int[] { 597, 445, 286, 277, 600 };
        public static readonly int[] speed1 = new int[] { 495, 637, 604, 227, 380, 320, 470, 385, 258, 95, 199, 142, 471, 384, 511, 240, 272, 219, 201, 184, 47, 285, 79, 665, 564, 81, 241, 48, 641, 388, 281 };
        public static readonly int[] speed2 = new int[] { 502, 516, 382, 278, 289, 415, 271 };
        public static readonly int[] acc1 = new int[] { 383, 111, 432, 116, 102, 230, 433 };
        public static readonly int[] eva1 = new int[] { 293, 197, 148, 134, 381, 393, 28, 108 };
        public static readonly int[] all1 = new int[] { 606, 160, 571, 601, 603, 607, 625, 166, 567 };
        public static readonly int[] CT = new int[] { 367, 193, 391, 214, 366 };
        public static readonly int[] reset = new int[] { 151, 97, 133, 213, 475, 112, 226, 347, 489, 538, 178, 464, 50, 104, 349, 203, 659, 266, 456, 505, 270, 334, 588, 73, 208, 107, 236, 234, 417, 195, 182, 483, 476, 105, 156, 397, 355, 504, 508, 667, 303, 135, 147, 164, 207, 256, 14, 235, 294 };
        public static readonly int[] Hp = new int[] { 312, 187, 176, 114, 215, 54, 244, 287, 180, 254, 100, 144 };
        public static readonly int[] Wish = new int[] { 262,575 };
        public static readonly int[] FollowMe = new int[] { 194,288 };

        public static int ZStatus(int move)
        {
            if (attack1.Contains(move))
                return 1;
            if (attack2.Contains(move))
                return 2;
            if (attack3.Contains(move))
                return 3;
            if (def1.Contains(move))
                return 4;
            if (spatk1.Contains(move))
                return 5;
            if (spatk2.Contains(move))
                return 6;
            if (spdef1.Contains(move))
                return 7;
            if (spdef2.Contains(move))
                return 8;
            if (speed1.Contains(move))
                return 9;
            if (speed2.Contains(move))
                return 10;
            if (acc1.Contains(move))
                return 11;
            if (eva1.Contains(move))
                return 12;
            if (all1.Contains(move))
                return 13;
            if (CT.Contains(move))
                return 14;
            if (reset.Contains(move))
                return 15;
            if (Hp.Contains(move))
                return 16;
            if (Wish.Contains(move))
                return 17;
            if (FollowMe.Contains(move))
                return 18;
            return 0;
        }

        public static bool CantLoseAbility(int ability) //未完
        {
            return ability == As.STANCE_CHANGE || ability == As.MULTITYPE || ability==As.RKS_System || ability==As.Comatose;
        }

        private static readonly int[] alola= {19,20,26,27,28,37,38,50,51,52,53,74,75,76,88,89,103,105};
        public static bool Alola(int num)
        {
            return alola.Contains(num);
        }

        public static bool IsZmove(int move)
        {
            return move >= 673 && move <= 719;
        }
    }
}
