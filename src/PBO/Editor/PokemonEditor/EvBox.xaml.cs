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

namespace PokemonBattleOnline.PBO.Editor
{
  public partial class EvBox : UserControl
  {
    public EvBox()
    {
      InitializeComponent();
    }

    private int Value
    {
      get { return (int)(DataContext ?? 0); }
      set { DataContext = value;}
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      Buttons.Visibility = System.Windows.Visibility.Visible;
    }
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      Buttons.Visibility = System.Windows.Visibility.Collapsed;
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var value = Value;
      TextBox.Text = value.ToString();
      Drag.Width = new GridLength(value >> 2);
    }

    string lastText;
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(TextBox.Text))
      {
        int v;
        if (!TextBox.Text.Contains(" ") && Int32.TryParse(TextBox.Text, out v))
        {
          if (v != Value) Value = v;
        }
        else
        {
          var i = TextBox.CaretIndex;
          TextBox.Text = lastText;
          TextBox.CaretIndex = i;
        }
      }
      lastText = TextBox.Text;
    }
    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox.Text = Value.ToString();
    }

    private void SetNearestValue(int value)
    {
      sbyte t = 10;
      int last = value;
      Value = last;
      while ((Value > last ? Value - last : last - Value) > 4)
      {
        last = (Value + last) / 8 * 4;
        Value = last;
        if (--t < 0) break;
      }
      if (Value < value) Value += 4;
      else if (Value > value) Value -= 4;
    }

    private void Drag_DragDelta(object sender, DragDeltaEventArgs e)
    {
      var w = Drag.Width.Value;
      if (!double.IsNaN(w))
      {
        SetNearestValue((int)w << 2);
      }
    }

    private void IncreaseButton_Click(object sender, RoutedEventArgs e)
    {
      var v0 = Value;
      var v = v0 + 4;
      if (v > 252) v = 252;
      if (v0 != v) Value = v;
    }
    private void DecreaseButton_Click(object sender, RoutedEventArgs e)
    {
      var v0 = Value;
      var v = v0 - 4;
      if (v < 0) v = 0;
      if (v0 != v) Value = v;
    }
  }
}
