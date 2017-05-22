using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Network
{
    [DataContract(Namespace = PBOMarks.JSON)]
    public class Room : ObservableObject
    {
        private User[] players;

        public Room(int id, GameSettings settings)
        {
            Id = id;
            _settings = settings;
            players = new User[4];
            _spectators = new ObservableList<User>();
        }

        [DataMember(Name = "a")]
        public int Id
        { get; private set; }

        [DataMember(Name = "b_")]
        private readonly GameSettings _settings;
        public GameSettings Settings
        { get { return _settings; } }

        private static PropertyChangedEventArgs BATTLING = new PropertyChangedEventArgs("Battling");
        [DataMember(Name = "c", EmitDefaultValue = false)]
        private bool _battling;
        public bool Battling
        {
            get { return _battling; }
            set
            {
                if (_battling != value)
                {
                    _battling = value;
                    OnPropertyChanged(BATTLING);
                }
            }
        }

        private User this[int index]
        {
            get
            { return players.ValueOrDefault(index); }
            set
            {
                if (0 <= index && index < 4)
                {
                    if (value == null) players[index].Room = null; //离开房间
                    else
                    {
                        if (value.Room != this) value.Room = this; //进入房间
                        else if (value.Seat == Seat.Spectator) _spectators.Remove(value); //本房间内换座位
                        else players[(int)value.Seat] = null;
                        value.Seat = (Seat)index;
                    }
                    players[index] = value;
                    OnPropertyChanged();
                }
            }
        }
        public User this[Seat seat]
        {
            get { return this[(int)seat]; }
            set { this[(int)seat] = value; }
        }
        public User this[int teamIndex, int playerIndex]
        {
            get { return players.ValueOrDefault(teamIndex * 2 + playerIndex); }
            set { this[teamIndex * 2 + playerIndex] = value; }
        }

        private ObservableList<User> _spectators;
        public IEnumerable<User> Spectators
        { get { return _spectators; } }
        public IEnumerable<User> Players
        { get { return players.Where((p) => p != null); } }

        public bool IsValidSeat(Seat Seat)
        {
            return Settings.Mode.PlayersPerTeam() == 2 || Seat != Seat.Player01 && Seat != Seat.Player11;
        }

        public void AddSpectator(User user)
        {
            if (user.Room == this) players[(int)user.Seat] = null;//房间内换座位
            else user.Room = this;
            user.Seat = Seat.Spectator;
            _spectators.Add(user);
        }
        public void RemoveSpectator(User user)
        {
            if (_spectators.Remove(user)) user.Room = null;
        }

        #region for serialization
        private void SetPij(int i, int j, User value)
        {
            if (players == null)
            {
                players = new User[4];
                if (_spectators == null) _spectators = new ObservableList<User>();
            }
            this[i * 2 + j] = value;
        }
        [DataMember(EmitDefaultValue = false, Order = 1)]
        private User p00
        {
            get { return this[0, 0]; }
            set { SetPij(0, 0, value); }
        }
        [DataMember(EmitDefaultValue = false, Order = 2)]
        private User p01
        {
            get { return this[0, 1]; }
            set { SetPij(0, 1, value); }
        }
        [DataMember(EmitDefaultValue = false, Order = 3)]
        private User p10
        {
            get { return this[1, 0]; }
            set { SetPij(1, 0, value); }
        }
        [DataMember(EmitDefaultValue = false, Order = 4)]
        private User p11
        {
            get { return this[1, 1]; }
            set { SetPij(1, 1, value); }
        }
        [DataMember(EmitDefaultValue = false, Order = 0)]
        private User[] ss
        {
            get { return _spectators.Any() ? _spectators.ToArray() : null; }
            set
            {
                if (value != null)
                {
                    foreach (var u in value)
                    {
                        u.Room = this;
                        u.Seat = Seat.Spectator;
                    }
                    _spectators = new ObservableList<User>(value);
                }
            }
        }
        #endregion

        public void RemoveUsers()
        {
            for (int i = 0; i < 4; ++i)
                if (players[i] != null)
                {
                    players[i].Room = null;
                    players[i] = null;
                }
            foreach (var u in _spectators) u.Room = null;
            _spectators.Clear();
        }
    }
}
