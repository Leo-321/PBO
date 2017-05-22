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
    /// Interaction logic for Room.xaml
    /// </summary>
    public partial class RoomView : UserControl
    {
        public RoomView()
        {
            InitializeComponent();
        }

        private void SetSeat(Seat seat)
        {
            PBOClient.Current.EnterRoom((Room)DataContext, seat);
        }
        private void P00_Click(object sender, RoutedEventArgs e)
        {
            SetSeat(Seat.Player00);
        }
        private void P10_Click(object sender, RoutedEventArgs e)
        {
            SetSeat(Seat.Player10);
        }
        private void P01_Click(object sender, RoutedEventArgs e)
        {
            SetSeat(Seat.Player01);
        }
        private void P11_Click(object sender, RoutedEventArgs e)
        {
            SetSeat(Seat.Player11);
        }
        private void Watch_Click(object sender, RoutedEventArgs e)
        {
            SetSeat(Seat.Spectator);
        }
    }
}
