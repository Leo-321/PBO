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

namespace PokemonBattleOnline.PBO.Battle
{
    public class GameButton : Button
    {
        static GameButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameButton), new FrameworkPropertyMetadata(typeof(GameButton)));
        }

        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register("Shape", typeof(Geometry), typeof(GameButton));
        public Geometry Shape
        {
            get { return (Geometry)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(GameButton));
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty HorizontalFlipProperty = DependencyProperty.Register("HorizontalFlip", typeof(bool), typeof(GameButton));
        public bool HorizontalFlip
        {
            get { return (bool)GetValue(HorizontalFlipProperty); }
            set { SetValue(HorizontalFlipProperty, value); }
        }

        public static readonly DependencyPropertyKey SimPressedKey = DependencyProperty.RegisterReadOnly("SimPressed", typeof(bool), typeof(GameButton), new PropertyMetadata(false));
        private bool _SimPressed;
        public bool SimPressed
        {
            get { return (bool)GetValue(SimPressedKey.DependencyProperty); }
            set
            {
                _SimPressed = value;
                SetValue(SimPressedKey, value | IsPressed);
            }
        }

        public event DependencyPropertyChangedEventHandler IsPressedChanged;

        protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsPressedChanged(e);
            SetValue(SimPressedKey, _SimPressed | IsPressed);
            if (IsPressedChanged != null) IsPressedChanged(this, e);
        }
    }
}
