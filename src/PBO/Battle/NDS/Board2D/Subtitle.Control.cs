using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.PBO.Battle
{
  public partial class Subtitle : System.Windows.Controls.Canvas
  {
    private class Control
    {
      Subtitle nest;

      public Control(Subtitle subtitle)
      {
        nest = subtitle;
      }

      public void ControlPanel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
        var cp = (ControlPanelVM)sender;
        if (e.PropertyName == null || e.PropertyName == "SelectedPanel")
          switch (cp.SelectedPanel)
          {
            case ControlPanelVM.RUN:
              nest.SetText("真的要放弃战斗么？");
              break;
            case ControlPanelVM.MAIN:
              if (cp.ControllingPokemon != null) nest.SetTextForcibly(cp.ControllingPokemon.Pokemon.Name + "要做什么？");
              break;
          }
        else if (e.PropertyName == "ControllingPokemon" && cp.SelectedPanel == ControlPanelVM.MAIN && cp.ControllingPokemon != null)
            nest.SetTextForcibly(cp.ControllingPokemon.Pokemon.Name + "要做什么？");
      }
      public void ControlPanel_InputFailed(string message)
      {
        if (message != null) nest.SetTextForcibly(message);
        else System.Windows.MessageBox.Show("InputFailed");
      }
    }
  }
}
