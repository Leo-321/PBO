using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
    public enum PokemonGender : byte
    {
        None,
        Male,
        Female,
    }
    public enum PokemonColor : byte
    {
        Red = 0,
        Blue = 1,
        Yellow = 2,
        Green = 3,
        Black = 4,
        Brown = 5,
        Purple = 6,
        Gray = 7,
        White = 8,
        Pink = 9
    }
    public class PokemonSpecies
    {
        internal PokemonSpecies(int number, int gender, EggGroup e1, EggGroup e2, PokemonColor color, int forms)
        {
            _number = number;
            _genderBoundary = (byte)gender;
            _forms = new PokemonForm[forms];
            for (int i = 0; i < forms; ++i) _forms[i] = new PokemonForm(this, i);
            _eggGroup1 = e1;
            _eggGroup2 = e2;
            _color = color;
        }

        private readonly int _number;
        public int Number
        { get { return _number; } }

        private readonly byte _genderBoundary;
        private static readonly IEnumerable<PokemonGender> NONE = new[] { PokemonGender.None };
        private static readonly IEnumerable<PokemonGender> MALE = new[] { PokemonGender.Male };
        private static readonly IEnumerable<PokemonGender> FEMALE = new[] { PokemonGender.Female };
        private static readonly IEnumerable<PokemonGender> BOTH = new[] { PokemonGender.Male, PokemonGender.Female };
        private IEnumerable<PokemonGender> genders;
        public IEnumerable<PokemonGender> Genders
        {
            get
            {
                if (genders == null)
                    switch (_genderBoundary)
                    {
                        case 0x00:
                            genders = MALE;
                            break;
                        case 0xfe:
                            genders = FEMALE;
                            break;
                        case 0xff:
                            genders = NONE;
                            break;
                        default:
                            genders = BOTH;
                            break;
                    }
                return genders;
            }
        }

        private readonly EggGroup _eggGroup1;
        public EggGroup EggGroup1
        { get { return _eggGroup1; } }

        private readonly EggGroup _eggGroup2;
        public EggGroup EggGroup2
        { get { return _eggGroup2; } }

        private readonly PokemonColor _color;
        public PokemonColor Color
        { get { return _color; } }

        private readonly PokemonForm[] _forms;
        
        public IEnumerable<PokemonForm> Forms
        { get { return _forms; } }
        /// <summary>
        /// public for Binding
        /// </summary>
        public IEnumerable<PokemonForm> CanSelectForms
        {
            get
            {
                if (Number == 718)
                {
                    var f = new PokemonForm[4];
                    for (int i = 0; i < 4; i++)
                        f[i] = _forms[i];
                    return f; 
                }
                else return _forms;
            }
        }

        internal void SetFormData(int index, PokemonFormData data)
        {
            if (index == 0)
            {
                foreach (var f in _forms)
                    if (f.Data == null) f.Data = data;
            }
            else _forms[index].Data = data;
        }

        public PokemonForm GetForm(int index)
        {
            return _forms.ValueOrDefault(index);
        }

        public override string ToString()
        {
            return _number.ToString();
        }
    }
}
