using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PokemonBattleOnline.Network
{
    /// <summary>
    /// 提供静态全局访问
    /// </summary>
    public static class PBOClient
    {
        public static event Action Disconnected;
        public static event Action CurrentChanged;
        public static event Action LoginFailed_Full
        {
            add { LoginClient.Full += value; }
            remove { LoginClient.Full -= value; }
        }
        public static event Action LoginFailed_Name
        {
            add { LoginClient.BadName += value; }
            remove { LoginClient.BadName -= value; }
        }
        public static event Action<UInt16> LoginFailed_Version
        {
            add { LoginClient.BadVersion += value; }
            remove { LoginClient.BadVersion -= value; }
        }
        public static event Action LoginFailed_Disconnect
        {
            add { LoginClient.Disconnect += value; }
            remove { LoginClient.Disconnect -= value; }
        }
        private static readonly object Locker = new object();

        static PBOClient()
        {
            LoginClient.LoginSucceed += LoginSucceed;
            LoginClient.BadName += LoginFailed;
            LoginClient.BadVersion += LoginClient_BadVersion; ;
            LoginClient.Disconnect += LoginFailed;
            LoginClient.Full += LoginFailed;
            ClientController.Disconnected += OnDisconnected;
        }

        private static void LoginClient_BadVersion(ushort obj)
        {
            LoginFailed();
        }

        private static Client _current;
        /// <summary>
        /// get is not thread safe
        /// </summary>
        public static ClientController Current
        { get { return _current == null ? null : _current.Controller; } }

        private static void LoginSucceed(Client obj)
        {
            currentLogin = null;
            _current = obj;
            UIDispatcher.Invoke(CurrentChanged);
        }
        private static void LoginFailed()
        {
            currentLogin = null;
        }

        private static LoginClient currentLogin;
        public static bool Login(string server, string name, ushort avatar)
        {
            return Login(server, PBOMarks.DEFAULT_PORT, name, avatar);
        }
        public static bool Login(string server, int port, string name, ushort avatar)
        {
            lock (Locker)
            {
                if (_current == null && currentLogin == null)
                {
                    currentLogin = new LoginClient(server, port, name, avatar);
                    currentLogin.BeginLogin();
                    return true;
                }
                return false;
            }
        }

        public static void Exit()
        {
            _current.Controller.Exit();
            _current = null;
            CurrentChanged();
        }
        private static void OnDisconnected()
        {
            UIDispatcher.Invoke(Disconnected);
            _current = null;
            UIDispatcher.Invoke(CurrentChanged);
        }
    }
}
