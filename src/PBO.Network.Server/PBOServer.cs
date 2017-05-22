using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Network
{
  /// <summary>
  /// 提供静态全局访问
  /// </summary>
  public static class PBOServer
  {
    private static readonly object Locker;

    static PBOServer()
    {
      Locker = new object();
    }

    private static Server _current;
    public static Server Current
    {
      get
      {
        lock (Locker)
        {
          return _current;
        }
      }
      set
      {
        lock (Locker)
        {
          _current = value;
        }
      }
    }
    
    public static void NewServer(int port = PBOMarks.DEFAULT_PORT)
    {
      Current = new Server(port);
      Current.Start();
    }
  }
}
