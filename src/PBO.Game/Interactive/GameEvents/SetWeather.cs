using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
  [DataContract(Namespace = PBOMarks.JSON)]
  public class SetWeather : GameEvent
  {
    [DataMember(Name = "a")]
    Weather Weather;

    public SetWeather(Weather weather)
    {
      Weather = weather;
    }

    protected override void Update()
    {
      Game.Board.Weather = Weather;
    }
  }
}
