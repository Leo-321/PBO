using PokemonBattleOnline;
using PokemonBattleOnline.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace XYPBOService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                FileLogger.Instance.Info("Loading data");
                PBOServer.NewServer(PBOMarks.DEFAULT_PORT);
                PBOServer.Current.Start();
                FileLogger.Instance.Info(@"Server is ready.");
            }
            catch (Exception e)
            {
                FileLogger.Instance.Error(e);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (PBOServer.Current != null)
                {
                    PBOServer.Current.Dispose();
                }

                FileLogger.Instance.Info(@"Server is Dispose.");
            }
            catch (Exception e)
            {
                FileLogger.Instance.Error(e);
            }
        }
    }
}
