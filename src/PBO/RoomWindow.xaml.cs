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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO
{
    /// <summary>
    /// Interaction logic for BattleWindow.xaml
    /// </summary>
    public partial class RoomWindow : Window
    {
        #region static
        private static RoomWindow Current;

        public static void Init()
        {
            RoomController.Quited += RoomController_Quited;
            RoomController.GameStop += (r, p) => Current.OnGameStop(r, p);
            RoomController.RoomChat += RoomController_RoomChat;
            RoomController.TimeReminder += RoomController_TimeReminder;
            RoomController.TimeUp += (st) => Current.OnTimeUp(st);
            RoomController.Entered += RoomController_Entered;
            RoomController.GameInited += RoomController_GameInited;
            PBOClient.CurrentChanged += PBOClient_CurrentChanged;
            Current = new RoomWindow() { Visibility = Visibility.Collapsed };
        }

        static void PBOClient_CurrentChanged()
        {
            if (PBOClient.Current == null) Current.Reset(null);
        }

        static void RoomController_GameInited()
        {
            Current.OnGameInited();
        }

        static void RoomController_Entered()
        {
            Current.Reset(PBOClient.Current.Room);
        }

        static void RoomController_TimeReminder(User[] users)
        {
            var br = Current.br;
            switch (users.Length)
            {
                case 1: //双打三打pm复数
                    br.AddLogText(string.Format("等待{0}给精灵下达命令。\r\n", users[0].Name));
                    break;
                case 2:
                    br.AddLogText(string.Format("等待{0}和{1}给精灵下达命令。\r\n", users[0].Name, users[1].Name));
                    break;
                case 3:
                    br.AddLogText(string.Format("等待{0}、{1}和{2}给精灵下达命令。\r\n", users[0].Name, users[1].Name, users[2].Name));
                    break;
            }
        }
        static void RoomController_RoomChat(string arg1, User arg2)
        {
            var br = Current.br;
            br.AddChatText(arg1, arg2);
        }
        static void RoomController_Quited()
        {
            Current.Reset(null);
        }

        public static bool Window_Closing(Window mainWindow)
        {
            if (PBOClient.Current != null && PBOClient.Current.User.Room != null)
            {
                ShowMessageBox.CantCloseMainWindow(mainWindow);
                return true;
            }
            return false;
        }
        #endregion

        private RoomController Room;

        public RoomWindow()
        {
            Current = this;
            InitializeComponent();
            //nds.ReviewPokemon += (p) => pmReview.Content = p;
            Teams.ItemsSource = Editor.EditorVM.Current.BattleTeams;
            Chat.Speak += Chat_Speak;
        }

        private void Chat_Speak(string chat)
        {
            Room.Chat(chat);
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (Room.Room.Settings.Mode.IsRandom())
            {
                PokemonData[] pt = new PokemonData[6];
                for (int i = 0; i < 6; i++)
                    pt[i] = new PokemonData(Helper.Random.Next(), "suiji");
                Room.GamePrepare(pt);
                Start.Content = "等待其他玩家开始对战...";
                Teams.Visibility = System.Windows.Visibility.Collapsed;
                Start.IsEnabled = false;
            }
            else
            {
                var team = Teams.SelectedItem as Editor.TeamVM;
                if (team != null && team.Model.Pokemons[0] != null)
                {
                    var pt = team.Model.Pokemons.Where((p) => p != null).ToArray();
                    Room.GamePrepare(pt);
                    PrepareTeam.DataContext = pt;
                    Teams.Visibility = System.Windows.Visibility.Collapsed;
                    Start.Content = "等待其他玩家开始对战...";
                    Start.IsEnabled = false;
                }
            }
        }
        
        private void ResetPrepare()
        {
            PrepareTeam.DataContext = null;
            Prepare.Visibility = Visibility.Visible;
            if (Room.User.Seat == Seat.Spectator)
            {
                Spectating.Visibility = Visibility.Visible;
                PrepareTeam.Visibility = Visibility.Collapsed;
                Teams.Visibility = Visibility.Collapsed;
                Start.Visibility = Visibility.Collapsed;
            }
            else
            {
                Spectating.Visibility = Visibility.Collapsed;
                PrepareTeam.Visibility = Visibility.Visible;
                Teams.Visibility = Visibility.Visible;
                Start.Visibility = Visibility.Visible;
                Start.IsEnabled = true;
                Start.Content = "使用所选队伍开始对战！";
                if (Room.Room.Settings.Mode.IsRandom())
                {
                    Start.Content = "开始随机对战！";
                    Teams.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void Reset(RoomController room)
        {
            Game.Init(null);
            if (room != null)
            {
                Prepare.DataContext = Current.Room = room;
                if (room.Room.Settings.Mode == GameMode.Multi)
                {
                    PX1.Height = new GridLength(32);
                    Title = "合作对战房间";
                    p3.Visibility = Visibility.Visible;
                }
                else if (room.Room.Settings.Mode == GameMode.Random4p)
                {
                    PX1.Height = new GridLength(32);
                    Title = "随机4p房间";
                    p3.Visibility = Visibility.Collapsed;
                }
                else if (room.Room.Settings.Mode == GameMode.RandomSingle)
                {
                    PX1.Height = new GridLength(0);
                    Title = "随机单打房间";
                    p3.Visibility = Visibility.Collapsed;
                }
                else
                {
                    PX1.Height = new GridLength(0);
                    Title = "单打对战房间";
                    p3.Visibility = Visibility.Visible;
                }
                Visibility = Visibility.Visible;
                ResetPrepare();
            }
            else
            {
                Current.Visibility = Visibility.Collapsed;
                br.Reset();
                Prepare.DataContext = null;
                Room = null;
            }
        }

        private void OnGameInited()
        {
            StringBuilder t0 = new StringBuilder();
            StringBuilder t1 = new StringBuilder();
            foreach (var p in Room.Room.Players)
                if (p.Seat.TeamId() == 0)
                {
                    t0.Append(p.Name);
                    t0.Append(' ');
                }
                else
                {
                    t1.Append(' ');
                    t1.Append(p.Name);
                }
            t0.Append("VS");
            t0.Append(t1);
            var title = t0.ToString();
            Title = title;

            Game.Init(Room);
            br.Reset();
            br.Init(Room.Game);
            Room.Game.GameStart += () =>
                {
                    br.FontSize = 14;
                    Prepare.Visibility = Visibility.Collapsed;
                };
        }

        private void OnTimeUp(IEnumerable<KeyValuePair<User, int>> spentTime)
        {
            var br = Current.br;
            br.AddLogText("Time Up\r\n");
            foreach (var pair in spentTime)
                br.AddUserText(string.Format("{0}使用了{1}秒", pair.Key.Name, pair.Value), pair.Key);
            OnGameStop();
        }
        private void OnGameStop(GameStopReason reason, User player)
        {
            if (reason != GameStopReason.GameEnd)
            {
                br.FontSize = 12;
                string key;
                switch (reason)
                {
                    case GameStopReason.PlayerStop:
                        key = Ls.PLAYER_STOP;
                        break;
                    case GameStopReason.PlayerDisconnect:
                        key = Ls.PLAYER_DISCONNECT;
                        break;
                    case GameStopReason.InvalidInput:
                        key = Ls.INVALID_INPUT;
                        break;
                    default: //Error
                        key = Ls.SERROR;
                        break;
                }
                br.AddLogText(string.Format(GameString.Current.BattleLog(key).LineBreak(), player == null ? null : player.Name));
            }
            OnGameStop();
        }
        private void OnGameStop()
        {
            br.Save(Title, Room.Client.User.Name);
            Title = Room.Room.Settings.Mode == GameMode.Multi ? "合作对战房间" : Room.Room.Settings.Mode == GameMode.Random4p ? "随机4p房间" : Room.Room.Settings.Mode == GameMode.RandomSingle ? "随机单打房间" : "单打对战房间";
            ResetPrepare();
            Game.Init(null);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            if (Room != null && (Room.PlayerController == null || ShowMessageBox.ClosingInBattle(this) == MessageBoxResult.Yes)) Room.Quit();
        }
    }
}
