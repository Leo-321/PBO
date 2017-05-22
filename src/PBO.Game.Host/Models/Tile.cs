using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
    internal sealed class Tile : ConditionalObject
    {
        public const int NOPM_INDEX = -1;

        public readonly Field Field;
        public readonly int X;

        public Tile(Field team, int x)
        {
            Field = team;
            X = x;
        }

        public PokemonProxy Pokemon;

        public int WillSendOutPokemonIndex;

        private int _speed;
        public int Speed
        {
            get
            {
                if (Pokemon != null) _speed = Pokemon.Speed;
                return _speed;
            }
        }

        public void Debut()
        {
            var pm = Pokemon;
            var h = pm.Hp != pm.Pokemon.MaxHp;
            var s = pm.State != PokemonState.Normal;
            if(HasCondition(Cs.Zheal)&&h)
                pm.Pokemon.Hp = pm.Pokemon.MaxHp;
            else if ((h || s) && HasCondition(Cs.HealingWish))
            {
                if (h) pm.Pokemon.Hp = pm.Pokemon.MaxHp;
                if (s) pm.Pokemon.State = PokemonState.Normal;
                pm.ShowLogPm("HealingWish");
            }
            else if (HasCondition(Cs.LunarDance))
            {
                if (h) pm.Pokemon.Hp = pm.Pokemon.MaxHp;
                if (s) pm.Pokemon.State = PokemonState.Normal;
                var pp = false;
                foreach (var m in pm.Moves)
                    if (m.PP != m.Move.PP.Origin)
                    {
                        m.PP = m.Move.PP.Origin;
                        pp = true;
                    }
                if (h || s || pp) pm.ShowLogPm("LunarDance");
            }
        }
    }
}
