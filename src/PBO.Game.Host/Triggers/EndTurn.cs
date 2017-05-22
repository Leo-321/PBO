using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class EndTurn
    {
        public static void Execute(Controller c)
        {
            WeatherEffect(c); if (!c.CanContinue) return;
            FSDD(c); if (!c.CanContinue) return;
            Wish(c); if (!c.CanContinue) return;
            PropertyChange(c); if (!c.CanContinue) return;
            HpRecover(c); if (!c.CanContinue) return;
            PmState(c); if (!c.CanContinue) return;
            Curse(c); if (!c.CanContinue) return;
            Trap(c); if (!c.CanContinue) return;
            PokemonCondition(c); if (!c.CanContinue) return;
            FieldCondition(c); if (!c.CanContinue) return;
            BoardCondition(c); if (!c.CanContinue) return;
            Pokemon(c); if (!c.CanContinue) return;
            ZenMode(c);
        }
        //1.0 weather ends, Sandstorm/Hail damage, Rain Dish/Dry Skin/Ice Body/SolarPower
        private static void WeatherEffect(Controller c)
        {
            var spweather = c.Board.GetCondition<int>(Cs.SpWeather);
            if (c.Board.GetCondition<int>(Cs.Weather) == c.TurnNumber && spweather != As.DESOLATE_LAND && spweather != As.PRIMORDIAL_SEA)
            {
                var w = c.Board.Weather;
                c.ReportBuilder.ShowLog(w == Weather.IntenseSunlight ? Ls.DeIntenseSunlight : w == Weather.Rain ? Ls.DeRain : w == Weather.Hailstorm ? Ls.DeHailstorm : Ls.DeSandstorm);
                c.Weather = Weather.Normal;
                c.Board.RemoveCondition(Cs.Weather);
            }
            else
            {
                if (c.Board.Weather == Weather.Sandstorm) c.ReportBuilder.ShowLog("Sandstorm");
                else if (c.Board.Weather == Weather.Hailstorm) c.ReportBuilder.ShowLog("Hailstorm");
                else if (c.Board.Weather == Weather.IntenseSunlight) c.ReportBuilder.ShowLog("IntenseSunlight");
                else if (c.Board.Weather == Weather.Rain) c.ReportBuilder.ShowLog("Rain");
                switch (c.Weather)
                {
                    case Weather.Sandstorm:
                        foreach (var pm in c.OnboardPokemons.ToArray())
                        {
                            var types = pm.OnboardPokemon.Types;
                            if (types.Contains(BattleType.Rock) || types.Contains(BattleType.Steel) || types.Contains(BattleType.Ground) || pm.ItemE(Is.SAFETY_GOGGLES)) continue;
                            int ab = pm.Ability;
                            if (ab == As.OVERCOAT || ab == As.SAND_VEIL || ab == As.SAND_RUSH || ab == As.SAND_FORCE) continue;
                            if (pm.EffectHurtByOneNth(16, Ls.SandstormHurt)) pm.CheckFaint();
                        }
                        break;
                    case Weather.Hailstorm:
                        foreach (var pm in c.OnboardPokemons.ToArray())
                            if (pm.AbilityE(As.ICE_BODY))
                            {
                                if (pm.CanHpRecover())
                                {
                                    pm.RaiseAbility();
                                    pm.HpRecoverByOneNth(16);
                                }
                            }
                            else if (
                              !(pm.AbilityE(As.OVERCOAT) || pm.AbilityE(As.SNOW_CLOAK) || pm.AbilityE(As.Slush_Rush) || pm.OnboardPokemon.HasType(BattleType.Ice))
                               && pm.EffectHurtByOneNth(16, Ls.HailstormHurt))
                                pm.CheckFaint();
                        break;
                    case Weather.Rain:
                        foreach (var pm in c.OnboardPokemons)
                            if (pm.CanHpRecover())
                                if (pm.RaiseAbility(As.RAIN_DISH)) pm.HpRecoverByOneNth(16);
                                else if (pm.RaiseAbility(As.DRY_SKIN)) pm.HpRecoverByOneNth(8);
                        break;
                    case Weather.IntenseSunlight:
                        foreach (var pm in c.OnboardPokemons.ToArray())
                            if ((pm.RaiseAbility(As.SOLAR_POWER) || pm.RaiseAbility(As.DRY_SKIN)) && pm.EffectHurtByOneNth(8)) pm.CheckFaint();
                        break;
                }
            }
        }
        //3.0 Future Sight, Doom Desire
        private static void FSDD(Controller c)
        {
            foreach (var t in c.Board.Tiles)
            {
                var o = t.GetCondition(Cs.FSDD);
                if (o != null && o.Turn == c.TurnNumber)
                {
                    t.RemoveCondition(Cs.FSDD);
                    if (t.Pokemon != null) o.Atk.StartExecute(o.Atk.GetCondition<MoveTypeE>(Cs.FSDD), t, "FSDD");
                }
            }
        }
        //4.0 Wish
        private static void Wish(Controller c)
        {
            foreach (var t in c.Tiles)
            {
                var o = t.GetCondition(Cs.Wish);
                if (o != null && o.Turn == c.TurnNumber)
                {
                    t.RemoveCondition(Cs.Wish);
                    if (t.Pokemon != null) t.Pokemon.HpRecover(o.Int, false, Ls.Wish);
                }
            }
        }
        //5.0 Fire Pledge + Grass Pledge damage
        //5.1 Shed Skin, Hydration, Healer
        //5.2 Leftovers, Black Sludge
        private static void PropertyChange(Controller c)
        {
            if (c.Board.HasCondition(Cs.GrassyTerrain)) //顺序未测
            {
                foreach (var pm in c.OnboardPokemons)
                    if (HasEffect.IsGroundAffectable(pm, true, false)) pm.HpRecoverByOneNth(16);
            }
            bool? hydration = null;
            foreach (var pm in c.OnboardPokemons.ToArray())
            {
                if (pm.Field.HasCondition(Cs.FireSea) && !pm.OnboardPokemon.HasType(BattleType.Fire)) pm.EffectHurtByOneNth(8, Ls.FireSea);
                switch (pm.Ability)
                {
                    case As.SHED_SKIN:
                        if (pm.State != PokemonState.Normal && c.RandomHappen(30))
                        {
                            pm.RaiseAbility();
                            pm.DeAbnormalState();
                        }
                        break;
                    case As.HYDRATION:
                        if (pm.State != PokemonState.Normal)
                        {
                            if (hydration == null) hydration = c.Weather == Weather.Rain;
                            if (hydration == true)
                            {
                                pm.RaiseAbility();
                                pm.DeAbnormalState();
                            }
                        }
                        break;
                    case As.HEALER:
                        var ps = new List<PokemonProxy>();
                        foreach (var p in pm.Field.Pokemons)
                            if (p != pm && p.State != PokemonState.Normal) ps.Add(p);
                        if (ps.Count != 0 && c.RandomHappen(30))
                        {
                            pm.RaiseAbility();
                            ps[c.GetRandomInt(0, ps.Count - 1)].DeAbnormalState();
                        }
                        break;
                }
                switch (pm.Item)
                {
                    case Is.LEFTOVERS:
                        pm.HpRecoverByOneNth(16, false, Ls.ItemHpRecover2, Is.LEFTOVERS);
                        break;
                    case Is.BLACK_SLUDGE:
                        if (pm.OnboardPokemon.HasType(BattleType.Poison))
                        {
                            if (pm.CanHpRecover()) pm.HpRecoverByOneNth(16, false, Ls.ItemHpRecover2, Is.BLACK_SLUDGE);
                        }
                        else pm.EffectHurtByOneNth(8, Ls.ItemHurt, Is.BLACK_SLUDGE);
                        break;
                }
                pm.CheckFaint();
            }
        }
        //6.0 Aqua Ring
        //7.0 Ingrain
        //8.0 Leech Seed
        private static void HpRecover(Controller c)
        {
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.HasCondition(Cs.AquaRing))
                {
                    int hp = pm.Pokemon.MaxHp;
                    if (pm.ItemE(Is.BIG_ROOT)) hp = (int)(hp * 1.3);
                    hp /= 16;
                    pm.HpRecover(hp, false, Ls.AquaRing);
                }
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.HasCondition(Cs.Ingrain))
                {
                    int hp = pm.Pokemon.MaxHp;
                    if (pm.ItemE(Is.BIG_ROOT)) hp = (int)(hp * 1.3);
                    hp /= 16;
                    pm.HpRecover(hp, false, Ls.Ingrain);
                }
            foreach (var pm in c.OnboardPokemons.ToArray())
            {
                var tile = pm.OnboardPokemon.GetCondition<Tile>(Cs.LeechSeed);
                if (tile != null && tile.Pokemon != null)
                {
                    var hp = pm.Hp;
                    if (pm.EffectHurtByOneNth(8, Ls.LeechSeed))
                    {
                        hp -= pm.Hp;
                        var recover = tile.Pokemon;
                        if (hp > 0 && recover.CanHpRecover())
                        {
                            if (recover.ItemE(Is.BIG_ROOT)) hp = (int)(hp * 1.3);
                            if (!recover.AbilityE(As.MAGIC_GUARD) && pm.RaiseAbility(As.LIQUID_OOZE))
                            {
                                recover.EffectHurt(hp);
                                recover.CheckFaint();
                            }
                            else recover.HpRecover(hp);
                        }
                    }
                }
                pm.CheckFaint();
            }
        }
        //9.0 (bad) poison damage, burn damage, Poison Heal
        //9.1 Nightmare
        private static void PmState(Controller c)
        {
            foreach (var pm in c.OnboardPokemons.ToArray())
            {
                switch (pm.State)
                {
                    case PokemonState.BadlyPSN:
                    case PokemonState.PSN:
                        var id = pm.Ability;
                        if (id == As.POISON_HEAL)
                        {
                            if (pm.CanHpRecover())
                            {
                                pm.RaiseAbility();
                                pm.HpRecoverByOneNth(8, false, Ls.PoisonHeal);
                            }
                        }
                        else if (pm.State == PokemonState.BadlyPSN)
                        {
                            int turn = 1 + c.TurnNumber - pm.OnboardPokemon.GetCondition<int>(Cs.BadlyPSN);
                            int hp = pm.Pokemon.MaxHp * (turn > 15 ? 15 : turn) / 16;
                            pm.EffectHurt(hp, Ls.PSN);
                        }
                        else pm.EffectHurtByOneNth(8, Ls.PSN);
                        break;
                    case PokemonState.BRN:
                        pm.EffectHurtByOneNth(16, Ls.BRN);
                        break;
                    case PokemonState.SLP:
                        if (pm.OnboardPokemon.HasCondition(Cs.Nightmare)) pm.EffectHurtByOneNth(4, Ls.Nightmare);
                        break;
                }
                pm.CheckFaint();
            }
        }
        //10.0 Curse (from a Ghost-type)
        private static void Curse(Controller c)
        {
            foreach (var pm in c.OnboardPokemons.ToArray())
                if (pm.OnboardPokemon.HasCondition(Cs.Curse) && pm.EffectHurtByOneNth(4, Ls.Curse)) pm.CheckFaint();
        }
        //11.0 Bind, Wrap, Fire Spin, Clamp, Whirlpool, Sand Tomb, Magma Storm
        private static void Trap(Controller c)
        {
            foreach (var pm in c.OnboardPokemons.ToArray())
            {
                var trap = pm.OnboardPokemon.GetCondition(Cs.Trap);
                if (trap != null)
                    if (trap.Turn == c.TurnNumber)
                    {
                        pm.OnboardPokemon.RemoveCondition(Cs.Trap);
                        pm.ShowLogPm("TrapFree", trap.Move.Id);
                    }
                    else if (pm.EffectHurtByOneNth(trap.Bool ? 6 : 8, Ls.TrapHurt, trap.Move.Id)) pm.CheckFaint();
            }
        }
        //12.0 Taunt ends
        //13.0 Encore ends
        //14.0 Disable ends, Cursed Body ends
        //15.0 Magnet Rise ends
        //16.0 Telekinesis ends
        //17.0 Heal Block ends
        //18.0 Embargo ends
        //19.0 Yawn
        //20.0 Perish Song
        private static void PokemonCondition(Controller c)
        {
            foreach (var pm in c.OnboardPokemons)
            {
                int turn = pm.OnboardPokemon.GetCondition(Cs.Taunt, -1);
                if (turn == 0)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.Taunt);
                    pm.ShowLogPm("DeTaunt");
                }
            }
            foreach (var pm in c.OnboardPokemons)
            {
                var o = pm.OnboardPokemon.GetCondition(Cs.Encore);
                if (o != null && o.Turn == 0)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.Encore);
                    pm.ShowLogPm("DeEncore");
                }
            }
            foreach (var pm in c.OnboardPokemons)
            {
                var o = pm.OnboardPokemon.GetCondition(Cs.Disable);
                if (o != null && o.Turn == c.TurnNumber)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.Disable);
                    pm.ShowLogPm("DeDisable");
                }
            }
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.GetCondition<int>(Cs.MagnetRise) == c.TurnNumber)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.MagnetRise);
                    pm.ShowLogPm("DeMagnetRise");
                }
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.GetCondition<int>(Cs.Telekinesis) == c.TurnNumber)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.Telekinesis);
                    pm.ShowLogPm("DeTelekinesis");
                }
            foreach (var pm in c.OnboardPokemons)
            {
                var o = pm.OnboardPokemon.GetCondition(Cs.HealBlock);
                if (o != null && o.Turn == c.TurnNumber)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.HealBlock);
                    pm.ShowLogPm("DeHealBlock");
                }
            }
            foreach (var pm in c.OnboardPokemons)
            {
                var o = pm.OnboardPokemon.GetCondition(Cs.SoundBlock);
                if(o!=null && o.Turn == c.TurnNumber)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.SoundBlock);
                    pm.ShowLogPm("DeSoundBlock");
                }
            }
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.GetCondition<int>(Cs.Embargo) == c.TurnNumber)
                {
                    pm.OnboardPokemon.RemoveCondition(Cs.Embargo);
                    pm.ShowLogPm("DeEmbargo");
                    ITs.Attach(pm);
                }
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.GetCondition<int>(Cs.Charge) == c.TurnNumber)
                    pm.OnboardPokemon.RemoveCondition(Cs.Charge);
            foreach (var pm in c.OnboardPokemons)
                if (pm.OnboardPokemon.GetCondition<int>(Cs.Laser_Focus) == c.TurnNumber)
                    pm.OnboardPokemon.RemoveCondition(Cs.Laser_Focus);
            foreach (var pm in c.OnboardPokemons)
            {
                var o = pm.OnboardPokemon.GetCondition(Cs.Yawn);
                if (o != null && o.Turn == c.TurnNumber)
                {
                    pm.AddState(o.By, AttachedState.SLP, false);
                    pm.OnboardPokemon.RemoveCondition(Cs.Yawn);
                }
            }
            foreach (var pm in c.OnboardPokemons.ToArray())
            {
                int turn = pm.OnboardPokemon.GetCondition<int>(Cs.PerishSong, -1);
                if (turn != -1)
                {
                    pm.ShowLogPm("PerishSong", turn);
                    if (turn == 0) pm.Faint();
                    else pm.OnboardPokemon.SetCondition(Cs.PerishSong, turn - 1);
                }
            }
        }
        //21.0 Reflect ends
        //21.1 Light Screen ends
        //21.2 Safeguard ends
        //21.3 Mist ends
        //21.4 Tailwind ends
        //21.5 Lucky Chant ends
        //21.6 Water Pledge + Fire Pledge ends, Fire Pledge + Grass Pledge ends, Grass Pledge + Water Pledge ends
        private static void FieldCondition(Controller c)
        {
            foreach (var f in c.Board.Fields)
            {
                FieldCondition(Cs.Reflect, f, c);
                FieldCondition(Cs.LightScreen, f, c);
                FieldCondition(Cs.Aurora_Veil, f, c);
                FieldCondition(Cs.Safeguard, f, c);
                FieldCondition(Cs.Mist, f, c);
                FieldCondition(Cs.Tailwind, f, c);
                FieldCondition(Cs.LuckyChant, f, c);
                FieldCondition(Cs.Rainbow, f, c);
                FieldCondition(Cs.FireSea, f, c);
                FieldCondition(Cs.Swamp, f, c);
                f.RemoveCondition(Cs.MatBlock);
            }
        }
        private static void FieldCondition(Cs condition, Field f, Controller c)
        {
            if (f.GetCondition<int>(condition) == c.TurnNumber)
            {
                f.RemoveCondition(condition);
                c.ReportBuilder.ShowLog("De" + condition, f.Team);
            }
        }
        //22.0 Gravity ends
        //23.0 Trick Room ends
        //24.0 Wonder Room ends
        //25.0 Magic Room ends
        private static void BoardCondition(Controller c)
        {
            var board = c.Board;
            int turn = c.TurnNumber;
            if (board.GetCondition<int>(Cs.Gravity) == turn)
            {
                board.RemoveCondition(Cs.Gravity);
                c.ReportBuilder.ShowLog("DeGravity");
            }
            if (board.GetCondition<int>(Cs.TrickRoom) == turn)
            {
                board.RemoveCondition(Cs.TrickRoom);
                c.ReportBuilder.ShowLog("DeTrickRoom");
            }
            if (board.GetCondition<int>(Cs.WonderRoom) == turn)
            {
                foreach (var pm in c.OnboardPokemons)
                {
                    var stats = pm.OnboardPokemon.FiveD;
                    var d = stats.Def;
                    stats.Def = stats.SpDef;
                    stats.SpDef = d;
                }
                board.RemoveCondition(Cs.WonderRoom);
                c.ReportBuilder.ShowLog("DeWonderRoom");
            }
            if (board.GetCondition<int>(Cs.MagicRoom) == turn)
            {
                board.RemoveCondition(Cs.MagicRoom);
                c.ReportBuilder.ShowLog("DeMagicRoom");
                foreach (var pm in c.OnboardPokemons) ITs.Attach(pm);
            }
            if (board.GetCondition<int>(Cs.WaterSport) == turn)
            {
                board.RemoveCondition(Cs.WaterSport);
                c.ReportBuilder.ShowLog("DeWaterSport");
            }
            if (board.GetCondition<int>(Cs.MudSport) == turn)
            {
                board.RemoveCondition(Cs.MudSport);
                c.ReportBuilder.ShowLog("DeMudSport");
            }
            var t = board.GetCondition<int>(Cs.GrassyTerrain);
            if (t == turn)
            {
                board.RemoveCondition(Cs.GrassyTerrain);
                c.ReportBuilder.ShowLog("DeGrassyTerrain");
            }
            else if (t == 0)
            {
                t = board.GetCondition<int>(Cs.ElectricTerrain);
                if (t == turn)
                {
                    board.RemoveCondition(Cs.ElectricTerrain);
                    c.ReportBuilder.ShowLog("DeElectricTerrain");
                }
                else if (t == 0 && board.GetCondition<int>(Cs.MistyTerrain) == turn)
                {
                    board.RemoveCondition(Cs.MistyTerrain);
                    c.ReportBuilder.ShowLog("DeMistyTerrain");
                }
                else if (t == 0 && board.GetCondition<int>(Cs.PsychicTerrain) == turn)
                {
                    board.RemoveCondition(Cs.PsychicTerrain);
                    c.ReportBuilder.ShowLog("DePsychicTerrain");
                }
            }
        }
        //26.0 Uproar message
        //26.1 Speed Boost, Bad Dreams, Harvest, Moody
        //26.2 Toxic Orb activation, Flame Orb activation, Sticky Barb
        //26.3 pickup
        private static void Pokemon(Controller c)
        {
            foreach (var pm in c.OnboardPokemons.ToArray())
            {
                int ab = pm.Ability;
                if (pm.AtkContext != null && pm.AtkContext.Move.Id == Ms.UPROAR)
                {
                    if (pm.Action == PokemonAction.Moving) pm.ShowLogPm("Uproar");
                    else if (pm.AtkContext.GetCondition(Cs.MultiTurn).Turn == 0)
                    {
                        pm.AtkContext.RemoveCondition(Cs.MultiTurn);
                        pm.ShowLogPm("DeUproar");
                    }
                }
                switch (ab)
                {
                    case As.SPEED_BOOST:
                        if (pm.LastMoveTurn != 0) pm.ChangeLv7D(pm, StatType.Speed, 1, false, true);
                        break;
                    case As.BAD_DREAMS:
                        {
                            bool first = true;
                            foreach (var target in c.Board[1 - pm.Pokemon.TeamId].Pokemons)
                                if (target.State == PokemonState.SLP)
                                {
                                    if (first)
                                    {
                                        pm.RaiseAbility();
                                        first = false;
                                    }
                                    target.EffectHurtByOneNth(8, Ls.BadDreams);
                                }
                        }
                        break;
                    case As.HARVEST:
                        if (pm.Pokemon.Item == 0 && pm.Pokemon.UsedBerry != 0 && (c.Weather == Game.Weather.IntenseSunlight || c.RandomHappen(50)))
                        {
                            pm.RaiseAbility();
                            pm.SetItem(pm.Pokemon.UsedBerry);
                            pm.ShowLogPm("Harvest", pm.Pokemon.UsedBerry);
                            ITs.Attach(pm);
                        }
                        break;
                    case As.MOODY:
                        {
                            var up = new List<StatType>(7);
                            var down = new List<StatType>(7);
                            foreach (var s in GameHelper.SEVEN_D)
                            {
                                if (pm.CanChangeLv7D(pm, s, 2, false) != 0) up.Add(s);
                                if (pm.CanChangeLv7D(pm, s, -1, false) != 0) down.Add(s);
                            }
                            if (up.Count != 0 && down.Count != 0)
                            {
                                pm.RaiseAbility();
                                if (up.Count != 0) pm.ChangeLv7D(pm, up[c.GetRandomInt(0, up.Count - 1)], 2, false);
                                if (down.Count != 0) pm.ChangeLv7D(pm, down[c.GetRandomInt(0, down.Count - 1)], -1, false);
                            }
                        }
                        break;
                }
                switch (pm.Item)
                {
                    case Is.TOXIC_ORB:
                        pm.AddState(pm, AttachedState.PSN, false, 15, "ItemEnBadlyPSN", Is.TOXIC_ORB);
                        break;
                    case Is.FLAME_ORB:
                        pm.AddState(pm, AttachedState.BRN, false, 0, "ItemEnBRN", Is.FLAME_ORB);
                        break;
                    case Is.STICKY_BARB:
                        pm.EffectHurtByOneNth(8, Ls.ItemHurt, Is.STICKY_BARB);
                        break;
                }
                if (ab == As.PICKUP && pm.Pokemon.Item == 0)
                {
                    var items = new List<int>();
                    var owners = new List<OnboardPokemon>();
                    foreach (var p in c.Board[1 - pm.Pokemon.TeamId].GetAdjacentPokemonsByOppositeX(pm.OnboardPokemon.X))
                    {
                        var i = p.OnboardPokemon.GetCondition<int>(Cs.UsedItem);
                        if (i != 0)
                        {
                            items.Add(i);
                            owners.Add(p.OnboardPokemon);
                        }
                    }
                    if (!items.Any())
                        foreach (var p in pm.Field.GetAdjacentPokemonsByX(pm.OnboardPokemon.X))
                            if (p != pm)
                            {
                                var i = p.OnboardPokemon.GetCondition<int>(Cs.UsedItem);
                                if (i != 0)
                                {
                                    items.Add(i);
                                    owners.Add(p.OnboardPokemon);
                                }
                            }
                    if (items.Any())
                    {
                        var i = c.GetRandomInt(0, items.Count - 1);
                        owners[i].RemoveCondition(Cs.UsedItem);
                        pm.RaiseAbility();
                        pm.SetItem(items[i]);
                        pm.ShowLogPm("Pickup", items[i]);
                        ITs.Attach(pm);
                    }
                }
                pm.CheckFaint();
            }
        }
        //27.0 Zen Mode
        private static void ZenMode(Controller c)
        {
            foreach (var pm in c.OnboardPokemons)
            {
                var form = pm.Hp << 1 <= pm.Pokemon.MaxHp ? 1 : 0;
                if (pm.CanChangeForm(555, form) && pm.RaiseAbility(As.ZEN_MODE)) pm.ChangeForm(form, true, form == 0 ? "DeZenMode" : "EnZenMode");
                if (pm.CanChangeForm(774, form) && pm.RaiseAbility(As.Shields_Down)) pm.ChangeForm(form, true, form == 0 ? "DeShields_Down" : "EnShields_Down");
                //z神
                if (form == 1 && pm.CanChangeForm(718, 4) && pm.RaiseAbility(As.Power_Construct))
                {
                    int maxhp = pm.Pokemon.GetMaxHp;
                    pm.ChangeForm(4, true, "EnPower_Construct");
                    pm.Pokemon.MaxHp = pm.Pokemon.GetMaxHp;
                    pm.HpRecover(pm.Pokemon.MaxHp - maxhp);
                }
                form = pm.Hp << 2 <= pm.Pokemon.MaxHp ? 0 : 1;
                if (pm.CanChangeForm(746, form) && pm.RaiseAbility(As.Schooling)) pm.ChangeForm(form, true, form == 0 ? "EnSchooling" : "DeSchooling");
            }
        }
    }
}
