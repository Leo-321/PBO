using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO.Editor
{
    class PokemonVM : ObservableObject
    {
        public PokemonVM(TeamVM container, int index)
        {
            _container = container;
            _index = index;
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
                Model.Ev.PropertyChanged += Model_PropertyChanged;
                ((ObservableCollection<LearnedMove>)Model.Moves).CollectionChanged += PokemonVM_CollectionChanged;
            }
        }

        void PokemonVM_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("IsRare");
        }

        private TeamVM _container;
        public TeamVM Container
        { get { return _container; } }

        public PokemonVM Actual
        {
            get
            {
                var pms = _container.Model.Pokemons;
                for (int i = _index; i >= 0; --i)
                    if (i == 0 || pms[i - 1] != null) return _container[i];
                return this;
            }
        }

        private int _index;
        public int Index
        { get { return _index; } }

        public bool IsEditing
        { get { return EditorVM.Current.EditingPokemon.Origin == this; } }

        private bool _isDragging;
        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {
                if (_isDragging != value)
                {
                    _isDragging = value;
                    OnPropertyChanged("IsDragging");
                }
            }
        }
        private int _dropState;
        public int DropState
        {
            get { return _dropState; }
            set
            {
                if (_dropState != value)
                {
                    _dropState = value;
                    OnPropertyChanged("DropState");
                }
            }
        }

        public PokemonData Model
        {
            get { return _container.Model.Pokemons[_index]; }
            set
            {
                if (value == null)//editing?
                {
                    Model.PropertyChanged -= Model_PropertyChanged;
                    if (_index == 5 || _container.Model.Pokemons[_index + 1] == null)
                    {
                        _container.Model.Pokemons[_index] = null;
                        if (_index < 5) _container[_index + 1].OnPropertyChanged("Icon");
                    }
                    else
                    {
                        _container.Model.Pokemons[_index] = _container.Model.Pokemons[_index + 1];
                        _container[_index + 1].Model = null;
                        if (EditorVM.Current.EditingPokemon != null && EditorVM.Current.EditingPokemon.Origin == _container[_index + 1]) EditorVM.Current.EditingPokemon.Origin = this;
                        Model.PropertyChanged += Model_PropertyChanged;
                    }
                    OnPropertyChanged();
                }
                else if (Actual == this)
                {
                    _container.Model.Pokemons[_index] = value;
                    if (_index < 5) _container[_index + 1].OnPropertyChanged("Icon");
                    Model.PropertyChanged += Model_PropertyChanged;
                    OnPropertyChanged();
                }
            }
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender == Model.Ev) OnPropertyChanged("IsRare");
            else if (sender == Model)
            {
                if (e.PropertyName == null || e.PropertyName == "Form" || e.PropertyName == "Gender")
                {
                    OnPropertyChanged("Icon");
                    OnPropertyChanged("RIcon");
                }
            }
        }

        public ImageSource Icon
        { get { return Model == null ? Index == 0 || Container[Index - 1].Model != null ? R.P00000 : null : ImageService.GetPokemonIcon(Model.Form, Model.Gender); } }
        public ImageSource RIcon
        { get { return Model == null ? null : ImageService.GetPokemonIcon(Model.Form, Model.Gender); } }

        public bool IsRare
        {
            get
            {
                if (Model == null) return false;
                var ev = Model.Ev;
                return Model.Form.Species.Number != 132 && Model.Moves.Count() != 4 || 508 > ev.Sum();
            }
        }

        public void Edit()
        {
            EditorVM.Current.EditingPokemon.Origin = Actual;
        }

        internal void OnIsEditingChanged()
        {
            OnPropertyChanged("IsEditing");
        }
    }
}
