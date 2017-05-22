using System;
using System.Text;
using System.Linq;
using System.Windows;

namespace PokemonBattleOnline.Game.DataEditor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var sb = new StringBuilder();

            Clipboard.SetText(sb.ToString());
            Console.ReadKey();
        }
    }
}
