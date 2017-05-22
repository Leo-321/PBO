using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.IO.Packaging;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO
{
    public class ImageService : ZipData
    {
        private static ImageService Current;
        public static void Load(string path)
        {
            Current = new ImageService(path);
        }

        private static BitmapImage[] icons;

        private ImageService(string pack)
          : base(pack)
        {
            icons = new BitmapImage[RomData.Pokemons.Count() + 1];
        }

        private static BitmapImage GetImage(string path, string id)
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = Current.GetStream(string.Format("/{0}/{1}.png", path, id));
                image.EndInit();
                return image;
            }
            catch
            {
                return null;
            }
        }
        private static BitmapImage GetImage(string path, int number, int form, bool female)
        {
            var id = (number * 100 + form).ToString("00000");
            BitmapImage i = null;
            if (female) i = GetImage(path, id + "f");
            if (i == null) i = GetImage(path, id);
            return i;
        }
        private static BitmapImage GetPokemonImage(string category, PokemonForm form, PokemonGender gender, bool shiny)
        {
            var path = /*shiny ? "shiny/" : */"normal/" + category;
            return GetImage(path, form.Species.Number, form.Index, gender == PokemonGender.Female);
        }
        public static BitmapImage GetPokemonIcon(PokemonForm form, PokemonGender gender)
        {
            int n = form.Species.Number, f = form.Index;
            BitmapImage r;
            if (gender == PokemonGender.Female && (n == 521 || n == 592 || n == 593 || n == 668)) r = GetImage("icon", n.ToString("000") + ".99");
            else if (f == 0 || n == 493 || n == 649 || n == 710 || n == 711 || n==773) //arceus/genesect/pumpkaboo/gourgeist
            {
                if (icons[n] == null) icons[n] = GetImage("icon", n.ToString("000") + ".00");
                r = icons[n];
            }
            else r = GetImage("icon", string.Format("{0:000}.{1:00}", n, f));
            return r;
        }
        public static BitmapImage GetPokemonFront(PokemonForm form, PokemonGender gender, bool shiny)
        {
            return GetPokemonImage("front", form, gender, shiny);
        }
        public static BitmapImage GetPokemonBack(PokemonForm form, PokemonGender gender, bool shiny)
        {
            return GetPokemonImage("back", form, gender, shiny);
        }
        public static BitmapImage GetSpFront(string id)
        {
            return GetImage("sp/front", id);
        }
        public static BitmapImage GetSpBack(string id)
        {
            return GetImage("sp/back", id);
        }
        public static BitmapImage GetAvatar(int id)
        {
            return GetImage("avatar", id.ToString());
        }
    }
}
