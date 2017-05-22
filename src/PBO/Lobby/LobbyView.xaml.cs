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
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO.Lobby
{
    /// <summary>
    /// Interaction logic for LobbyView.xaml
    /// </summary>
    public partial class LobbyView : UserControl
    {
        private ClientController Controller;

        public LobbyView()
        {
            InitializeComponent();
        }

        internal void Init(ClientController controller)
        {
            Controller = controller;
            Rooms.ItemsSource = controller.Rooms;
            Users.ItemsSource = controller.Users;
            chat.Init(controller);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth > 500)
            {
                Grid.SetRowSpan(upper, 2);
                Grid.SetColumnSpan(upper, 1);
                Grid.SetRow(chat, 0);
                Grid.SetRowSpan(chat, 2);
                Grid.SetColumn(chat, 1);
                Grid.SetColumnSpan(chat, 1);
                Grid.SetColumnSpan(split, 1);
                Grid.SetRowSpan(split, 2);
                upper.Margin = new Thickness(0, 0, 3, 0);
                split.Cursor = Cursors.SizeWE;
                split.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                split.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            }
            else
            {
                Grid.SetRowSpan(upper, 1);
                Grid.SetColumnSpan(upper, 2);
                Grid.SetRow(chat, 1);
                Grid.SetRowSpan(chat, 1);
                Grid.SetColumn(chat, 0);
                Grid.SetColumnSpan(chat, 2);
                Grid.SetColumnSpan(split, 2);
                Grid.SetRowSpan(split, 1);
                upper.Margin = new Thickness(0, 0, 0, 3);
                split.Cursor = Cursors.SizeNS;
                split.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                split.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            }
        }

        private void SingleBattle_Click(object sender, RoutedEventArgs e)
        {
            Controller.NewRoom(new GameSettings(Game.GameMode.Single));
        }

        private void MultiBattle_Click(object sender, RoutedEventArgs e)
        {
            Controller.NewRoom(new GameSettings(Game.GameMode.Multi));
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("真的要退出么？", "PBO", MessageBoxButton.YesNo) == MessageBoxResult.Yes) PBOClient.Exit();
        }

        private void Random4p_Click(object sender, RoutedEventArgs e)
        {
            Controller.NewRoom(new GameSettings(Game.GameMode.Random4p));
        }

        private void RandomSingle_Click(object sender, RoutedEventArgs e)
        {
            Controller.NewRoom(new GameSettings(Game.GameMode.RandomSingle));
        }
    }
}
