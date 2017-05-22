using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.PBO.Converters
{
    public class SimPokemonIcon : Converter<SimPokemon>
    {
        public static readonly SimPokemonIcon C = new SimPokemonIcon();

        protected override object Convert(SimPokemon value)
        {
            return ImageService.GetPokemonIcon(value.Form, value.Gender);
        }
    }
    public class PokemonOutwardIcon : Converter<PokemonOutward>
    {
        public static readonly PokemonOutwardIcon C = new PokemonOutwardIcon();

        protected override object Convert(PokemonOutward value)
        {
            return ImageService.GetPokemonIcon(value.Form, value.Gender);
        }
    }
    public class PokemonDataIcon : Converter<IPokemonData>
    {
        public static readonly PokemonDataIcon C = new PokemonDataIcon();

        protected override object Convert(IPokemonData value)
        {
            return ImageService.GetPokemonIcon(value.Form, value.Gender);
        }
    }
    public class PokemonSpeciesIcon : Converter<PokemonSpecies>
    {
        public static readonly PokemonSpeciesIcon C = new PokemonSpeciesIcon();

        protected override object Convert(PokemonSpecies value)
        {
            return ImageService.GetPokemonIcon(value.GetForm(0), value.Genders.First());
        }
    }
    public class PokemonFormIcon : Converter<PokemonForm>
    {
        public static readonly PokemonFormIcon C = new PokemonFormIcon();

        protected override object Convert(PokemonForm value)
        {
            return ImageService.GetPokemonIcon(value, value.Species.Genders.First());
        }
    }
}
