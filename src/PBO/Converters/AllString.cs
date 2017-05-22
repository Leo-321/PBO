using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.PBO.Converters
{
  class AllString : Converter<object>
  {
    public static readonly AllString C = new AllString();
    
    protected override object Convert(object value)
    {
      return value.ToString();
    }
  }
}
