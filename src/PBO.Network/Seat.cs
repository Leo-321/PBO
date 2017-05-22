using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Network
{
  public enum Seat
  {
    Player00,
    Player01,
    Player10,
    Player11,
    Spectator
  }
  public static class SeatExtensions
  {
    public static int TeamId(this Seat seat)
    {
      return (int)seat >> 1;
    }
    public static int TeamIndex(this Seat seat)
    {
      return (int)seat % 2;
    }
  }
}
