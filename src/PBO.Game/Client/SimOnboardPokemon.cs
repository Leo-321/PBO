using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public class SimOnboardPokemon
    {
        public readonly PokemonOutward Outward;
        public int X;

        internal SimOnboardPokemon(SimPokemon pokemon, PokemonOutward outward)
        {
            _pokemon = pokemon;
            Outward = outward;
            X = outward.Position.X;
            Moves = new SimMove[4];
            for (int i = 0; i < pokemon.Moves.Length; ++i)
            {
                    Moves[i] = new SimMove(pokemon.Moves[i]);
            }
            ZMoves = new SimMove[4];
            for (int i = 0; i < pokemon.Moves.Length; ++i)
            {
                var x = GameHelper.Zmove(pokemon.Moves[i], pokemon.Item, pokemon.Form.Species.Number, pokemon.Form.Index);
                if (x != null)
                    ZMoves[i] = new SimMove(x);
            }

            DetailOfMove = new string[4];
            ZDetailOfMove = new string[4];
            for (int i = 0; i < pokemon.Moves.Length; ++i)
            {
                DetailOfMove[i] = detail(Moves[i],pokemon);
                if(ZMoves[i]!=null)
                    ZDetailOfMove[i] = detail(ZMoves[i], pokemon);
            }
        }

        public int Id
        { get { return Pokemon.Id; } }

        private readonly SimPokemon _pokemon;
        public SimPokemon Pokemon
        { get { return _pokemon; } }

        public SimMove[] Moves
        { get; private set; }

        public SimMove[] ZMoves
        { get; private set; }

        public string[] DetailOfMove
        { get; private set; }

        public string[] ZDetailOfMove
        { get; private set; }

        internal void ChangeMoves(int[] moves)
        {
            int i = -1;
            while (++i < moves.Length) Moves[i] = new SimMove(RomData.GetMove(moves[i]));
            while (i < 4) Moves[i++] = null;
        }

        public void ChangeMove(int from, int to)
        {
            for (int i = 0; i < 4; ++i)
                if (Moves[i] != null && Moves[i].Type.Id == from)
                {
                    Moves[i] = new SimMove(RomData.GetMove(to));
                    break;
                }
        }

        private string detail(SimMove move, SimPokemon pokemon)
        {
            int p = move.Type.Power;
            int a = move.Type.Accuracy;

            string power = p.ToString();
            string acc = a.ToString();
            if (p == 0)
                power = "--";
            if (p == 1)
                power = "不定";
            if (a == 0)
                acc = "--";

            if (move.Type.Id == Ms.NATURAL_GIFT)
                power = GameHelper.NaturalGiftPower(pokemon.Item).ToString();
            if (move.Type.Id == Ms.FLING)
                power = GameHelper.FlingPower(pokemon.Item).ToString();
            if (move.Type.Id == Ms.RETURN)
                power = (pokemon.Happiness * 4 / 10).ToString();
            if (move.Type.Id == Ms.FRUSTRATION)
                power = ((255 - pokemon.Happiness) * 4 / 10).ToString();

            return "威力:" + power + " 命中率:" + acc + '\n' + GameString.Current.MoveD(move.Type.Id);
        }

    }
}
