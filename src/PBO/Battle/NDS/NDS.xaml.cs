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
using System.Collections.ObjectModel;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO.Battle
{
    /// <summary>
    /// Interaction logic for Simulation.xaml
    /// </summary>
    public partial class NDS : UserControl
    {
        public NDS()
        {
            InitializeComponent();
            subtitle.VisibilityChanged += (v) => opms.Visibility = v == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        internal void Init(RoomController userController)
        {
            if (userController != null)
            {
                var game = userController.Game;
                int observerTeamId;
                if (userController.PlayerController == null)
                {
                    cp.Init(null);
                    subtitle.Init(null);
                    observerTeamId = 0;
                }
                else
                {
                    var controlPanel = new ControlPanelVM(userController);
                    cp.Init(controlPanel);
                    subtitle.Init(controlPanel);
                    observerTeamId = userController.PlayerController.Player.Team;
                }
                opms.DataContext = game.Board.Pokemons[observerTeamId];
                rpms.DataContext = game.Board.Pokemons[1 - observerTeamId];
                board.Init(game.Board, observerTeamId);
                Visibility = Visibility.Visible;
            }
            else
            {
                cp.Init(null);
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
