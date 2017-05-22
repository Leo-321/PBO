using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline
{
  /// <summary>
  /// this should not be used as a dispatcher
  /// </summary>
  public static class UIDispatcher
  {
    private static System.Windows.Threading.Dispatcher wpf;

    public static void Init(System.Windows.Threading.Dispatcher dispatcher)
    {
      wpf = dispatcher;
    }

    public static void Invoke(Action action)
    {
#if DEBUG
      try
      {
#endif
        if (action != null)
        {
          if (wpf == null || wpf.CheckAccess()) action();
          else wpf.Invoke(action);
        }
#if DEBUG
      }
      catch
      {
        System.Diagnostics.Debugger.Break();
      }
#endif
    }
    public static void Invoke(Delegate method, params object[] args)
    {
#if DEBUG
      try
      {
#endif
        if (method != null)
        {
          if (wpf == null || wpf.CheckAccess()) method.DynamicInvoke(args);
          else wpf.Invoke(method, args);
        }
#if DEBUG
      }
      catch
      {
        System.Diagnostics.Debugger.Break();
      }
#endif
    }
    public static void BeginInvoke(Delegate method, params object[] args)
    {
#if DEBUG
      try
      {
#endif
        if (method != null)
        {
          if (wpf == null) method.DynamicInvoke(args);
          else wpf.BeginInvoke(method, args);
        }
#if DEBUG
      }
      catch
      {
        System.Diagnostics.Debugger.Break();
      }
#endif
    }
  }
}
