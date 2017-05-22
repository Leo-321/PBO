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

namespace PokemonBattleOnline.PBO.Editor
{
  /// <summary>
  /// Interaction logic for Team.xaml
  /// </summary>
  public partial class TeamView : UserControl
  {
    public TeamView()
    {
      InitializeComponent();
    }

    private TeamVM ViewModel;
  
    protected override void OnDragEnter(DragEventArgs e)
    {
      base.OnDragEnter(e);
    }
    protected override void OnDragLeave(DragEventArgs e)
    {
      base.OnDragLeave(e);
    }
    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
    }
    protected override void OnDrop(DragEventArgs e)
    {
      base.OnDrop(e);
    }
    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      BattleCheckBox.Visibility = System.Windows.Visibility.Visible;
      Remove.Visibility = System.Windows.Visibility.Visible;
    }
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      BattleCheckBox.Visibility = System.Windows.Visibility.Collapsed;
      Remove.Visibility = System.Windows.Visibility.Collapsed;
    }

    private void NameBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      BeginEdit();
      e.Handled = true;
    }
    private void NameEditor_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter) EndEdit();
    }
    private void NameEditor_LostFocus(object sender, RoutedEventArgs e)
    {
      EndEdit();
    }

    private void BeginEdit()
    {
      NameEditor.Text = ViewModel.Name;
      NameBlock.Visibility = Visibility.Collapsed;
      NameEditor.Visibility = Visibility.Visible;
      NameEditor.Focus();
    }
    private void EndEdit()
    {
      ViewModel.Name = NameEditor.Text;
      NameEditor.Visibility = System.Windows.Visibility.Collapsed;
      NameBlock.Visibility = System.Windows.Visibility.Visible;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      ViewModel = DataContext as TeamVM;
      if (TeamVM.New == ViewModel && ViewModel != null) BeginEdit();
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
      var editing = EditorVM.Current.EditingPokemon.Origin != null && EditorVM.Current.EditingPokemon.Origin.Container == ViewModel;
      if (MessageBox.Show(editing ? "这个队伍里的精灵正在编辑，确实要删除么？" : "删除队伍？", "PBO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        if (editing) EditorVM.Current.EditingPokemon.Origin = null;
        if (ViewModel.CanBattle) EditorVM.Current.BattleTeams.Remove(ViewModel);
        EditorVM.Current.Teams.Remove(ViewModel);
      }
    }
  }
}
