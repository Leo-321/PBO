using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO.Battle
{
    class Pokemon2D : Canvas, IPokemonOutwardEvents
    {
        private readonly Image Image;
        private readonly DoubleAnimation FaintAnimation;
        private readonly DoubleAnimation BeginChangeImageAnimation;
        private readonly DoubleAnimation EndChangeImageAnimation;
        private readonly BlurEffect ImageEffect;
        private PokemonOutward Pokemon;

        public Pokemon2D()
        {
            IsHitTestVisible = false;
            UseLayoutRounding = true;
            Image = new Image() { Stretch = Stretch.UniformToFill };
            Image.SnapsToDevicePixels = true;
            Image.Effect = ImageEffect = new BlurEffect() { Radius = 0, KernelType = KernelType.Box };
            Children.Add(Image);
            FaintAnimation = new DoubleAnimation(0, 0, Duration.Automatic);
            FaintAnimation.Completed += (sender, e) =>
              {
                  Image.Source = null;
                  Pokemon.RemoveListener(this);
                  Pokemon = null;
                  Image.BeginAnimation(Image.HeightProperty, null);
              };
            BeginChangeImageAnimation = new DoubleAnimation(0, 15, new Duration(TimeSpan.FromSeconds(0.5)));
            EndChangeImageAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(0.5)));
            BeginChangeImageAnimation.Completed += (sender, e) =>
              {
                  RefreshImage();
                  ImageEffect.BeginAnimation(BlurEffect.RadiusProperty, EndChangeImageAnimation);
              };
        }

        public bool Back
        { get; set; }

        private void RefreshImage()
        {
            var s = Pokemon.IsSubstitute ? Back ? ImageService.GetSpBack("substitute") : ImageService.GetSpFront("substitute") : Back ? ImageService.GetPokemonBack(Pokemon.Form, Pokemon.Gender, Pokemon.Shiny) : ImageService.GetPokemonFront(Pokemon.Form, Pokemon.Gender, Pokemon.Shiny);
            if (s == null) s = ImageService.GetPokemonIcon(Pokemon.Form, Pokemon.Gender);
            Image.Source = s;
            if (s != null)
            {
                double scale;
                int n = Pokemon.Form.Species.Number;
                bool gen7 = n >= 722 && n <= 802 || GameHelper.Alola(n) && Pokemon.Form.Index == 1;
                var o = s.PixelHeight <= 96 || s.PixelHeight == 240 && s.PixelWidth < 320; //gen5 小图 非满下屏的图源为原始尺寸
                if (Back && o)
                {
                    scale = 2;
                    Image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.NearestNeighbor);
                }
                else if (!Back && !o && !gen7)
                {
                    scale = 0.5;
                    Image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Fant);
                }
                else scale = 1;
                Image.SetValue(Canvas.LeftProperty, -s.PixelWidth * scale / 2);
                Image.SetValue(Canvas.BottomProperty, -s.PixelHeight * scale / 2);
                Image.Width = s.PixelWidth * scale;
                Image.Height = s.PixelHeight * scale;
                FaintAnimation.From = Image.Height;
            }
            BeginChangeImageAnimation.To = Back ? 30 : 15;
        }
        public void SendOut(PokemonOutward pm)
        {
            Pokemon = pm;
            if (Pokemon != null)
            {
                Image.Visibility = System.Windows.Visibility.Visible;
                Pokemon.AddListener(this);
                RefreshImage();
            }
        }
        void IPokemonOutwardEvents.Faint()
        {
            Image.BeginAnimation(Image.HeightProperty, FaintAnimation);
        }
        void IPokemonOutwardEvents.Hurt()
        {
        }
        void IPokemonOutwardEvents.PositionChanged()
        {
            Image.Visibility = Pokemon.Position.Y == CoordY.Plate ? Visibility.Visible : Visibility.Collapsed;
        }
        void IPokemonOutwardEvents.SubstituteAppear()
        {
            RefreshImage();
        }
        void IPokemonOutwardEvents.SubstituteDisappear()
        {
            RefreshImage();
        }
        void IPokemonOutwardEvents.ImageChanged()
        {
            ImageEffect.BeginAnimation(BlurEffect.RadiusProperty, BeginChangeImageAnimation);
        }
        void IPokemonOutwardEvents.Withdrawn()
        {
            Image.Source = null;
            if (Pokemon != null)
            {
                Pokemon.RemoveListener(this);
                Pokemon = null;
            }
        }
        public void SetPokemon(PokemonOutward pokemon)
        {
            if (Pokemon != pokemon)
            {
                if (Pokemon != null) Pokemon.RemoveListener(this);
                Pokemon = pokemon;
                if (Pokemon == null) Image.Source = null;
                else
                {
                    Image.Visibility = Pokemon.Position.Y == CoordY.Plate ? Visibility.Visible : Visibility.Collapsed;
                    Pokemon.AddListener(this);
                    RefreshImage();
                }
            }
        }
    }
}
