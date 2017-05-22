using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PokemonBattleOnline.Network;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO
{
  /// <summary>
  /// Interaction logic for LobbyWindow.xaml
  /// </summary>
  public partial class LobbyPanel : UserControl
  {
    public LobbyPanel()
    {
      InitializeComponent();
      gridbg.Fill = PBO.Elements.SBrushes.GetGridTileBrush(16, SBrushes.NewBrush(0x20ffffff));
      PBOClient.Disconnected += () => MessageBox.Show("连接到服务器中断。");
      PBOClient.CurrentChanged += PBOClient_CurrentChanged;
      PBOClient_CurrentChanged();
    }

    private void PBOClient_CurrentChanged()
    {
      //already in lock
      if (PBOClient.Current == null)
      {
        lobby.IsEnabled = false;
        lobby.Visibility = Visibility.Collapsed;
        login.Visibility = Visibility.Visible;
        login.IsEnabled = true;
      }
      else
      {
        login.Visibility = Visibility.Collapsed;
        lobby.Init(PBOClient.Current);
        lobby.IsEnabled = true;
        lobby.Visibility = Visibility.Visible;
      }
    }
    
    public void SetGridBg(double x, double y)
    {
      gridbg.Margin = new Thickness(x, y, 0, 0);
    }

    public bool Window_Closing()
    {
      if (PBOClient.Current != null)
        if (ShowMessageBox.ExitLobby() == MessageBoxResult.Yes) PBOClient.Exit();
        else return true;
      return false;
    }
  }
}
