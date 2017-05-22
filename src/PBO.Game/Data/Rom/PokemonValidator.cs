using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokemonBattleOnline.Game
{
  public static class PokemonValidator
  {

    #region 6D

    public static bool ValidateIv(this I6D iv)
    {
      throw new NotImplementedException();
    }

    public static bool ValidateEv(this I6D ev)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IPokemonData

    public static bool ValidateName(string name)
    {
      //return Regex.IsMatch(name, @"^\w{1,20}$", RegexOptions.Compiled);
      return name == null || name.Length < 11 && !name.Any(char.IsWhiteSpace);
    }

    public static bool Shiney(IPokemonData pm, int random)
    {
      return random % 1366 == 0;
    }

    public static bool ValidateLv(int lv)
    {
      return 0 < lv && lv <= 100;
    }

    public static bool ValidateMoves(IPokemonData pm)
    {
      return pm.Moves.Count() <= 4;
    }

    public static bool Validate(this IPokemonData pm)
    {
      return true;
      return
        pm.Form != null &&
        ValidateEv(pm.Ev) &&
        ValidateLv(pm.Lv) &&
        ValidateIv(pm.Iv) &&
        ValidateMoves(pm);
    }

    public static bool ValueEquals(this IPokemonData a, IPokemonData b)
    {
      return
        a.Name == b.Name &&
        a.AbilityIndex == b.AbilityIndex &&
        a.Ev.ValueEquals(b.Ev) &&
        a.Iv.ValueEquals(b.Iv) &&
        a.Form == b.Form &&
        a.Gender == b.Gender &&
        a.Happiness == b.Happiness &&
        a.Item == b.Item &&
        a.Lv == b.Lv &&
        a.Moves.SequenceEqual(b.Moves) &&
        a.Nature == b.Nature;
    }

    #endregion

  }
}