using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
  internal abstract class ControllerComponent
  {
    protected readonly Controller Controller;

    protected ControllerComponent(Controller controller)
    {
      Controller = controller;
    }

    protected ReportBuilder ReportBuilder
    { get { return Controller.ReportBuilder; } }
    protected IGameSettings GameSettings
    { get { return Controller.GameSettings; } }
    protected Board Board
    { get { return Controller.Board; } }
  }
}
