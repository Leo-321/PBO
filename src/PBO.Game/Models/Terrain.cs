using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game
{
  public enum Terrain : byte
  {
    Path,
    Bridge,
    Cave,
    Desert,
    Grass,
    Water,
    Puddles,
    Snow,
    Ice
  }
  public static class TerrainExtension
  {
    private static BattleType[] TYPES =
    { 
      BattleType.Ground,
      BattleType.Normal,
      BattleType.Rock,
      BattleType.Ground,
      BattleType.Grass,
      BattleType.Water,
      BattleType.Ground,
      BattleType.Ice,
      BattleType.Ice
    };
    public static BattleType GetBattleType(this Terrain terrain)
    {
      return TYPES[(int)terrain];
    }
  }
}
