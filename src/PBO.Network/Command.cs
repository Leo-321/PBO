using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PokemonBattleOnline.Network.Commands;

namespace PokemonBattleOnline.Network
{
  public interface IS2C
  {
    void Execute(Client client);
  }
  public interface IC2S
  {
  }
}
