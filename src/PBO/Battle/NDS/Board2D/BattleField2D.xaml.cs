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
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO.Battle
{
    /// <summary>
    /// Interaction logic for BattleField2D.xaml
    /// </summary>
    public partial class BattleField2D : UserControl
    {
        BoardOutward Board;
        int ObserveTeam;

        public BattleField2D()
        {
            InitializeComponent();
            P0.Back = P1.Back = true;
        }

        internal void Init(BoardOutward board, int observeTeam)
        {
            Board = board;
            ObserveTeam = observeTeam;
            Player0.ItemsSource = board.Players[observeTeam, 0].Balls;
            PlayerO0.ItemsSource = board.Players[1 - observeTeam, 0].Balls;
            P0.SetPokemon(board[observeTeam, 0]);
            PO0.SetPokemon(board[1 - observeTeam, 0]);
            if (Board.Settings.Mode.PlayersPerTeam() == 2)
            {
                Player1.ItemsSource = board.Players[observeTeam, 1].Balls;
                PlayerO1.ItemsSource = board.Players[1 - observeTeam, 1].Balls;
                P1.SetPokemon(board[observeTeam, 1]);
                PO1.SetPokemon(board[1 - observeTeam, 1]);
            }
            else
            {
                Player1.ItemsSource = PlayerO1.ItemsSource = null;
                P1.SetPokemon(null);
                PO1.SetPokemon(null);
            }
            board.PokemonSentOut += OnPokemonSentOut;

            if (Board.Settings.Mode.PlayersPerTeam() != 2)
            {
                p4p.Visibility = System.Windows.Visibility.Collapsed;
                p4p2.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                p4p.Visibility = System.Windows.Visibility.Visible;
                p4p2.Visibility = System.Windows.Visibility.Visible;
            }
        }
        void OnPokemonSentOut(int team, int x)
        {
            var c = team == ObserveTeam ? x == 0 ? P0 : P1 : x == 0 ? PO0 : PO1;
            c.SendOut(Board[team, x]);
        }
    }
}
