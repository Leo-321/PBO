using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace PokemonBattleOnline.Network
{
    public class Server : IDisposable
    {
        internal readonly object Locker;
        internal readonly TcpServer Network;
        private readonly LoginServer Login;
        private readonly Dictionary<int, ServerUser> Users;
        private readonly Dictionary<int, RoomHost> Rooms;
        public List<IPAddress> Banlist;

        internal Server(int port)
        {
            Locker = new object();
            Network = new TcpServer(port);
            Login = new LoginServer(Network, this);
            Users = new Dictionary<int, ServerUser>();
            Rooms = new Dictionary<int, RoomHost>();
            Banlist = Network.Banlist;
        }

        public void Start()
        {
            Network.IsListening = true;
        }
        /// <summary>
        /// non-lock
        /// </summary>
        internal ServerUser GetUser(int id)
        {
            return Users.ValueOrDefault(id);
        }
        internal void AddUser(ServerUser user)
        {
            lock (Locker)
            {
                user.Network.Sender.Send(GetCII(user.Id).ToPack());
                Send(Commands.UserS2C.AddUser(user.Id, user.User.Name, user.User.Avatar));
                Users.Add(user.Id, user);
            }
            Console.WriteLine("({0}) {1} has entered the lobby.", DateTime.Now, user.User.Name);
        }

        public void ListUsers()
        {
            foreach (ServerUser user in Users.Values)
            {
                Console.WriteLine(user.User.Name + " " + user.Network.EndPoint.Address.ToString());
            }
        }
        public void BanIp(IPAddress ip)
        {
            Banlist.Add(ip);
            List<ServerUser> tmp = new List<ServerUser>();
            foreach (KeyValuePair<int, ServerUser> ea in Users)
                if (ea.Value.Network.EndPoint.Address.Equals(ip))
                    tmp.Add(ea.Value);

            foreach (var i in tmp)
                RemoveUser(i);
        }
        public void UnbanIp(IPAddress ip)
        {
            Banlist.Remove(ip);
        }
        private ClientInitInfo GetCII(int user)
        {
            //already in lock
            var lus = new List<User>();
            var rs = new Room[Rooms.Count];
            foreach (var u in Users.Values)
                if (u.Room == null) lus.Add(u.User);
            int i = 0;
            foreach (var r in Rooms.Values) rs[i++] = r.Room;
            return new ClientInitInfo(user, lus.ToArray(), rs);
        }


        internal void RemoveUser(ServerUser user)
        {
            lock (Locker)
            {
                if (user.Room != null) user.Room.RemoveUser(user);
                Users.Remove(user.Id);
                Send(Commands.UserS2C.RemoveUser(user.Id));
            }
            Login.RemoveName(user.User.Name);
            Console.WriteLine("({0}) {1} has left the lobby.", DateTime.Now, user.User.Name);
        }
        internal RoomHost GetRoom(int id)
        {
            return Rooms.ValueOrDefault(id);
        }

        private int RoomId;
        internal RoomHost AddRoom(GameSettings settings)
        {
            var id = Interlocked.Increment(ref RoomId);
            var rc = new RoomHost(this, id, settings);
            Rooms.Add(id, rc);
            Send(Commands.RoomS2C.NewRoom(id, settings));
            return rc;
        }

        internal void RemoveRoom(RoomHost rc)
        {
            var room = rc.Room;
            if (Rooms.Remove(room.Id))
            {
                rc.Dispose();
                room.RemoveUsers();
                Send(Commands.RoomS2C.RemoveRoom(room.Id));
            }
        }

        internal void Send(IS2C s2c)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var ds = new DeflateStream(ms, CompressionMode.Compress))
                        ServerUser.S2CSerializer.WriteObject(ds, s2c);
                    var pack = ms.ToArray();
                    foreach (var u in Users.Values) u.Send(pack);
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Network.Dispose();
        }
    }
}
