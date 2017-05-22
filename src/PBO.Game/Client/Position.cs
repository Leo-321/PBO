using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
    public interface IPosition
    {
        int Team { get; }
        int X { get; }
        CoordY Y { get; }
    }
    /// <summary>
    /// reference type, dont share among pokemons
    /// </summary>
    [DataContract(Name = "p", Namespace = PBOMarks.JSON)]
    public class Position : IPosition
    {
        [DataMember(EmitDefaultValue = false)]
        public int Team { get; private set; }
        [DataMember(EmitDefaultValue = false)]
        public int X { get; set; } //转盘用负数横坐标会不会很有趣
        [DataMember(EmitDefaultValue = false)]
        public CoordY Y { get; set; }

        public Position(int team, int x, CoordY y = CoordY.Plate)
        {
            Team = team;
            X = x;
            Y = y;
        }
    }
}
