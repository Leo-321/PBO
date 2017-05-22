using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace PokemonBattleOnline.Network
{
    internal class LoginServer
    {
        private readonly Server Server;
        private readonly ConcurrentDictionary<int, LoginUser> Users;
        private readonly HashSet<string> Names;
        private readonly object NameLocker;

        public LoginServer(TcpServer network, Server server)
        {
            network.NewComingUser += OnNewUser;
            Server = server;
            Names = new HashSet<string>();
            Users = new ConcurrentDictionary<int, LoginUser>();
            NameLocker = new object();
        }

        private TcpServer Network
        { get { return Server.Network; } }

        private void OnNewUser(TcpUser user)
        {
            if (!Users.TryAdd(user.Id, new LoginUser(user, this))) user.Dispose();
        }

        public void RemoveName(string name)
        {
            lock (NameLocker)
            {
                Names.Remove(name);
            }
        }
        public bool RegisterName(LoginUser user, string name)
        {
            lock (NameLocker)
            {
                return Names.Add(name);
            }
        }
        public void BadLogin(LoginUser user)
        {
            LoginUser u;
            if (Users.TryRemove(user.Network.Id, out u) && user.Name != null) RemoveName(user.Name);
        }

        private int UserId;
        public void LoginComplete(LoginUser user)
        {
            LoginUser u;
            if (Users.TryRemove(user.Network.Id, out u) && user == u)
            {
                //Console.WriteLine(user.Name + " has entered the lobby.");
                Server.AddUser(new ServerUser(Interlocked.Increment(ref UserId), user, Server));
            }
            else
            {
                if (u != null) u.Dispose();
                user.Dispose();
            }
        }
    }
}
