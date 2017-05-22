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
using PokemonBattleOnline.Network;
using PokemonBattleOnline.PBO.Elements;
using SoundPlayer = System.Media.SoundPlayer;

namespace PokemonBattleOnline.PBO.Lobby
{
  /// <summary>
  /// Interaction logic for Chat.xaml
  /// </summary>
  public partial class Chat : UserControl
  {
    public static readonly SimpleCommand NewChatCommand;
    private static readonly SoundPlayer sound;
    private static Chat Current;
    static Chat()
    {
      NewChatCommand = new SimpleCommand((_u) =>
        {
          var u = (User)_u;
          if (PBOClient.Current != null && u != PBOClient.Current.User) Current.NewChat(u);
        });
      sound = new SoundPlayer(Application.GetResourceStream(new Uri(@"pack://application:,,,/PBO;component/images/chat.wav", UriKind.Absolute)).Stream);
      try
      {
        sound.LoadAsync();
      }
      catch { }
    }
    static void PlaySound()
    {
      if (sound.IsLoadCompleted) sound.Play();
    }

    ScrollViewer scroll;
    ClientController Controller;

    public Chat()
    {
      Current = this;
      InitializeComponent();
      whom.SelectionChanged += (sender, e) =>
        {
          if (whom.SelectedIndex > 0)
            ((TabItem)whom.SelectedItem).Foreground = System.Windows.Media.Brushes.Black;
        };
      ClientController.PublicChat += OnPublicChat;
      ClientController.PrivateChat += OnPrivateChat;
      Speaking.Speak += OnSpeak;
    }

    ScrollViewer Scroll
    { 
      get
      {
        if (scroll == null) scroll = chatViewer.Template.FindName("PART_ContentHost", chatViewer) as ScrollViewer;
        return scroll;
      }
    }

    /// <summary>
    /// it's ok to reinit.
    /// </summary>
    private void OnSpeak(string chat)
    {
      if (whom.SelectedIndex > 0)
      {
        var ti = (TabItem)whom.SelectedItem;
        Controller.ChatPrivate((User)ti.Header, chat);
        ((TextBox)ti.Content).AppendText(PBOClient.Current.User.Name + ": " + chat + "\n");
      }
      else Controller.ChatPublic(chat);
    }
    private TabItem GetChatTab(User user)
    {
      foreach (TabItem ti in whom.Items)
        if (ti.Header == user) return ti;
      var t = new TabItem() { Header = user, Content = new TextBox() };
      whom.Items.Add(t);
      return t;
    }
    internal void Init(ClientController controller)
    {
      Controller = controller;
      Speaking.Clear();
      chat.Inlines.Clear();
      whom.Items.MoveCurrentToFirst();
      for (int i = whom.Items.Count - 1; i != 0; i--) whom.Items.RemoveAt(i);
    }
    internal void NewChat(User user)
    {
      whom.SelectedItem = GetChatTab(user);
    }

    void OnPublicChat(string content, User user)
    {
      Run r = new Run(user.Name + ": " + content + "\n");
      r.Foreground = Cartes.GetChatBrush(user.Name);
      if (Scroll.ScrollableHeight - Scroll.ExtentHeight < 5)
      {
        chat.Inlines.Add(r);
        Scroll.ScrollToEnd();
      }
      else chat.Inlines.Add(r);
    }
    void OnPrivateChat(string content, User user)
    {
      //私聊要开新Tab
      TabItem ti = GetChatTab(user);
      ((TextBox)ti.Content).AppendText(user.Name + ": " + content + "\n");
      if (whom.SelectedItem != ti)
      {
        ti.Foreground = Elements.SBrushes.OrangeM;
        PlaySound();
      }
    }

    private void close_Click(object sender, RoutedEventArgs e)
    {
      var ti = Helper.GetParent<TabItem>((DependencyObject)sender);
      if (ti != null) whom.Items.Remove(ti);
    }
  }
}
