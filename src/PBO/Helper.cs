using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics.Contracts;
using System.ComponentModel;

namespace PokemonBattleOnline.PBO
{
  internal static class Helper
  {
    public static readonly Random Random = new Random();

    public static BitmapImage GetImage(string filename)
    {
      return new BitmapImage(new Uri(@"pack://application:,,,/PBO;component/images/" + filename, UriKind.Absolute));
    }

    internal static ResourceDictionary GetDictionary(string group, string name)
    {
      return (ResourceDictionary)Application.LoadComponent(
        new Uri(string.Format(@"/PBO;component/{0}/{1}.xaml", group, name), UriKind.Relative));
    }
    internal static T GetObject<T>(string group, string filename, string key)
    {
      return (T)GetDictionary(group, filename)[key];
    }
    internal static T GetObject<T>(string group, string name)
    {
      return GetObject<T>(group, name, name);
    }

    #region file
    /// <summary>
    /// get a file name selected by user to open 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static string OpenFile(string filter)
    {
      var dialog = new OpenFileDialog();
      dialog.Filter = filter;
      return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <summary>
    /// get a file name selected by user to save
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public static string SaveFile(string filter, string defaultName)
    {
      var dialog = new SaveFileDialog();
      dialog.FileName = defaultName;
      dialog.Filter = filter;
      dialog.AddExtension = true;
      return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    /// <summary>
    /// open an existing file and process
    /// </summary>
    /// <returns>return the file name</returns>
    public static string OpenFile(string filter, Action<string, Stream> action)
    {
      string fileName = OpenFile(filter);
      if (!string.IsNullOrEmpty(fileName)) ProcessFile(fileName, FileMode.Open, action);
      return fileName;
    }

    /// <summary>
    /// create a file and process
    /// </summary>
    /// <returns>return the file name</returns>
    public static string SaveFile(string filter, string defaultName, Action<string, Stream> action)
    {
      string fileName = SaveFile(filter, defaultName);
      if (!string.IsNullOrEmpty(fileName)) ProcessFile(fileName, FileMode.Create, action);
      return fileName;
    }

    private static void ProcessFile(string fileName, FileMode fileMode, Action<string, Stream> action)
    {
      using (var stream = new FileStream(fileName, fileMode))
        action(Path.GetFileNameWithoutExtension(fileName), stream);
    }
    #endregion

    public static T GetParent<T>(DependencyObject reference) where T : DependencyObject
    {
      do
      {
        reference = VisualTreeHelper.GetParent(reference);
      }
      while (reference != null && !(reference is T));
      return (T)reference;
    }
    public static void SortOnClick(ICollectionView view, string property)
    {
      var shift = Keyboard.Modifiers == ModifierKeys.Shift;
      ListSortDirection order = ListSortDirection.Ascending;
      foreach (var sd in view.SortDescriptions)
        if (sd.PropertyName == property)
        {
          order = sd.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
          if (shift) view.SortDescriptions.Remove(sd);
          break;
        }
      if (!shift) view.SortDescriptions.Clear();
      view.SortDescriptions.Add(new SortDescription(property, order));
    }
  }
}
