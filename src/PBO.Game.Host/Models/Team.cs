using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal class Team
    {
        public readonly int Id;
        private readonly IGameSettings Settings;

        public Team(int id, Player[] players, IGameSettings settings)
        {
            Id = id;
            _players = players;
            Settings = settings;
        }

        private readonly Player[] _players;
        public IEnumerable<Player> Players
        { get { return _players; } }

        public Player GetPlayer(int index)
        {
            return _players.ValueOrDefault(index);
        }
    }
}
