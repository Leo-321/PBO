using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokemonBattleOnline.Network
{
    internal class LoginUser : IPackReceivedListener, IDisposable
    {
        public readonly TcpUser Network;
        private readonly LoginServer Server;

        public LoginUser(TcpUser network, LoginServer server)
        {
            Network = network;
            network.Disconnected += Dispose;
            network.Listener = this;
            Server = server;
        }

        public string Name
        { get; private set; }
        public ushort Avatar
        { get; private set; }

        private byte state;
        void IPackReceivedListener.OnPackReceived(byte[] pack)
        {
            try
            {
                //当前版本失败一律直接结束，不给重试
                switch (state)
                {
                    case 0: //version
                        var v = pack.ToUInt16();
                        if (v != PBOMarks.VERSION)
                        {
                            Dispose();
                        }
                        else
                        {
                            state = 1;
                            Network.Sender.SendEmpty();
                        }
                        break;
                    case 1: //Name
                        var name = pack.ToUnicodeString();
                        if (Server.RegisterName(this, name))
                        {
                            state = 2;
                            Name = name;
                            Network.Sender.SendEmpty();
                        }
                        else Dispose();
                        break;
                    case 2: //Avatar
                        var av = pack.ToUInt16();
                        if (av.HasValue)
                        {
                            Avatar = av.Value;
                            Server.LoginComplete(this);
                        }
                        else Dispose();
                        break;
                }
            }
            catch
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            Server.BadLogin(this);
            Network.Dispose();
        }
    }
}
