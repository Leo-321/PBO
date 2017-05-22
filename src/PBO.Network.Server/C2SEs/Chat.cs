using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PokemonBattleOnline.Network.Commands;

namespace PokemonBattleOnline.Network.C2SEs
{
  [DataContract(Namespace = PBOMarks.JSON)]
  internal class ChatC2S : Commands.ChatC2S, IC2SE
  {
    public void Execute(ServerUser su)
    {
      var id = su.User.Id;
      var server = su.Server;
      var s2c = new ChatS2C(Mode, id, Chat);
      switch (Mode)
      {
        case ChatMode.Public:
          server.Send(s2c);
          break;
        case ChatMode.Room:
          var room = su.Room;
          if (room != null) room.Send(s2c);
          break;
        case ChatMode.Private:
          var to = server.GetUser(To);
          if (to != null) to.Send(s2c);
          break;
      }
    }
  }
}
