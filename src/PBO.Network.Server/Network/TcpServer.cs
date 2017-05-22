using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace PokemonBattleOnline.Network
{
    internal class TcpServer
    {
        private static void OnKeepAlive(object state)
        {
#if !TEST
            var server = (TcpServer)state;
            var lastPack = DateTime.Now.AddMilliseconds(-2d * PBOMarks.TIMEOUT);
            var users = new List<TcpUser>(50);
            lock (server.Locker)
            {
                foreach (var u in server.Users.Values)
                    if (u.LastPack < lastPack) users.Add(u);
            }
            foreach (var u in users) u.Dispose();
#endif
        }

        public event Action<TcpUser> NewComingUser;

        private readonly Dictionary<int, TcpUser> Users;
        private readonly Stack<int> Ids;
        private int NewId;
        private readonly int Port;
        private readonly Timer KeepAliveTimer;
        private readonly object ListenerLocker;
        public readonly List<IPAddress> Banlist;
        private readonly object Locker;

        public TcpServer(int port)
        {
            Users = new Dictionary<int, TcpUser>(100);
            Ids = new Stack<int>(100);
            Port = port;
            KeepAliveTimer = new Timer(OnKeepAlive, this, PBOMarks.TIMEOUT << 1, PBOMarks.TIMEOUT << 1);
            ListenerLocker = new object();
            Banlist = new List<IPAddress>();
            Locker = new object();
        }

        private Socket listener;
        private bool _isListening;
        public bool IsListening
        {
            get
            {
                lock (ListenerLocker)
                {
                    return _isListening;
                }
            }
            set
            {
                lock (ListenerLocker)
                {
                    if (_isListening != value)
                    {
                        _isListening = value;
                        if (value)
                        {
                            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                            listener.Bind(new IPEndPoint(IPAddress.Any, Port));
                            listener.Listen(32);
                            StartAccept(null);
                        }
                        else
                        {
                            listener.Close();
                            listener.Dispose();
                            listener = null;
                        }
                    }
                }
            }
        }
        public IPEndPoint ListenerEndPoint
        { get { return (IPEndPoint)listener.LocalEndPoint; } }

        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += ProcessAccept;
            }
            else acceptEventArg.AcceptSocket = null;
            bool willRaiseEvent = listener.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent) ProcessAccept(null, acceptEventArg);
        }
        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            Socket s = e.AcceptSocket;
            IPAddress ip = ((IPEndPoint)s.RemoteEndPoint).Address;
            Console.WriteLine("({0}) {1} is trying to login.", DateTime.Now, ip);
            if (!Banlist.Contains(ip))
            {
                s.LingerState = new LingerOption(true, 5);
                TcpUser u;
                lock (Locker)
                {
                    var id = Ids.Count == 0 ? ++NewId : Ids.Pop();
                    u = new TcpUser(id, this, s);
                    Users.Add(id, u);
                }
                NewComingUser(u);
            }
            if (IsListening) StartAccept(e);
        }

        internal void Remove(TcpUser user)
        {
            TcpUser u;
            lock (Locker)
            {
                if (Users.TryGetValue(user.Id, out u) && user == u)
                {
                    Users.Remove(user.Id);
                    Ids.Push(user.Id);
                }
            }
        }

        public void Dispose()
        {
            NewComingUser = delegate { };
            IsListening = false;
            lock (Locker)
            {
                foreach (var u in Users.Values) u.Dispose();
            }
        }
    }
}
