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
using System.Windows.Shapes;
using System.Windows.Threading;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO.Lobby
{
    /// <summary>
    /// Interaction logic for GlanceLobbies.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            PBOClient.LoginFailed_Name += () => LoginFailed(R.LOGINFAILED_NAME);
            PBOClient.LoginFailed_Version += (sv) =>
            {
                if (sv < PBOMarks.VERSION) LoginFailed(R.LOGINFAILED_VERSION_OLDSERVER);
                else LoginFailed(R.LOGINFAILED_VERSION_OLDCLIENT);
            };
            PBOClient.LoginFailed_Full += () => LoginFailed(R.LOGINFAILED_FULL);
            PBOClient.LoginFailed_Disconnect += () => LoginFailed("连接到服务器失败。");
            PBOClient.CurrentChanged += () =>
              {
                  if (PBOClient.Current != null)
                  {
                      var server = servers.Text;
                      var ss = Config.Current.Servers;
                      if (ss.FirstOrDefault() != server)
                      {
                          ss.Remove(server);
                          var n = ss.Count;
                          if (n > 30) ss.RemoveRange(30, n - 30);
                          ss.Insert(0, server);
                          servers.Items.Refresh();
                      }
                  }
              };
            var av = Config.Current.Avatar;
            if (av < 651 || av >= 868 || av == 790 || av == 856 || av == 857 || av == 858)
            {
                av = new Random().Next(651, 868);
                if (av == 790 || av == 856 || av == 857 || av == 858) av = 821;
            }
            avatar.Content = av;
            var na = Config.Current.Name;
            if (na != null) name.Text = na;
            servers.ItemsSource = Config.Current.Servers;
            servers.SelectedIndex = 0;
        }

        private void LoginFailed(string message)
        {
            MessageBox.Show(message);
            IsEnabled = true;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(servers.Text) && !string.IsNullOrWhiteSpace(name.Text))
            {
                var na = name.Text.Trim();
                var av = (int)avatar.Content;
                PBOClient.Login(servers.Text.Trim(), na, (ushort)av);
                Config.Current.Name = na;
                Config.Current.Avatar = av;
                IsEnabled = false;
            }
        }

        private void avs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (avs.SelectedItem != null)
            {
                avs.Visibility = System.Windows.Visibility.Collapsed;
                login.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void avatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            login.Visibility = System.Windows.Visibility.Collapsed;
            avs.Visibility = System.Windows.Visibility.Visible;
            avs.SelectedItem = null;
        }
    }
}
