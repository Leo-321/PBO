using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
    [DataContract(Name = "pd", Namespace = PBOMarks.JSON)]
    public class PokemonData : ObservableObject, ICloneable, IPokemonData
    {
        private static bool CanChangeIv(I6D sender, int oldValue, int newValue)
        {
            return 0 <= newValue && newValue < 32;
        }

        private static bool CanChangeEv(I6D sender, int oldValue, int newValue)
        {
            return 0 <= newValue && newValue <= 252 && sender.Sum() + newValue - oldValue <= 510;
        }

        [DataMember(Name = "n")]
        private int number;
        [DataMember(Name = "f", EmitDefaultValue = false)]
        private int form;

        public PokemonData(int number, int form)
        {
            _moves = new ObservableCollection<LearnedMove>();
            _ev = new Observable6D();
            Form = RomData.GetPokemon(number, form);
        }

        public static int[] MegaPokemon = {3, 6, 6, 9, 65, 94, 115, 127, 130, 142, 150, 150, 181, 212, 214, 229, 248, 257, 282, 303,
                                 306, 308, 310, 354, 359, 445, 448, 460, 15, 18, 80, 208, 254, 260, 302,
                                 319, 323, 334, 362, 373, 376, 380, 381, 382, 383, 428, 475, 531, 719 };

        public static int[] UselessItem = {1,2,6,8016,8017,8018,8019,8020,8021,8022,8023,8024,8025,8026,8027,8028,8029,8030,8031,8032,8033,8034,8035 };

        public PokemonData(int seed, string suiji)  //随机生成pm
        {
            _moves = new ObservableCollection<LearnedMove>();
            _ev = new Observable6D();
            
            Random random = new Random(seed);
            bool item_ = true;

            number = random.Next(1, 803);
            Form = RomData.GetPokemon(number, form);

            int ff;
            ff = random.Next(RomData.Pokemons.ElementAtOrDefault(number - 1).Forms.Count());
            if (MegaPokemon.Contains(number) && (ff > 0))
            {
                item_=SuitMegaStone(ff);
                ff = 0;
            }
            if (number == 670 && ff <= 4)
                ff = 0;
            if (number == 025 || number == 351 || number == 384 || number == 421 || number == 493 || number == 555 || number == 647 || number == 648 || number == 649 || number == 658 || number == 666 || number == 669 || number == 671 || number == 676 || number == 678 || number == 681 || number == 710 || number == 711 || number == 718 && form == 4 || number==773 || number == 774 || number == 778)
                ff = 0;
            if (number == 382 && form == 1)
            {
                _item = 9044;
                item_ = false;
            }
            if (number == 383 && form == 1)
            {
                _item = 9045;
                item_ = false;
            }
            if (number == 487 && ff == 1)
            {
                _item = 1003;
                item_ = false;
            }
            if (number == 718 && ff == 4)
            {
                ff = 2;
            }
            Form = RomData.GetPokemon(number, ff);

            Random genderrandom = new Random(random.Next());
            Gender = Form.Species.Genders.ElementAtOrDefault(genderrandom.Next(0, Form.Species.Genders.Count()));

            Nature = (PokemonNature)random.Next(0, 25);

            //for (int i = 0; i != 0; i = Form.Data.Abilities[random.Next(0, Form.Data.Abilities.Count())])
                //Ability = i;
            _abilityIndex = (Byte)random.Next(0, Form.Data.Abilities.Count());

            _Iv = new Observable6D(random.Next(0, 32), random.Next(0, 32), random.Next(0, 32), random.Next(0, 32), random.Next(0, 32), random.Next(0, 32));

            int[] ev = new int[] { 0, 0, 0, 0, 0, 0 };
            int index = 0;
            for (int i = 0; i < 127; i++)
            {
                while (true)
                {
                    index = random.Next(0, 6);
                    if (ev[index] <= 251)
                    {
                        ev[index] += 4;
                        break;
                    }
                }
            }
            _ev = new Observable6D(ev[0], ev[1], ev[2], ev[3], ev[4], ev[5]);

            GetLearnset();
            while (moveIds.Count() < 4 && moveIds.Count() < learnset.Count())
            {
                int x = learnset[random.Next(0, learnset.Count())];
                if(x!=-1)
                    AddMove(RomData.GetMove(x));
            }
            if (number == 647 && HasMove(Ms.SECRET_SWORD))
                Form = RomData.GetPokemon(number, 1);

            Happiness = random.Next(0, 256);

            if (item_)
            {
                ItemLoop:
                Item = RomData.Items.ElementAtOrDefault(random.Next(0, RomData.Items.Count()));
                if(UselessItem.Contains(Item) || (9000 <= Item && Item < 10004) || (11000 <= Item && Item < 12000 && number != 773) || (1300 <= Item && Item < 1400 && number != 649) ) goto ItemLoop;
            }
        }

        #region properties
        [DataMember(Name = "nc", EmitDefaultValue = false)]
        private string _name;
        string IPokemonData.Name
        { get { return _name; } }
        public string Name
        {
            get { return _name ?? GameString.Current.Pokemon(number); }
            set
            {
                if (string.IsNullOrWhiteSpace(value)) value = null;
                else value = value.Trim();
                if (_name != value && PokemonValidator.ValidateName(value))
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private static readonly int[] CAN_CHOOSE_FORM = { Ps.UNOWN, Ps.DEOXYS, Ps.BURMY, Ps.WORMADAM, Ps.SHELLOS, Ps.GASTRODON, Ps.ROTOM, Ps.SHAYMIN, Ps.BASCULIN, Ps.DEERLING, Ps.SAWSBUCK, Ps.TORNADUS, Ps.THUNDURUS, Ps.LANDORUS, Ps.KYUREM, Ps.VIVILLON, Ps.FLABEBE, Ps.FLOETTE, Ps.FLORGES, Ps.FURFROU, Ps.PUMPKABOO, Ps.GOURGEIST, Ps.HOOPA, Ps.Lycanroc, Ps.Zygarde, Ps.Oricorio };
        public bool CanChooseForm
        { get { return CAN_CHOOSE_FORM.Contains(number) || number == Ps.KELDEO && HasMove(Ms.SECRET_SWORD) || GameHelper.Alola(number); } }

        private PokemonForm _form;
        public PokemonForm Form
        {
            get
            {
                if (_form == null)
                {
                    CheckSpForm();
                    _form = RomData.GetPokemon(number, form);
                }
                return _form;
            }
            set
            {
                if (!(number == value.Species.Number && form == value.Index))
                {
                    _form = null;
                    var oldNumber = number;
                    var oldForm = form;

                    number = value.Species.Number;
                    form = (byte)value.Index;
                    CheckSpForm();

                    if (oldNumber != number)
                    {
                        _moves.Clear();
                        _gender = value.Species.Genders.First();
                        _ev.SetAll(0);
                        _lv = 0;
                        _nature = default(PokemonNature);
                        Iv.SetAll(31);
                        _happiness = 0;
                        _item = 0;
                    }
                    else if ( number == Ps.DEOXYS || number == Ps.WORMADAM || number == Ps.ROTOM || number == Ps.KYUREM || number == Ps.MEOWSTIC || number == Ps.Lycanroc || GameHelper.Alola(number)) _moves.Clear();
                    _abilityIndex = 0;

                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Name = "l", EmitDefaultValue = false)]
        private byte _lv;
        public int Lv
        {
            get { return 100 - _lv; }
            set
            {
                if (_lv != value && PokemonValidator.ValidateLv(value))
                {
                    _lv = (byte)(100 - value);
                    OnPropertyChanged("Lv");
                }
            }
        }

        [DataMember(Name = "g", EmitDefaultValue = false)]
        private PokemonGender _gender;
        public PokemonGender Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value && Form.Species.Genders.Contains(value))
                {
                    _gender = value;
                    if (CheckSpForm()) OnPropertyChanged();
                    else OnPropertyChanged("Gender");
                }
            }
        }

        [DataMember(Name = "t", EmitDefaultValue = false)]
        private PokemonNature _nature;
        public PokemonNature Nature
        {
            get { return _nature; }
            set
            {
                if (_nature != value)
                {
                    _nature = value;
                    OnPropertyChanged("Nature");
                }
            }
        }

        [DataMember(Name = "a", EmitDefaultValue = false)]
        private byte _abilityIndex;
        int IPokemonData.AbilityIndex
        { get { return _abilityIndex; } }
        public int Ability
        {
            get { return Form.Data.GetAbility(_abilityIndex); }
            set
            {
                if (Ability != value)
                {
                    _abilityIndex = 0;
                    if (Form.Data.Abilities[1] == value) _abilityIndex = 1;
                    else if (Form.Data.Abilities[2] == value) _abilityIndex = 2;
                    else return;
                    OnPropertyChanged("Ability");
                }
            }
        }

        private Observable6D _iv;
        public Observable6D Iv
        {
            get
            {
                if (_iv == null)
                {
                    _iv = new Observable6D(31, 31, 31, 31, 31, 31);
                    _iv.CanChange6D += CanChangeIv;
                }
                return _iv;
            }
        }
        I6D IPokemonData.Iv
        { get { return Iv; } }
        [DataMember(Name = "iv", EmitDefaultValue = false)]
        private Observable6D _Iv
        {
            get { return new Observable6D(31 - Iv.Hp, 31 - Iv.Atk, 31 - Iv.Def, 31 - Iv.SpAtk, 31 - Iv.SpDef, 31 - Iv.Speed); }
            set
            {
                _iv = new Observable6D(31 - value.Hp, 31 - value.Atk, 31 - value.Def, 31 - value.SpAtk, 31 - value.SpDef, 31 - value.Speed);
                _iv.CanChange6D += CanChangeIv;
            }
        }

        [DataMember(Name = "i", EmitDefaultValue = false)]
        private int _item;
        public int Item
        {
            get { return _item; }
            set
            {
                if (Item != value)
                {
                    _item = value;
                    if (CheckSpForm()) OnPropertyChanged();
                    else OnPropertyChanged("Item");
                }
            }
        }

        [DataMember(Name = "h", EmitDefaultValue = false)]
        private byte _happiness;
        public int Happiness
        {
            get { return 255 - _happiness; }
            set
            {
                if (Happiness != value && 0 <= value && value <= 255)
                {
                    _happiness = (byte)(255 - value);
                    OnPropertyChanged("Happiness");
                }
            }
        }

        [DataMember(Name = "e")]
        private Observable6D _ev;
        I6D IPokemonData.Ev
        { get { return _ev; } }
        bool got;
        public Observable6D Ev
        {
            get
            {
                if (!got)
                {
                    got = true;
                    _ev.CanChange6D += CanChangeEv;
                }
                return _ev;
            }
        }

        public BattleType HiddenPowerType
        {
            get{ return GameHelper.HiddenPower(Iv); }
            set
            {
                switch (value)
                {
                    case BattleType.Bug:
                        _iv.SetAll(31, 30, 30, 31, 30, 31);
                        break;
                    case BattleType.Dark:
                        _iv.SetAll(31, 31, 31, 31, 31, 31);
                        break;
                    case BattleType.Dragon:
                        _iv.SetAll(31, 31, 30, 31, 31, 31);
                        break;
                    case BattleType.Electric:
                        _iv.SetAll(31, 31, 31, 30, 31, 31);
                        break;
                    case BattleType.Fighting:
                        _iv.SetAll(31, 31, 30, 30, 30, 30);
                        break;
                    case BattleType.Fire:
                        _iv.SetAll(31, 30, 31, 30, 31, 30);
                        break;
                    case BattleType.Flying:
                        _iv.SetAll(30, 30, 30, 30, 30, 31);
                        break;
                    case BattleType.Ghost:
                        _iv.SetAll(31, 31, 30, 31, 30, 31);
                        break;
                    case BattleType.Grass:
                        _iv.SetAll(31, 30, 31, 30, 31, 31);
                        break;
                    case BattleType.Ground:
                        _iv.SetAll(31, 31, 31, 30, 30, 31);
                        break;
                    case BattleType.Ice:
                        _iv.SetAll(31, 30, 30, 31, 31, 31);
                        break;
                    case BattleType.Poison:
                        _iv.SetAll(31, 31, 30, 30, 30, 31);
                        break;
                    case BattleType.Psychic:
                        _iv.SetAll(31, 30, 31, 31, 31, 30);
                        break;
                    case BattleType.Rock:
                        _iv.SetAll(31, 31, 30, 31, 30, 30);
                        break;
                    case BattleType.Steel:
                        _iv.SetAll(31, 31, 31, 31, 30, 31);
                        break;
                    case BattleType.Water:
                        _iv.SetAll(31, 30, 30, 30, 31, 31);
                        break;
                }
            }
        }

        public bool SuitMegaStone(int form=1)
        {
            int megastone = 0;
            for (megastone = 0; megastone < 49; megastone++)
            {
                if (number == MegaPokemon[megastone])
                {
                    megastone += 9001;
                    if (number == 6 || number == 150)
                        megastone += (form - 1);
                    Item = megastone;
                    return false;
                }
            }
            return true;
        }

        private byte GetMoveIds_PPx(int x)
        {
            byte r = 0;
            byte i = 1;
            foreach (var m in _moves)
            {
                if (m.PPUp == x) r |= i;
                i <<= 1;
            }
            return r;
        }
        private void SetMoveIds_PPx(int x, byte value)
        {
            if ((value & 1) != 0) _moves[0].PPUp = 2;
            if ((value & 2) != 0) _moves[1].PPUp = 2;
            if ((value & 4) != 0) _moves[2].PPUp = 2;
            if ((value & 8) != 0) _moves[3].PPUp = 2;
        }
        [DataMember]
        private int[] moveIds
        {
            get
            {
                var r = new int[_moves.Count];
                var i = 0;
                foreach (var m in _moves) r[i++] = m.Move.Id;
                return r;
            }
            set
            {
                _moves = new ObservableCollection<LearnedMove>();
                foreach (var m in value) _moves.Add(new LearnedMove(RomData.GetMove(m)));
            }
        }
        [DataMember(EmitDefaultValue = false)]
        private byte moveIds_PP2
        {
            get { return GetMoveIds_PPx(2); }
            set { SetMoveIds_PPx(2, value); }
        }
        [DataMember(EmitDefaultValue = false)]
        private byte moveIds_PP1
        {
            get { return GetMoveIds_PPx(1); }
            set { SetMoveIds_PPx(1, value); }
        }
        [DataMember(EmitDefaultValue = false)]
        private byte moveIds_PP0
        {
            get { return GetMoveIds_PPx(0); }
            set { SetMoveIds_PPx(0, value); }
        }

        private ObservableCollection<LearnedMove> _moves;
        public IEnumerable<LearnedMove> Moves
        { get { return _moves; } }
        #endregion

        private bool CheckSpForm()
        {
            switch (number)
            {
                case Ps.ARCEUS:
                    form = _item / 1000 == Is.FLAME_PLATE / 1000 ? _item - Is.FLAME_PLATE + 1 : 0;
                    break;
                case Ps.Silvally:
                    form = _item / 1000 == Is.Fire_Memory / 1000 ? _item - Is.Fire_Memory + 1 : 0;
                    break;
                case Ps.GIRATINA:
                    form = _item == Is.GRISEOUS_ORB ? 1 : 0;
                    break;
                case Ps.GENESECT:
                    form = _item / 100 == Is.DOUSE_DRIVE / 100 ? _item - Is.DOUSE_DRIVE + 1 : 0;
                    break;
                case Ps.KELDEO:
                    if (!HasMove(Ms.SECRET_SWORD)) form = 0;
                    break;
                case Ps.MEOWSTIC:
                    form = _gender == PokemonGender.Female ? 1 : 0;
                    break;
            }
            if (_form != null && _form.Index != form) _form = null;
            return _form == null;
        }

        public bool HasMove(int moveId)
        {
            return _moves.Any((m) => m.Move.Id == moveId);
        }

        public bool AddMove(MoveType move)
        {
            if (_moves.Count < 4 && !HasMove(move.Id))
            {
                _moves.Add(new LearnedMove(move));
                if (number == Ps.KELDEO && move.Id == Ms.SECRET_SWORD) OnPropertyChanged("CanChooseForm");
                return true;
            }
            return false;
        }

        public bool RemoveMove(MoveType move)
        {
            foreach (var m in _moves)
                if (m.Move == move)
                {
                    _moves.Remove(m);
                    if (CheckSpForm()) OnPropertyChanged();
                    if (number == Ps.KELDEO && move.Id == Ms.SECRET_SWORD) OnPropertyChanged("CanChooseForm");
                    return true;
                }
            return false;
        }

        #region ICloneable
        public PokemonData Clone()
        {
            var clone = MemberwiseClone() as PokemonData;
            clone._propertyChanged = null;
            clone._iv = new Observable6D(Iv);
            clone._ev = new Observable6D(Ev);
            clone.got = false;
            clone._moves = new ObservableCollection<LearnedMove>();
            foreach (var m in Moves) clone._moves.Add(new LearnedMove(m.Move, m.PPUp));
            return clone;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion

        private List<int> learnset;

        private void GetLearnset()
        {
            int formx = form;
            learnset = new List<int>();

            if (!(number == Ps.WORMADAM || number == Ps.ROTOM || number == Ps.KYUREM || number == Ps.FLOETTE && formx == 5 || number == Ps.MEOWSTIC || number == Ps.Lycanroc || GameHelper.Alola(number))) formx = 0;
            GetGenericLearnset(number, formx);
            switch (number)
            {
                case Ps.SMEARGLE:
                    for (var m = 1; m <= RomData.Moves.Count(); ++m)
                        if (m != Ms.STRUGGLE && m != Ms.CHATTER) GetLearnVM(m);
                    break;
                case Ps.ROTOM:
                    if (formx != 0) GetLearnVM(formx == 1 ? Ms.OVERHEAT : formx == 2 ? Ms.HYDRO_PUMP : formx == 3 ? Ms.BLIZZARD : formx == 4 ? Ms.AIR_SLASH : Ms.LEAF_STORM);
                    break;
                case Ps.SHAYMIN:
                    GetGenericLearnset(Ps.SHAYMIN, 1 - formx);
                    break;
                default:
                    if (!(number == Ps.FLOETTE && formx == 5))
                        foreach (var e1 in RomData.Evolutions)
                            if (e1.To == number)
                            {
                                int x = 0;
                                if (GameHelper.Alola(number)) x = formx;
                                GetGenericLearnset(e1.From, x);
                                foreach (var e2 in RomData.Evolutions)
                                    if (e2.To == e1.From)
                                    {
                                        GetGenericLearnset(e2.From, x);
                                        break;
                                    }
                                break;
                            }
                    break;
            }
        }
        private void GetGenericLearnset(int number, int formx)
        {
            if (number != 0)
            {
                foreach (var gen in RomData.LearnSet.Index)
                    foreach (var game in gen.Games)
                    {
                        var l = game.Lv.Get(number, formx);
                        if (l != null)
                        {
                            foreach (var pair in l) GetLearnVM(pair.Key);
                            if (game.Tutor != null) foreach (var tutor in game.Tutor.Get(number, formx)) GetLearnVM(tutor);
                            if (game.TM != null) foreach (var tm in game.TM.Get(number, formx)) GetLearnVM(tm);
                            if (game.HM != null) foreach (var hm in game.HM.Get(number, formx)) GetLearnVM(hm);
                            if (game.Egg != null) foreach (var egg in game.Egg.Get(number)) GetLearnVM(egg);
                            if (game.E1 != null) foreach (var e in game.E1.Get(number, formx)) GetLearnVM(e);
                            if (game.E2 != null) foreach (var e in game.E2.Get(number, formx)) GetLearnVM(e);
                        }
                    }
            }
        }
        private void GetLearnVM(int move)
        {
            if (!learnset.Contains(move))
                learnset.Add(move);
        }
    }
}
