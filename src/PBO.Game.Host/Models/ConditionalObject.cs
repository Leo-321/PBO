using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host
{
  internal abstract class ConditionalObject
  {
    private readonly Dictionary<Cs, object> conditions;
    private readonly HashSet<Cs> turn;

    protected ConditionalObject()
    {
      conditions = new Dictionary<Cs, object>();
      turn = new HashSet<Cs>();
    }

    public bool HasCondition(Cs name)
    {
      return conditions.ContainsKey(name);
    }
    public bool AddCondition(Cs name, object value = null)
    {
      if (conditions.ContainsKey(name)) return false;
      conditions[name] = value;
      return true;
    }
    public void SetCondition(Cs name, object value = null)
    {
      conditions[name] = value;
    }
    public T GetCondition<T>(Cs name)
    {
      return GetCondition(name, default(T));
    }
    public T GetCondition<T>(Cs name, T defaultValue)
    {
      object o;
      if (conditions.TryGetValue(name, out o) && o is T) return (T)o;
      return defaultValue;
    }
    public bool RemoveCondition(Cs name)
    {
      return conditions.Remove(name);
    }
    public bool AddTurnCondition(Cs name, object value = null)
    {
      if (AddCondition(name, value))
      {
        turn.Add(name);
        return true;
      }
      return false;
    }
    public void SetTurnCondition(Cs name, object value = null)
    {
      SetCondition(name, value);
      turn.Add(name);
    }
    public Condition GetCondition(Cs name)
    {
      return GetCondition<Condition>(name);
    }
    public void ClearTurnCondition()
    {
      foreach (var c in turn) RemoveCondition(c);
      turn.Clear();
    }
  }
}
