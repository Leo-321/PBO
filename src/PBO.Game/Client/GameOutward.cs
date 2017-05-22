using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PokemonBattleOnline.Game
{
    public class GameOutward : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// game start, or an observer
        /// </summary>
        public event Action<Exception> Error;
        public event Action GameStart;
        public event Action GameEnd;
        public event Action<string, LogStyle> LogAppended;
        public event Action TurnEnd;
        public readonly IGameSettings Settings;
        public readonly BoardOutward Board;

        public GameOutward(IGameSettings settings, string[,] players, ReportFragment fragment)
        {
            Settings = settings;
            Board = new BoardOutward(Settings);
            TurnNumber = fragment.TurnNumber;
            var ppp = settings.Mode.PokemonsPerPlayer();
            Board.Players[0, 0] = new PlayerOutward(players[0, 0], ppp);
            Board.Players[1, 0] = new PlayerOutward(players[1, 0], ppp);
            Board.Players[0, 0].SetAll(fragment.P00);
            Board.Players[1, 0].SetAll(fragment.P10);
            if (Settings.Mode.PlayersPerTeam() == 2)
            {
                Board.Players[0, 1] = new PlayerOutward(players[0, 1], ppp);
                Board.Players[1, 1] = new PlayerOutward(players[1, 1], ppp);
                Board.Players[0, 1].SetAll(fragment.P01);
                Board.Players[1, 1].SetAll(fragment.P11);
            }
            Board.Weather = fragment.Weather;
            foreach (var pm in fragment.Pokemons) pm.Init(this);
#if TEST
            LogAppended = delegate { };
            TurnEnd = delegate { };
#endif
        }
        public int TurnNumber
        { get; set; }

        public PokemonOutward GetPokemon(int id)
        {
            foreach (var team in Board.Pokemons)
                foreach (var pm in team)
                    if (pm != null && pm.Id == id) return pm;
            return null;
        }
        public void Start()
        {
            LogAppended(PBOMarks.TITLE, LogStyle.Bold);
            AppendGameLog("GameMode", LogStyle.Default, Settings.Mode == GameMode.Single ? "单打" : Settings.Mode == GameMode.Multi ? "合作" :Settings.Mode == GameMode.Random4p ? "随机4p" : Settings.Mode == GameMode.RandomSingle ? "随机单打" : Settings.Mode.ToString());
            if (Settings.SleepRule) AppendGameLog("GameRule", LogStyle.Default, "催眠条款");
            if (TurnNumber >= 0) AppendGameLog("GameContinue", LogStyle.Default);
            UIDispatcher.Invoke(GameStart);
        }
        public void Update(IEnumerable<GameEvent> events)
        {
            try
            {
                foreach (GameEvent e in events)
                {
                    UIDispatcher.Invoke((Action<GameOutward>)e.Update, this);
#if !TEST
                    System.Threading.Thread.Sleep(e.Sleep);
#endif
                }
            }
            catch (Exception ex)
            {
                UIDispatcher.Invoke(Error, ex);
            }
        }
        public void EndGame()
        {
            UIDispatcher.Invoke(GameEnd);
        }

        public void AppendGameLog(string key, LogStyle style, object arg0 = null, object arg1 = null, object arg2 = null)
        {
            var log = GameString.Current.BattleLog(key);
            if (log != null) LogAppended(string.Format(this, log, arg0, arg1, arg2), style);
            else if (key != Ls.NO_KEY) AppendGameLog(Ls.NO_KEY, LogStyle.SYS, key);
        }
        public void EndTurn()
        {
            TurnEnd();
        }

        object IFormatProvider.GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            else return null;
        }
        string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
        {
            string r = null;
            if (arg != null)
            {
                var type = arg.GetType();
                if (type == typeof(BattleType)) r = GameString.Current.BattleType((BattleType)arg);
                else if (type == typeof(int))
                {
                    int id = (int)arg;
                    if (format == null) r = id.ToString();
                    else
                    {
                        switch (format)
                        {
                            case "p":
                                {
                                    var pm = GetPokemon(id);
                                    if (pm != null) r = string.Format(GameString.Current.BattleLog("OwnersPokemon"), pm.Owner.Name, pm.Name);
                                }
                                break;
                            case "m":
                                r = GameString.Current.Move(id);
                                break;
                            case "a":
                                r = GameString.Current.Ability(id);
                                break;
                            case "i":
                                r = GameString.Current.Item(id);
                                break;
                            case "b":
                                r = GameString.Current.BattleType((BattleType)id);
                                break;
                            case "s":
                                r = GameString.Current.StatType((StatType)id);
                                break;
                            case "t":
                                if (id == 0 || id == 1)
                                {
                                    var p0 = Board.Players[id, 0].Name;
                                    r = Settings.Mode.PlayersPerTeam() == 1 ? p0 : string.Format(GameString.Current.BattleLog("Team2"), p0, Board.Players[id, 1].Name);
                                }
                                break;
                            default:
                                if (format.StartsWith("pm."))
                                {
                                    var pm = GetPokemon(id);
                                    if (pm != null) r = pm.GetProperty(format.Substring(3));
                                }
                                break;
                        }//switch
                        if (r == null) r = "<error>";
                    }
                }
                else r = GameString.Current.BattleLog(arg.ToString());
            }// if (arg != null
            return r;
        }
    }
}
