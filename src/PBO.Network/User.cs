using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Network
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class User : ObservableObject
  {
    public User(int id, string name, int avatar)
    {
      _id = id;
      _name = name;
      _avatar = avatar;
    }

    [DataMember(Name = "a")]
    private readonly int _id;
    public int Id
    { get { return _id; } }
    [DataMember(Name = "b")]
    private readonly string _name;
    public string Name
    { get { return _name; } }
    [DataMember(Name = "c")]
    private readonly int _avatar;
    public int Avatar
    { get { return _avatar; } }

    [DataMember(Name = "d")]
    internal int RoomId;
    
    private Room _room;
    /// <summary>
    /// setter is only for Room class
    /// </summary>
    public Room Room
    {
      get { return _room; }
      internal set
      {
        if (_room != value)
        {
          _room = value;
          RoomId = _room == null ? 0 : _room.Id;
        }
      }
    }
    /// <summary>
    /// setter is only for Room class
    /// </summary>
    public Seat Seat
    { get; internal set; }
  }
}
