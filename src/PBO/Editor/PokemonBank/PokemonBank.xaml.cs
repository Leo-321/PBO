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

namespace PokemonBattleOnline.PBO.Editor
{
    /// <summary>
    /// Interaction logic for PokemonBank.xaml
    /// </summary>
    public partial class PokemonBank : UserControl
    {
        public static PokemonBank Current
        { get; private set; }

        public PokemonBank()
        {
            Current = this;
            InitializeComponent();
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);
            var data = (PokemonIcon)e.Data.GetData(typeof(PokemonIcon));
            if (data != null)
            {
                data.icon.Source = null;
                var p = e.GetPosition(this);
                Canvas.SetLeft(DragIcon, p.X - DragIcon.ActualWidth / 2);
                Canvas.SetTop(DragIcon, p.Y - DragIcon.ActualHeight / 2);
            }
        }
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }
        private void NewTeam_Click(object sender, RoutedEventArgs e)
        {
            EditorVM.Current.Save();
            EditorVM.Current.NewTeam();
        }

        private void PasteTeam_Click(object sender, RoutedEventArgs e)
        {
            EditorVM.Current.ImportTeam(Clipboard.GetText());
            EditorVM.Current.Save();
        }
    }
}
