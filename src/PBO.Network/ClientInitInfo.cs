using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Network
{
  [DataContract(Name = "cii", Namespace = PBOMarks.JSON)]
  public class ClientInitInfo
  {
    [DataMember(Name = "a")]
    public readonly int User;

    public ClientInitInfo(int user, User[] lobbyUsers, Room[] rooms)
    {
      User = user;
      _lobbyUsers = lobbyUsers;
      _rooms = rooms;
    }

    [DataMember(Name = "b_")]
    private readonly User[] _lobbyUsers;
    public IEnumerable<User> LobbyUsers
    { get { return _lobbyUsers; } }

    [DataMember(Name = "c_")]
    private readonly Room[] _rooms;
    public IEnumerable<Room> Rooms
    { get { return _rooms; } }
  }
}
