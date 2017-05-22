using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game
{
  public interface IGameSettings
  {
    GameMode Mode { get; }
    Terrain Terrain { get; }
    bool SleepRule { get; }
    }
}
