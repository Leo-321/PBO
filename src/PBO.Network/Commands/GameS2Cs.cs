using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Network.Commands
{
    [DataContract(Name = "sp", Namespace = PBOMarks.JSON)]
    public class SetPrepareS2C : IS2C
    {
        [DataMember(Name = "a", EmitDefaultValue = false)]
        Seat Seat;
        [DataMember(Name = "b", EmitDefaultValue = false)]
        bool Prepare;

        public SetPrepareS2C(Seat seat, bool prepare)
        {
            Seat = seat;
            Prepare = prepare;
        }

        void IS2C.Execute(Client client)
        {
            var room = client.Controller.Room;
            switch (Seat)
            {
                case Seat.Player00:
                    room.Prepare00 = Prepare;
                    break;
                case Seat.Player01:
                    room.Prepare01 = Prepare;
                    break;
                case Seat.Player10:
                    room.Prepare10 = Prepare;
                    break;
                case Seat.Player11:
                    room.Prepare11 = Prepare;
                    break;
            }
        }
    }

    [DataContract(Name = "pi", Namespace = PBOMarks.JSON)]
    public class PartnerInfoS2C : IS2C
    {
        [DataMember]
        PokemonData[] a_;

        public PartnerInfoS2C(IPokemonData[] pms)
        {
            a_ = new PokemonData[pms.Length];
            for (int i = 0; i < pms.Length; ++i) a_[i] = (PokemonData)pms[i];
        }

        void IS2C.Execute(Client client)
        {
            client.Controller.Room.Partner = a_;
        }
    }

    [DataContract(Name = "ri", Namespace = PBOMarks.JSON)]
    public class RequireInputS2C : InputRequest, IS2C
    {
        public RequireInputS2C(InputRequest ir)
          : base(ir)
        {
        }

        void IS2C.Execute(Client client)
        {
            client.Controller.Room.InputRequest = this;
        }
    }

    [DataContract(Name = "wi", Namespace = PBOMarks.JSON)]
    public class WaitingForInputS2C : IS2C
    {
        [DataMember(Name = "a")]
        int[] Players;

        public WaitingForInputS2C(int[] players)
        {
            Players = players;
        }

        void IS2C.Execute(Client client)
        {
            RoomController.OnTimeReminder(Players.Select((p) => client.Controller.GetUser(p)).ToArray());
        }
    }

    [DataContract(Name = "gs", Namespace = PBOMarks.JSON)]
    public class GameStartS2C : ReportFragment, IS2C
    {
        public GameStartS2C(ReportFragment rf)
          : base(rf)
        {
        }

        void IS2C.Execute(Client client)
        {
            client.Controller.Room.GameStart(this);
        }
    }

    [DataContract(Name = "gu", Namespace = PBOMarks.JSON)]
    public class GameUpdateS2C : IS2C
    {
        [DataMember]
        public GameEvent[] Es;

        public GameUpdateS2C(GameEvent[] events)
        {
            Es = events;
        }

        void IS2C.Execute(Client client)
        {
            client.Controller.Room.Update(Es);
        }
    }

    [DataContract(Name = "ge", Namespace = PBOMarks.JSON)]
    public class GameEndS2C : IS2C
    {
        public static GameEndS2C GameStop(int player, GameStopReason reason)
        {
            return new GameEndS2C() { Player = player, Reason = reason };
        }
        public static GameEndS2C TimeUp(KeyValuePair<int, int>[] time)
        {
            return new GameEndS2C() { Time = time };
        }
        [DataMember(EmitDefaultValue = false)]
        int Player;
        [DataMember(EmitDefaultValue = false)]
        GameStopReason Reason;

        [DataMember(EmitDefaultValue = false)]
        KeyValuePair<int, int>[] Time;

        private GameEndS2C()
        {
        }
        void IS2C.Execute(Client client)
        {
            if (Player != 0) client.Controller.Room.OnGameStop(Reason, client.Controller.GetUser(Player));
            else if (Time != null) RoomController.OnTimeUp(Time.Select((p) => new KeyValuePair<User, int>(client.Controller.GetUser(p.Key), p.Value)).ToArray());
            else client.Controller.Room.OnGameStop(Reason, null);
            client.Controller.Room.Reset();
        }
    }
}
