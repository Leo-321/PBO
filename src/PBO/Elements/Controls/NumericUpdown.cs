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
using System.Windows.Controls.Primitives;

namespace PokemonBattleOnline.PBO.Elements
{
  [TemplatePart(Name = PART_IncreaseButton, Type = typeof(ButtonBase))]
  [TemplatePart(Name = PART_DecreaseButton, Type = typeof(ButtonBase))]
  [TemplatePart(Name = PART_TextBox,Type = typeof(TextBox))]
  public class NumericUpdown : RangeBase
  {
    const string PART_IncreaseButton = "PART_IncreaseButton";
    const string PART_DecreaseButton = "PART_DecreaseButton";
    const string PART_TextBox = "PART_TextBox";
    static NumericUpdown()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpdown), new FrameworkPropertyMetadata(typeof(NumericUpdown)));
    }

    TextBox textBox;
    LimitedValueRule validationRule;

    public NumericUpdown()
    {
      validationRule = new LimitedValueRule(Minimum, Maximum);
    }

    #region Median
    static readonly DependencyProperty MidValueProperty = DependencyProperty.Register("Median", typeof(double), typeof(NumericUpdown));
    public double Median
    { 
      get { return (double)GetValue(MidValueProperty); }
      set { SetValue(MidValueProperty, value); }
    }
    #endregion
    
    private void SetNearestValue(double value)
    {
      sbyte t = 60;
      while ((Value > value ? Value - value : value - Value) >= 2)
      {
        value = (Value + value) / 2;
        Value = value;
        if (--t < 0) break;
      }
      if (Value < value) Value += 1;
      else if (Value > value) Value -= 1;
    }
    protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
    {
      base.OnMinimumChanged(oldMinimum, newMinimum);
      validationRule.Min = newMinimum;
    }
    protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
    {
      base.OnMaximumChanged(oldMaximum, newMaximum);
      validationRule.Max = newMaximum;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      var b = GetTemplateChild(PART_IncreaseButton) as Button;
      if (b != null) b.Click += (sender, e) => Value = Value < Median ? Median : Maximum;
      b = GetTemplateChild(PART_DecreaseButton) as Button;
      if (b != null) b.Click += (sender, e) => Value = Value > Median ? Median : Minimum;

      textBox = GetTemplateChild(PART_TextBox) as TextBox;
      if (textBox != null)
      {
        Binding text = new Binding("Value");
        text.RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent);
        text.Mode = BindingMode.TwoWay;
        text.ValidationRules.Add(validationRule);
        text.NotifyOnValidationError = true;
        textBox.SetBinding(TextBox.TextProperty, text);
        Validation.AddErrorHandler(textBox, (sender, e) =>
          {
            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            Application.Current.Dispatcher.BeginInvoke(new Action(binding.UpdateTarget));
          });
      }//if (textBox != null)
    }
  }
}
