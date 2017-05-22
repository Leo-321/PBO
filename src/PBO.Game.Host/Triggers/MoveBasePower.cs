using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class MoveBasePower
    {
        public static void Execute(DefContext def)
        {
            var der = def.Defender;
            var atk = def.AtkContext;
            var aer = atk.Attacker;
            var move = atk.Move;

            switch (move.Id)
            {
                case Ms.GUST: //16
                case Ms.TWISTER: //239
                    def.BasePower = def.Defender.CoordY == CoordY.Air ? 80 : 40;
                    break;
                case Ms.TRIPLE_KICK: //167
                    def.BasePower = 10 * def.AtkContext.Hit;
                    break;
                case Ms.ROLLOUT: //205
                case Ms.ICE_BALL: //301
                    def.BasePower = 30 * (1 << ((aer.OnboardPokemon.HasCondition(Cs.DefenseCurl) ? 6 : 5) - atk.GetCondition(Cs.MultiTurn).Turn));
                    break;
                case Ms.PRESENT: //217
                    def.BasePower = atk.GetCondition<int>(Cs.Present);
                    break;
                case Ms.PURSUIT: //228
                    def.BasePower = atk.IgnoreSwitchItem ? 80 : 40;
                    break;
                case Ms.HIDDEN_POWER: //237
                    HiddenPower(def);
                    break;
                case Ms.BEAT_UP: //251:
                    def.BasePower = def.AtkContext.GetCondition<int>(Cs.BeatUp) / 10 + 5;
                    break;
                case Ms.SPIT_UP: //255
                    def.BasePower = 100 * aer.OnboardPokemon.GetCondition<int>(Cs.Stockpile);
                    break;
                case Ms.REVENGE: //279
                case Ms.AVALANCHE:
                    Revenge(def);
                    break;
                case Ms.Stomping_Tantrum:
                    if (atk.Attacker.LastMiss) def.BasePower = 150;
                    else def.BasePower = 75;
                    break;
                case Ms.ERUPTION: //284
                case Ms.WATER_SPOUT: //323
                    def.BasePower = 150 * aer.Hp / aer.Pokemon.MaxHp;
                    break;
                case Ms.WEATHER_BALL: //311
                    def.BasePower = der.Controller.Weather == Weather.Normal ? 50 : 100;
                    break;
                case Ms.NATURAL_GIFT: //363
                    NaturalGift(def);
                    break;
                case Ms.PAYBACK: //371
                    def.BasePower = der.LastMoveTurn == der.Controller.TurnNumber ? 100 : 50;
                    break;
                case Ms.ASSURANCE: //372
                    def.BasePower = der.OnboardPokemon.HasCondition(Cs.Assurance) ? 100 : 50;
                    break;
                case Ms.FLING: //374
                    def.BasePower = FlingPower(aer.Pokemon.Item);
                    break;
                case Ms.TRUMP_CARD: //376
                    TrumpCard(def);
                    break;
                case Ms.WRING_OUT: //378
                case Ms.CRUSH_GRIP: //462
                    def.BasePower = 120 * der.Hp / der.Pokemon.MaxHp;
                    break;
                case Ms.PUNISHMENT: //386
                    OLevel(def, 60);
                    break;
                case Ms.STORED_POWER: //500
                case Ms.Power_Trip:
                    SLevel(def, 20);
                    break;
                case Ms.HEX: //506
                    def.BasePower = der.Pokemon.State == PokemonState.Normal ? 50 : 100;
                    break;
                case Ms.MAGNITUDE:
                    def.BasePower = 10 + 20 * atk.GetCondition<int>(Cs.Magnitude);
                    break;
                case Ms.HEAVY_SLAM:
                case Ms.HEAT_CRASH:
                    HeavySlam(def);
                    break;
                case Ms.LOW_KICK:
                case Ms.GRASS_KNOT:
                    GrassKnot(def);
                    break;
                case Ms.FURY_CUTTER:
                    {
                        var c = atk.Attacker.OnboardPokemon.GetCondition(Cs.LastMove);
                        if (c != null && c.Move == move) def.BasePower = 20 * (1 << (c.Int > 3 ? 3 : c.Int));
                        else def.BasePower = 20;
                    }
                    break;
                case Ms.FLAIL:
                case Ms.REVERSAL:
                    Flail(def);
                    break;
                case Ms.ECHOED_VOICE:
                    EchoedVoice(def);
                    break;
                case Ms.ELECTRO_BALL:
                    ElectroBall(def);
                    break;
                case Ms.GYRO_BALL:
                    GyroBall(def);
                    break;
                case Ms.ACROBATICS:
                    def.BasePower = aer.Pokemon.Item == 0 ? 110 : 55;
                    break;
                case Ms.RETURN:
                    def.BasePower = aer.Pokemon.Happiness * 4 / 10;
                    break;
                case Ms.FRUSTRATION:
                    def.BasePower = (255 - aer.Pokemon.Happiness) * 4 / 10;
                    break;
                case Ms.SMELLING_SALTS: //265
                    DeAbnormalState(def, PokemonState.PAR);
                    break;
                case Ms.WAKEUP_SLAP: //358
                    DeAbnormalState(def, PokemonState.SLP);
                    break;
                case Ms.ROUND: //496
                    def.BasePower = aer.Field.AddTurnCondition(Cs.Round) ? 60 : 120;
                    foreach (var pm in aer.Controller.ActingPokemons)
                        if (pm.SelectedMove != null && pm.SelectedMove.MoveE == move && pm.Pokemon.TeamId == aer.Pokemon.TeamId && pm.CanMove)
                        {
                            aer.Controller.Board.SetTurnCondition(Cs.NextActingPokemon, pm);
                            break;
                        }
                    break;
                case Ms.WATER_PLEDGE:
                case Ms.FIRE_PLEDGE:
                case Ms.GRASS_PLEDGE:
                    var pledge = aer.Field.GetCondition<int>(Cs.Plege);
                    def.BasePower = pledge != 0 && pledge != move.Id ? 150 : move.Move.Power;
                    break;
                default:
                    def.BasePower = move.Move.Power;
                    break;
            }
            if (move.CommonZmove)
                def.BasePower = GameHelper.CommonZmovePower(def.AtkContext.BaseOfZmove);
            if (def.BasePower == 0) def.BasePower = 1;
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
        private static void NaturalGift(DefContext def)
        {
            switch (def.AtkContext.Attacker.Pokemon.Item)
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
                    def.BasePower = 60;
                    break;
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
                    def.BasePower = 70;
                    break;
                case Is.WATMEL_BERRY:
                case Is.DURIN_BERRY:
                case Is.BELUE_BERRY:
                    def.BasePower = 80;
                    break;
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
                    def.BasePower = 60;
                    break;
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
                    def.BasePower = 80;
                    break;
            }
        }

        private static void TrumpCard(DefContext def)
        {
            if (def.AtkContext.MoveProxy == null || def.AtkContext.MoveProxy.MoveE.Id != Ms.TRUMP_CARD) def.BasePower = 40;
            else
            {
                int pwa = def.AtkContext.MoveProxy.PP;
                if (pwa >= 5) def.BasePower = 40;
                else if (pwa == 4) def.BasePower = 50;
                else if (pwa == 3) def.BasePower = 60;
                else if (pwa == 2) def.BasePower = 80;
                else def.BasePower = 200;
            }
        }

        private static int Positive(int x)
        {
            return x > 0 ? x : 0;
        }
        private static void SLevel(DefContext def, int @const)
        {
            var ao = def.AtkContext.Attacker.OnboardPokemon;
            var l5 = ao.Lv5D;
            int sst = Positive(l5.Atk);
            sst += Positive(l5.Def);
            sst += Positive(l5.SpAtk);
            sst += Positive(l5.SpDef);
            sst += Positive(l5.Speed);
            sst += Positive(ao.AccuracyLv);
            sst += Positive(ao.EvasionLv);
            def.BasePower = sst * 20 + @const;
        }
        private static void OLevel(DefContext def, int @const)
        {
            var ao = def.Defender.OnboardPokemon;
            var l5 = ao.Lv5D;
            int sst = Positive(l5.Atk);
            sst += Positive(l5.Def);
            sst += Positive(l5.SpAtk);
            sst += Positive(l5.SpDef);
            sst += Positive(l5.Speed);
            sst += Positive(ao.AccuracyLv);
            sst += Positive(ao.EvasionLv);
            def.BasePower = sst * 20 + @const;
        }

        private static void HeavySlam(DefContext def)
        {
            double w = def.AtkContext.Attacker.Weight / def.Defender.Weight;
            if (w >= 5) def.BasePower = 120;
            else if (w >= 4) def.BasePower = 100;
            else if (w >= 3) def.BasePower = 80;
            else if (w >= 2) def.BasePower = 60;
            else def.BasePower = 40;
        }

        private static void GrassKnot(DefContext def)
        {
            double w = def.Defender.Weight;
            if (w >= 200) def.BasePower = 120;
            else if (w >= 100) def.BasePower = 100;
            else if (w >= 50) def.BasePower = 80;
            else if (w >= 25) def.BasePower = 60;
            else if (w >= 10) def.BasePower = 40;
            else def.BasePower = 20;
        }

        private static void Flail(DefContext def)
        {
            int pwd = def.AtkContext.Attacker.Hp * 48 / def.AtkContext.Attacker.Pokemon.MaxHp;

            if (pwd == 1) def.BasePower = 200;
            else if (pwd <= 4) def.BasePower = 150;
            else if (pwd <= 9) def.BasePower = 100;
            else if (pwd <= 16) def.BasePower = 80;
            else if (pwd <= 32) def.BasePower = 40;
            else def.BasePower = 20;
        }

        private static void EchoedVoice(DefContext def)
        {
            var c = def.AtkContext.Attacker.OnboardPokemon.GetCondition(Cs.LastMove);
            if (c != null && c.Move == def.AtkContext.Move)
            {
                def.BasePower = 40 * (c.Int + 1);
                if (def.BasePower > 200) def.BasePower = 200;
            }
            else def.BasePower = 40;
        }

        private static void ElectroBall(DefContext def)
        {
            int pwb = (int)(def.AtkContext.Attacker.Speed / def.Defender.Speed);
            if (pwb >= 4) def.BasePower = 150;
            else if (pwb >= 3) def.BasePower = 120;
            else if (pwb >= 2) def.BasePower = 80;
            else def.BasePower = 60;
        }

        private static void GyroBall(DefContext def)
        {
            int pwb = (int)(25 * def.Defender.Speed / def.AtkContext.Attacker.Speed);
            if (pwb > 150) def.BasePower = 150;
            else if (pwb < 1) def.BasePower = 1;
            else def.BasePower = pwb;
        }

        private static void DeAbnormalState(DefContext def, PokemonState state)
        {
            def.BasePower = def.Defender.State == state ? 120 : 60;
        }

        private static void HiddenPower(DefContext def)
        {
            var iv = def.AtkContext.Attacker.Pokemon.Iv;
            int pI = (iv.Hp & 2) >> 1;
            pI |= (iv.Atk & 2);
            pI |= (iv.Def & 2) << 1;
            pI |= (iv.Speed & 2) << 2;
            pI |= (iv.SpAtk & 2) << 3;
            pI |= (iv.SpDef & 2) << 4;
            def.BasePower = (int)(pI * 40 / 63 + 30);
        }

        private static void Revenge(DefContext def)
        {
            var o = def.AtkContext.Attacker.OnboardPokemon.GetCondition(Cs.Damage);
            if (o != null && o.By == def.Defender) def.BasePower = 120;
            else def.BasePower = 60;
        }
    }
}
