using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PokemonBattleOnline.PBO.Elements
{
  public class LimitedValueRule : ValidationRule
  {
    static readonly ValidationResult VALID = new ValidationResult(true, null);
    static readonly ValidationResult INVALID = new ValidationResult(false, null);

    public LimitedValueRule(double min, double max)
    {
      Min = min;
      Max = max;
    }

    public double Max { get; set; }
    public double Min { get; set; }
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      try
      {
        double d = double.Parse((string)value);
        if (d >= Min && d <= Max) return VALID;
        return INVALID;
      }
      catch
      {
        return INVALID;
      }
    }
  }
}
