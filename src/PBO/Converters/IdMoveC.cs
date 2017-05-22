using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO.Editor
{
    class IdMoveC : Converter<int>
    {
      public static readonly IdMoveC C = new IdMoveC();

      protected override object Convert(int value)
      {
        return RomData.GetMove(value);
      }
    }
}
