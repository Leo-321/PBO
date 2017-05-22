using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace PokemonBattleOnline.Game
{
    public enum BallState : byte
    {
        None,
        Normal,
        Abnormal,
        Faint
    }
    public class PlayerOutward
    {
        private static readonly BallState[] DEFAULT = new BallState[12];

        public string Name;

        public PlayerOutward(string name, int pokemons)
        {
            Name = name;
            _balls = new ObservableList<BallState>();
            while (pokemons-- != 0) _balls.Add(BallState.None);
        }

        private ObservableList<BallState> _balls;
        public IEnumerable<BallState> Balls
        { get { return _balls; } }

        public void SetAll(BallState[] state)
        {
            for(int i = 0; i < state.Length; ++i) _balls[i] = state[i];
        }
        internal void StateChanged(PokemonOutward pm)
        {
            _balls[pm.PokemonIndex] = pm.Hp.Value == 0 ? BallState.Faint : pm.State == PokemonState.Normal ? BallState.Normal : BallState.Abnormal;
        }
        internal void SwitchPokemon(int a, int b)
        {
            var t = _balls[a];
            _balls[a] = _balls[b];
            _balls[b] = t;
        }
    }
}
