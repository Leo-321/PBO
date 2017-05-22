using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host
{
    internal class Pokemon
    {
        public readonly Controller Controller;
        public readonly int Id;
        public readonly Player Owner;
        public readonly int TeamId;

        public int MaxHp;   //z神变身
        public readonly int AbilityIndex;
        public readonly I6D Iv;
        public readonly I6D Ev;

        public readonly string Name;
        public readonly PokemonGender Gender;
        public readonly int Lv;
        public readonly Move[] Moves;
        public readonly int Happiness;
        public readonly PokemonNature Nature;

        public bool Shiny;

        internal Pokemon(Controller controller, int id, Player owner, IPokemonData custom)
        {
            Controller = controller;
            Id = id;
            Owner = owner;
            TeamId = owner.TeamId;

            Form = custom.Form;
            Name = custom.Name;
            Happiness = custom.Happiness;
            Gender = custom.Gender;
            Lv = custom.Lv;
            Nature = custom.Nature;
            AbilityIndex = custom.AbilityIndex;
            Moves = new Move[custom.Moves.Count()];
            int i = 0;
            foreach (var m in custom.Moves) Moves[i++] = new Move(m.Move, m.PP);
            _item = custom.Item;
            Iv = new ReadOnly6D(custom.Iv);
            Ev = new ReadOnly6D(custom.Ev);
            _hp = MaxHp = GetMaxHp;
        }
        public int GetMaxHp
        { get { return GameHelper.GetHp(Form.Data.Base.Hp, (byte)Iv.Hp, (byte)Ev.Hp, (byte)Lv); } }

        public PokemonForm Form; //shaymi
        private int _item;
        public int Item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    Controller.ReportBuilder.SetItem(this);
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
                    if (value != PokemonState.Faint) Controller.ReportBuilder.SetState(this);
                }
            }
        }
        public int SLPTurn;
        public int UsedItem;
        public int UsedBerry;

        private bool _mega;
        public bool Mega
        {
            get { return _mega; }
            set
            {
                _mega = value;
                Owner.Mega = true;
            }
        }

        //gen7
        private bool _zmove;
        public bool Zmove
        {
            get { return _zmove; }
            set
            {
                _zmove = value;
                Owner.Zmove = true;
            }
        }

        public int IndexInOwner
        { get { return Owner.GetPokemonIndex(Id); } }
        /// <summary>
        /// for PokemonProxy only
        /// </summary>
        public int _hp;
        public int Hp
        {
            get { return _hp; }
            set
            {
                if (value < 0) value = 0;
                else if (value > MaxHp) value = MaxHp;
                if (_hp != value)
                {
                    _hp = value;
                    Controller.ReportBuilder.SetHp(this);
                }
            }
        }

        private int Get5D(StatType type)
        {
            return GameHelper.Get5D(type, Nature, Form.Data.Base.GetStat(type), Iv.GetStat(type), Ev.GetStat(type), Lv);
        }

        /// <summary>
        /// no battle report
        /// </summary>
        public void SetHp(int value)
        {
            if (value < 0) value = 0;
            else if (value > MaxHp) value = MaxHp;
            _hp = value;
            Controller.ReportBuilder.SetHp(this);
        }
    }
}
