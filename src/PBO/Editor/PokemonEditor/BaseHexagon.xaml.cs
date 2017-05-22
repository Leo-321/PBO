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

namespace PokemonBattleOnline.PBO.Editor
{
  /// <summary>
  /// Interaction logic for BaseHexagon.xaml
  /// </summary>
  public partial class BaseHexagon : Canvas
  {
    private readonly List<Point> Points;
    private const double SQRT3 = 1.73205;

    public BaseHexagon()
    {
      InitializeComponent();
      Points = new List<Point>(6);
    }

    private void Canvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var stats = DataContext as I6D;
      if (stats != null)
      {
        Points.Clear();
        if (stats.Hp != 1) Points.Add(new Point(0, -stats.Hp / 5));
        Points.Add(new Point(stats.Atk * SQRT3 / 10, -stats.Atk / 10));
        Points.Add(new Point(stats.Def * SQRT3 / 10, stats.Def / 10));
        Points.Add(new Point(0, stats.Speed / 5));
        Points.Add(new Point(-stats.SpDef * SQRT3 / 10, stats.SpDef / 10));
        Points.Add(new Point(-stats.SpAtk * SQRT3 / 10, -stats.SpAtk / 10));
        Hexagon.Points = new PointCollection(Points);
      }
    }
  }
}
