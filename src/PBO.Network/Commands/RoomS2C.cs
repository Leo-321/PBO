using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Network.Commands
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class RoomS2C : IS2C
  {
    public static RoomS2C NewRoom(int id, GameSettings settings)
    {
      return new RoomS2C() { Id = id, Settings = settings };
    }
    public static RoomS2C RemoveRoom(int id)
    {
      return new RoomS2C() { Id = id };
    }
    public static RoomS2C ChangeBattling(int id)
    {
      return new RoomS2C() { Id = id, Battling = true };
    }
    
    [DataMember(Name = "a")]
    private int Id;
    [DataMember(Name = "b_", EmitDefaultValue = false)]
    private GameSettings Settings;
    [DataMember(Name = "c", EmitDefaultValue = false)]
    private bool Battling;

    private RoomS2C()
    {
    }

    void IS2C.Execute(Client client)
    {
      if (Battling)
      {
        var room = client.Controller.GetRoom(Id);
        room.Battling = !room.Battling;
      }
      else if (Settings == null) client.Controller.RemoveRoom(Id);
      else client.Controller.AddRoom(new Room(Id, Settings));
    }
  }
}
