using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PokemonBattleOnline.PBO.Elements
{
    public static class Cartes
    {
        public static readonly DataTemplate Avatar;
        public static readonly DataTemplate User;
        public static readonly DataTemplate UserR;
        public static readonly SolidColorBrush[] Brushes;

        static Cartes()
        {
            var rd = Helper.GetDictionary("Elements/Cartes", "User");
            Avatar = (DataTemplate)rd["Avatar"];
            User = (DataTemplate)rd["User"];
            UserR = (DataTemplate)rd["UserR"];
            Brushes = new SolidColorBrush[9];
            Brushes[0] = (SolidColorBrush)rd["Red"];
            Brushes[1] = (SolidColorBrush)rd["Blue"];
            Brushes[2] = (SolidColorBrush)rd["Yellow"];
            Brushes[3] = (SolidColorBrush)rd["Green"];
            Brushes[4] = (SolidColorBrush)rd["Black"];
            Brushes[5] = (SolidColorBrush)rd["Brown"];
            Brushes[6] = (SolidColorBrush)rd["Purple"];
            Brushes[7] = (SolidColorBrush)rd["Gray"];
            Brushes[8] = (SolidColorBrush)rd["Pink"];
        }

        public static SolidColorBrush GetChatBrush(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            var flag = username;
            if (username[0] == '[')
            {
                var r = username.IndexOf(']', 1);
                if (r != -1) flag = username.Substring(1, r - 1);
            }
            var i = flag.GetHashCode() % 9;
            if (i < 0) i += 9;
            return Brushes[i];
        }
    }
}
