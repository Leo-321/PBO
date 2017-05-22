using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.Network.Commands;

namespace PokemonBattleOnline.Network
{
    public class PlayerController
    {
        public event Action<InputRequest> RequireInput;

        private readonly Client Client;

        internal PlayerController(RoomController room, IPokemonData[] pokemons, IPokemonData[] partner)
        {
            Client = room._Client;
            var team = room.User.Seat.TeamId();
            var pli = room.User.Seat.TeamIndex();
            var pl = new SimPlayer(team, pli, pokemons);
            var pa = partner == null ? null : new SimPlayer(team, 1 - pli, partner);
            _game = new SimGame(room.Room.Settings, pl, pa);
        }

        public SimPlayer Player
        { get { return _game.Player; } }
        private readonly SimGame _game;
        public SimGame Game
        { get { return _game; } }

        public void Input(ActionInput input)
        {
            Client.Send(new InputC2S(input));
        }
        public void GiveUp()
        {
            Client.Send(new GiveUpC2S());
        }

        internal void OnRequireInput(InputRequest inputRequest)
        {
#if TEST
            if (RequireInput != null)
#endif
                RequireInput(inputRequest);
        }
    }
}
