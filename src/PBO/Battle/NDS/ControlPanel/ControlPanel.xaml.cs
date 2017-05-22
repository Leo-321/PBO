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
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : Canvas
    {
        ControlPanelVM vm;

        public ControlPanel()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Current.Content = null;
        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            if (controlPanel.SelectedIndex == ControlPanelVM.TARGET) controlPanel.SelectedIndex = ControlPanelVM.FIGHT;
            else
            {
                vm.Mega = false;
                vm.Zmove = false;
                controlPanel.SelectedIndex = ControlPanelVM.MAIN;
                Current.Content = null;
            }
        }
        private void Fight_Click(object sender, RoutedEventArgs e)
        {
            vm.Fight_Click();
        }
        private void Pokemons_Click(object sender, RoutedEventArgs e)
        {
            controlPanel.SelectedIndex = ControlPanelVM.POKEMONS;
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            controlPanel.SelectedIndex = ControlPanelVM.RUN;
        }
        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (sender != zmove0 && sender != zmove1 && sender != zmove2 && sender != zmove3)
                vm.Move_Click((SimMove)((GameButton)sender).Content);
        }
        private void Target_IsPressedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!vm.TargetPanel.CanSelect)
            {
                var v = (bool)e.NewValue;
                if (tO1.IsHitTestVisible) tO1.SimPressed = v;
                if (tO0.IsHitTestVisible) tO0.SimPressed = v;
                if (t0.IsHitTestVisible) t0.SimPressed = v;
                if (t1.IsHitTestVisible) t1.SimPressed = v;
            }
        }
        private void Target_Click(object sender, RoutedEventArgs e)
        {
            vm.Target_Click((TargetVM)((GameButton)sender).Content);
        }
        private void Pokemon_Click(object sender, RoutedEventArgs e)
        {
            SimPokemon pm = (SimPokemon)((GameButton)sender).Content;
            if (pm == Current.Content)
            {
                vm.Pokemon_Click(pm);
                Current.Content = null;
            }
            else Current.Content = pm;
        }
        private void GiveUp_Click(object sender, RoutedEventArgs e)
        {
            vm.GiveUp_Click();
        }
        internal void Init(ControlPanelVM cp)
        {
            Current.Content = null;
            DataContext = vm = cp;
            if (cp == null)
            {
                controlPanel.SelectedIndex = ControlPanelVM.INACTIVE;
                Time.Visibility = Visibility.Collapsed;
            }
            else
            {
                Time.Visibility = Visibility.Visible;
                pl.Style = (Style)pl.Resources[vm.Partner == 1 ? "PL" : "L"];
                pr.Style = (Style)pr.Resources[vm.Partner == 2 ? "PR" : "R"];
            }
        }

        private void Zmove_Click(object sender, RoutedEventArgs e)
        {
            if (vm.Zmove)
                zmove();
            else
                move();
        }

        private void zmove0_Click(object sender, RoutedEventArgs e)
        {
            vm.Move_Click((SimMove)(move0.Content));
            move();

        }

        private void zmove1_Click(object sender, RoutedEventArgs e)
        {
            vm.Move_Click((SimMove)(move1.Content));
            move();
        }

        private void zmove2_Click(object sender, RoutedEventArgs e)
        {
            vm.Move_Click((SimMove)(move2.Content));
            move();
        }

        private void zmove3_Click(object sender, RoutedEventArgs e)
        {
            vm.Move_Click((SimMove)(move3.Content));
            move();
        }
        private void zmove()
        {
            move0.Visibility = Visibility.Collapsed;
            move1.Visibility = Visibility.Collapsed;
            move2.Visibility = Visibility.Collapsed;
            move3.Visibility = Visibility.Collapsed;
            zmove0.Visibility = Visibility.Visible;
            zmove1.Visibility = Visibility.Visible;
            zmove2.Visibility = Visibility.Visible;
            zmove3.Visibility = Visibility.Visible;
        }
        private void move()
        {
            move0.Visibility = Visibility.Visible;
            move1.Visibility = Visibility.Visible;
            move2.Visibility = Visibility.Visible;
            move3.Visibility = Visibility.Visible;
            zmove0.Visibility = Visibility.Collapsed;
            zmove1.Visibility = Visibility.Collapsed;
            zmove2.Visibility = Visibility.Collapsed;
            zmove3.Visibility = Visibility.Collapsed;
        }
    }
}
