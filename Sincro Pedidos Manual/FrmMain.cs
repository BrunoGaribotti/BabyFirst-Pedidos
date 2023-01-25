using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Negocio;
using System.IO;

namespace Sincro_Pedidos_Manual
{
    public partial class FrmMain : Form
    {
        bool prueba = true;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void sincronizarAhoraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            menuStrip1.Enabled = false;
            try
            {
                Procesos oProc = new Procesos();
                textBox1.Text = oProc.Correr("MAN", false,false);
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message.ToString();
                textBox1.Text += ex.StackTrace.ToString();
            }
            textBox1.Enabled = true;
            menuStrip1.Enabled = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void sincronizacionConMaestrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = false;
            menuStrip1.Enabled = false;
            try
            {
                Procesos oProc = new Procesos();
                //textBox1.Text = oProc.Correr("MAN",false,true);
                oProc.CargarConfig();
                textBox1.Text += @"
Fase 1 Iniciada
                ";
                oProc.ConectarDb();
                textBox1.Text += @"
Fase 2 Iniciada
                ";
                textBox1.Text += oProc.CrearMaestros();
                textBox1.Text += @"
Fase 3 Iniciada
                ";
                oProc.DesonectarDb();
                textBox1.Text += @"
Fase 4 Iniciada
                ";
                //oProc.SubirPendientes();

            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message.ToString();
                textBox1.Text += ex.StackTrace.ToString();
            }
            textBox1.Enabled = true;
            menuStrip1.Enabled = true;
        }
    }
}
