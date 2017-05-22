using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host
{
  internal class Condition
  {
    public PokemonProxy By;
    public Tile ByTile;
    //public MoveProxy MoveProxy; //moveProxy是不稳定的存在，一个变身就可以让MoveProxy消失，然后这个可以算内存泄漏...
    public MoveTypeE Move;
    public AtkContext Atk;
    public BattleType BattleType;
    public int Turn;
    public int Damage;
    public int Int;
    public bool Bool;
  }
}
