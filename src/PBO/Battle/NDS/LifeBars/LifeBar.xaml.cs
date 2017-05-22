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
using System.Windows.Media.Animation;
using System.Threading;
using System.ComponentModel;

namespace PokemonBattleOnline.PBO.Battle
{
    /// <summary>
    /// Interaction logic for LifeBar.xaml
    /// </summary>
    public partial class LifeBar : Canvas
    {
        private const int PERIOD = 33;
        public static double GetWidth(int hp, int maxHp)
        {
            int x2 = hp * 48 / maxHp;
            return x2 == 0 && hp != 0 ? 1 : x2;
        }
        private static void TimerCallback(object o)
        {
            ((LifeBar)o).TimerCallback();
        }

        private readonly Timer timer;
        private readonly Delegate RefreshBothDelegate;
        private readonly Delegate RefreshWidthDelegate;
        private readonly Delegate RefreshColorDelegate;
        private readonly Brush Green;
        private readonly Brush Yellow;
        private readonly Brush Red;

        public LifeBar()
        {
            InitializeComponent();
            Green = (Brush)Resources["Green"];
            Yellow = (Brush)Resources["Yellow"];
            Red = (Brush)Resources["Red"];
            timer = new Timer(TimerCallback, this, Timeout.Infinite, PERIOD);
            RefreshBothDelegate = new Action(RefreshBoth);
            RefreshWidthDelegate = new Action(RefreshWidth);
            RefreshColorDelegate = new Action(RefreshColor);
        }

        int maxHp;
        int redHp, yellowHp;
        int hp;
        int current;
        byte currentColor;
        double currentWidth;
        private void RefreshColor()
        {
            if (currentColor == 0) Background = Red;
            else if (currentColor == 1) Background = Yellow;
            else Background = Green;
        }
        private void RefreshWidth()
        {
            Width = currentWidth;
        }
        private void RefreshBoth()
        {
            RefreshColor();
            RefreshWidth();
        }
        private void CurrentChanged()
        {
            byte c;
            double w = GetWidth(current, maxHp);
            c = (byte)(current <= redHp ? 0 : current <= yellowHp ? 1 : 2);
            Delegate d;
            if (w != currentWidth)
            {
                currentWidth = w;
                if (c != currentColor)
                {
                    currentColor = c;
                    d = RefreshBothDelegate;
                }
                else d = RefreshWidthDelegate;
            }
            else if (c != currentColor)
            {
                currentColor = c;
                d = RefreshColorDelegate;
            }
            else return;
            Dispatcher.BeginInvoke(d);
        }
        #region timer
        bool animating;
        private void StartTimer()
        {
            lock (this)
            {
                animating = true;
                timer.Change(0, PERIOD);
            }
        }
        private void StopTimer()
        {
            lock (this)
            {
                animating = false;
                timer.Change(Timeout.Infinite, PERIOD);
            }
        }
        private void TimerCallback()
        {
            if (current == hp) StopTimer();
            else
            {
                if (current < hp)
                {
                    current += 3;
                    if (current > hp) current = hp;
                }
                else if (current > hp)
                {
                    current -= 3;
                    if (current < hp) current = hp;
                }
                CurrentChanged();
            }
        }
    #endregion

        private void LifeChanged(object sender, PropertyChangedEventArgs e)
        {
            hp = ((PairValue)sender).Value;
#if DEBUG
            if (hp < 0)
            {
                System.Diagnostics.Debugger.Break();
                hp = 0;
            }
#endif
            if (!(animating || current == hp)) StartTimer();
        }
        private void LifeBar_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PairValue) ((PairValue)e.OldValue).PropertyChanged -= LifeChanged;
            PairValue pair = DataContext as PairValue;
            if (pair != null)
            {
                pair.PropertyChanged += LifeChanged;
                maxHp = pair.Origin;
                yellowHp = maxHp >> 1;
                redHp = maxHp / 5;
                current = hp = pair.Value;
                CurrentChanged();
            }
        }
    }
}
