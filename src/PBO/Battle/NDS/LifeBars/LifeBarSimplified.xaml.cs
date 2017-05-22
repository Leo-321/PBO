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
using System.Windows.Media.Animation;
using System.ComponentModel;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Battle
{
  /// <summary>
  /// Interaction logic for LifeBar.xaml
  /// </summary>
  public partial class LifeBarSimplified : Canvas
  {
    public static double GetWidth(int hp, int maxHp)
    {
      int x2 = hp * 64 / maxHp;
      return x2 == 0 && hp != 0 ? 1 : x2;
    }
    private static void LifeBarSimplified_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      ((LifeBarSimplified)sender).OnDataContextChanged(e);
    }

    private readonly Brush Green;
    private readonly Brush Yellow;
    private readonly Brush Red;
    
    public LifeBarSimplified()
    {
      InitializeComponent();
      DataContextChanged += LifeBarSimplified_DataContextChanged;
      Green = (Brush)Resources["Green"];
      Yellow = (Brush)Resources["Yellow"];
      Red = (Brush)Resources["Red"];
    }

    private void HpChanged(object sender, PropertyChangedEventArgs e)
    {
      var hp = ((PairValue)sender).Value;
      var max = ((PairValue)sender).Origin;
      Bar.Width = GetWidth(hp, max);
      Bar.Fill = hp * 5 <= max ? Red : (hp << 1) <= max ? Yellow : Green;
    }

    private void OnDataContextChanged(DependencyPropertyChangedEventArgs e)
    {
      if (e.OldValue is PairValue) ((PairValue)e.OldValue).PropertyChanged -= HpChanged;
      if (e.NewValue is PairValue)
      {
        ((PairValue)e.NewValue).PropertyChanged += HpChanged;
        HpChanged(e.NewValue, null);
      }
    }
  }
}
