using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokemonBattleOnline.Game
{
    internal static class Xxporter
    {
        public static void Import(string source, PokemonData[] target)
        {
            source = Regex.Replace(source, @"\r\n|\r", "\n") + "\n";
            if (Regex.IsMatch(source, @".+?（.+?） *Lv\.\d+")) ImportFromPBO(source, target);
            else if (Regex.IsMatch(source, @".+?(\-\w){0,1} (\(.+?\) ){0,1}(\([FM]\) ){0,1}@ .+?\n", RegexOptions.IgnoreCase))
            {
                if (source.Contains("Trait"))
                    ImportFromPO(source, target);
                else
                    ImportFromPS(source, target);
            }
            //Gliscor (M) @ Flying Gem
            //Trait: Sand Veil
            //EVs: 4 HP / 252 Atk / 252 Spd
            //Jolly Nature (+Spd, -SAtk)
            //- Guillotine
            //- Earthquake
            //- Acrobatics
            //- U-turn
        }

        private static PokemonData ImportFromPO(Match m)
        {
            // 1: Nickname
            // 2: Form
            // 3: Pokemon
            // 4: Gender
            // 5: Item
            // 6: Ability
            // 7: EVs
            // 8: Nature
            // 9~12: Moves

            var hasNickname = m.Groups[3].Value.Length > 0;
            var pname = m.Groups[3].Value;
            if (!hasNickname) pname = m.Groups[1].Value;
            var pm = new PokemonData(GameString.PokemonSpecies(pname).Number, 0);
            var Item = GameString.Item(m.Groups[5].Value);
            var Ability = GameString.Ability(m.Groups[6].Value);
            var Nature = GameString.Nature(m.Groups[8].Value);

            pm.Gender = GetGender(m.Groups[4].Value);
            if (hasNickname) pm.Name = m.Groups[1].Value;
            pm.Item = Item;
            if (Ability != 0) pm.Ability = Ability;
            if (Nature != null) pm.Nature = Nature.Value;
            pm.Ev.Hp = TryMatch(m.Groups[7].Value, @"(\d+) HP", 1, 0);
            pm.Ev.Atk = TryMatch(m.Groups[7].Value, @"(\d+) Atk", 1, 0);
            pm.Ev.Def = TryMatch(m.Groups[7].Value, @"(\d+) Def", 1, 0);
            pm.Ev.SpAtk = TryMatch(m.Groups[7].Value, @"(\d+) SAtk", 1, 0);
            pm.Ev.SpDef = TryMatch(m.Groups[7].Value, @"(\d+) SDef", 1, 0);
            pm.Ev.Speed = TryMatch(m.Groups[7].Value, @"(\d+) Spd", 1, 0);
            foreach (Match m2 in Regex.Matches(m.Groups[9].Value, @"\- (.+?)(?: \[(.+?)\])*\n"))
            {
                var Move = GameString.Move(m2.Groups[1].Value);
                if (Move != null) pm.AddMove(Move);
            }
            return pm;
        }

        private static PokemonData ImportFromPS(Match m)
        {
            // 1: Nickname
            // 2: Form
            // 3: Pokemon
            // 4: Gender
            // 5: Item
            // 6: Ability
            // 7: EVs
            // 8: Nature
            // 9: IVs(?)
            // 10~13: Moves

            var hasNickname = m.Groups[3].Value.Length > 0;
            var pname = m.Groups[3].Value;
            if (!hasNickname) pname = m.Groups[1].Value;
            int form = 0;
            if (m.Groups[2].Value.Contains("Alola") || m.Groups[2].Value.Contains("Therian") || m.Groups[2].Value.Contains("Attack") || m.Groups[2].Value.Contains("Heat") || m.Groups[2].Value.Contains("White") || m.Groups[2].Value.Contains("Unbound"))
                form = 1;
            if (m.Groups[2].Value.Contains("Attack") || m.Groups[2].Value.Contains("Wash") || m.Groups[2].Value.Contains("Black"))
                form = 2;
            if (m.Groups[2].Value.Contains("Defence") || m.Groups[2].Value.Contains("Frost"))
                form = 3;
            if (m.Groups[2].Value.Contains("Speed") || m.Groups[2].Value.Contains("Fan"))
                form = 4;
            if (m.Groups[2].Value.Contains("Mow"))
                form = 5;
            var pm = new PokemonData(GameString.PokemonSpecies(pname).Number, form);
            var Item = GameString.Item(m.Groups[5].Value);
            var Ability = GameString.Ability(m.Groups[6].Value);
            var Nature = GameString.Nature(m.Groups[8].Value);

            pm.Gender = GetGender(m.Groups[4].Value);
            if (hasNickname) pm.Name = m.Groups[1].Value;
            pm.Item = Item;
            if (Ability != 0) pm.Ability = Ability;
            if (Nature != null) pm.Nature = Nature.Value;
            pm.Ev.Hp = TryMatch(m.Groups[7].Value, @"(\d+) HP", 1, 0);
            pm.Ev.Atk = TryMatch(m.Groups[7].Value, @"(\d+) Atk", 1, 0);
            pm.Ev.Def = TryMatch(m.Groups[7].Value, @"(\d+) Def", 1, 0);
            pm.Ev.SpAtk = TryMatch(m.Groups[7].Value, @"(\d+) SpA", 1, 0);
            pm.Ev.SpDef = TryMatch(m.Groups[7].Value, @"(\d+) SpD", 1, 0);
            pm.Ev.Speed = TryMatch(m.Groups[7].Value, @"(\d+) Spe", 1, 0);
            if(m.Groups[9].Value.Contains("IVs"))
            {
                pm.Iv.Hp = TryMatch(m.Groups[9].Value, @"(\d+) HP", 1, 31);
                pm.Iv.Atk = TryMatch(m.Groups[9].Value, @"(\d+) Atk", 1, 31);
                pm.Iv.Def = TryMatch(m.Groups[9].Value, @"(\d+) Def", 1, 31);
                pm.Iv.SpAtk = TryMatch(m.Groups[9].Value, @"(\d+) SpA", 1, 31);
                pm.Iv.SpDef = TryMatch(m.Groups[9].Value, @"(\d+) SpD", 1, 31);
                pm.Iv.Speed = TryMatch(m.Groups[9].Value, @"(\d+) Spe", 1, 31);
            }
            foreach (Match m2 in Regex.Matches(m.Groups[m.Groups.Count-1].Value, @"\- (.+?)(?: \[(.+?)\])*( )*\n"))
            {
                var Move = GameString.Move(m2.Groups[1].Value);
                if (Move != null) pm.AddMove(Move);
            }
            return pm;
        }

        private static PokemonData ImportFromPBO(Match m)
        {
            // 1: Nickname
            // 2: Pokemon
            // 3: Level
            // 4: Gender
            // 5: Ability
            // 6: Nature
            // 7: IVs
            // 8: EVs
            // 9: Happiness
            // 10: Items
            // 11: Moves
            int i;
            var form = GameString.PokemonForm(m.Groups[2].Value);
            var pm = new PokemonData(form.Species.Number, form.Index);
            pm.Name = m.Groups[1].Value;
            if (int.TryParse(m.Groups[3].Value, out i)) pm.Lv = i;
            pm.Gender = GetGender(m.Groups[4].Value);
            var ab = GameString.Ability(m.Groups[5].Value);
            if (ab != 0) pm.Ability = ab;
            pm.Nature = GameString.Nature(m.Groups[6].Value) ?? PokemonNature.Hardy;
            if (m.Groups[9].Value.Length > 0 && int.TryParse(m.Groups[9].Value, out i)) pm.Happiness = i;
            pm.Item = GameString.Item(m.Groups[10].Value);

            if (!string.IsNullOrEmpty(m.Groups[7].Value))
            {
                var ivs = m.Groups[7].Value.Split('/');
                if (int.TryParse(ivs[0], out i)) pm.Iv.Hp = i;
                if (int.TryParse(ivs[1], out i)) pm.Iv.Atk = i;
                if (int.TryParse(ivs[2], out i)) pm.Iv.Def = i;
                if (int.TryParse(ivs[3], out i)) pm.Iv.SpAtk = i;
                if (int.TryParse(ivs[4], out i)) pm.Iv.SpDef = i;
                if (int.TryParse(ivs[5], out i)) pm.Iv.Speed = i;
            }

            var evs = m.Groups[8].Value.Split('/');
            if (int.TryParse(evs[0], out i)) pm.Ev.Hp = i;
            if (int.TryParse(evs[1], out i)) pm.Ev.Atk = i;
            if (int.TryParse(evs[2], out i)) pm.Ev.Def = i;
            if (int.TryParse(evs[3], out i)) pm.Ev.SpAtk = i;
            if (int.TryParse(evs[4], out i)) pm.Ev.SpDef = i;
            if (int.TryParse(evs[5], out i)) pm.Ev.Speed = i;

            foreach (var s in Regex.Replace(m.Groups[11].Value, @"\[.+?\]", "").Split('/'))
            {
                var move = GameString.Move(s);
                if (move != null) pm.AddMove(move);
            }

            return pm;
        }

        private static void ImportFromPO(string source, PokemonData[] target)
        {
            int i = 0;
            foreach (Match m in Regex.Matches(source, @"(.+?)(\-\w){0,1} (?:\((.{2,}?)(?:\-\w){0,1}\) ){0,1}(?:\(([FM])\) ){0,1}@ (.+?)\nTrait: (.+?)\nEVs: (.+?)\n(.+?) Nature.*\n((?:\- .+?\n)+)"))
            {
                try
                {
                    target[i++] = ImportFromPO(m);
                }
                catch { }
                if (i == target.Length) break;
            }
        }

        private static void ImportFromPS(string source, PokemonData[] target)
        {
            int i = 0;
            foreach (Match m in Regex.Matches(source, @"(.+?)(\-\S*){0,1} (?:\((.{2,}?)(?:\-\w){0,1}\) ){0,1}(?:\(([FM])\) ){0,1}@ (.+?)  \nAbility: (.+?)\nEVs: (.+?)\n(.+?) Nature.*\n(IVs: (.+?)\n)?((?:\- .+?\n)+)"))
            {
                try
                {
                    target[i++] = ImportFromPS(m);
                }
                catch { }
                if (i == target.Length) break;
            }
        }

        private static void ImportFromPBO(string source, PokemonData[] target)
        {
            int i = 0;  
            foreach (Match m in Regex.Matches(source, @"(.+?)（(.+?)） *Lv.(\d+)(?: *(.)){0,1}\n\* 特性：[ 　]*(.+?)\n\* 性格：[ 　]*(.+?)\n(?:\* 个体值{0,1}：[ 　]*(.+?)\n){0,1}\* 努力值{0,1}：[ 　]*(.+?)\n(?:\* 亲密度：[ 　]*(\d+?)\n){0,1}\* 道具：[ 　]*(.+?)\n\* 技能：[ 　]*(.+?)\n"))
            {
                try
                {
                    target[i++] = ImportFromPBO(m);
                }
                catch { }
                if (i == target.Length) break;
            }
        }

        public static void Export(StringBuilder sb, PokemonData pm)
        {
            const string space = "";//"　";
            sb.Append(pm.Name, "（", GameString.Current.Pokemon(pm.Form.Species.Number, pm.Form.Index), "）", " Lv.", pm.Lv);
            if (pm.Gender == PokemonGender.Male) sb.Append(" ♂");
            else if (pm.Gender == PokemonGender.Female) sb.Append(" ♀");
            sb.AppendLine();
            sb.AppendLine("* 特性：", space, GameString.Current.Ability(pm.Ability));
            sb.AppendLine("* 性格：", space, GameString.Current.Nature(pm.Nature));
            {
                var ss = pm.Iv;
                if (ss.Hp != 31 || ss.Atk != 31 || ss.Def != 31 || ss.SpAtk != 31 || ss.SpDef != 31 || ss.Speed != 31)
                    sb.AppendLine("* 个体：", space, ss.Hp, "/", ss.Atk, "/", ss.Def, "/", ss.SpAtk, "/", ss.SpDef, "/", ss.Speed);
                ss = pm.Ev;
                sb.AppendLine("* 努力：", space, ss.Hp, "/", ss.Atk, "/", ss.Def, "/", ss.SpAtk, "/", ss.SpDef, "/", ss.Speed);
            }
            if (pm.Happiness < 255) sb.AppendLine("* 亲密度：", pm.Happiness);
            sb.AppendLine("* 道具：", space, pm.Item == 0 ? "无" : GameString.Current.Item(pm.Item));
            sb.Append("* 技能：", space);
            if (pm.Moves.Count() == 0)
            {
                sb.Append("无");
            }
            else
            {
                bool first = true;
                foreach (var m in pm.Moves)
                {
                    if (first) first = false;
                    else sb.Append("/");
                    sb.Append(GameString.Current.Move(m.Move.Id));
                    if (m.Move.Id == Ms.HIDDEN_POWER) sb.Append('[', GameString.Current.BattleType(GameHelper.HiddenPower(pm.Iv)), ']');
                }
            }
        }

        private static PokemonGender GetGender(string s)
        {
            switch (s.ToUpper())
            {
                case "M":
                case "♂": return PokemonGender.Male;
                case "F":
                case "♀": return PokemonGender.Female;
                default: return PokemonGender.None;
            }
        }

        private static int TryMatch(string input, string pattern, short group = 0, int defaultvalue = 0)
        {
            var m = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            return m.Success ? int.Parse(m.Groups[group].Value) : defaultvalue;
        }
    }
}
