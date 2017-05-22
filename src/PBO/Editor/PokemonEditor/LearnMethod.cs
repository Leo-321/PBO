using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public enum LearnCategory : byte
    {
        /// <summary>
        /// 净化，剧情，素描
        /// </summary>
        Other,
        Lv,
        Machine,
        Egg,
        Tutor,
        Event
    }
    public class LearnMethod
    {
        public LearnMethod(LearnCategory method)
        {
            Method = method;
        }

        public LearnCategory Method
        { get; private set; }
        public int Gen
        { get; private set; }
        public int Detail
        { get; private set; }
    }
}
