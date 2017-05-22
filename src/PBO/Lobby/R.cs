using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.PBO.Lobby
{
  internal static class R
  {
    public static string Username
    { get { return "用户名"; } }
    public static string LoginToServer
    { get { return "登陆到服务器"; } }

    public static string LOGINFAILED_NAME
    { get { return "不能使用的登陆名。"; } }
    public static string LOGINFAILED_VERSION_OLDSERVER
    { get { return "客户端与服务器的版本不兼容，客户端版本较新。"; } }
    public static string LOGINFAILED_VERSION_OLDCLIENT
    { get { return "客户端与服务器的版本不兼容，请到http://pbo.codeplex.com/下载最新版客户端。"; } }
    public static string LOGINFAILED_FULL
    { get { return "服务器已满。"; } }
  }
}
