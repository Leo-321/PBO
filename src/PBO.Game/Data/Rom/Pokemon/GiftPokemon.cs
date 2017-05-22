using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public class GiftPokemon
    {
        private readonly short number;

        private readonly byte form;

        private readonly short ownerId; //N精灵素大坑，日后再填

        private readonly ushort personality; //0xffffffff for random

        private readonly bool neverShiney; //闪光素大坑，日后再填

        private GiftPokemon()
        {
        }

        private readonly byte _gen;
        public int Gen
        { get { return _gen; } }

        private readonly string _name;

        public PokemonForm Form
        { get { return RomData.GetPokemon(number, form); } }

        private readonly PokemonGender _gender;
        public PokemonGender Gender
        { get { return _gender; } }

        private readonly int _lv;
        public int Lv
        { get { return _lv; } }

        private readonly int _ability; //不是AbilityIndex大丈夫？

        private readonly int _item;

        private readonly PokemonNature? _nature;

        private readonly ReadOnly6D _ivs; //0 for random

        private readonly int[] _moveIds;
        public IEnumerable<int> MoveIds
        { get { return _moveIds; } }

        private readonly bool _fatefulEncounter;
        public bool FatefulEncounter
        { get { return _fatefulEncounter; } }
    }
}
