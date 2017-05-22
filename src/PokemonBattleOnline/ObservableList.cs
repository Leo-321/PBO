using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PokemonBattleOnline
{
  public class ObservableList<T> : ObservableCollection<T>
  {
    private readonly Action<NotifyCollectionChangedEventArgs> BaseOnCollectionChanged;
    private readonly Action<PropertyChangedEventArgs> BaseOnPropertyChanged;

    public ObservableList()
      : base()
    {
      BaseOnCollectionChanged = base.OnCollectionChanged;
      BaseOnPropertyChanged = base.OnPropertyChanged;
    }
    public ObservableList(IEnumerable<T> list)
      : base(list)
    {
      BaseOnCollectionChanged = base.OnCollectionChanged;
      BaseOnPropertyChanged = base.OnPropertyChanged;
    }
    
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      UIDispatcher.BeginInvoke(BaseOnCollectionChanged, e);
    }
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      UIDispatcher.BeginInvoke(BaseOnPropertyChanged, e);
    }
  }
}
