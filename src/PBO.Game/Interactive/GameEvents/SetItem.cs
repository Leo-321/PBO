using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class SetItem : GameEvent
  {
    [DataMember(EmitDefaultValue = false)]
    public int Pm;
    [DataMember(EmitDefaultValue = false)]
    public int Item;

    public override void Update(SimGame game)
    {
      var pm = GetPokemon(game, Pm);
      if (pm != null) pm.Item = Item;
    }
  }
}
