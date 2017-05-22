using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    /// <summary>
    /// thread unsafe, do not access properties or methods concurrently
    /// </summary>
    public class InitingGame
    {
        private readonly IPokemonData[,][] pokemons;
        public readonly int Id;

        public InitingGame(int id, IGameSettings settings)
        {
            Id = id;
            _settings = settings;
            pokemons = new IPokemonData[2, settings.Mode.PlayersPerTeam()][];
        }

        private readonly IGameSettings _settings;
        public IGameSettings Settings
        { get { return _settings; } }
        public bool CanComplete
        {
            get
            {
                foreach (var p in pokemons)
                    if (p == null) return false;
                return true;
            }
        }

        public GameContext Complete()
        {
            if (CanComplete)
            {
                var game = new GameContext(Id, Settings, pokemons);
                return game;
            }
            return null;
        }
        private bool CheckPokemon(IPokemonData pokemon)
        {
            return PokemonValidator.Validate(pokemon); //TODO: more
        }
        private bool CheckPokemons(IPokemonData[] pokemons)
        {
            return pokemons != null && pokemons.Any() && pokemons.Length <= 6 && pokemons.All(CheckPokemon); //TODO: more
        }
        public bool Prepare(int teamId, int teamIndex, IPokemonData[] pms)
        {
            if (
              CheckPokemons(pms) &&
              (teamId == 0 || teamId == 1) &&
              0 <= teamIndex && teamIndex < Settings.Mode.PlayersPerTeam()
               )
            {
                pokemons[teamId, teamIndex] = pms;
                return true;
            }
            return false;
        }
        public void UnPrepare(int teamId, int teamIndex)
        {
            pokemons[teamId, teamIndex] = null;
        }
        public IPokemonData[] GetPokemons(int teamId, int teamIndex)
        {
            return pokemons[teamId, teamIndex];
        }
    }
}
