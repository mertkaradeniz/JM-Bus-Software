using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ULUSOY;

namespace JMOtobusYazilimi
{
    public partial class frmGuncellemeShowPaneli : Form
    {
        public frmGuncellemeShowPaneli()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        StreamWriter server;
        bool Cikis = true;
       
        private void frmGuncellemeShowPaneli_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 450)
                progressBar1.Value = progressBar1.Value + 1;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 5; i >= 0; i--)
            {
                lblText.Text = i + " Saniye sonra güncelleme başlatılacak.";
                System.Threading.Thread.Sleep(1000);
            }

            lblText.Text = "Program Kapatılacak...";

            System.Threading.Thread.Sleep(2500);

            try
            {
                server = new StreamWriter(Application.StartupPath + "\\Srm.jmt");
                server.WriteLine(Application.ProductVersion);
            }
            finally
            {
                server.Flush();
                server.Close();
            }

            Cikis = false;
            if (!Cikis)
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "\\JM Otobüs Yazılımı Güncellemesi.exe");
                timer1.Stop();
                Application.Exit();
            }
        }

        private void frmGuncellemeShowPaneli_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        } 
   
    }
}
