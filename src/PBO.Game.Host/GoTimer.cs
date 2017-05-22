using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokemonBattleOnline.Game.Host
{
  internal class GoTimer : IDisposable
  {
    public event Action<int[,]> TimeUp;
    public event Action<bool[,]> WaitingNotify;
    private readonly Timer timer;
    private readonly IEnumerable<Player> Players;
    private readonly object Locker;

    internal GoTimer(IEnumerable<Player> players)
    {
      Locker = new object();
      timer = new Timer(TimeTick, null, Timeout.Infinite, 1000);
      Players = players;
    }

    public void Start()
    {
      timer.Change(0, 1000);
    }

    private int require;
    private int done;
    private int reminderCount;
    public void Resume(IEnumerable<Player> players)
    {
      lock (Locker)
      {
        require = players.Count();
        done = 0;
        reminderCount = require == 1 ? 30 : 0;
        foreach (var p in players) p.Timing = true;
      }
    }
    public void Pause(Player player)
    {
      lock (Locker)
      {
        if (player.Timing)
        {
          player.Timing = false;
          done++;
          reminderCount = require == done ? 0 : 30;
        }
      }
    }
    public void NewTurn()
    {
      lock (Locker)
      {
        foreach (Player p in Players) p.SpentTime -= 30;
      }
    }
    private void TimeTick(object state)
    {
      lock (Locker)
      {
        bool timeup = false;
        foreach (Player p in Players)
          if (p.Timing)
          {
            p.SpentTime++;
            timeup |= p.SpentTime > PBOMarks.GAMETIMEUP;
          }
        if (timeup)
        {
          timer.Change(Timeout.Infinite, 0);
          var tu = new int[2, 2];
          foreach (var p in Players) tu[p.TeamId, p.TeamIndex] = p.SpentTime;
          TimeUp(tu);
        }
        else if (reminderCount > 0)
        {
          reminderCount--;
          var r = new bool[2, 2];
          foreach (var p in Players) r[p.TeamId, p.TeamIndex] = p.Timing;
          if (reminderCount == 0) WaitingNotify(r);
        }
      }
    }

    public void Dispose()
    {
      timer.Dispose();
    }
  }
}
