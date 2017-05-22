using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Network
{
  [DataContract(Name = "gs", Namespace = PBOMarks.JSON)]
  public class GameSettings : IGameSettings
  {
    private bool isLocked;

    /// <summary>
    /// HostOnly
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="terrain"></param>
    public GameSettings(GameMode mode, Terrain terrain = Terrain.Path, bool sleepRule = true)
    {
      Mode = mode;
      Terrain = terrain;
      SleepRule = sleepRule;
    }

    [DataMember(Name = "a", EmitDefaultValue = false)]
    private GameMode _mode;
    public GameMode Mode
    {
      get { return _mode; }
      set { if (!isLocked) _mode = value; }
    }
    [DataMember(Name = "b", EmitDefaultValue = false)]
    private Terrain _terrain;
    public Terrain Terrain
    {
      get { return _terrain; }
      set { if (!isLocked) _terrain = value; }
    }
    [DataMember(Name = "c", EmitDefaultValue = false)]
    private bool _noSR;
    public bool SleepRule
    {
      get { return !_noSR; }
      set { if (!isLocked) _noSR = !value; }
    }

    public string ModeName
        {
            get { return Mode.Name(); }
        }

    public void Lock()
    {
      lock (this)
      {
        isLocked = true;
      }
    }
  }
}
