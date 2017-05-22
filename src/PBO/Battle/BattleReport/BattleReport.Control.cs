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
using System.Threading;
using System.IO;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Battle
{
    /// <summary>
    /// Interaction logic for BattleReport.xaml
    /// </summary>
    public partial class BattleReport : UserControl
    {
        private class Control
        {
            private readonly BattleReport Nest;
            private readonly DocumentBattleReport RealTime;
            private readonly DocumentBattleReport Full;
            private readonly StringBuilder TextReport;

            public Control(BattleReport battlereport)
            {
                Nest = battlereport;
                beginTurn = true;
                RealTime = new DocumentBattleReport(Nest.Battling);
                Full = new DocumentBattleReport(Nest.Full);
                TextReport = new StringBuilder();
            }

            bool beginTurn;
            public void TurnEnd()
            {
                beginTurn = true;
            }
            public void GameLogAppended(string text, LogStyle style)
            {
                if (!style.HasFlag(LogStyle.HiddenAfterBattle))
                {
                    if (style.HasFlag(LogStyle.NoBr)) TextReport.Append(text);
                    else TextReport.AppendLine(text);
                }
                UIDispatcher.Invoke(() =>
                  {
                      var align = style.HasFlag(LogStyle.Center) ? TextAlignment.Center : style.HasFlag(LogStyle.EndTurn) ? TextAlignment.Right : TextAlignment.Left;
                      var color = SBrushes.GetBrush(style);
                      var bold = style.HasFlag(LogStyle.Bold);
                      if (!style.HasFlag(LogStyle.NoBr)) text = text.LineBreak();
                      if (!style.HasFlag(LogStyle.HiddenInBattle)) RealTime.AddText(text, color, align, bold);
                      if (!style.HasFlag(LogStyle.HiddenAfterBattle)) Full.AddText(text, color, align, bold);
                      Nest.AutoScroll();
                      if (beginTurn)
                      {
                  //Nest.nowTurn = new LinkedListNode<TextElement>(currentRealTime.Inlines.Last());
                  //Nest.turnsBookmark.AddLast(Nest.nowTurn);
                  beginTurn = false;
                      }
                  });
            }

            public void AddText(string text, Brush foreground, TextAlignment alignment = TextAlignment.Left, bool bold = false)
            {
                RealTime.AddText(text, foreground, alignment, bold);
                Full.AddText(text, foreground, alignment, bold);
            }

            public void Save(string title, string player)
            {
                try
                {
                    var r =
#if DEBUG
        "..\\"
#else
        string.Empty
#endif
        ;
                    var path = r + "MyPBO\\Logs\\" + player;
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    using (StreamWriter sw = new System.IO.StreamWriter(path + string.Format("\\[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd-HHmmss"), title) + ".txt", false, Encoding.Unicode))
                        sw.Write(TextReport);
                }
                catch
                {
                    ShowMessageBox.SaveLogFail();
                }
            }
        }
    }
}