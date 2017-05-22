using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Editor
{
  class TeamVM : ObservableObject
  {
    public static TeamVM New;

    public PokemonTeam Model
    { get; private set; }

    public TeamVM(PokemonTeam model)
    {
      Model = model;
      raw = new PokemonVM[6];
      for (int i = 0; i < 6; ++i) raw[i] = new PokemonVM(this, i);
      if (model.CanBattle) EditorVM.Current.BattleTeams.Add(this);
    }

    public string Name
    {
      get { return Model.Name; }
      set
      {
        if (Model.Name != value)
        {
          Model.Name = value;
          OnPropertyChanged("Name");
        }
      }
    }

    public bool CanBattle
    {
      get { return Model.CanBattle; }
      set
      {
        if (Model.CanBattle != value)
        {
          Model.CanBattle = value;
          if (value) EditorVM.Current.BattleTeams.Add(this);
          else EditorVM.Current.BattleTeams.Remove(this);
          OnPropertyChanged("CanBattle");
        }
      }
    }

    private readonly PokemonVM[] raw;
    public PokemonVM this[int index]
    { get { return raw.ValueOrDefault(index); } }
  }
}
