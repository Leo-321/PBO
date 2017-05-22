using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
  public class LearnedMove : ObservableObject
  {
    public LearnedMove(MoveType move, int ppUp = 3)
    {
      _move = move;
      _ppUp = (byte)ppUp;
    }
    
    private MoveType _move;
    public MoveType Move
    { get { return _move; } }
    
    private byte _ppUp;
    public int PPUp
    {
      get { return _ppUp; }
      set
      {
        var v = (byte)value;
        if (_ppUp != v && 0 <= v && v <= 3)
        {
          _ppUp = v;
          OnPropertyChanged("PPUp");
          OnPropertyChanged("PP");
        }
      }
    }

    public int PP
    { get { return Move.PP * (5 + PPUp) / 5; } }

    public override bool Equals(object obj)
    {
      var m = obj as LearnedMove;
      return m != null && m.Move == Move && m.PPUp == PPUp;
    }
  }
  public interface IPokemonData
  {
    string Name { get; }
    PokemonForm Form { get; }
    int Lv { get; }
    PokemonGender Gender { get; }
    PokemonNature Nature { get; }
    int AbilityIndex { get; }
    int Item { get; }
    int Happiness { get; }
    I6D Iv { get; }
    I6D Ev { get; }
    IEnumerable<LearnedMove> Moves { get; }
  }
}
