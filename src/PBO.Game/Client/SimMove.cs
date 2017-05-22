using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game
{
  public class SimMove
  {
    public MoveType Type
    { get; private set; }
    public PairValue PP
    { get; private set; }

    internal SimMove(Move move)
    {
      Type = move.Type;
      PP = (PairValue)move.PP;
    }
    internal SimMove(MoveType type)
    {
      Type = type;
      PP = new PairValue(5);
    }
  }
}
