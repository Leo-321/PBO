using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.Test
{
    class Program
    {
        static TestClient C00;
        static TestClient C01;
        static TestClient C10;
        static TestClient C11;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => EndLog();
            GameString.Load("..\\..\\res", "zh", "en");
            PBOServer.NewServer(9999);
            Thread.Sleep(1000);
            RoomController.GameStop += (r, u) => LogLine(r.ToString() + (u == null ? " " : " " + u.Name));
            LoginClient.LoginSucceed += (c) =>
            {
                switch (c.Controller.User.Name)
                {
                    case "P00":
                        C00 = new TestClient(c.Controller, Seat.Player00);
                        C00.C.NewRoom(new GameSettings(GameMode.Multi));
                        break;
                    case "P01":
                        C01 = new TestClient(c.Controller, Seat.Player01);
                        break;
                    case "P10":
                        C10 = new TestClient(c.Controller, Seat.Player10);
                        break;
                    case "P11":
                        C11 = new TestClient(c.Controller, Seat.Player11);
                        break;
                }
                Console.WriteLine(c.Controller.User.Name + "logined.");
            };

            var l00 = new LoginClient("127.0.0.1", 9999, "P00", 1);
            var l01 = new LoginClient("127.0.0.1", 9999, "P01", 1);
            var l10 = new LoginClient("127.0.0.1", 9999, "P10", 1);
            var l11 = new LoginClient("127.0.0.1", 9999, "P11", 1);
            l00.BeginLogin();
            l01.BeginLogin();
            l10.BeginLogin();
            l11.BeginLogin();
            Console.ReadKey();

            TEAM:
            C00.EditTeam(null);
            C01.EditTeam(C00.Team);
            C10.EditTeam(C01.Team);
            C11.EditTeam(C10.Team);

            LogLine("============BATTLE============");
            BATTLE:
            Thread.Sleep(500);
            if (C00.Battle() && C01.Battle() && C10.Battle() && C11.Battle()) goto BATTLE;
            else
            {
                Console.WriteLine("------------------------------");
                EndLog();
                goto TEAM;
            }
        }
        static StreamWriter log;
        static void BeginLog()
        {
            if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
            log = new StreamWriter("logs\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt", true, Encoding.Unicode);
        }
        public static void LogLine()
        {
            Console.WriteLine();
            if (log != null) log.WriteLine();
        }
        public static void LogLine(string text)
        {
            Console.WriteLine(text);
            if (log != null) log.WriteLine(text);
        }
        public static void Log(string text)
        {
            Console.Write(text);
            if (log != null) log.Write(text);
        }
        static void EndLog()
        {
            if (log != null)
            {
                log.Dispose();
                log = null;
            }
        }
    }
}
