using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.GameEvents;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    class SwitchController : ControllerComponent
    {
        public SwitchController(Controller controller)
            : base(controller)
        {
        }

        public bool CanWithdraw(PokemonProxy pm)
        {
            //黑眼神状态不能防止任何强制交换的技能或道具效果，阻止的是CanSelectSwitch
            return pm.Tile != null && (pm.Hp == 0 || pm.Pokemon.Owner.PmsAlive > GameSettings.Mode.OnboardPokemonsPerPlayer());
        }
        public bool CanSendOut(Tile tile)
        {
            Player p = Controller.GetPlayer(tile);
            return tile.Pokemon == null &&
              (p.PmsAlive > GameSettings.Mode.OnboardPokemonsPerPlayer() ||
              (p.PmsAlive == GameSettings.Mode.OnboardPokemonsPerPlayer() && p.GetPokemon(GameSettings.Mode.GetPokemonIndex(tile.X)).Hp == 0));
        }
        public bool CanSendOut(PokemonProxy pm)
        {
            return pm != null && pm.Hp > 0 && pm.Pokemon.IndexInOwner >= GameSettings.Mode.OnboardPokemonsPerPlayer();
        }

        public bool Withdraw(PokemonProxy pm, string log, int arg1, bool canPursuit)
        {
            if (CanWithdraw(pm))
            {
                if (log != null) pm.ShowLogPm(log, arg1);
                STs.Withdrawing(pm, canPursuit);
                if (pm.Tile != null)
                {
                    ReportBuilder.Withdraw(pm);
                    var ability = pm.Ability;
                    pm.Action = PokemonAction.InBall;
                    pm.Tile.Pokemon = null;
                    pm.OnboardPokemon = pm.NullOnboardPokemon;
                    Controller.ActingPokemons.Remove(pm);
                    ATs.Withdrawn(pm, ability);
                    return true;
                }
            }
            return false;
        }
        private PokemonProxy SendOutImplement(Tile tile)
        {
            var origin = tile.WillSendOutPokemonIndex;
            var pm = Controller.GetPlayer(tile).GetPokemon(origin);
            pm.Action = PokemonAction.Debuting;
            tile.Pokemon = pm;
            tile.WillSendOutPokemonIndex = Tile.NOPM_INDEX;
            pm.OnboardPokemon = new OnboardPokemon(pm.Pokemon, tile.X);
            STs.SendingOut(pm);
            Controller.ActingPokemons.Insert(0, pm);
            ReportBuilder.SendOut(pm, origin);
            return pm;
        }
        public void GameStartSendOut(IEnumerable<Tile> tiles)
        {
            var pms = tiles.Select(SendOutImplement).ToArray();
            string log;
            int pm1, pm2;
            if (pms.Length == 1)
            {
                log = Ls.SendOut1;
                pm1 = pm2 = 0;
            }
            else if (pms.Length == 2)
            {
                log = pms[0].Pokemon.Owner == pms[1].Pokemon.Owner ? Ls.SendOut2 : Ls.SendOut22;
                pm1 = pms[1].Id;
                pm2 = 0;
            }
            else
            {
                log = Ls.SendOut3;
                pm1 = pms[1].Id;
                pm2 = pms[2].Id;
            }
            ReportBuilder.ShowLog(log, pms[0].Id, pm1, pm2);
        }
        public bool SendOut(Tile tile, bool debut, string log)
        {
            Player p = Controller.GetPlayer(tile);
            int origin = GameSettings.Mode.GetPokemonIndex(tile.X);
            int sendout = tile.WillSendOutPokemonIndex;
            if (CanSendOut(tile) && CanSendOut(p.GetPokemon(sendout)))
            {
                var pm = SendOutImplement(tile);
                p.SwitchPokemon(origin, sendout);
                pm.ShowLogPm(log);
                pm.OnboardPokemon.AddTurnCondition(Cs.Switched);
                ATs.Trace(pm);
                if (debut)
                {
                    ATs.AttachUnnerve(Controller);
                    pm.Debut();
                    if (pm.Hp != 0) ATs.AttachWeatherObserver(pm);
                    ATs.WeatherChanged(Controller);
                }
                return true;
            }
            return false;
        }
    }
}
