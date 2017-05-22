using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal class Board : ConditionalObject
    {
        private readonly Terrain terrain;
        public Weather Weather;

        internal Board(IGameSettings settings)
        {
            Weather = Weather.Normal;
            terrain = settings.Terrain;
            {
                _tiles = new Tile[2 * settings.Mode.XBound()];
                _fields = new Field[2];
                int j = 0;
                for (int i = 0; i < 2; i++)
                {
                    _fields[i] = new Field(i, settings);
                    foreach (var t in _fields[i].Tiles) _tiles[j++] = t;
                }
            }
            _pokemons = new List<PokemonProxy>(_tiles.Length);
        }

        private readonly Field[] _fields;
        public IEnumerable<Field> Fields
        { get { return _fields; } }

        private readonly Tile[] _tiles;
        public IEnumerable<Tile> Tiles
        { get { return _tiles; } }

        public Field this[int team]
        { get { return _fields.ValueOrDefault(team); } }

        private bool refresh;
        private List<PokemonProxy> _pokemons;
        public IEnumerable<PokemonProxy> Pokemons
        {
            get
            {
                if (refresh)
                {
                    _pokemons.Clear();
                    foreach (var t in _tiles)
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
    }
}