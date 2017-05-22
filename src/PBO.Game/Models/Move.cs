using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
  public class Move
  {
    public MoveType Type { get; private set; }
    public IPairValue PP { get; private set; }

    public Move(MoveType move, int pp)
    {
      Type = move;
      PP = new PairValue(pp);
    }
  }
}