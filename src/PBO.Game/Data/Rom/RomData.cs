using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Windows;

namespace PokemonBattleOnline.Game
{
    [DataContract(Namespace = PBOMarks.PBO)]
    public static class RomData
    {
        public const int GEN = 7;
        public const int BATTLETYPES = 18;
        public const int NATURES = 25;
        public const int POKEMONS = 802;
        public const int MOVES = 719;
        public const int ABILITIES = 232;
        public const int MOVECATEGORIES = 3;

        static RomData()
        {
            var dll = System.Reflection.Assembly.GetExecutingAssembly();
            using (var sr = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Pokemons.txt")))
            {
                _pokemons = new PokemonSpecies[POKEMONS];
                for (int i = 0; i < POKEMONS; ++i)
                {
                    //31	Monster	Grass	Green	1
                    var line = sr.ReadLine();
                    var s = line.Split('\t');
                    _pokemons[i] = new PokemonSpecies(i + 1, int.Parse(s[0]), (EggGroup)Enum.Parse(typeof(EggGroup), s[1]), string.IsNullOrEmpty(s[2]) ? EggGroup.Invalid : (EggGroup)Enum.Parse(typeof(EggGroup), s[2]), (PokemonColor)Enum.Parse(typeof(PokemonColor), s[3]), int.Parse(s[4]));
                }
            }
            using (var sr = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Forms.txt")))
            {
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    //#	F	T1	T2	A1	A2	A3	HP	A	D	SA	SD	S	H	W
                    //1	0	Grass	Poison	65		34	45	49	49	65	65	45	0.7	6.9
                    var s = line.Split('\t');
                    var t1 = (BattleType)Enum.Parse(typeof(BattleType), s[2]);
                    var t2 = string.IsNullOrEmpty(s[3]) ? BattleType.Invalid : (BattleType)Enum.Parse(typeof(BattleType), s[3]);
                    var a1 = int.Parse(s[4]);
                    var a2 = string.IsNullOrEmpty(s[5]) ? 0 : int.Parse(s[5]);
                    var a3 = string.IsNullOrEmpty(s[6]) ? 0 : int.Parse(s[6]);
                    var d6 = new ReadOnly6D(int.Parse(s[7]), int.Parse(s[8]), int.Parse(s[9]), int.Parse(s[10]), int.Parse(s[11]), int.Parse(s[12]));
                    var h = float.Parse(s[13]);
                    var w = float.Parse(s[14]);
                    _pokemons[int.Parse(s[0]) - 1].SetFormData(int.Parse(s[1]), new PokemonFormData(t1, t2, a1, a2, a3, d6, h, w));
                }
            }
            using (var sr = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Evolutions.txt")))
            {
                var es = new List<Evolution>(354);
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                {
                    var s = line.Split('\t');
                    es.Add(new Evolution(int.Parse(s[0]), int.Parse(s[1])));
                }
                _evolutions = es.ToArray();
            }
            using (var sr = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Moves.txt")))
            {
                _moves = new MoveType[MOVES];
                for (int i = 0; i < MOVES; ++i)
                {
                    //Normal	Physical	40	100	35	Single
                    var line = sr.ReadLine();
                    var s = line.Split('\t');
                    _moves[i] = new MoveType(i + 1, (BattleType)Enum.Parse(typeof(BattleType), s[0]), (MoveCategory)Enum.Parse(typeof(MoveCategory), s[1]), int.Parse(s[2]), int.Parse(s[3]), int.Parse(s[4]), (MoveRange)Enum.Parse(typeof(MoveRange), s[5]));
                }
            }
            using (var sr = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Items.txt")))
            {
                var items = new List<int>(326);
                items.Add(0);
                for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine()) items.Add(int.Parse(line));
                _items = items.ToArray();
            }
                _learnSet = new Learnset();
        }

        private static readonly PokemonSpecies[] _pokemons;
        public static IEnumerable<PokemonSpecies> Pokemons
        { get { return _pokemons; } }

        private static readonly Evolution[] _evolutions;
        public static IEnumerable<Evolution> Evolutions
        { get { return _evolutions; } }

        private static readonly MoveType[] _moves;
        public static IEnumerable<MoveType> Moves
        { get { return _moves; } }
        /*
        private static PokemonData[] _eggs;
        public static IEnumerable<PokemonData> Eggs
        { get { return _eggs; } }
        public static void initEggs()
        {
            var dll = System.Reflection.Assembly.GetExecutingAssembly();
            using (var sr = new StreamReader(dll.GetManifestResourceStream("PokemonBattleOnline.Game.dat.Eggs.txt")))
            {
                _eggs = new PokemonData[3];
                var all = sr.ReadToEnd();
                Xxporter.Import(all, _eggs);
            }
        }
        */
        private static readonly int[] _items;
        /// <summary>
        /// include 0
        /// </summary>
        public static IEnumerable<int> Items
        { get { return _items; } }

        private static readonly Learnset _learnSet;
        public static Learnset LearnSet
        { get { return _learnSet; } }

        public static PokemonSpecies GetPokemon(int number)
        {
            return _pokemons.ValueOrDefault(number - 1);
        }
        public static PokemonForm GetPokemon(int number, int form)
        {
            if (number == 0 || number > _pokemons.Length) return null;
            return _pokemons[number - 1].GetForm(form);
        }
        public static MoveType GetMove(int moveId)
        {
            return _moves.ValueOrDefault(moveId - 1);
        }

        public static BattleType[] HiddenPower = { BattleType.Bug, BattleType.Dark, BattleType.Dragon, BattleType.Electric, BattleType.Fighting, BattleType.Fire, BattleType.Flying, BattleType.Ghost, BattleType.Grass, BattleType.Ground, BattleType.Ice, BattleType.Poison, BattleType.Psychic, BattleType.Rock, BattleType.Steel, BattleType.Water };
    }
}
