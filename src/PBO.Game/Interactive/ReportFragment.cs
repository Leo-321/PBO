using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
    [KnownType("KnownEvents")]
    [DataContract(Namespace = PBOMarks.JSON)]
    public class ReportFragment
    {
        static Type[] knownGameEvents;
        static IEnumerable<Type> KnownEvents()
        {
            if (knownGameEvents == null) knownGameEvents = typeof(GameEvent).SubClasses();
            return knownGameEvents;
        }

        [DataMember(Name = "b", EmitDefaultValue = false)]
        private int _turnNumber;
        public int TurnNumber
        {
            get { return _turnNumber - 1; }
            set { _turnNumber = value + 1; }
        }

        [DataMember(Name = "e", EmitDefaultValue = false)]
        public Weather Weather;

        [DataMember(EmitDefaultValue = false)]
        public readonly BallState[] P00;
        [DataMember(EmitDefaultValue = false)]
        public readonly BallState[] P10;
        [DataMember(EmitDefaultValue = false)]
        public readonly BallState[] P01;
        [DataMember(EmitDefaultValue = false)]
        public readonly BallState[] P11;
        [DataMember(Name = "d_", EmitDefaultValue = false)]
        public PokemonOutward[] Pokemons; //在场精灵

        public ReportFragment(IGameSettings settings)
        {
            var ppp = settings.Mode.PokemonsPerPlayer();
            P00 = new BallState[ppp];
            P10 = new BallState[ppp];
            if (settings.Mode.PlayersPerTeam() == 2)
            {
                P01 = new BallState[ppp];
                P11 = new BallState[ppp];
            }
        }
        protected ReportFragment(ReportFragment fragment)
        {
            _turnNumber = fragment._turnNumber;
            Weather = fragment.Weather;
            P00 = fragment.P00;
            P10 = fragment.P10;
            P01 = fragment.P01;
            P11 = fragment.P11;
            Pokemons = fragment.Pokemons;
        }
    }
}
