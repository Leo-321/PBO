using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PokemonBattleOnline.Network
{
    internal class TcpUser : IDisposable
    {
        private event Action _disconnected;
        /// <summary>
        /// always one handler, no need to remove handler
        /// </summary>
        public event Action Disconnected
        {
            add { _disconnected = value; }
            remove { }
        }

        public readonly int Id;
        private readonly TcpServer Server;
        private readonly Socket Socket;
        public readonly TcpPackSender Sender;
        private readonly TcpPackReceiver Receiver;
        private readonly object Locker;

        public TcpUser(int id, TcpServer server, Socket socket)
        {
            Id = id;
            Server = server;
            Socket = socket;
            Sender = new TcpPackSender(socket);
            Receiver = new TcpPackReceiver(socket);
            Locker = new object();
            lock(Locker)
            {
                Sender.Disconnect += OnDisconnect;
                Receiver.Disconnect += OnDisconnect;
            }
        }

        public IPEndPoint EndPoint
        { get { return (IPEndPoint)Socket.RemoteEndPoint; } }
        public IPackReceivedListener Listener
        {
            get { return Receiver.Listener; }
            set { Receiver.Listener = value; }
        }
        internal DateTime LastPack
        { get { return Receiver.LastPack; } }

        private bool _isDisconnected;
        private void OnDisconnect()
        {
            lock (Locker)
            {
                if (_isDisconnected) return;
                _isDisconnected = true;
            }
            OnDisconnectImplement();
        }
        private void OnDisconnectImplement()
        {
            try
            {
                Sender.Disconnect -= OnDisconnect;
                Receiver.Disconnect -= OnDisconnect;
                Socket.Close(5);
                Socket.Dispose();
                _disconnected();
            }
            catch { }
        }

        private bool _isDisposed;
        public void Dispose()
        {
            var dc = false;
            lock (Locker)
            {
                if (_isDisposed) return;
                _isDisposed = true;
                if (!_isDisconnected) dc = _isDisconnected = true;
            }
            Server.Remove(this);
            if (dc) OnDisconnectImplement();
        }
    }
}
