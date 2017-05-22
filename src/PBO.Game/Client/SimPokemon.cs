using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PokemonBattleOnline.Game
{
    public class SimPokemon : ObservableObject
    {
        private static readonly PropertyChangedEventArgs STATE = new PropertyChangedEventArgs("State");
        private static readonly PropertyChangedEventArgs ITEM = new PropertyChangedEventArgs("Item");

        public readonly int Id;
        public readonly SimPlayer Owner;
        public readonly int TeamId;
        private readonly PokemonNature nature;
        private readonly int abilityIndex;
        private readonly I6D iv;
        private readonly I6D ev;

        public readonly BattleType HiddenPower;
        public readonly int Happiness;

        internal SimPokemon(int id, SimPlayer owner, IPokemonData custom)
        {
            Id = id;
            Owner = owner;
            TeamId = owner.Team;

            Gender = custom.Gender;
            Lv = custom.Lv;
            nature = custom.Nature;
            abilityIndex = custom.AbilityIndex;
            Moves = custom.Moves.Select((m) => new Move(m.Move, m.PP)).ToArray();

            Item = custom.Item;
            Happiness = custom.Happiness;

            iv = new ReadOnly6D(custom.Iv);
            ev = new ReadOnly6D(custom.Ev);
            _hp = new PairValue(GameHelper.GetHp(custom.Form.Data.Base.Hp, (byte)iv.Hp, (byte)ev.Hp, (byte)Lv));

            Form = custom.Form;
            originForm = Form;
            Name = custom.Name ?? GameString.Current.Pokemon(_form.Species.Number);

            HiddenPower = GameHelper.HiddenPower(iv);
            for (int i = 0; i < Moves.Length; ++i)
            {
                if (Moves[i].Type.Id == Ms.HIDDEN_POWER)
                    Moves[i] = new Move(new MoveType(Moves[i].Type.Id, HiddenPower, Moves[i].Type.Category, Moves[i].Type.Power, Moves[i].Type.Accuracy, Moves[i].Type.PP, Moves[i].Type.Range), Moves[i].PP.Value);
                if(Moves[i].Type.Id == Ms.NATURAL_GIFT)
                    Moves[i] = new Move(new MoveType(Moves[i].Type.Id, GameHelper.NatureGift(Item), Moves[i].Type.Category, Moves[i].Type.Power, Moves[i].Type.Accuracy, Moves[i].Type.PP, Moves[i].Type.Range), Moves[i].PP.Value);
            }
        }

        public string Name
        { get; private set; }
        private PairValue _hp;
        public IPairValue Hp
        { get { return _hp; } }
        private PokemonForm originForm; //shaymi
        private PokemonForm _form;
        public PokemonForm Form
        {
            get { return _form; }
            private set
            {
                _form = value;
                FiveD = new ReadOnly6D(0, Get5D(StatType.Atk), Get5D(StatType.Def), Get5D(StatType.SpAtk), Get5D(StatType.SpDef), Get5D(StatType.Speed));
                OnPropertyChanged();
            }
        }
        public bool Shiny
        { get; internal set; }
        public PokemonGender Gender
        { get; private set; }
        public int Lv
        { get; private set; }
        public int Ability
        { get { return Form.Data.GetAbility(abilityIndex); } }
        public Move[] Moves
        { get; private set; }
        public ReadOnly6D FiveD
        { get; private set; }

        private int _item;
        public int Item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged(ITEM);
                }
            }
        }
        private PokemonState _state;
        public PokemonState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(STATE);
                }
            }
        }

        internal int IndexInOwner
        { get { return Owner.GetPokemonIndex(Id); } }
        private int Get5D(StatType type)
        {
            return GameHelper.Get5D(type, nature, Form.Data.Base.GetStat(type), (byte)iv.GetStat(type), (byte)ev.GetStat(type), (byte)Lv);
        }
        public void SetHp(int value)
        {
            if (value < 0) value = 0;
            else if (value > Hp.Origin) value = Hp.Origin;
            _hp.Value = value;
        }

        public void ChangeForm(int form, bool forever)
        {
            Form = Form.Species.GetForm(form);
            if (forever) originForm = Form;
        }
        public void ResetForm()
        {
            Form = originForm;
        }
    }
}
