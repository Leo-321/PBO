using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public class PokemonForm
    {
        internal PokemonForm(PokemonSpecies species, int index)
        {
            _species = species;
            _index = index;
        }

        private readonly PokemonSpecies _species;
        public PokemonSpecies Species
        { get { return _species; } }

        private readonly int _index;
        public int Index
        { get { return _index; } }

        private PokemonFormData _data;
        public PokemonFormData Data
        {
            get { return _data; }
            internal set { _data = value; }
        }

        private static readonly BattleType[] ARCEUS = new BattleType[] { BattleType.Normal, BattleType.Fire, BattleType.Water, BattleType.Electric, BattleType.Grass, BattleType.Ice, BattleType.Fighting, BattleType.Poison, BattleType.Ground, BattleType.Flying, BattleType.Psychic, BattleType.Bug, BattleType.Rock, BattleType.Ghost, BattleType.Dragon, BattleType.Dark, BattleType.Steel, BattleType.Fairy };
        public BattleType Type1
        { get { return Species.Number == Ps.ARCEUS  ? ARCEUS[_index] : Species.Number == Ps.Silvally? ARCEUS[_index] : _data.Type1; } }
        public BattleType Type2
        { get { return _data.Type2; } }

        private static readonly int[] MegaNumbers = { 3, 6, 9, 15, 18, 65, 80, 94, 115, 127, 130, 142, 150, 181, 208, 212, 214, 229, 248, 254, 257, 260, 282, 302, 303, 306, 308, 310, 319, 323, 334, 354, 359, 362, 373, 376, 380, 381, 384, 428, 445, 448, 460, 475, 531, 719 };
        public bool IsMega
        { get { return Index != 0 && MegaNumbers.Contains(Species.Number); } }
    }
}
