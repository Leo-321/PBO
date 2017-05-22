using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Runtime.Serialization.Json;

namespace PokemonBattleOnline.Network
{
    internal class ServerUser : IPackReceivedListener, IDisposable
    {
        private static readonly DataContractJsonSerializer C2SSerializer;
        public static readonly DataContractJsonSerializer S2CSerializer;

        static ServerUser()
        {
            var c2s = typeof(IC2SE);
            C2SSerializer = new DataContractJsonSerializer(c2s, c2s.SubClasses());
            var s2c = typeof(IS2C);
            S2CSerializer = new DataContractJsonSerializer(s2c, s2c.SubClasses());
        }

        public readonly TcpUser Network;
        public readonly Server Server;
        public readonly int Id;

        public ServerUser(int id, LoginUser user, Server server)
        {
            Id = id;
            Network = user.Network;
            Network.Disconnected += Dispose;
            Network.Listener = this;
            _user = new User(Id, user.Name, user.Avatar);
            Server = server;
        }

        private readonly User _user;
        public User User
        { get { return _user; } }
        public RoomHost Room
        { get { return User.Room == null ? null : Server.GetRoom(User.Room.Id); } }

        void IPackReceivedListener.OnPackReceived(byte[] pack)
        {
            try
            {
                if (!pack.IsEmpty())
                    using (var ms = new MemoryStream(pack, false))
                    using (var ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        var c2s = (IC2SE)C2SSerializer.ReadObject(ds);
                        lock (Server.Locker)
                        {
                            c2s.Execute(this);
                        }
                    }
            }
            catch
            {
                Dispose();
            }
        }
        public void Send(IS2C s2c)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var ds = new DeflateStream(ms, CompressionMode.Compress))
                        S2CSerializer.WriteObject(ds, s2c);
                    Network.Sender.Send(ms.ToArray());
                }
            }
            catch
            {
                Dispose();
            }
        }
        public void Send(byte[] pack)
        {
            try
            {
                Network.Sender.Send(pack);
            }
            catch
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            Server.RemoveUser(this);
            Network.Dispose();
        }
    }
}
