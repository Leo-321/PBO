using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public enum StatType : byte
    {
        Invalid,
        Atk,
        Def,
        SpAtk,
        SpDef,
        Speed,
        Accuracy,
        Evasion,
        /// <summary>
        /// 是指A/D/SA/SD/S，共5个
        /// </summary>
        All,
        Hp
    }
}
