using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace PokemonBattleOnline.PBO
{
  public class SimpleCommand : ICommand
  {
    public event EventHandler CanExecuteChanged;
    Action c1;
    Action<object> c2;

    public SimpleCommand()
    {
      c1 = delegate { };
    }
    public SimpleCommand(Action command)
    {
      c1 = command;
    }
    public SimpleCommand(Action<object> command)
    {
      c2 = command;
    }

    bool ICommand.CanExecute(object parameter)
    { return true; }

    public void Execute(object parameter)
    {
      if (c1 != null) c1();
      else c2(parameter);
    }
  }

  public class MenuCommand : ICommand, INotifyPropertyChanged
  {
    private static readonly PropertyChangedEventArgs HEADER = new PropertyChangedEventArgs("Header");
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    private object _header;
    public object Header
    {
      get { return _header; }
      set
      {
        if (_header != value)
        {
          _header = value;
          if (PropertyChanged != null) PropertyChanged(this, HEADER);
        }
      }
    }

    public object Icon
    { get; set; }

    private bool _isEnabled;
    public bool IsEnabled
    {
      get
      {
        return _isEnabled;
      }
      set
      {
        if (_isEnabled != value)
        {
          _isEnabled = value;
          OnCanExecuteChanged();
        }
      }
    }

    private Action action;

    public MenuCommand(object header, Action commandAction)
    {
      this.Header = header;
      this.action = commandAction;
      this.IsEnabled = true;
    }

    public void Execute(object parameter)
    {
      UIDispatcher.Invoke(action);
    }

    public bool CanExecute(object parameter)
    {
      return IsEnabled;
    }

    public event EventHandler CanExecuteChanged = delegate { };
    private void OnCanExecuteChanged()
    {
      UIDispatcher.Invoke(CanExecuteChanged, this, EventArgs.Empty);
    }
  }
}
