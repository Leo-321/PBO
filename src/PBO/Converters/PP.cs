using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Converters
{
  public class PPColor : Converter<double>
  {
    public static readonly PPColor C;
    
    private static readonly SolidColorBrush WHITE;
    private static readonly SolidColorBrush YELLOW; //50%
    private static readonly SolidColorBrush ORANGE; //25%
    private static readonly SolidColorBrush RED;
    
    static PPColor()
    {
      WHITE = SBrushes.NewBrush(0xfff8f8f8);
      YELLOW = SBrushes.NewBrush(0xfff8d000);
      ORANGE = SBrushes.NewBrush(0xfff87000);
      RED = SBrushes.NewBrush(0xfff80848);
      C = new PPColor();
    }

    protected override object Convert(double value)
    {
      Brush b;
      if (value == 0) b = RED;
      else if (value <= 0.25) b = ORANGE;
      else if (value <= 0.5) b = YELLOW;
      else b = WHITE;
      return b;
    }
  }
  public class PPShadow : Converter<double>
  {
    public static readonly PPShadow C;
    
    private static readonly SolidColorBrush WHITE;
    private static readonly SolidColorBrush YELLOW; //50%
    private static readonly SolidColorBrush ORANGE; //25%
    private static readonly SolidColorBrush RED;

    static PPShadow()
    {
      WHITE = SBrushes.NewBrush(0xff707070);
      YELLOW = SBrushes.NewBrush(0xff786000);
      ORANGE = SBrushes.NewBrush(0xff703800);
      RED = SBrushes.NewBrush(0xff780830);
      C = new PPShadow();
    }

    protected override object Convert(double value)
    {
      Brush b;
      if (value == 0) b = RED;
      else if (value <= 0.25) b = ORANGE;
      else if (value <= 0.5) b = YELLOW;
      else b = WHITE;
      return b;
    }
  }
}
