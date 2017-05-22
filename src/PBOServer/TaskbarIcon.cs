using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PokemonBattleOnline.PBO.Server
{
  static class TaskbarIcon
  {
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private readonly static NotifyIcon NI;
    private readonly static IntPtr hWnd;

    static TaskbarIcon()
    {
      NI = new NotifyIcon();
      NI.Icon = new System.Drawing.Icon(typeof(TaskbarIcon), "server.ico");
      NI.Text = PBOMarks.TITLE + " Server";
      NI.ContextMenu = new ContextMenu(new MenuItem[] { new MenuItem("退出", Quit_Click) });
      hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
      if (hWnd != IntPtr.Zero) NI.MouseClick += NI_MouseClick;
    }

    private static int sw;
    private static readonly object Locker = new object();
    private static void NI_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        lock (Locker)
        {
          ShowWindow(hWnd, sw); // 0 = SW_HIDE
          sw = 1 - sw;
        }
      }
    }
    private static void Quit_Click(object sender, EventArgs e)
    {
      Close();
      Environment.Exit(0);
    }

    public static void Init()
    {
      NI.Visible = true;
      Application.Run();
    }

    public static void Close()
    {
      NI.Visible = false;
      NI.Dispose();
      Application.Exit();
    }
  }
}
