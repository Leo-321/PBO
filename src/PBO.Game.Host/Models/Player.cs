using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal class Player
    {
        public readonly int TeamId;
        public readonly int TeamIndex;

        public Player(Controller controller, int teamId, int teamIndex, IPokemonData[] pokemons)
        {
            TeamId = teamId;
            TeamIndex = teamIndex;
            var tiles = new List<Tile>();
            foreach (var t in controller.Board[teamId].Tiles)
                if (controller.GameSettings.Mode.GetPlayerIndex(t.X) == teamIndex) tiles.Add(t);
            _tiles = tiles.ToArray();
            _pokemons = new PokemonProxy[pokemons.Length];
            for (int i = 0; i < pokemons.Length; i++)
                _pokemons[i] = new PokemonProxy(new Pokemon(controller, teamId * 50 + teamIndex * 10 + i, this, pokemons[i]));
        }

        private readonly Tile[] _tiles;
        public IEnumerable<Tile> Tiles
        { get { return _tiles; } }
        private readonly PokemonProxy[] _pokemons;
        public IEnumerable<PokemonProxy> Pokemons
        { get { return _pokemons; } }
        public int PmsAlive
        {
            get
            {
                var r = 0;
                foreach (var pm in _pokemons) if (pm.Hp > 0) r++;
                return r;
            }
        }
        public bool Mega;
        public bool Zmove;
        public bool Timing;
        public int SpentTime;
        public bool GiveUp;

        public PokemonProxy GetPokemon(int pmIndex)
        {
            return _pokemons.ValueOrDefault(pmIndex);
        }
        public int GetPokemonIndex(int pmId)
        {
            for (int i = 0; i < _pokemons.Length; i++)
                if (_pokemons[i].Id == pmId) return i;
            return -1;
        }
        public void SwitchPokemon(int origin, int sendout)
        {
            if (origin >= 0 && origin < _pokemons.Length && sendout >= 0 && sendout < _pokemons.Length)
            {
                var temp = _pokemons[origin];
                _pokemons[origin] = _pokemons[sendout];
                _pokemons[sendout] = temp;
            }
        }

        public void GetOutward(BallState[] outward)
        {
            for (int i = 0; i < _pokemons.Length; ++i)
                outward[i] = _pokemons[i].Hp == 0 ? BallState.Faint : _pokemons[i].State == PokemonState.Normal ? BallState.Normal : BallState.Abnormal;
        }
    }
}
