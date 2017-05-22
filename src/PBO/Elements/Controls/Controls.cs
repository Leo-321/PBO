using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PokemonBattleOnline.PBO.Elements
{
  static class Controls
  {
    public static readonly Style XButton;
    public static readonly ItemsPanelTemplate WrapPanel;
    public static readonly ItemsPanelTemplate VerticalWrapPanel;
    public static readonly Style STextBlock;

    static Controls()
    {
      ResourceDictionary rd = Helper.GetDictionary("Elements/Controls", "Controls");
      WrapPanel = (ItemsPanelTemplate)rd["WrapPanelTemplate"];
      VerticalWrapPanel = (ItemsPanelTemplate)rd["VerticalWrapPanelTemplate"];
      XButton = (Style)rd["XButton"];
      STextBlock = (Style)rd["STextBlock"];
    }
  }
}
