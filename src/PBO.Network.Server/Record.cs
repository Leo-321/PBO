using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Remoting.Messaging;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Game.Host;

namespace PokemonBattleOnline.Network
{
    public static class Record
    {
        private static string IgPath;
        private static StreamWriter Ig;
        private static string EPath;
        private static StreamWriter E;

        public static void Init(string dir)
        {
            dir += "\\server";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var time = DateTime.Now;
            IgPath = string.Format("{0}\\[{1:yyyy-MM-dd-HHmmss}]pm.csv", dir, time);
            EPath = string.Format("{0}\\[{1:yyyy-MM-dd-HHmmss}]error.csv", dir, time);
        }
        public static void UnInit()
        {
            if (Ig != null)
            {
                lock (igLocker)
                {
                    Ig.Close();
                    Ig = null;
                }
            }
            if (E != null)
            {
                lock (eLocker)
                {
                    E.Close();
                    E = null;
                }
            }
        }

        private static void Append(StreamWriter sw, ValueType arg)
        {
            sw.Write(arg);
            sw.Write(',');
        }
        private static void Append(StreamWriter sw, object[] args)
        {
            foreach (var arg in args)
            {
                sw.Write(arg.ToString());
                sw.Write(',');
            }
        }
        private static void Callback(IAsyncResult ar)
        {
            try
            {
                ((Action<int, InitingGame>)((AsyncResult)ar).AsyncDelegate).EndInvoke(ar);
            }
            catch { }
        }

        public static void Add(Room room, InitingGame ig)
        {
            if (room != null && ig != null && IgPath != null)
            {
                try
                {
                    ((Action<int, InitingGame>)AddInitingGameImplement).BeginInvoke(room.Id, ig, Callback, null);
                }
                catch { }
            }
        }
        private static readonly object igLocker = new object();
        private static void AddInitingGameImplement(int room, InitingGame ig)
        {
            var pms = ig.GetPokemons(0, 0);
            lock (igLocker)
            {
                if (Ig == null) Ig = new StreamWriter(IgPath, true);
                if (pms != null)
                    foreach (var pm in pms)
                    {
                        Append(Ig, new object[] { room, ig.Id, ig.Settings.Mode, "00", pm.Form.Species.Number, pm.Form.Index, pm.Nature, pm.AbilityIndex, pm.Item, pm.Ev.Hp, pm.Ev.Atk, pm.Ev.SpAtk, pm.Ev.Def, pm.Ev.SpDef, pm.Ev.Speed });
                        foreach (var m in pm.Moves) Append(Ig, m.Move.Id);
                        Ig.WriteLine();
                    }
                if (ig.Settings.Mode.PlayersPerTeam() == 2)
                {
                    pms = ig.GetPokemons(0, 1);
                    if (pms != null)
                        foreach (var pm in pms)
                        {
                            Append(Ig, new object[] { room, ig.Id, ig.Settings.Mode, "01", pm.Form.Species.Number, pm.Form.Index, pm.Nature, pm.AbilityIndex, pm.Item, pm.Ev.Hp, pm.Ev.Atk, pm.Ev.SpAtk, pm.Ev.Def, pm.Ev.SpDef, pm.Ev.Speed });
                            foreach (var m in pm.Moves) Append(Ig, m.Move.Id);
                            Ig.WriteLine();
                        }
                    pms = ig.GetPokemons(1, 1);
                    if (pms != null)
                        foreach (var pm in pms)
                        {
                            Append(Ig, new object[] { room, ig.Id, ig.Settings.Mode, "11", pm.Form.Species.Number, pm.Form.Index, pm.Nature, pm.AbilityIndex, pm.Item, pm.Ev.Hp, pm.Ev.Atk, pm.Ev.SpAtk, pm.Ev.Def, pm.Ev.SpDef, pm.Ev.Speed });
                            foreach (var m in pm.Moves) Append(Ig, m.Move.Id);
                            Ig.WriteLine();
                        }
                }
                pms = ig.GetPokemons(1, 0);
                if (pms != null)
                    foreach (var pm in pms)
                    {
                        Append(Ig, new object[] { room, ig.Id, ig.Settings.Mode, "10", pm.Form.Species.Number, pm.Form.Index, pm.Nature, pm.AbilityIndex, pm.Item, pm.Ev.Hp, pm.Ev.Atk, pm.Ev.SpAtk, pm.Ev.Def, pm.Ev.SpDef, pm.Ev.Speed });
                        foreach (var m in pm.Moves) Append(Ig, m.Move.Id);
                        Ig.WriteLine();
                    }
                Ig.Flush();
            }
        }
        public static void Add(Room room, GameContext game)
        {
        }
        public static void AddImplement(Room room, GameContext game)
        {
        }
        public static void Add(Room room, GameContext game, string result)
        {

        }
        public static void Add(Room room, GameContext game, GameStopReason reason, int userId)
        {

        }
        public static void Error(Room room, GameContext game)
        {
            if (room != null && game != null && EPath != null)
            {
                try
                {
                    ((Action<int, int>)ErrorImplement).BeginInvoke(room.Id, game.Id, Callback, null);
                }
                catch { }
            }
        }
        private static readonly object eLocker = new object();
        public static void ErrorImplement(int room, int game)
        {
            lock (eLocker)
            {
                if (E == null) E = new StreamWriter(EPath, true);
                E.WriteLine("{0},{1}", room, game);
                E.Flush();
            }
        }
    }
}
