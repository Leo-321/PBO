using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
  [DataContract(Namespace = PBOMarks.PBO)]
  public class Evolution
  {
    internal Evolution(int from, int to)
    {
      _from = (short)from;
      _to = (short)to;
    }
    
    [DataMember(Name = "F")]
    private readonly int _from;  
    public int From
    { get { return _from; } }
    
    [DataMember(Name = "T")]
    private readonly int _to;  
    public int To
    { get { return _to; } }
    
    public bool NeedLvUp
    { get { return false; } }
  }
}
