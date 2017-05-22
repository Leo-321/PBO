using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO.Battle
{
    class ControlPanelVM : ObservableObject
    {
        public const int INACTIVE = 0;
        public const int MAIN = 1;
        public const int FIGHT = 2;
        public const int TARGET = 3;
        public const int POKEMONS = 4;
        public const int RUN = 5;

        public event Action<string> InputFailed;

        private readonly PlayerController Controller;
        private readonly GameOutward Game;
        /// <summary>
        /// 0 非合作对战, 1 队友精灵在左列, 2 队友精灵在右列
        /// </summary>
        public readonly int Partner;
        private readonly DispatcherTimer Timer;
        private InputRequest Request;

        public ControlPanelVM(RoomController c)
        {
            Controller = c.PlayerController;
            Game = c.Game;
            //if (Game.Settings.Mode == GameMode.Multi) Partner = 2 - Controller.Player.TeamIndex;
            if (Game.Settings.Mode.NeedTarget()) _targetPanel = new TargetPanelVM(Controller.Player.Team);
            Timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            _time = 180;
            _selectedPanel = INACTIVE;
            _pokemons = new SimPokemon[6];
            Controller.RequireInput += RequireInput;
            c.Game.GameEnd += () => Timer.Stop();
            Timer.Tick += (sender, e) => Time--;
        }

        private int _time;
        public int Time
        {
            get { return _time; }
            private set
            {
                if (_time != value)
                {
                    _time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        private int _selectedPanel;
        public int SelectedPanel
        {
            get { return _selectedPanel; }
            set
            {
                if (_selectedPanel != value)
                {
                    _selectedPanel = value;
                    OnPropertyChanged("SelectedPanel");
                }
            }
        }

        public Weather Weather
        { get { return Game.Board.Weather; } }

        private bool _mega;
        public bool Mega
        {
            get { return Request != null && Request.CanMega && _mega; }
            set
            {
                if (_mega != value)
                {
                    _mega = value;
                    OnPropertyChanged("Mega");
                }
            }
        }

        public Visibility MegaVisibility
        { get { return Request != null && ZmoveVisibility!=Visibility.Visible && Request.CanMega ? Visibility.Visible : Visibility.Collapsed; } }

        private bool _zmove;
        public bool Zmove
        {
            get { return Request != null && Request.CanZmove && _zmove; }
            set
            {
                if(_zmove!=value)
                {
                    _zmove = value;
                    OnPropertyChanged("Zmove");
                }
            }
        }

        public Visibility ZmoveVisibility
        { get { return Request != null && Request.CanZmove ? Visibility.Visible : Visibility.Collapsed; } }

        public SimOnboardPokemon ControllingPokemon
        { get { return Request == null || Request.CurrentI == -1 ? null : Controller.Game.OnboardPokemons[Controller.Player.TeamIndex + Request.CurrentI]; } }

        public IEnumerable<SimPokemon> OnboardPokemons
        { get; private set; }

        public Visibility UndoVisibility
        { get { return Visibility.Collapsed; } }

        public bool IsReturnEnabled
        { get { return ControllingPokemon != null; } }

        private readonly SimPokemon[] _pokemons;
        public SimPokemon[] Pokemons
        { get { return _pokemons; } }

        private TargetPanelVM _targetPanel;
        public TargetPanelVM TargetPanel
        { get { return _targetPanel; } }

        private void RequireInput(InputRequest request)
        {
            request.playerIndex = Controller.Player.TeamIndex;
            _time = 180 - request.Time;
            Request = request;
            request.Init(Controller.Game);
            request.InputFinished += (i) =>
            {
                SelectedPanel = INACTIVE;
                Controller.Input(i);
                Timer.Stop();
            };
            if (request.IsSendOut)
            {
                System.Threading.Thread.Sleep(1000);
                _selectedPanel = POKEMONS;
            }
            else
            {
                _selectedPanel = MAIN;
                _mega = false;
                var op = new SimPokemon[Controller.Game.OnboardPokemons.Length];
                for(int i = 0; i < op.Length; ++i)
                    if (Controller.Game.OnboardPokemons[i] != null) op[i] = Controller.Game.OnboardPokemons[i].Pokemon;
                OnboardPokemons = op;
                if (_targetPanel != null)
                {
                    _targetPanel.PO1.Pokemon = Game.Board[1 - Controller.Player.Team, 1];
                    _targetPanel.PO0.Pokemon = Game.Board[1 - Controller.Player.Team, 0];
                    _targetPanel.P0.Pokemon = op[0];
                    _targetPanel.P1.Pokemon = op[1];
                }
            }
            {
                var step = Game.Settings.Mode.PlayersPerTeam();
                var i = 0;

                if (Game.Settings.Mode == GameMode.Random4p|| Game.Settings.Mode == GameMode.Multi)
                {
                    foreach (var pm in Controller.Game.Team[Controller.Player.TeamIndex].Pokemons)
                    {
                        _pokemons[i] = pm;
                        i++;
                    }
                }
                else
                {
                    foreach (var pm in Controller.Game.Team[0].Pokemons)
                    {
                        _pokemons[i] = pm;
                        i += step;
                    }
                    if (step == 2)
                    {
                        i = 1;
                        foreach (var pm in Controller.Game.Team[1].Pokemons)
                        {
                            _pokemons[i] = pm;
                            i += step;
                        }
                    }
                }

            }
            Timer.Start();
            OnPropertyChanged();
        }

        public void Pokemon_Click(SimPokemon pokemon)
        {
            if (Request.IsSendOut)
            {
                if (!Request.Pokemon(pokemon, Request.Index ?? 0)) InputFailed(Request.GetErrorMessage());
            }
            else
            {
                if (!Request.Pokemon(pokemon)) InputFailed(Request.GetErrorMessage());
            }
        }
        public void Fight_Click()
        {
            if (!Request.Fight()) SelectedPanel = FIGHT;
        }
        public void Move_Click(SimMove move)
        {
            if (move.PP.Value == 0) return;
            if (Request.Move(move, Mega, Zmove))
            {
                if (Request.NeedTarget)
                {
                    var range = Request.MoveRange;
                    if (Zmove)
                        range = GameHelper.Zmove(move.Type, ControllingPokemon.Pokemon.Item, ControllingPokemon.Pokemon.Form.Species.Number, ControllingPokemon.Pokemon.Form.Index ).Range;
                    else
                        range = Request.MoveRange;
                    TargetPanel.Set(Controller.Player.TeamIndex + Request.CurrentI, range);
                    SelectedPanel = TARGET;
                }
            }
            else InputFailed(Request.GetErrorMessage());

            if (Request.NeedTarget)
                if (Request.Pms[Request.CurrentI].IsEncore && TargetPanel.PO0.IsEnabled && TargetPanel.PO1.IsEnabled)
                {
                    if (Helper.Random.Next(0, 2) == 0)
                        Target_Click(TargetPanel.PO0);
                    else
                        Target_Click(TargetPanel.PO1);
                }
        }
        public void Target_Click(TargetVM target)
        {
            if (TargetPanel.CanSelect) Request.Target(target.Team, target.X);
            else Request.Target();
        }
        public void GiveUp_Click()
        {
            Request.GiveUp();
        }
    }
}
