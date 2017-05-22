using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace PokemonBattleOnline.PBO.Converters
{
  public class AccuracyString : Converter<int>
  {
    public static readonly AccuracyString C = new AccuracyString();

    protected override object Convert(int value)
    {
      return value == 0 ? "-" : value.ToString();
    }
  }

  public class PowerString : Converter<int>
  {
    public static readonly PowerString C = new PowerString();

    protected override object Convert(int value)
    {
      return value == 0 || value == 1 ? "-" : value.ToString();
    }
  }
}
