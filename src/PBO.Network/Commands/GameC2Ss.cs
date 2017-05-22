using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Network.Commands
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class PrepareC2S : IC2S
  {
    public static PrepareC2S Prepare(PokemonData[] pms)
    {
      return new PrepareC2S(pms);
    }
    public static PrepareC2S UnPrepare()
    {
      return new PrepareC2S(null);
    }
    
    [DataMember(Name = "a_", EmitDefaultValue = false)]
    protected readonly PokemonData[] Pms;

    private PrepareC2S(PokemonData[] pms)
    {
      Pms = pms;
    }
    protected PrepareC2S()
    {
    }
  }
  [DataContract(Namespace = PBOMarks.JSON)]
  public class InputC2S : ActionInput, IC2S
  {
    public InputC2S(ActionInput action)
      : base(action)
    {
    }
    protected InputC2S()
      : base()
    {
    }
  }
  [DataContract(Namespace = PBOMarks.JSON)]
  public class GiveUpC2S : IC2S
  {
    public GiveUpC2S()
    {
    }
  }
}
