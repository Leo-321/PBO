using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PokemonBattleOnline.Game;
using PokemonBattleOnline.PBO.Elements;

namespace PokemonBattleOnline.PBO.Editor
{
  internal class LearnVM : ObservableObject
  {
    private readonly PokemonEditorVM pokemon;

    public LearnVM(PokemonEditorVM pm, int move)
    {
      pokemon = pm;
      Move = RomData.GetMove(move);
      _isLearned = pokemon.Model.HasMove(move);
      _methods = new List<LearnMethod>();
    }
    
    public MoveType Move
    { get; private set; }

    public string Name
    { get { return GameString.Current.Move(Move.Id); } }
    public string BackupName
    { get { return GameString.Backup.Move(Move.Id); } }

    private readonly List<LearnMethod> _methods;
    public IEnumerable<LearnMethod> Methods
    { get { return _methods; } }

    private static readonly PropertyChangedEventArgs ISLEARNED = new PropertyChangedEventArgs("IsLearned");
    private bool _isLearned;
    public bool IsLearned
    {
      get
      {
        return _isLearned;
      }
      set
      {
        if (_isLearned != value)
        {
          _isLearned = value;
          OnPropertyChanged(ISLEARNED);
        }
      }
    }

    internal void AddMethod(LearnCategory method)
    {
      _methods.Add(new LearnMethod(method));
    }
  }
}
