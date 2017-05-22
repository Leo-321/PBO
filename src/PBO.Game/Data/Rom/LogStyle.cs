using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace PokemonBattleOnline.Game
{
  [Flags]
  public enum LogStyle
  {
    Default = 0,
    NoBr = 1,
    Detail = 2,
    Bold = 4,
    Center = 8,
    EndTurn = 16,
    HiddenInBattle = 32,
    HiddenAfterBattle = 64,
    SYS = 128,
    Blue = 256
  }
}
