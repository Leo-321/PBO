using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
    [DataContract(Namespace = PBOMarks.JSON)]
    public class GameEnd : GameEvent
    {
        [DataMember(EmitDefaultValue = false)]
        public int Lose;

        public GameEnd(bool lose0, bool lose1)
        {
            if (lose1)
                if (lose0) Lose = 2;
                else Lose = 1;
        }

        protected override void Update()
        {
            int[] r = new int[2];
            for (int t = 0; t < 2; ++t)
                for (int i = 0; i < Game.Settings.Mode.PlayersPerTeam(); ++i)
                    foreach (var p in Game.Board.Players[t, i].Balls)
                        if (p == BallState.Normal || p == BallState.Abnormal) ++r[t];
            if (Lose == 2)
            {
                AppendGameLog("GameResultTie", LogStyle.Center | LogStyle.Bold, 0, 1);
                if (r[0] != 0) AppendGameLog("GameResult1", LogStyle.Center | LogStyle.Bold, r[0], r[1]);
            }
            else
            {
                AppendGameLog(Ls.br, LogStyle.Center | LogStyle.Bold);
                AppendGameLog("GameResult0", LogStyle.Center | LogStyle.Bold, Lose == 0 ? 1 : 0);
                AppendGameLog("GameResult1", LogStyle.Center | LogStyle.Bold, r[0], r[1]);
            }
            Game.EndGame();
        }
    }
}
