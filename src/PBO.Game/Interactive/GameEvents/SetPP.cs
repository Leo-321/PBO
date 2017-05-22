using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class SetPP : GameEvent
  {
    [DataMember(EmitDefaultValue = false)]
    public int Pm;
    [DataMember(EmitDefaultValue = false)]
    public int Move;
    [DataMember(EmitDefaultValue = false)]
    public int PP;

    public override void Update(SimGame game)
    {
      var pm = GetOnboardPokemon(game, Pm);
      if (pm != null)
      {
        foreach (var m in pm.Moves)
          if (m != null && m.Type.Id == Move)
          {
            m.PP.Value = PP;
            break;
          }
      }
    }
  }
}
