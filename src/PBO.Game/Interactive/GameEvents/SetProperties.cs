using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game.GameEvents
{
  [DataContract(Name = "est", Namespace = PBOMarks.JSON)]
  public class SetState : GameEvent
  {
    [DataMember(Name = "a", EmitDefaultValue = false)]
    public int Pm;
    [DataMember(Name = "b", EmitDefaultValue = false)]
    public PokemonState State;

    protected override void Update()
    {
      var pm = GetPokemon(Pm);
      if (pm != null) pm.State = State;
    }
    public override void Update(SimGame game)
    {
      SimPokemon p = GetPokemon(game, Pm);
      if (p != null) p.State = State;
    }
  }

  [DataContract(Name = "eh", Namespace = PBOMarks.JSON)]
  public class ShowHp : GameEvent
  {
    [DataMember(Name = "a", EmitDefaultValue = false)]
    public int Pm;
    [DataMember(Name = "b", EmitDefaultValue = false)]
    public int Hp;

    protected override void Update()
    {
      var pm = GetPokemon(Pm);
      var h = Hp - pm.Hp.Value;
      pm.Hp.Value = Hp;
      AppendGameLog(h > 0 ? Ls.php : Ls.nhp, LogStyle.Detail | LogStyle.NoBr | LogStyle.HiddenInBattle, h);
      AppendGameLog(Ls.br);
      Sleep = 11 * Math.Abs(h);
    }
    public override void Update(SimGame game)
    {
      var pm = GetPokemon(game, Pm);
      if (pm != null) pm.SetHp(Hp);
    }
  }
  [DataContract(Namespace = PBOMarks.JSON)]
  public class SetHp : GameEvent
  {
    [DataMember(Name = "a", EmitDefaultValue = false)]
    public int Pm;
    [DataMember(Name = "b", EmitDefaultValue = false)]
    public int Hp;

    protected override void Update()
    {
      var pm = GetPokemon(Pm);
      if (pm != null)
      {
        var fh = pm.Hp.Value;
        pm.Hp.Value = Hp;
        Sleep = 11 * (fh > Hp ? fh - Hp : Hp - fh);
      }
    }
    public override void Update(SimGame game)
    {
      var pm = GetPokemon(game, Pm);
      if (pm != null) pm.SetHp(Hp);
    }
  }

  [DataContract(Namespace = PBOMarks.JSON)]
  public class SetY : GameEvent
  {
    [DataMember(EmitDefaultValue = false)]
    public int Pm;
    [DataMember(EmitDefaultValue = false)]
    public CoordY Y;

    protected override void Update()
    {
      var pm = GetPokemon(Pm);
      pm.ChangePosition(pm.Position.X, Y);
    }
  }

  [DataContract(Namespace = PBOMarks.JSON)]
  public class SetX : GameEvent
  {
    [DataMember(EmitDefaultValue = false)]
    public int Pm;
    [DataMember(EmitDefaultValue = false)]
    public int X;

    protected override void Update()
    {
      var pm = GetPokemon(Pm);
      pm.ChangePosition(X, pm.Position.Y);
    }
  }

  [DataContract(Name = "eo", Namespace = PBOMarks.JSON)]
  public class SetOutward : GameEvent
  {
    [DataMember(Name = "a", EmitDefaultValue = false)]
    public int Pm;
    [DataMember(Name = "d", EmitDefaultValue = false)]
    public string Name;
    [DataMember(Name = "e", EmitDefaultValue = false)]
    public int Number;
    [DataMember(Name = "f", EmitDefaultValue = false)]
    public int Form;
    [DataMember(Name = "g", EmitDefaultValue = false)]
    public PokemonGender? Gender;
    [DataMember(Name = "c", EmitDefaultValue = false)]
    public int[] Moves;
    [DataMember(Name = "h", EmitDefaultValue = false)]
    public int Arg;
    [DataMember(Name = "i", EmitDefaultValue = false)]
    private bool _forever;
    public bool Forever
    { 
      get { return _forever; }
      set { _forever = value; }
    }

    protected override void Update()
    {
      var pm = GetPokemon(Pm);
      if (Name != null) pm.Name = Name;
      if (Gender != null) pm.Gender = Gender.Value;
      pm.ChangeImage(Number == 0 ? pm.Form.Species.Number : Number, Form);
      Sleep = 500;
    }
    public override void Update(SimGame game)
    {
      if (Number == 0)
      {
        var pm = GetPokemon(game, Pm);
        if (pm != null) pm.ChangeForm(Form, Forever);
      }
      else if (Moves != null)
      {
        var pm = GetOnboardPokemon(game, Pm);
        if (pm != null && pm.Pokemon.Owner == game.Player) pm.ChangeMoves(Moves);
      }
    }
  }
}
