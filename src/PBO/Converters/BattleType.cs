using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Converters
{
  public class BattleTypeBg : Converter<BattleType>
  {
    public readonly static BattleTypeBg C = new BattleTypeBg();
    static readonly SolidColorBrush[] c;
    static BattleTypeBg()
    {
      c = new SolidColorBrush[RomData.BATTLETYPES];
      c[0] = SBrushes.NewBrush(0xffdedecf);//normal
      c[1] = SBrushes.NewBrush(0xffde6a52);//fight
      c[2] = SBrushes.NewBrush(0xff7ebbff);//flying
      c[3] = SBrushes.NewBrush(0xffd06abb);//poison
      c[4] = SBrushes.NewBrush(0xfff4dc69);//ground
      c[5] = SBrushes.NewBrush(0xffdccd7e);//rock
      c[6] = SBrushes.NewBrush(0xffccdc2c);//bug
      c[7] = SBrushes.NewBrush(0xff7f7fdb);//ghost
      c[8] = SBrushes.NewBrush(0xffcdcddb);//steel
      c[9] = SBrushes.NewBrush(0xffff552d);//fire
      c[10] = SBrushes.NewBrush(0xff3fbbff);//water
      c[11] = SBrushes.NewBrush(0xff94ea6c);//grass
      c[12] = SBrushes.NewBrush(0xfff8c030);//electric
      c[13] = SBrushes.NewBrush(0xffff6abb);//psychic
      c[14] = SBrushes.NewBrush(0xff93f5ff);//ice
      c[15] = SBrushes.NewBrush(0xff947efd);//dragon
      c[16] = SBrushes.NewBrush(0xff946952);//dark
      c[17] = SBrushes.NewBrush(0xffffcfff);//fairy
    }

    protected override object Convert(BattleType value)
    {
      return value == BattleType.Invalid ? null : c[(int)(byte)value - 1];
    }
  }
  public class BattleTypeBorder : Converter<BattleType>
  {
    public static readonly BattleTypeBorder C = new BattleTypeBorder();
    static readonly SolidColorBrush[] c;
    static BattleTypeBorder()
    {
      c = new SolidColorBrush[RomData.BATTLETYPES];
      c[0] = SBrushes.NewBrush(0x80555553);//normal
      c[1] = SBrushes.NewBrush(0x80562c2d);//fight
      c[2] = SBrushes.NewBrush(0x802a3f7e);//flying
      c[3] = SBrushes.NewBrush(0x80552b40);//poison
      c[4] = SBrushes.NewBrush(0x8054542c);//ground
      c[5] = SBrushes.NewBrush(0x80543f2c);//rock
      c[6] = SBrushes.NewBrush(0x80545400);//bug
      c[7] = SBrushes.NewBrush(0x802b2b53);//ghost
      c[8] = SBrushes.NewBrush(0x803f3f52);//steel
      c[9] = SBrushes.NewBrush(0x807f1500);//fire
      c[10] = SBrushes.NewBrush(0x802a3f69);//water
      c[11] = SBrushes.NewBrush(0x8040552d);//grass
      c[12] = SBrushes.NewBrush(0x80695413);//electric
      c[13] = SBrushes.NewBrush(0x806b2c41);//psychic
      c[14] = SBrushes.NewBrush(0x803f5469);//ice
      c[15] = SBrushes.NewBrush(0x803f2a69);//dragon
      c[16] = SBrushes.NewBrush(0x802a2a13);//dark
      c[17] = SBrushes.NewBrush(0x807f556c);//fairy
    }

    protected override object Convert(BattleType value)
    {
      return value == BattleType.Invalid ? null : c[(int)(byte)value - 1];
    }
  }
  public class BattleTypeMoveButton : Converter<BattleType>
  {
    public static readonly BattleTypeMoveButton C = new BattleTypeMoveButton();
    static readonly ImageSource[] c;
    static BattleTypeMoveButton()
    {
      c = new ImageSource[RomData.BATTLETYPES + 1];
      c[1] = Helper.GetImage(@"ControlPanel/Fight/Normal.png");
      c[2] = Helper.GetImage(@"ControlPanel/Fight/Fighting.png");
      c[3] = Helper.GetImage(@"ControlPanel/Fight/Flying.png");
      c[4] = Helper.GetImage(@"ControlPanel/Fight/Poison.png");
      c[5] = Helper.GetImage(@"ControlPanel/Fight/Ground.png");
      c[6] = Helper.GetImage(@"ControlPanel/Fight/Rock.png");
      c[7] = Helper.GetImage(@"ControlPanel/Fight/Bug.png");
      c[8] = Helper.GetImage(@"ControlPanel/Fight/Ghost.png");
      c[9] = Helper.GetImage(@"ControlPanel/Fight/Steel.png");
      c[10] = Helper.GetImage(@"ControlPanel/Fight/Fire.png");
      c[11] = Helper.GetImage(@"ControlPanel/Fight/Water.png");
      c[12] = Helper.GetImage(@"ControlPanel/Fight/Grass.png");
      c[13] = Helper.GetImage(@"ControlPanel/Fight/Electric.png");
      c[14] = Helper.GetImage(@"ControlPanel/Fight/Psychic.png");
      c[15] = Helper.GetImage(@"ControlPanel/Fight/Ice.png");
      c[16] = Helper.GetImage(@"ControlPanel/Fight/Dragon.png");
      c[17] = Helper.GetImage(@"ControlPanel/Fight/Dark.png");
      c[18] = Helper.GetImage(@"ControlPanel/Fight/Fairy.png");
    }

    protected override object Convert(BattleType value)
    {
      return c[(int)value];
    }
  }
}
