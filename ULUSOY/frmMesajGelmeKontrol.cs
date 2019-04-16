using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ULUSOY;

namespace JMOtobusYazilimi
{
    public partial class frmMesajGelmeKontrol : Form
    {
        public frmMesajGelmeKontrol()
        {
            InitializeComponent();
        }
      
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base()); 
        string gelenid = "";   

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,  // Sol üst köşesi x-koordinatı 
            int nTopRect,  // y koordinatı sol üst köşesi 
            int nRightRect,  // x-koordinatı sağ alt köşesi 
            int nBottomRect,  // sağ alt köşe y-koordinatı 
            int nWidthEllipse,  // elips yüksekliği 
            int nHeightEllipse // Elips genişliği 
         );

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            kapa.Start();
        }

        private void ac_Tick(object sender, EventArgs e)
        {
            if (this.Opacity <= 1.0)   // Eğer formun opacity değer % 100 den az ise;
            {
                this.Opacity += 0.1;   // bu değeri % 10 arttır.
            }
            if (this.Opacity == 1.0)
            {
                ac.Stop();
            }
        }

        private void kapa_Tick(object sender, EventArgs e)
        {
            if (this.Opacity >= 0.0)   // Eğer formun opacity değer % 100 den az ise;
            {
               this.Opacity -= 0.1;   // bu değeri % 10 arttır..
            }
            if (this.Opacity == 0.0)
            {
                Close();
            }
        }

        private void Oto_kapama_Tick(object sender, EventArgs e)
        {
            kapa.Start();
        }
       
        string mesaj_kısalt(string mesaj)
        {
            if (mesaj.Length > 15)
            {
                mesaj = mesaj.Substring(0, 16);
            }
         
            return mesaj+"...";
        }
       
        private void frmMesajGelmeKontrol_Load(object sender, EventArgs e)
        {
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            try
            {
                Location = new Point(frmAna.ActiveForm.Location.X + frmAna.ActiveForm.Size.Width - this.Size.Width - 22, frmAna.ActiveForm.Location.Y + frmAna.ActiveForm.Size.Height - this.Size.Height - 20);
                gelenid = frmAna.GelenMesajID;
                         
                frmAna.GelenMesajID = "";
                baglan.Open();
                SqlDataReader reader2 = new SqlCommand("select gonderen,mesaj  from OnlineMesajlasma where S_ID=" + gelenid, baglan).ExecuteReader();
                string gonderen="";
                while (reader2.Read())
                {
                    label5.Text = reader2[0].ToString();
                    gonderen = reader2[0].ToString();
                    label7.Text = mesaj_kısalt(reader2[1].ToString());
                }
                reader2.Close();

                SqlDataReader reader = new SqlCommand("select acente_adi,sistem_adi  from OnlineAcenteler where ID='" +gonderen.Trim() + "'", baglan).ExecuteReader();
                while (reader.Read())
                {
                    label5.Text = reader[0].ToString();
                    label6.Text = reader[1].ToString();
                }
                reader.Close();
                baglan.Close();
            }
            catch { Close(); } 
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            kapa.Start();
        }
   
    }
}
