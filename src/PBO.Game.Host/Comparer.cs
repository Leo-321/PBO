using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal sealed class Comparer : IComparer<PokemonProxy>, IComparer<Tile>
    {
        private readonly Board board;
        private readonly bool mega;

        public Comparer(Board board, bool mega=false)
        {
            this.board = board;
            this.mega = mega;
        }

        public int Compare(PokemonProxy a, PokemonProxy b)
        {
            int aS = 1, bS = 1;

            if (a.Action == PokemonAction.WillSwitch && b.Action == PokemonAction.WillSwitch) goto SPEED;
            if (a.Action == PokemonAction.WillSwitch) return -1;
            if (b.Action == PokemonAction.WillSwitch) return 1;

            if (a.Priority != b.Priority) return b.Priority - a.Priority;

            {//1=先制爪/先制果发动 0=无道具 -1=后攻尾/满腹香炉发动<--同时带这个慢的先出 只计算不发动
                if (a.ItemSpeedValue != b.ItemSpeedValue) return (b.ItemSpeedValue - a.ItemSpeedValue);
                if (a.ItemSpeedValue == -1) aS = bS = -1;
            }

            {
                bool aIsStall = a.AbilityE(As.STALL);
                bool bIsStall = b.AbilityE(As.STALL);
                if (aIsStall == bIsStall) goto SPEED;
                if (aIsStall) return 1;
                if (bIsStall) return -1;
            }

        SPEED:
            if (mega)
            {
                aS *= a.BeforeMegaSpeed;
                bS *= b.BeforeMegaSpeed;
            }
            else
            {
                aS *= a.Speed;
                bS *= b.Speed;
            }
            return CompareSpeed(aS, bS);
        }
        public int Compare(Tile a, Tile b)
        {
            return CompareSpeed(a.Speed, b.Speed);
        }
        private int CompareSpeed(int a, int b)
        {
            const int N = 1813; //如果一方的实际速度能力值≥N，则速度快的一方先行动。（1806＜N≤1812）
            if (a < N && b < N && board.HasCondition(Cs.TrickRoom)) return a - b;
            else return b - a;
        }
    }
}