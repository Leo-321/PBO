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
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Editor
{
    /// <summary>
    /// Interaction logic for PokemonIcon.xaml
    /// </summary>
    public partial class PokemonIcon : UserControl
    {
        public PokemonIcon()
        {
            InitializeComponent();
        }

        PokemonVM _vm;
        internal PokemonVM VM
        {
            get
            {
                if (_vm == null) _vm = (PokemonVM)DataContext;
                return _vm;
            }
        }

        private void Overwrite(Game.PokemonData pm)
        {
            var va = VM.Actual;
            if (va.Model == null || MessageBox.Show(va.IsEditing ? "正在编辑的精灵，放弃编辑并覆盖？" : "覆盖原有精灵？", "PBO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                va.Model = pm;
                if (va.IsEditing)
                {
                    EditorVM.Current.EditingPokemon.Origin = null;
                    EditorVM.Current.EditingPokemon.Origin = va; //change from va to va
                }
            }
        }

        bool drag;
        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);
            var data = (PokemonIcon)e.Data.GetData(typeof(PokemonIcon));
            if (data != null && data != this)
            {
                var ctrl = e.KeyStates.HasFlag(DragDropKeyStates.ControlKey);
                var va = VM.Actual;
                if (ctrl)
                {
                    data.icon.Source = data.VM.Icon;
                    va.DropState = va.Model != null ? 2 : 1;
                }
                else
                {
                    data.icon.Source = va.Model == null ? null : va.Icon;
                    va.DropState = 1;
                }
                icon.Source = null;
            }
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            var data = (PokemonIcon)e.Data.GetData(typeof(PokemonIcon));
            if (data != null && data != this)
            {
                data.icon.ClearValue(Image.SourceProperty);
                icon.ClearValue(Image.SourceProperty);
                VM.Actual.DropState = 0;
            }
        }
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            var data = (PokemonIcon)e.Data.GetData(typeof(PokemonIcon));
            var va = VM.Actual;
            if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey)) Overwrite(data.VM.Model.Clone());
            else
            {
                var t = data.VM.Model;
                data.VM.Model = va.Model;
                va.Model = t;
                if (va.IsEditing) EditorVM.Current.EditingPokemon.Origin = data.VM;
                else if (data.VM.IsEditing) EditorVM.Current.EditingPokemon.Origin = va;
            }
            icon.ClearValue(Image.SourceProperty);
            va.DropState = 0;
        }

        bool click;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            click = true;
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Stroke.StrokeThickness = 3;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (click && e.LeftButton == MouseButtonState.Pressed)
            {
                click = false;
                drag = true;
                Stroke.Stroke = SBrushes.BlueM;
                Stroke.StrokeThickness = 3;
                PokemonBank.Current.DragIcon.Source = VM.Icon;
                PokemonBank.Current.DragIcon.Visibility = Visibility.Visible;
                Cursor = Cursors.None;
                if (VM.Model != null) DragDrop.DoDragDrop(this, this, DragDropEffects.All);
                Cursor = Cursors.Hand;
                drag = false;
                icon.ClearValue(Image.SourceProperty);
                icon.Visibility = System.Windows.Visibility.Visible;
                PokemonBank.Current.DragIcon.Visibility = Visibility.Collapsed;
                Stroke.ClearValue(Polygon.StrokeProperty);
                Stroke.ClearValue(Polygon.StrokeThicknessProperty);
            }
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            click = false;
            if (!drag) Stroke.ClearValue(Polygon.StrokeThicknessProperty);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (click) VM.Edit();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(VM.IsEditing ? "正在编辑该精灵，放弃编辑并删除？" : "删除精灵？", "PBO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (VM.IsEditing) EditorVM.Current.EditingPokemon.Origin = null;
                VM.Model = null;
            }
        }
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            var pm = Game.UserData.ImportPokemon(Clipboard.GetText());
            if (pm == null) MessageBox.Show("不是合法的精灵。");
            else Overwrite(pm);
        }
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (VM.Model != null) Clipboard.SetText(Game.UserData.Export(VM.Model));
        }
        private void CopyAll_Click(object sender, RoutedEventArgs e)
        {
            var pms = VM.Container.Model.Pokemons;
            if (pms[0] != null)
                Clipboard.SetText(Game.UserData.Export(pms.Where((p) => p != null)));
        }
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            var model = VM.Model;
            Copy.IsEnabled = Remove.IsEnabled = model != null;
            CopyAll.IsEnabled = VM.Container.Model.Pokemons[0] != null;
            Paste.IsEnabled = Clipboard.ContainsText();
        }
    }
}
