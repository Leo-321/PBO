using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PokemonBattleOnline.Network
{
  internal class TcpClient : IDisposable
  {
    /// <summary>
    /// block
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <exception cref=""></exception>
    public static void BeginConnect(IPAddress address, int port, Action<TcpClient> callback)
    {
      var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      try
      {
        socket.Blocking = true;
        socket.LingerState = new LingerOption(true, 5);
        socket.BeginConnect(address, port, OnConnectCompleted, new object [] { socket, callback });
      }
      catch (Exception e)
      {
        socket.Dispose();
        throw e;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <exception cref=""></exception>
    public static void BeginConnect(string address, int port, Action<TcpClient> callback)
    {
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      try
      {
        socket.Blocking = true;
        socket.LingerState = new LingerOption(true, 5);
        socket.BeginConnect(address, port, OnConnectCompleted, new object[] { socket, callback });
      }
      catch (Exception e)
      {
        socket.Dispose();
        throw e;
      }
    }
    private static void OnConnectCompleted(IAsyncResult ar)
    {
      var mo = (object[])ar.AsyncState;
      var s = (Socket)mo[0];
      var cb = (Action<TcpClient>)mo[1];
      try
      {
        s.EndConnect(ar);
        cb(new TcpClient(s));
      }
      catch (Exception e)
      {
        s.Dispose();
        cb(null);
      }
    }

    private event Action _disconnected;
    public event Action Disconnected
    {
      add { _disconnected = value; }
      remove { System.Diagnostics.Debugger.Break(); }
    }
    private readonly Socket Socket;
    public readonly TcpPackSender Sender;
    private readonly TcpPackReceiver Receiver;

    private TcpClient(Socket socket)
    {
      Socket = socket;
      Sender = new TcpPackSender(socket);
      Sender.Disconnect += OnDisconnect;
      Receiver = new TcpPackReceiver(socket);
      Receiver.Disconnect += OnDisconnect;
    }

    public IPEndPoint Server
    { get { return (IPEndPoint)Socket.RemoteEndPoint; } }
    public IPackReceivedListener Listener
    {
      get { return Receiver.Listener; }
      set { Receiver.Listener = value; }
    }

    private readonly object Locker = new object();
    private bool _isDisconnected;
    private void OnDisconnect()
    {
      lock (Locker)
      {
        if (!_isDisconnected)
        {
          try
          {
            Socket.Close(5);
            Socket.Dispose();
            _isDisconnected = true;
            Sender.Disconnect += delegate { };
            Sender.Disconnect -= OnDisconnect;
            Receiver.Disconnect += delegate { };
            Receiver.Disconnect -= OnDisconnect;
            _disconnected();
          }
          catch { }
        }
      }
    }

    public void Dispose()
    {
      OnDisconnect();
    }
  }
}
