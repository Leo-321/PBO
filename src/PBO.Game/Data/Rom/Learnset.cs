using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PokemonBattleOnline.Game
{
    public class LvLearnset
    {
        private readonly KeyValuePair<int, int>[][] Lvs;
        private readonly Dictionary<int, KeyValuePair<int, int>[]> Forms;

        public LvLearnset()
        {
            Lvs = new KeyValuePair<int, int>[RomData.POKEMONS][]; //怎么想都是件不可思议的事..
            Forms = new Dictionary<int, KeyValuePair<int, int>[]>();
        }

        internal void Set(int number, int form, KeyValuePair<int, int>[] moves)
        {
            if (form == 0) Lvs[number - 1] = moves;
            else Forms[number * 100 + form] = moves;
        }
        public IEnumerable<KeyValuePair<int, int>> Get(int number, int form)
        {
            return form == 0 ? Lvs.ValueOrDefault(number - 1) : Forms.ValueOrDefault(number * 100 + form);
        }
    }
    public class EggLearnset
    {
        private readonly Dictionary<int, int[]> Eggs;

        public EggLearnset()
        {
            Eggs = new Dictionary<int, int[]>();
        }

        internal void Set(int number, int[] moves)
        {
            Eggs[number] = moves;
        }
        public IEnumerable<int> Get(int number)
        {
            return Eggs.ValueOrDefault(number) ?? Enumerable.Empty<int>();
        }
    }
    public class TMHMTutorLearnset
    {
        private readonly int[][] Raw;
        private readonly Dictionary<int, int[]> Forms;

        public TMHMTutorLearnset()
        {
            Raw = new int[RomData.POKEMONS][];
            Forms = new Dictionary<int, int[]>();
        }

        internal void Set(int number, int form, int[] moves)
        {
            if (form == 0) Raw[number - 1] = moves;
            else Forms[number * 100 + form] = moves;
        }
        public IEnumerable<int> Get(int number, int form)
        {
            return (form == 0 ? Raw.ValueOrDefault(number - 1) : Forms.ValueOrDefault(number * 100 + form)) ?? Enumerable.Empty<int>();
        }
    }
    public class GameLearnset
    {
        public readonly string Name;
        public readonly LvLearnset Lv;
        public readonly EggLearnset Egg;
        public readonly TMHMTutorLearnset TM;
        public readonly TMHMTutorLearnset Tutor;
        public readonly TMHMTutorLearnset HM;
        public readonly TMHMTutorLearnset E1;
        public readonly TMHMTutorLearnset E2;

        public GameLearnset(string name, LvLearnset lv, EggLearnset egg, TMHMTutorLearnset tm, TMHMTutorLearnset tutor, TMHMTutorLearnset hm, TMHMTutorLearnset e1, TMHMTutorLearnset e2)
        {
            Name = name;
            Lv = lv;
            Egg = egg;
            TM = tm;
            Tutor = tutor;
            HM = hm;
            E1 = e1;
            E2 = e2;
        }
    }
    public class GenLearnset
    {
        public readonly int Gen;

        internal GenLearnset(int gen)
        {
            Gen = gen;
            games = new Dictionary<string, GameLearnset>();
        }

        private readonly Dictionary<string, GameLearnset> games;
        public IEnumerable<GameLearnset> Games
        { get { return games.Values; } }

        internal void Add(string game, LvLearnset lv, EggLearnset egg, TMHMTutorLearnset tm, TMHMTutorLearnset tutor, TMHMTutorLearnset hm, TMHMTutorLearnset e1, TMHMTutorLearnset e2)
        {
            games.Add(game, new GameLearnset(game, lv, egg, tm, tutor, hm, e1, e2));
        }
    }
    public class Learnset
    {
        public readonly GenLearnset[] Index;

        internal Learnset()
        {
            Index = new GenLearnset[RomData.GEN - 2];
            var dll = System.Reflection.Assembly.GetExecutingAssembly();
            using (var index = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Learnset.index.txt")))
            {
                var gll = new GenLearnset(3);
                var lvs = new Dictionary<string, LvLearnset>();
                var tmhmts = new Dictionary<string, TMHMTutorLearnset>();
                var eggs = new Dictionary<string, EggLearnset>();
                for (var line = index.ReadLine(); !string.IsNullOrWhiteSpace(line); line = index.ReadLine())
                {
                    var s = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    var gen = int.Parse(s[0]);
                    if (gen != gll.Gen)
                    {
                        Index[gll.Gen - 3] = gll;
                        gll = new GenLearnset(gen);
                    }
                    LvLearnset lv = null;
                    EggLearnset egg = null;
                    TMHMTutorLearnset tm = null, hm = null, tutor = null, e1 = null, e2 = null;
                    for (int i = 2; i < s.Length; ++i)
                    {
                        var table = s[i];
                        switch (table[1])
                        {
                            case 'e': //Level
                                {
                                    lv = lvs.ValueOrDefault(table);
                                    if (lv == null)
                                    {
                                        lv = new LvLearnset();
                                        LoadLevel(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Learnset." + table + ".txt"), lv);
                                        lvs.Add(table, lv);
                                    }
                                }
                                break;
                            case 'g': //Egg
                                {
                                    egg = eggs.ValueOrDefault(table);
                                    if (egg == null)
                                    {
                                        egg = new EggLearnset();
                                        LoadEgg(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Learnset." + table + ".txt"), egg);
                                        eggs.Add(table, egg);
                                    }
                                }
                                break;
                            case 'M': //TMHM
                                {
                                    var needHM = gen == 6;
                                    var g = table.Substring(5);
                                    var tmk = "TM_" + g;
                                    tm = tmhmts.ValueOrDefault(tmk);
                                    if (tm == null)
                                    {
                                        tm = new TMHMTutorLearnset();
                                        if (needHM) hm = new TMHMTutorLearnset();
                                        LoadTMHM(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Learnset." + table + ".txt"), tm, hm);
                                        tmhmts.Add(tmk, tm);
                                        if (needHM) tmhmts.Add("HM_" + g, hm);
                                    }
                                    else if (needHM) hm = tmhmts["HM_" + g];
                                }
                                break;
                            case 'u': //Tutor
                                {
                                    tutor = tmhmts.ValueOrDefault(table);
                                    if (tutor == null)
                                    {
                                        tutor = new TMHMTutorLearnset();
                                        LoadTutor(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Learnset." + table + ".txt"), tutor);
                                        tmhmts.Add(table, tutor);
                                    }
                                }
                                break;
                            case 'v': //Event
                                {
                                    var e = tmhmts.ValueOrDefault(table);
                                    if (e == null)
                                    {
                                        e = new TMHMTutorLearnset();
                                        LoadEvents(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Learnset." + table + ".txt"), e);
                                        tmhmts.Add(table, e);
                                    }
                                    if (e1 == null) e1 = e;
                                    else e2 = e;
                                }
                                break;
                        }
                    }//for (int i = 2;
                    gll.Add(s[1], lv, egg, tm, tutor, hm, e1, e2);
                }
                Index[gll.Gen - 3] = gll;
            }
        }

        private static readonly char[] SPLIT_CHARS = new char[] { ',', '\t', '.' };
        private static void LoadLevel(Stream stream, LvLearnset lv)
        {
            var mls = new List<KeyValuePair<int, int>>();
            using (var sr = new StreamReader(stream))
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    //[0].[1] [2],[3]...
                    var s = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 2; i < s.Length; ++i)
                    {
                        //move:lv
                        var ml = s[i].Split(':');
                        mls.Add(new KeyValuePair<int, int>(int.Parse(ml[0]), int.Parse(ml[1])));
                    }
                    if (mls.Any())
                    {
                        lv.Set(int.Parse(s[0]), int.Parse(s[1]), mls.ToArray());
                        mls.Clear();
                    }
                }
        }
        private static void LoadEgg(Stream stream, EggLearnset egg)
        {
            var moves = new List<int>();
            using (var sr = new StreamReader(stream))
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    //[0].[1] [2],[3]...
                    var s = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 2; i < s.Length; ++i) moves.Add(int.Parse(s[i]));
                    if (moves.Any())
                    {
                        egg.Set(int.Parse(s[0]), moves.ToArray());
                        moves.Clear();
                    }
                }
        }
        private static void LoadTMHM(Stream stream, TMHMTutorLearnset tm, TMHMTutorLearnset hm)
        {
            var tmmoves = new List<int>();
            var hmmoves = hm == null ? null : new List<int>();
            using (var sr = new StreamReader(stream))
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    //[0].[1] [2],[3]...
                    var s = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 2; i < s.Length; ++i)
                    {
                        //move:TM## move:HM##
                        var mm = s[i];
                        var colon = mm.Length - 5;
                        if (mm[colon] != ':') colon--;
                        var move = int.Parse(mm.Substring(0, colon));
                        if (mm[colon + 1] == 'T') tmmoves.Add(move);
                        else if (hmmoves == null) break;
                        else hmmoves.Add(move);
                    }
                    var number = int.Parse(s[0]);
                    var form = int.Parse(s[1]);
                    if (tmmoves.Any())
                    {
                        tm.Set(number, form, tmmoves.ToArray());
                        tmmoves.Clear();
                    }
                    if (hmmoves != null && hmmoves.Any())
                    {
                        hm.Set(number, form, hmmoves.ToArray());
                        hmmoves.Clear();
                    }
                }
        }
        private static void LoadTutor(Stream stream, TMHMTutorLearnset tutor)
        {
            var moves = new List<int>();
            using (var sr = new StreamReader(stream))
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    //[0].[1] [2],[3]...
                    var s = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 2; i < s.Length; ++i) moves.Add(int.Parse(s[i]));
                    if (moves.Any())
                    {
                        tutor.Set(int.Parse(s[0]), int.Parse(s[1]), moves.ToArray());
                        moves.Clear();
                    }
                }
        }
        private static void LoadEvents(Stream stream, TMHMTutorLearnset tutor)
        {
            var moves = new List<int>();
            using (var sr = new StreamReader(stream))
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    //[0].[1] [2],[3]...
                    var s = line.Split(SPLIT_CHARS, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 2; i < s.Length; ++i)
                    {
                        var m = s[i].Split(':');
                        moves.Add(int.Parse(m[0]));
                    }
                    if (moves.Any())
                    {
                        tutor.Set(int.Parse(s[0]), int.Parse(s[1]), moves.ToArray());
                        moves.Clear();
                    }
                }
        }
    }
}
