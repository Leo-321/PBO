using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Network.Commands
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class UserS2C : IS2C
  {
    public static UserS2C AddUser(int id, string name, int avatar)
    {
      return new UserS2C() { Id = id, Name = name, Avatar = avatar };
    }
    public static UserS2C RemoveUser(int id)
    {
      return new UserS2C() { Id = id };
    }

    private UserS2C()
    {
    }

    [DataMember(Name = "a")]
    int Id;
    [DataMember(Name = "b", EmitDefaultValue = false)]
    string Name;
    [DataMember(Name = "c", EmitDefaultValue = false)]
    int Avatar;

    void IS2C.Execute(Client client)
    {
      if (Name == null) client.Controller.RemoveUser(Id);
      else client.Controller.AddUser(new User(Id, Name, Avatar));
    }
  }
}
