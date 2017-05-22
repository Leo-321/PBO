using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.GameEvents;

namespace PokemonBattleOnline.Game.Host
{
    internal class ReportBuilder
    {
        private readonly Controller Controller;
        private readonly DateTime Begin;
        private readonly ReportFragment fragment;
        private readonly List<GameEvent> events;

        internal ReportBuilder(Controller controller)
        {
            Controller = controller;
            TurnNumber = -1;
            Begin = DateTime.Now;
            fragment = new ReportFragment(controller.GameSettings);
            events = new List<GameEvent>(15);
            RefreshFragment();
        }

        public int TurnNumber
        { get; private set; }

        private void RefreshFragmentImplement()
        {
            fragment.TurnNumber = TurnNumber;
            fragment.Weather = Controller.Board.Weather;
            Controller.Teams[0].GetPlayer(0).GetOutward(fragment.P00);
            Controller.Teams[1].GetPlayer(0).GetOutward(fragment.P10);
            if (Controller.GameSettings.Mode.PlayersPerTeam() == 2)
            {
                Controller.Teams[0].GetPlayer(1).GetOutward(fragment.P01);
                Controller.Teams[1].GetPlayer(1).GetOutward(fragment.P11);
            }
            var pms = new List<PokemonOutward>();
            foreach (PokemonProxy p in Controller.Board.Pokemons) pms.Add(p.GetOutward());
            fragment.Pokemons = pms.ToArray();
        }
        public GameEvent[] RefreshFragment()
        {
            RefreshFragmentImplement();
            var r = events.ToArray();
            events.Clear();
            return r;
        }
        public ReportFragment GetFragment()
        {
            return fragment;
        }
        public GameEvent[] GameEnd(bool lose0, bool lose1)
        {
            Add(new GameEnd(lose0, lose1));
            return events.ToArray();
        }
        internal void TimeTick()
        {
            var s = (int)((DateTime.Now - Begin).TotalSeconds);
            if (s != 0) Add(new TimeTick(s));
        }
        internal void NewTurn()
        {
            ++TurnNumber;
            Add(new BeginTurn());
        }

        public void Add(GameEvent e)
        {
            events.Add(e);
        }
        public void ShowLog(string key, int arg0 = 0, int arg1 = 0, int arg2 = 0)
        {
#if DEBUG
            if (key == null) System.Diagnostics.Debugger.Break();
#endif
            Add(new ShowLog(key, arg0, arg1, arg2));
        }
        public void AddHorizontalLine()
        {
            var last = events.LastOrDefault();
            if (!(last is HorizontalLine || last is TimeTick || last is BeginTurn)) Add(new HorizontalLine());
        }
        public void Mimic(PokemonProxy pm, MoveTypeE move)
        {
            Add(new Mimic() { Pm = pm.Id, Move = move.Id });
        }
        public void SetPP(MoveProxy move)
        {
            Add(new SetPP() { Pm = move.Owner.Id, Move = move.Move.Type.Id, PP = move.PP });
        }
        public void SetItem(Pokemon pm)
        {
            Add(new SetItem() { Pm = pm.Id, Item = pm.Item });
        }
        public void SetHp(Pokemon pm)
        {
            Add(new SetHp() { Pm = pm.Id, Hp = pm.Hp });
        }
        public void ShowHp(PokemonProxy pm)
        {
            Add(new ShowHp() { Pm = pm.Id, Hp = pm.Hp });
        }
        public void SetState(Pokemon pm)
        {
            Add(new SetState() { Pm = pm.Id, State = pm.State });
        }
        public void Transform(PokemonProxy pm)
        {
            var o = pm.GetOutward();
            var moves = new int[pm.Moves.Count()];
            var i = 0;
            foreach (var m in pm.Moves) moves[i++] = m.Move.Type.Id;
            Add(new SetOutward() { Pm = pm.Id, Number = o.Form.Species.Number, Form = o.Form.Index, Moves = moves });
        }
        public void DeIllusion(PokemonProxy pm)
        {
            var o = pm.GetOutward();
            Add(new SetOutward() { Pm = pm.Id, Number = o.Form.Species.Number, Form = o.Form.Index, Name = o.RawName, Gender = o.Gender });
        }
        public void ChangeForm(PokemonProxy pm, bool forever)
        {
            Add(new SetOutward() { Pm = pm.Id, Form = pm.OnboardPokemon.Form.Index, Forever = forever });
        }
        public void EnSubstitute(PokemonProxy pm)
        {
            Add(new Substitute() { Pm = pm.Id });
        }
        public void DeSubstitute(PokemonProxy pm)
        {
            Add(new Substitute() { Pm = pm.Id, De = true });
        }
        public void SendOut(PokemonProxy pm, int formerIndex)
        {
            Add(new SendOut() { Pm = pm.GetOutward(), FormerIndex = formerIndex });
        }
        public void Withdraw(PokemonProxy pm)
        {
            Add(new Withdraw() { Pm = pm.Id });
        }
        public void SetWeather(Weather weather)
        {
            Add(new SetWeather(weather));
        }
        public void SetY(PokemonProxy pm)
        {
            Add(new SetY() { Pm = pm.Id, Y = pm.CoordY });
        }
        public void SetX(PokemonProxy pm)
        {
            Add(new SetX() { Pm = pm.Id, X = pm.OnboardPokemon.X });
        }
    }
}
