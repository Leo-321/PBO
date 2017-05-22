using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PokemonBattleOnline.Game
{
    public class BoardOutward
    {
        public event Action<int, int> PokemonSentOut;

        public readonly ReadOnlyObservableCollection<PokemonOutward>[] Pokemons;
        public readonly Terrain Terrain;
        public readonly IGameSettings Settings;

        private readonly ObservableCollection<PokemonOutward>[] pokemons;

        internal BoardOutward(IGameSettings settings)
        {
            Settings = settings;
            Players = new PlayerOutward[2, Settings.Mode.PlayersPerTeam()];
            pokemons = new ObservableCollection<PokemonOutward>[2];
            Pokemons = new ReadOnlyObservableCollection<PokemonOutward>[2];
            _weather = Weather.Normal;
            Terrain = settings.Terrain;

            var empty = new PokemonOutward[settings.Mode.XBound()];
            for (int i = 0; i < 2; i++)
            {
                pokemons[i] = new ObservableCollection<PokemonOutward>(empty);
                Pokemons[i] = new ReadOnlyObservableCollection<PokemonOutward>(pokemons[i]);
            }
        }

        public PokemonOutward this[int team, int x]
        {
            get { return pokemons[team][x]; }
            set
            {
                //不一定是PmSendOut
                var old = this[team, x];
                if ((old == null && value == null) || ((old != null && value != null) && (old.Id == value.Id))) return;
                pokemons[team][x] = value;
            }
        }
        private Weather _weather;
        public Weather Weather
        {
            get { return _weather; }
            internal set
            {
                _weather = value;
            }
        }
        public PlayerOutward[,] Players
        { get; private set; }

        internal void OnPokemonSentOut(int team, int x)
        {
#if TEST
      if (PokemonSentOut != null)
#endif
            PokemonSentOut(team, x);
        }
    }
}
