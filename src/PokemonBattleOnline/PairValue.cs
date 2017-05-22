using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PokemonBattleOnline
{
  public interface IPairValue : INotifyPropertyChanged
  {
    int Origin { get; }
    int Value { get; }
  }

  [DataContract(Name = "pv", Namespace = PBOMarks.PBO)]
  public class PairValue : ObservableObject, IPairValue
  {
    public PairValue(int origin, int value)
    {
      this._origin = origin;
      this._value = value;
    }
    public PairValue(int origin)
      : this(origin, origin)
    {
    }

    [DataMember(Name = "o", EmitDefaultValue = false)]
    int _origin;
    public int Origin
    { get { return _origin; } }
    [DataMember(Name = "v", EmitDefaultValue = false)]
    int _value;
    public int Value
    { 
      get { return _value; }
      set
      {
        if (_value != value)
        {
          _value = value;
          OnPropertyChanged();
        }
      }
    }

    public override string ToString()
    {
      return _value.ToString();
    }
  }
}
