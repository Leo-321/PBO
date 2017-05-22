using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PokemonBattleOnline.Network
{
  public interface IPackReceivedListener
  {
    void OnPackReceived(byte[] pack);
  }
  public class NullPackReceivedListener : IPackReceivedListener
  {
    public static readonly IPackReceivedListener I = new NullPackReceivedListener();

    private NullPackReceivedListener()
    {
    }

    void IPackReceivedListener.OnPackReceived(byte[] pack)
    {
    }
  }
}
