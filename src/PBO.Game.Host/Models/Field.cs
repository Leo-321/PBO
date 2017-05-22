using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal class Field : ConditionalObject
    {
        public readonly int Team;
        private readonly Tile[] tiles;

        public Field(int team, IGameSettings settings)
        {
            Team = team;
            tiles = new Tile[settings.Mode.XBound()];
            for (int x = 0; x < tiles.Length; ++x)
                tiles[x] = new Tile(this, x) { WillSendOutPokemonIndex = settings.Mode.GetPokemonIndex(x) };
            _pokemons = new List<PokemonProxy>(tiles.Length);
        }

        public Tile this[int x]
        { get { return tiles.ValueOrDefault(x); } }

        public IEnumerable<Tile> Tiles
        { get { return tiles; } }

        private bool refresh;
        private List<PokemonProxy> _pokemons;
        public IEnumerable<PokemonProxy> Pokemons
        {
            get
            {
                if (refresh)
                {
                    _pokemons.Clear();
                    foreach (var t in tiles)
                        if (t.Pokemon != null) _pokemons.Add(t.Pokemon);
                    refresh = false;
                }
                return _pokemons;
            }
        }

        public void RefreshPokemons()
        {
            refresh = true;
        }

        private List<PokemonProxy> pms = new List<PokemonProxy>(3);
        public IEnumerable<PokemonProxy> GetAdjacentPokemonsByX(int x)
        {
            pms.Clear();
            foreach (var t in tiles)
                if (x - 1 <= t.X && t.X <= x + 1 && t.Pokemon != null) pms.Add(t.Pokemon);
            return pms;
        }
        public IEnumerable<PokemonProxy> GetAdjacentPokemonsByOppositeX(int x)
        {
            return GetAdjacentPokemonsByX(tiles.Length - 1 - x);
        }
    }
}
