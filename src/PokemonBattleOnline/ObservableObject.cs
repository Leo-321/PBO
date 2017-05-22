using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PokemonBattleOnline
{
  [DataContract(Namespace = PBOMarks.PBO)]
  public abstract class ObservableObject : INotifyPropertyChanged
  {
    private static readonly PropertyChangedEventArgs ALL = new PropertyChangedEventArgs(null);

    protected PropertyChangedEventHandler _propertyChanged;
    public event PropertyChangedEventHandler PropertyChanged
    {
      add { _propertyChanged += value; }
      remove { _propertyChanged -= value; }
    }

    protected ObservableObject()
    {
    }

    protected void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (_propertyChanged != null) UIDispatcher.BeginInvoke(_propertyChanged, this, e);
    }
    protected void OnPropertyChanged(string propertyName)
    {
      OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
    protected void OnPropertyChanged()
    {
      OnPropertyChanged(ALL);
    }
  }
}
