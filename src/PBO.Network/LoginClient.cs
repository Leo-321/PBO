using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PokemonBattleOnline.Network
{
#if TEST
    public
#else
  internal
#endif
    class LoginClient : IPackReceivedListener
    {
        private static void OnLoginTimeout(object state)
        {
            var c = (LoginClient)state;
            c.OnLoginFailed(Disconnect);
        }

        public static event Action<Client> LoginSucceed;
        public static event Action Disconnect;
        public static event Action<UInt16> BadVersion;
        public static event Action BadName;
        public static event Action Full;

        internal readonly string Server;
        internal readonly int Port;
        internal readonly string Name;
        internal readonly ushort Avatar;
        private readonly Timer TimeBomb;

        public LoginClient(string server, int port, string name, ushort avatar)
        {
            Server = server;
            Port = port;
            Name = name;
            Avatar = avatar;
            TimeBomb = new Timer(OnLoginTimeout, this, Timeout.Infinite, Timeout.Infinite);
        }

        public void BeginLogin()
        {
            System.Net.IPAddress ip;
            if (System.Net.IPAddress.TryParse(Server, out ip)) TcpClient.BeginConnect(ip, Port, TcpConnectCallback);
            else TcpClient.BeginConnect(Server, Port, TcpConnectCallback);
        }

        private TcpClient Network;
        private void TcpConnectCallback(TcpClient client)
        {
            if (client == null) OnLoginFailed(Disconnect);
            else
            {
                client.Disconnected += OnLoginFailed;
                TimeBomb.Change(PBOMarks.TIMEOUT, Timeout.Infinite);
                Network = client;
                Network.Listener = this;
                Network.Sender.Send(PBOMarks.VERSION.ToPack());
            }
        }

        private bool isFailed;
        private void OnLoginFailed(Action raiseEvent)
        {
            if (!isFailed)
            {
                TimeBomb.Dispose();
                UIDispatcher.Invoke(raiseEvent);
                if (Network != null) Network.Dispose();
                isFailed = true;
            }
        }
        private void OnBadVersion(UInt16 version)
        {
            if (!isFailed)
            {
                TimeBomb.Dispose();
                UIDispatcher.Invoke(BadVersion, version);
                if (Network != null) Network.Dispose();
                isFailed = true;
            }
        }
        private void OnLoginFailed()
        {
            if (state == 1) OnLoginFailed(BadName);
            else OnBadVersion(0);
        }

        private byte state;
        void IPackReceivedListener.OnPackReceived(byte[] pack)
        {
            switch (state)
            {
                case 0: //version，失败直接结束，满员
                    if (pack.IsEmpty())
                    {
                        state = 1;
                        Network.Sender.Send(Name.ToPack());
                    }
                    else if (pack.ToByte() == 'f') OnLoginFailed(Full); //实际上这条消息在version发送前就会收到
                    else
                    {
                        var v = pack.ToUInt16();
                        OnBadVersion(v ?? 0);
                    }
                    break;
                case 1:
                    if (pack.IsEmpty())
                    {
                        state = 2;
                        Network.Sender.Send(Avatar.ToPack());
                    }
                    else OnLoginFailed(BadName);
                    break;
                case 2: //ClientStartUpInfo
                    var init = pack.ToObject<ClientInitInfo>();
                    if (init == null) OnLoginFailed(Disconnect);
                    else
                    {
                        TimeBomb.Dispose();
                        LoginSucceed(new Client(Network, this, init));
                    }
                    break;
            }
        }
    }
}
