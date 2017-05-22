using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public class SimGame
    {
        public readonly IGameSettings Settings;
        public readonly SimPlayer Player;
        public readonly SimPlayer[] Team;
        public readonly SimOnboardPokemon[] OnboardPokemons;

        public SimGame(IGameSettings settings, SimPlayer player, SimPlayer partner)
        {
            Settings = settings;
            Player = player;
            Team = new SimPlayer[Settings.Mode.PlayersPerTeam()];
            Team[player.TeamIndex] = Player;
            if (partner != null) Team[partner.TeamIndex] = partner;
            OnboardPokemons = new SimOnboardPokemon[Settings.Mode.XBound()];
            Pokemons = new Dictionary<int, SimPokemon>();
            foreach(var p in Team)
                foreach (var pm in p.Pokemons) Pokemons.Add(pm.Id, pm);
        }

        public Dictionary<int, SimPokemon> Pokemons
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <returns>RequireInput</returns>
        public void Update(GameEvent[] events)
        {
            foreach (GameEvent e in events) e.Update(this);
        }
    }
}
