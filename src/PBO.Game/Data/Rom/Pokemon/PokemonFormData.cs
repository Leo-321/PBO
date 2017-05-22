using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public class PokemonFormData
    {
        internal PokemonFormData(BattleType t1, BattleType t2, int a1, int a2, int a3, ReadOnly6D d6, float height, float weight)
        {
            _type1 = t1;
            _type2 = t2;
            if (a1 == a2) a2 = 0;
            _abilities = new int[3] { a1, a2, a3 };
            _base = d6;
            _height = height;
            _weight = weight;
        }

        private readonly BattleType _type1;
        internal BattleType Type1
        { get { return _type1; } }

        private readonly BattleType _type2;
        internal BattleType Type2
        { get { return _type2; } }

        private readonly int[] _abilities;
        public int[] Abilities
        { get { return _abilities; } }

        private readonly ReadOnly6D _base;
        public I6D Base
        { get { return _base; } }

        private readonly float _height;
        public float Height
        { get { return _height; } }

        private readonly float _weight;
        public float Weight
        { get { return _weight; } }

        public int GetAbility(int index)
        {
            var ab = _abilities.ValueOrDefault(index);
            return ab == 0 ? _abilities[0] : ab;
        }
    }
}
