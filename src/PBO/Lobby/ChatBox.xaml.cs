using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokemonBattleOnline.PBO.Lobby
{
  /// <summary>
  /// Interaction logic for ChatBox.xaml
  /// </summary>
  public partial class ChatBox : UserControl
  {
    public event Action<string> Speak;
    
    public ChatBox()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      OnSpeak();
    }
    private void Speaking_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) OnSpeak();
    }

    private void OnSpeak()
    {
      if (!string.IsNullOrEmpty(Speaking.Text))
      {
        Speak(Speaking.Text);
        Speaking.Clear();
      }
    }

    public void Clear()
    {
      Speaking.Clear();
    }
  }
}
