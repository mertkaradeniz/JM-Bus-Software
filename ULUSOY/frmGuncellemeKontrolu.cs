using JMOtobusYazilimi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ULUSOY
{
    public partial class frmGuncellemeKontrolu : Form
    {
        public frmGuncellemeKontrolu()
        {         
            InitializeComponent();
        }
     
        private void frmGuncellemeKontrolu_Load(object sender, EventArgs e)
        {

            label2.Text = "Yeni JM Otobüs Yazılımı " + frmAna.YeniVersiyon + " mevcut.";
            label3.Text = "Sürümünüz: " + Application.ProductVersion;
          
        }
       
        private void button39_Click(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            new frmGuncellemeShowPaneli().ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Close();

        }

    }
}
