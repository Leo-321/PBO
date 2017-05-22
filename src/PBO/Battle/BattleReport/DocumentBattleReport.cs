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
  internal class DocumentBattleReport
  {
    private readonly FlowDocument Document;

    public DocumentBattleReport(FlowDocument doc)
    {
      Document = doc;
    }

    private Paragraph current;
    private TextAlignment lastAlignment = TextAlignment.Left;
    public void AddText(string text, Brush foreground, TextAlignment alignment = TextAlignment.Left, bool bold = false)
    {
      if (current == null || alignment != lastAlignment)
      {
        lastAlignment = alignment;
        if (current != null)
        {
          var run = current.Inlines.LastOrDefault() as Run;
          if (run != null) run.Text = run.Text.TrimEnd();
        }
        current = new Paragraph() { TextAlignment = alignment };
        Document.Blocks.Add(current);
      }
      current.Inlines.Add(new Run(text)
      {
        Foreground = foreground,
        FontWeight = bold ? FontWeights.Bold : FontWeights.Normal,
      });
    }
  }
}
