using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Network.Commands
{
  public enum ChatMode
  {
    Public,
    Room,
    Private
  }
  [DataContract(Namespace = PBOMarks.JSON)]
  public class ChatC2S : IC2S
  {
    public static ChatC2S PublicChat(string chat)
    {
      return new ChatC2S(ChatMode.Public, 0, chat);
    }
    public static ChatC2S RoomChat(string chat)
    {
      return new ChatC2S(ChatMode.Room, 0, chat);
    }
    public static ChatC2S PrivateChat(int to, string chat)
    {
      return new ChatC2S(ChatMode.Private, to, chat);
    }

    [DataMember(Name = "b", EmitDefaultValue = false)]
    protected readonly ChatMode Mode;

    /// <summary>
    /// private mode only
    /// </summary>
    [DataMember(Name = "c", EmitDefaultValue = false)]
    protected readonly int To;

    [DataMember(Name = "a")]
    protected readonly string Chat;

    private ChatC2S(ChatMode mode, int to, string chat)
    {
      Mode = mode;
      To = to;
      Chat = chat;
    }
    protected ChatC2S()
    {
    }
  }
  [DataContract(Name = "ch", Namespace = PBOMarks.JSON)]
  public class ChatS2C : IS2C
  {
    [DataMember(Name = "c", EmitDefaultValue = false)]
    private readonly ChatMode Mode;

    /// <summary>
    /// private mode only
    /// </summary>
    [DataMember(Name = "b", EmitDefaultValue = false)]
    private readonly int From;

    [DataMember(Name = "a")]
    private readonly string Chat;

    public ChatS2C(ChatMode mode, int from, string chat)
    {
      Mode = mode;
      From = from;
      Chat = chat;
    }

    void IS2C.Execute(Client client)
    {
      var user = client.Controller.GetUser(From);
      if (user != null)
        switch (Mode)
        {
          case ChatMode.Public:
            ClientController.OnPublicChat(Chat, user);
            break;
          case ChatMode.Room:
            RoomController.OnRoomChat(Chat, user);
            break;
          case ChatMode.Private:
            ClientController.OnPrivateChat(Chat, user);
            break;
        }
    }
  }
}
