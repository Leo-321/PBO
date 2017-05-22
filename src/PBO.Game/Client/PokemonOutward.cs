using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace PokemonBattleOnline.Game
{
    public interface IPokemonOutwardEvents
    {
        void Faint();
        void Hurt();
        void PositionChanged();
        void SubstituteAppear();
        void SubstituteDisappear();
        void ImageChanged(); //幻影 变身
        void Withdrawn();
    }

    [DataContract(Name = "pm", Namespace = PBOMarks.JSON)]
    public class PokemonOutward : ObservableObject
    {
        private static readonly PropertyChangedEventArgs NAME = new PropertyChangedEventArgs("Name");
        private static readonly PropertyChangedEventArgs GENDER = new PropertyChangedEventArgs("Gender");
        private static readonly PropertyChangedEventArgs STATE = new PropertyChangedEventArgs("State");

        [DataMember(Name = "a")]
        internal readonly int Id;
        [DataMember(EmitDefaultValue = false)]
        public bool Shiny;

        public PokemonOutward(int id, int teamId, int maxHp)
        {
            Id = id;
            _position = new Position(teamId, 0);
            Hp = new PairValue(maxHp);
        }
        /// <summary>
        /// will not notify property changes
        /// </summary>
        public void SetAll(string name, PokemonForm form, PokemonGender gender, int lv, Position position, bool substitute, int hp, PokemonState state, bool shiny)
        {
            _name = name;
            Form = form;
            _gender = gender;
            Lv = lv;
            _position.X = position.X;
            _position.Y = position.Y;
            IsSubstitute = substitute;
            Hp.Value = hp;
            _state = state;
            Shiny = shiny;
        }

        [DataMember(EmitDefaultValue = false)]
        private string _name;
        public string RawName
        { get { return _name; } }
        public string Name
        {
            get { return _name ?? GameString.Current.Pokemon(Form.Species.Number); }
            internal set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(NAME);
                }
            }
        }

        [DataMember(Name = "b")]
        private int number;
        [DataMember(EmitDefaultValue = false)]
        private int form;
        public PokemonForm Form
        {
            get { return RomData.GetPokemon(number, form); }
            set
            {
                number = value.Species.Number;
                form = value.Index;
            }
        }

        [DataMember(Name = "d", EmitDefaultValue = false)]
        private PokemonGender _gender;
        public PokemonGender Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged(GENDER);
                }
            }
        }

        [DataMember(Name = "e", EmitDefaultValue = false)]
        private PokemonState _state;
        public PokemonState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    var former = _state;
                    _state = value;
                    if (Owner != null) Owner.StateChanged(this);
                    OnPropertyChanged(STATE);
                }
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public bool IsSubstitute
        { get; set; }

        public PairValue Hp
        { get; private set; }

        [DataMember(Order = 0)]
        private int hpo
        {
            get { return Hp.Origin; }
            set { Hp = new PairValue(value); }
        }
        [DataMember(Order = 1)]
        private int hpv
        {
            get { return Hp.Value; }
            set { Hp.Value = value; }
        }

        public string Mega
        {
            get
            {
                if (Form.IsMega) return "Mega";
                if (Form.Index == 1)
                    if (Form.Species.Number == Ps.KYOGRE) return "Alpha";
                    else if (Form.Species.Number == Ps.GROUNDON) return "Omega";
                return null;
            }
        }

        [DataMember(Name = "l", EmitDefaultValue = false)]
        private int _lv;
        public int Lv
        {
            get { return 100 - _lv; }
            set { _lv = 100 - value; }
        }

        [DataMember]
        private readonly Position _position;
        public IPosition Position
        { get { return _position; } }

        #region Client
        private IPokemonOutwardEvents listener;
        public PlayerOutward Owner
        { get; private set; }
        internal int TeamIndex
        { get; private set; }
        internal int PokemonIndex
        { get; private set; }

        #region Events
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void Faint()
        {
            State = PokemonState.Faint;
#if TEST
            if (listener != null)
#endif
                listener.Faint();
        }
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void Hurt(int damage)
        {
            Hp.Value -= damage;
#if TEST
            if (listener != null)
#endif
                listener.Hurt();
        }
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void ChangePosition(int x, CoordY y)
        {
            if (Position.X == x && Position.Y == y) return;
            _position.X = x;
            _position.Y = y;
#if TEST
            if (listener != null)
#endif
                listener.PositionChanged();
        }
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void ShowSubstitute()
        {
#if TEST
            if (listener != null)
#endif
                listener.SubstituteAppear();
        }
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void HideSubstitute()
        {
#if TEST
            if (listener != null)
#endif
                listener.SubstituteDisappear();
        }
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void ChangeImage(int number, int form)
        {
            this.number = number;
            this.form = form;
#if TEST
            if (listener != null)
#endif
                listener.ImageChanged();
            OnPropertyChanged("Mega");
        }
        /// <summary>
        /// PokemonOutward是可以序列化的，主机端不要调用这些方法
        /// </summary>
        public void Withdraw()
        {
#if TEST
            if (listener != null)
#endif
                listener.Withdrawn();
        }
        #endregion

        public string GetProperty(string propertyName)
        {
            string r = null;
            switch (propertyName)
            {
                case "Name":
                    r = Name;
                    break;
                case "Lv":
                    r = Lv.ToString();
                    break;
                case "Form":
                    r = GameString.Current.Pokemon(number, form);
                    break;
                case "State":
                    r = GameString.Current.PokemonState(State);
                    break;
                case "Owner.Name":
                    r = Owner.Name;
                    break;
            }
            return r;
        }
        public void Init(GameOutward game)
        {
            TeamIndex = game.Settings.Mode.GetPlayerIndex(Position.X);
            PokemonIndex = game.Settings.Mode.GetPokemonIndex(Position.X);
            Owner = game.Board.Players[Position.Team, TeamIndex];
            game.Board[Position.Team, Position.X] = this;
        }
        public void AddListener(IPokemonOutwardEvents listener)
        {
#if DEBUG
      if (this.listener != null) System.Diagnostics.Debugger.Break();
#endif
            this.listener = listener;
        }
        public void RemoveListener(IPokemonOutwardEvents listener)
        {
#if DEBUG
      if (this.listener != listener) System.Diagnostics.Debugger.Break();
#endif
            this.listener = null;
        }
        #endregion
    }
}
