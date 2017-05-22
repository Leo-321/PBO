using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Globalization;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var r =
#if DEBUG
        "..\\"
#else
        string.Empty
#endif
        ;
            ImageService.Load(r + "res\\image.zip");
            GameString.Load(r + "res", "zh", "en");
            UserData.Load(r + "MyPBO\\user.dat");
            Config.Load(r + "MyPBO\\config.xml");

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            var font = new FontFamily("Microsoft YaHei");
            TextBlock.FontFamilyProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata(font));
            TextElement.FontFamilyProperty.OverrideMetadata(typeof(TextElement), new FrameworkPropertyMetadata(font));
            UIDispatcher.Init(Application.Current.Dispatcher);
            new MainWindow().Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (e.ApplicationExitCode != 0)
            {
                if (MessageBox.Show("PBO异常退出，是否保存编辑器中精灵的改动（否则恢复到PBO本次运行时前状态）？", "PBO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var ev = Editor.EditorVM.Current;
                        if (ev != null) ev.Save();
                    }
                    catch
                    {
                        MessageBox.Show("保存精灵失败。");
                    }
                }
                if (MessageBox.Show("是否保留本次运行改动的界面配置？", "PBO", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Config.Current.Save();
                    }
                    catch
                    {
                        MessageBox.Show("保存配置失败。");
                    }
                }
            }
            base.OnExit(e);
        }
    }
}
