using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public abstract class GameEvent
  {
    protected GameOutward Game
    { get; private set; }
    public int Sleep
    { get; protected set; }

    protected void AppendGameLog(string key, ValueType arg0 = null, ValueType arg1 = null, ValueType arg2 = null)
    {
      AppendGameLog(key, LogStyle.Default, arg0, arg1, arg2);
    }
    protected void AppendGameLog(string key, LogStyle style, ValueType arg0 = null, ValueType arg1 = null, ValueType arg2 = null)
    {
      Game.AppendGameLog(key, style, arg0, arg1, arg2);
    }
    protected PokemonOutward GetPokemon(int id)
    {
      return Game.GetPokemon(id);
    }
    protected SimPokemon GetPokemon(SimGame game, int id)
    {
      return game.Pokemons.ValueOrDefault(id);
    }
    protected SimOnboardPokemon GetOnboardPokemon(SimGame game, int id)
    {
      foreach (var p in game.OnboardPokemons)
        if (p != null && p.Id == id) return p;
      return null;
    }
    protected virtual void Update()
    {
    }
    public void Update(GameOutward game)
    {
      Game = game;
      Update();
    }
    public virtual void Update(SimGame game)
    {
    }
  }
}
