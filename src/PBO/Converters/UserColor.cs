using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.PBO.Converters
{
    class UserColor : Converter<string>
    {
        public static readonly UserColor C = new UserColor();

        protected override object Convert(string value)
        {
            return Elements.Cartes.GetChatBrush(value);
        }
    }
}
