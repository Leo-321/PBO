using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PokemonBattleOnline.Game
{
  public class GameString
  {
    private class Comparer : IComparer<KeyValuePair<string, string>>
    {
      public static readonly Comparer I = new Comparer();
      public int Compare(KeyValuePair<string, string> a, KeyValuePair<string, string> b)
      {
        return String.Compare(a.Key, b.Key, StringComparison.InvariantCultureIgnoreCase);
      }
    }
    public static GameString JP
    { get; private set; }
    public static GameString EN
    { get; private set; }
    public static GameString Current
    { get; private set; }
    public static GameString Backup
    { get; private set; }
    private static GameString InnerBackup;
    private static List<KeyValuePair<string, string>> Redirections;

    public static void Load(string path, string language, string backup)
    {
      Redirections = new List<KeyValuePair<string, string>>();
      Current = TryLoad(path, language);
      if (backup != null) Backup = TryLoad(path, backup);
      if (EN == null) TryLoad(path, "en");
      if (JP == null) TryLoad(path, "jp");
      InnerBackup = Backup ?? EN ?? JP ?? Current;
      if (Current == null) Current = InnerBackup;
      for (int i = 1; i < 100; ++i)
      {
        Redirections.Add(new KeyValuePair<string, string>(i.ToString("000"), "p" + i + "00"));
        Redirections.Add(new KeyValuePair<string, string>(i.ToString(), "p" + i + "00"));
      }
      for (int i = 100; i <= RomData.POKEMONS; ++i) Redirections.Add(new KeyValuePair<string, string>(i.ToString(), "p" + i + "00"));
      Redirections.Sort(Comparer.I);
      Redirections.TrimExcess();
    }
    private static GameString TryLoad(string basePath, string language)
    {
      try
      {
        var gs = new GameString(basePath + "\\" + language, language);
        if (language == "en") EN = gs;
        else if (language == "jp") JP = gs;
        return gs;
      }
      catch
      {
        return null;
      }
    }
    private static GameString GetLanguage(string str)
    {
      var c = str.FirstOrDefault((ch) => !char.IsDigit(ch));
      if ('a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' && EN != null) return EN;
      if (0x3040 < c && c < 0x309f && JP != null) return JP;
      return Current;
    }
    private static int IndexOf(string[] list, string name)
    {
      if (list != null)
      {
        int r;
        for (r = 0; r < list.Length; ++r)
          if (list[r] != null && list[r].StartsWith(name, StringComparison.CurrentCultureIgnoreCase)) break;
        if (r != list.Length)
        {
          for (int i = r; i < list.Length; ++i)
            if (list[i] != null && list[i].Equals(name, StringComparison.CurrentCultureIgnoreCase)) return i;
          return r;
        }
      }
      return -1;
    }
    /// <summary>
    /// return PokemonForm, Ability, Move, Item or Nature
    /// </summary>
    /// <param name="name"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static string Find(string name, out int end)
    {
      if (!string.IsNullOrWhiteSpace(name))
      {
        var i = Redirections.BinarySearch(new KeyValuePair<string, string>(name, null), Comparer.I);
        if (i < 0) i = ~i;
        if (i < Redirections.Count)
        {
          var pair = Redirections[i];
          var key = pair.Key;
          for (end = 0; end < key.Length && end < name.Length; end++)
            if (char.ToLowerInvariant(key[end]) != char.ToLowerInvariant(name[end])) break;
          if (end == name.Length || char.IsWhiteSpace(name[end]))
          {
            while (end < name.Length && char.IsWhiteSpace(name[end])) end++;
            return pair.Value;
          }
        }
      }
      end = 0;
      return null;
    }
    public static PokemonNature? Nature(string name)
    {
      int e;
      var r = Find(name, out e);
      return r != null && r[0] == 'n' ? (PokemonNature?)int.Parse(r.Substring(1)) : null;
    }
    public static PokemonSpecies PokemonSpecies(string name)
    {
      int e;
      var r = Find(name, out e);
      return r != null && r[0] == 'p' ? RomData.GetPokemon(int.Parse(r.Substring(1)) / 100) : null;
    }
    internal static PokemonForm PokemonForm(string name)
    {
      int e;
      var r = Find(name, out e);
      if (r == null || r[0] != 'p') return null;
      var n = int.Parse(r.Substring(1));
      return RomData.GetPokemon(n / 100, n % 100);
    }
    public static int Ability(string name)
    {
      int e;
      var r = Find(name, out e);
      return r != null && r[0] == 'a' ? int.Parse(r.Substring(1)) : 0;
    }
    public static int Item(string name)
    {
      int e;
      var r = Find(name, out e);
      return r != null && r[0] == 'i' ? int.Parse(r.Substring(1)) : 0;
    }
    public static MoveType Move(string name)
    {
      int e;
      var r = Find(name, out e);
      return r != null && r[0] == 'm' ? RomData.GetMove(int.Parse(r.Substring(1))) : null;
    }

    public readonly string Language;

    private string[] Pokemons;
    private Dictionary<int, string> Forms;
    private Dictionary<int, string> PokemonForms;
    private string[] Moves;
    private string[] Abilities;
    private Dictionary<int, string> Items;
    
    private string[] MovesD;
    private string[] AbilitiesD;
    private Dictionary<int, string> ItemsD;

    private string[] Natures;
    private string[] BattleTypes;
    private string[] MoveCategories;
    private string[] PokemonStates;
    private string[] StatTypes;

    private readonly Dictionary<string, string> BattleLogs;

    private GameString(string path, string language)
    {
      Language = language;
      Pokemons = new string[RomData.POKEMONS];
      Forms = new Dictionary<int, string>();
      PokemonForms = new Dictionary<int, string>();
      Moves = new string[RomData.Moves.Count()];
      Abilities = new string[RomData.ABILITIES];
      Items = new Dictionary<int, string>(RomData.Items.Count());
      using (var sr = new StreamReader(path))
        for (string line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
        {
          string str;
          var comma = line.IndexOf(':');
          str = line.Substring(comma + 1);
          if (char.IsDigit(line[1]))
          {
            var num = int.Parse(line.Substring(1, comma - 1));
            var h = line[0];
            switch (h)
            {
              case 'p':
                if (line[4] == ':')
                {
                  Pokemons[num - 1] = str;
                  num *= 100;
                }
                else PokemonForms[num] = str;
                Redirections.Add(new KeyValuePair<string, string>(str, "p" + num));
                break;
              case 'f':
                Forms[num] = str;
                Redirections.Add(new KeyValuePair<string, string>(str, "f" + num));
                break;
              case 'm':
                Moves[num - 1] = str;
                Redirections.Add(new KeyValuePair<string, string>(str, "m" + num));
                break;
              case 'a':
                Abilities[num - 1] = str;
                Redirections.Add(new KeyValuePair<string, string>(str, "a" + num));
                break;
              case 'i':
                Items[num] = str;
                Redirections.Add(new KeyValuePair<string, string>(str, "i" + num));
                break;
              case 'M':
                if (MovesD == null) MovesD = new string[Moves.Length];
                MovesD[num - 1] = str;
                break;
              case 'A':
                if (AbilitiesD == null) AbilitiesD = new string[Abilities.Length];
                AbilitiesD[num - 1] = str;
                break;
              case 'I':
                if (ItemsD == null) ItemsD = new Dictionary<int, string>(RomData.Items.Count());
                ItemsD[num] = str;
                break;
              case 'n':
                if (Natures == null) Natures = new string[RomData.NATURES];
                Natures[num] = str;
                Redirections.Add(new KeyValuePair<string, string>(str, "n" + num));
                break;
              case 'b':
                if (BattleTypes == null) BattleTypes = new string[RomData.BATTLETYPES];
                BattleTypes[num] = str;
                break;
              case 'c':
                if (MoveCategories == null) MoveCategories = new string[RomData.MOVECATEGORIES];
                MoveCategories[num] = str;
                break;
              case 'S':
                if (PokemonStates == null) PokemonStates = new string[7];
                PokemonStates[num] = str;
                break;
              case 's':
                if (StatTypes == null) StatTypes = new string[8];
                StatTypes[num] = str;
                break;
            }
          }
          else
          {
            var key = line.Substring(0, comma);
            if (BattleLogs == null) BattleLogs = new Dictionary<string, string>();
            BattleLogs[key] = str;
          }
        }//for (string line
    }

    public string Pokemon(int number)
    {
      var i = number - 1;
      return Pokemons[i] ?? InnerBackup.Pokemons[i];
    }
    public string Pokemon(int number, int form)
    {
      if (form == 0) return Pokemon(number);
      var i = number * 100 + form;
      return PokemonForms.ValueOrDefault(i) ?? InnerBackup.PokemonForms.ValueOrDefault(i) ?? Pokemon(number);
    }
    public string Form(int number, int form)
    {
      var i = number * 100 + form;
      return Forms.ValueOrDefault(i) ?? InnerBackup.Forms.ValueOrDefault(i);
    }
    public string Move(int move)
    {
      var i = move - 1;
      return Moves.ValueOrDefault(i) ?? InnerBackup.Moves.ValueOrDefault(i);
    }
    public string Ability(int ability)
    {
      var i = ability - 1;
      return Abilities.ValueOrDefault(i) ?? InnerBackup.Abilities.ValueOrDefault(i);
    }
    public string Item(int item)
    {
      return Items.ValueOrDefault(item) ?? InnerBackup.Items.ValueOrDefault(item);
    }
    public string MoveD(int move)
    {
      var i = move - 1;
      var backup = InnerBackup.MovesD;
      return MovesD == null ? backup == null ? null : backup.ValueOrDefault(i) : MovesD.ValueOrDefault(i);
    }
    public string AbilityD(int ability)
    {
      var i = ability - 1;
      var backup = InnerBackup.AbilitiesD;
      return AbilitiesD == null ? backup == null ? null : backup.ValueOrDefault(i) : AbilitiesD.ValueOrDefault(i);
    }
    public string ItemD(int item)
    {
      var backup = InnerBackup.ItemsD;
      return ItemsD == null ? backup == null ? null : backup.ValueOrDefault(item) : ItemsD.ValueOrDefault(item);
    }
    public string Nature(PokemonNature nature)
    {
      var i = (int)nature;
      var backup = InnerBackup.Natures;
      return Natures == null ? backup == null ? null : backup.ValueOrDefault(i) : Natures.ValueOrDefault(i);
    }
    public string BattleType(BattleType type)
    {
      if (type == Game.BattleType.Invalid) return null;
      var i = (int)type - 1;
      var backup = InnerBackup.BattleTypes;
      return BattleTypes == null ? backup == null ? null : backup.ValueOrDefault(i) : BattleTypes.ValueOrDefault(i);
    }
    public string MoveCategory(MoveCategory category)
    {
      var i = (int)category;
      var backup = InnerBackup.MoveCategories;
      return MoveCategories == null ? backup == null ? null : backup.ValueOrDefault(i) : MoveCategories.ValueOrDefault(i);
    }
    public string StatType(StatType stat)
    {
      if (stat == Game.StatType.Invalid || stat == Game.StatType.All || StatTypes == null) return null;
      return StatTypes.ValueOrDefault(stat == Game.StatType.Hp ? 7 : (int)stat - 1) ?? stat.ToString();
    }
    public string PokemonState(PokemonState state)
    {
      if (state == Game.PokemonState.Normal || PokemonStates == null) return null;
      return PokemonStates.ValueOrDefault((int)state - 1) ?? state.ToString();
    }
    public string BattleLog(string key)
    {
      return BattleLogs.ValueOrDefault(key);
    }
  }
}
