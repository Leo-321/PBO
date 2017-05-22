using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network;

namespace PokemonBattleOnline.PBO.Server
{
    class Program
    {
        static void PreLog(string message)
        {
            Console.Write(DateTime.Now.ToString("(hh:mm:ss) "));
            Console.Write(message);
            Console.Write("... ");
        }
        static int Main(string[] args)
        {
            Console.Title = PBOMarks.TITLE + " Server";
            var r =
#if DEBUG
        "..\\"
#else
        string.Empty
#endif
        ;
            Record.Init(r + "MyPBO");
            try
            {
                PreLog("Loading data");
                Console.WriteLine("OK!");

                PreLog("Opening server");
                PBOServer.NewServer(PBOMarks.DEFAULT_PORT);
                PBOServer.Current.Start();
                Console.WriteLine("OK!");

                PreLog("Initing taskbar notify icon");
                new Thread(TaskbarIcon.Init).Start();
                Console.WriteLine("OK!");

                Console.WriteLine();
                Console.WriteLine(@"Server is ready. To close server, input ""quit"" or close this window. To hide this window, click the notify icon in task.");
                Console.WriteLine();

                LOOP:
                //Console.Write("Input command: ");
                //Console.Write(">>>");
                var line = Console.ReadLine();
                if (line.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                {
                    PBOServer.Current.Dispose();
                    TaskbarIcon.Close();
                    Record.UnInit();
                    return 0;
                }
                ServerHelper.process(line);
                goto LOOP;
            }
            catch (Exception e)
            {
                Console.Write("Error: ");
                Console.WriteLine(e.ToString());
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey(false);
                Record.UnInit();
                return 0;
            }
        }
    }
}
