using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO.Battle
{
    internal class TargetVM : ObservableObject
    {
        public readonly int Team;
        public readonly int X;

        public TargetVM(int team, int x)
        {
            Team = team;
            X = x;
        }

        public object Pokemon
        { get; set; }
        public bool IsEnabled
        { get; set; }

        public new void OnPropertyChanged()
        {
            base.OnPropertyChanged();
        }
    }
    
    internal class TargetPanelVM : ObservableObject
    {
        public TargetPanelVM(int userTeam)
        {
            PO1 = new TargetVM(1 - userTeam, 1);
            PO0 = new TargetVM(1 - userTeam, 0);
            P0 = new TargetVM(userTeam, 0);
            P1 = new TargetVM(userTeam, 1);
        }

        public bool CanSelect
        { get; private set; }
        public TargetVM PO1
        { get; private set; }
        public TargetVM PO0
        { get; private set; }
        public TargetVM P0
        { get; private set; }
        public TargetVM P1
        { get; private set; }
        public Visibility V0
        { get { return P0.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility V1
        { get { return P1.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility VO0
        { get { return PO0.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility VO1
        { get { return PO1.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility V01
        { get { return !CanSelect && P0.IsEnabled && P1.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility V0O1
        { get { return !CanSelect && P0.IsEnabled && PO1.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility V1O0
        { get { return !CanSelect && P1.IsEnabled && PO0.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility VO1O0
        { get { return !CanSelect && PO1.IsEnabled && PO0.IsEnabled ? Visibility.Visible : Visibility.Collapsed; } }

        public void Set(int userX, MoveRange range)
        {
            CanSelect = false;
            //如果要支持三打对战，IsRemote要拿到客户端
            PO1.IsEnabled = false;
            PO0.IsEnabled = false;
            P0.IsEnabled = false;
            P1.IsEnabled = false;
            var user = userX == 0 ? P0 : P1;
            var partner = user == P0 ? P1 : P0;
            switch(range)
            {
                case MoveRange.SelectedTarget:
                    CanSelect = true;
                    PO1.IsEnabled = true;
                    PO0.IsEnabled = true;
                    partner.IsEnabled = true;
                    break;
                case MoveRange.SelectedTeammate:
                    CanSelect = true;
                    partner.IsEnabled = true;
                    break;
                case MoveRange.SelectedOpponent:
                    CanSelect = true;
                    PO1.IsEnabled = true;
                    PO0.IsEnabled = true;
                    break;
                case MoveRange.Adjacent:
                    PO1.IsEnabled = true;
                    PO0.IsEnabled = true;
                    partner.IsEnabled = true;
                    break;
                case MoveRange.OpponentPokemons:
                case MoveRange.OpponentField:
                    PO1.IsEnabled = true;
                    PO0.IsEnabled = true;
                    break;
                case MoveRange.TeamPokemons:
                case MoveRange.TeamField:
                    P0.IsEnabled = true;
                    P1.IsEnabled = true;
                    break;
                case MoveRange.All:
                case MoveRange.Board:
                    PO1.IsEnabled = true;
                    PO0.IsEnabled = true;
                    P0.IsEnabled = true;
                    P1.IsEnabled = true;
                    break;
                default:
                    user.IsEnabled = true;
                    break;
            }
            PO1.OnPropertyChanged();
            PO0.OnPropertyChanged();
            P0.OnPropertyChanged();
            P1.OnPropertyChanged();
            OnPropertyChanged();
        }
    }
}
