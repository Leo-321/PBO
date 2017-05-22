using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class Substitute : GameEvent
  {
    [DataMember(EmitDefaultValue = false)]
    public bool De;
    [DataMember(EmitDefaultValue = false)]
    public int Pm;

    protected override void Update()
    {
      var pm = Game.GetPokemon(Pm);
      pm.IsSubstitute = !De;
      if (De)
      {
        pm.HideSubstitute();
        AppendGameLog("DeSubstitute", Pm);
      }
      else
      {
        pm.ShowSubstitute();
        AppendGameLog(Ls.EnSubstitute, Pm);
      }
    }
  }
}
