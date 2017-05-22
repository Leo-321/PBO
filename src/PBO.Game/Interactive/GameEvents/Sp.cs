using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
    [DataContract(Name = "e0", Namespace = PBOMarks.JSON)]
    public class BeginTurn : GameEvent
    {
        protected override void Update()
        {
            ++Game.TurnNumber;
            if (Game.TurnNumber == 0) AppendGameLog("GameStart");
            else
            {
                AppendGameLog("BeginTurn", LogStyle.SYS, Game.TurnNumber);
                AppendGameLog("----");
            }
        }
    }

    [DataContract(Name = "e1", Namespace = PBOMarks.JSON)]
    public class EndTurn : GameEvent
    {
        protected override void Update()
        {
            if (Game.TurnNumber != 0)
            {
                AppendGameLog("EndTurn", LogStyle.EndTurn | LogStyle.SYS, Game.TurnNumber);
                for (int t = 0; t < 2; ++t)
                    for (int x = 0; x < Game.Settings.Mode.XBound(); ++x)
                    {
                        var pm = Game.Board[t, x];
                        if (pm != null)
                        {
                            if (pm.State == PokemonState.Normal) AppendGameLog("EndTurnNormalPm", LogStyle.EndTurn | LogStyle.HiddenInBattle, pm.Id, pm.Hp.Value);
                            else AppendGameLog("EndTurnAbnormalPm", LogStyle.EndTurn | LogStyle.HiddenInBattle, pm.Id, pm.Hp.Value);
                        }
                    }
            }
            AppendGameLog("br");
            Game.EndTurn();
        }
    }

    [DataContract(Name = "l", Namespace = PBOMarks.JSON)]
    public class HorizontalLine : GameEvent
    {
        protected override void Update()
        {
            AppendGameLog("----");
        }
    }

    [DataContract(Name = "tt", Namespace = PBOMarks.JSON)]
    public class TimeTick : GameEvent
    {
        [DataMember]
        int Seconds;

        public TimeTick(int seconds)
        {
            Seconds = seconds;
        }

        protected override void Update()
        {
            var style = LogStyle.Detail | LogStyle.NoBr;
            if (Seconds < 60) AppendGameLog("timeticks", style, Seconds);
            else if (Seconds % 60 == 0) AppendGameLog("timetickm", style, Seconds / 60);
            else AppendGameLog("timetickms", style, Seconds / 60, Seconds % 60);
        }
    }
}