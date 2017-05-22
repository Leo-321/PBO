using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
  public struct Modifier
  {
    public static Modifier operator*(Modifier x, Modifier y)
    {
      return x * y.n;
    }
    public static Modifier operator *(Modifier x, UInt16 y)
    {
      return new Modifier((UInt16)((x.n * y + 0x800) >> 12));
    }
    public static int operator *(int baseNum, Modifier modifier)
    {
      return (baseNum * modifier.n + 0x800) >> 12;
    }
    public static bool operator ==(Modifier x, Modifier y)
    {
      return x.n == y.n;
    }
    public static bool operator !=(Modifier x, Modifier y)
    {
      return x.n != y.n;
    }
    public static bool operator ==(Modifier x, UInt16 y)
    {
      return x.n == y;
    }
    public static bool operator !=(Modifier x, UInt16 y)
    {
      return x.n != y;
    }
    public static implicit operator Modifier(UInt16 code)
    {
      return new Modifier(code);
    }
    public static explicit operator Modifier(int code)
    {
      return new Modifier((UInt16)code);
    }

    UInt16 n;

    private Modifier(UInt16 code)
    {
      n = code;
    }

    public override bool Equals(object obj)
    {
      bool r;
      if (obj is Modifier) r = this == (Modifier)obj;
      else if (obj is UInt16) r = n == (UInt16)obj;
      else r = base.Equals(obj);
      return r;
    }
    public override int GetHashCode()
    {
      return n.GetHashCode();
    }
  }
}
