using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Negocio;
using System.Threading;
using System.IO;

namespace SrvBabyFisrt
{

    public partial class SrvBabyFirst : ServiceBase
    {
        Thread Hilo;

        //WebServiceHost host;

        int minutos = 2;

        public SrvBabyFirst()
        {
            InitializeComponent();
        }
        
        public void Iniciar()
        {
            Hilo = new Thread(new ThreadStart(tProcBF));

            Hilo.IsBackground = true;
            Hilo.Start();
        }

        private void tProcBF()
        {
            Procesos oProc = new Procesos();
            while (true)
            {

                oProc.Correr("SERV",true,false);
                Thread.Sleep(minutos * 60000);
            }
        }

        private void Procesar(string p)
        {

        }

        private void Write(string p)
        {
            
        }


        protected override void OnStart(string[] args)
        {
            Iniciar();
        }

        protected override void OnStop()
        {

        }
    }
}
