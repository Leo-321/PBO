using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Network.C2SEs
{
    [DataContract(Namespace = PBOMarks.JSON)]
    internal class SetSeatC2S : Commands.SetSeatC2S, IC2SE
    {
        private static Random Random = new Random();

        public void Execute(ServerUser su)
        {
            var user = su.User;
            var server = su.Server;
            var fr = su.Room;
            if (fr != null)
            {
                if (Room == 0) fr.RemoveUser(su);
                else fr.ChangeSeat(su, Seat);
            }
            else if (GameSettings == null)
            {
                var r = server.GetRoom(Room);
                if (r != null) r.AddUser(su, Seat);
            }
            else
            {
                var r = server.AddRoom(GameSettings);
                if (r != null)
                {
                    var teamId = Random.Next(2);
                    var teamIndex = Random.Next(GameSettings.Mode.PlayersPerTeam());
#if TEST
                    r.AddUser(su, Seat.Player00);
#else
                    r.AddUser(su, (Seat)(teamId * 2 + teamIndex));
#endif
                }
            }
        }
    }
}
