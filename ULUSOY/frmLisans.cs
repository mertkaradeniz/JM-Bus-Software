using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections.Specialized;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System.Data.Sql;

using System.Data.SqlClient;
using Microsoft.Win32;
using Lisans;
using LisanslamaAlgoritması;
namespace ULUSOY
{
    public partial class frmLisans : Form
    {
        public frmLisans()
        {
            InitializeComponent();
        }

        int maus_yukselik, maus_sol, fark_sol, fark_yukseklik;
        bool secildi = false;

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
       
        bool BtnLisansCtrl(string Key, string Parametre)
        {
            try
            {
                if (new ClsLiCtrl().LiKeyCompare(Key, Parametre))//Gelen Anahtar
                {
                    System.Threading.Thread.Sleep(500);
                    pictureBox3.Image = Image.FromFile(Application.StartupPath + "/True.png");
                    button17.Enabled = true;
                    return true;
                }
                else
                {
                    System.Threading.Thread.Sleep(500);
                    pictureBox3.Image = Image.FromFile(Application.StartupPath + "/False.png");
                    button17.Enabled = false;
                    return false;
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(1500);
                pictureBox3.Image = Image.FromFile(Application.StartupPath + "/False.png");
                button17.Enabled = false;
                return false;
            }
        }

        private void lisansfrm_Load(object sender, EventArgs e)
        {
           label1.Text = "JM Technology - Lisans Paneli Versiyon " + Application.ProductVersion;
           Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
         
            if (!new LiDataCtrl().IfDateFileNull(Application.StartupPath + "\\LiDaP.dll"))
           {
               MessageBox.Show("Sistem Dosyası eksiktir.Lütfen programı tekrar kurunuz ya da yazılım sahibine başvurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
               Application.Exit();
           }

           try
           {
               if (!new LiBackCount().LiDateDifference())
               {
                   Hide();
                   new frmYazilimGüvenlik().ShowDialog();

               }
           }
           catch { }

           label8.Text = new LiDataCtrl().LISHOWPARAMETRE().ToString();

           try
           {
               if (new LiBackCount().LiDemoFinishR())
               {
                   label9.Visible = true; 
                   radioButton2.Visible = false;
               }
           }
           catch { }
          
        }
      
        private void pictureBox2_Click(object sender, EventArgs e)
        { 
            kapa.Start();           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmIletisim().ShowDialog();
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (BtnLisansCtrl(textBox1.Text, label8.Text))
                {
                    new LiDateFile().PathWriteAdd(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                    Hide(); new frmYazilimGüvenlik().ShowDialog();
                }
                else
                {
                    MessageBox.Show("false");
                }

            }

            if (radioButton2.Checked)
            {
                new ClsLiCtrl().LiDEMO();
                new LiDateFile().PathWriteAdd(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                Hide(); new frmYazilimGüvenlik().ShowDialog();

            }
            
            try
            {
                string kopyalanacakDosya = Application.StartupPath + "\\DataBase\\VeriTabani.bak";
                string dosyanınKopyanacagiKlasor = "C:\\Program Files\\Microsoft SQL Server\\MSSQL.1\\MSSQL\\Backup";
                string kopyalanacakDosyaIsmi = "VeriTabani.bak";
            
                if (File.Exists(dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi) == false)
                {
                    File.Copy(kopyalanacakDosya, dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi);
                }

                SqlConnection myConnection = new SqlConnection("Data Source=" + System.Environment.MachineName + "\\SqlExpress; Initial Catalog='master'; Integrated security=true");
                string VeritataniAdi = "Data";
                SqlCommand komut2 = new SqlCommand("SELECT Count(name) FROM master.dbo.sysdatabases WHERE name=@prmVeritabani", myConnection);
                komut2.Parameters.AddWithValue("@prmVeriTabani", VeritataniAdi);
                myConnection.Open();
                int sonuc = (int)komut2.ExecuteScalar();

                if (sonuc != 1)
                {
                    string cmdText = "restore database Data from disk = 'VeriTabani.bak'";
                    SqlCommand komut = new SqlCommand(cmdText, myConnection);
                    komut.ExecuteNonQuery();
                    myConnection.Close();
                }
            }
            catch { MessageBox.Show("Yazılım Veri Tabanı Hatası oluşturdu.Lütfen SQL Server Management Studio'nun kurulu olduğundan emin olunuz", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }

           
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            { button17.Enabled = false; textBox1.Enabled = true; label4.Enabled = true; label8.Enabled = true; label5.Enabled = true;  }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            { button17.Enabled = true; textBox1.Enabled = false; label4.Enabled = false; label8.Enabled = false; label5.Enabled = false;}
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            secildi = false;
          
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (secildi)
            {
                fark_yukseklik = maus_yukselik - e.Y;
                fark_sol = maus_sol - e.X;

                this.Top = this.Top - fark_yukseklik;
                this.Left = this.Left - fark_sol;
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left))
            {
                maus_yukselik = e.Y;
                maus_sol = e.X;
                secildi = true;
            }
        }

        private void lisansfrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void lisans_key_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void ac_Tick(object sender, EventArgs e)
        {
            if (this.Opacity <= 1.0) 
            {
                this.Opacity += 0.1; 
            }
            if (this.Opacity == 1.0)
            {
                ac.Stop();
            }
        }

        private void kapa_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0.0)  
            {
                this.Opacity -= 0.1; 
            }
            if (this.Opacity == 0.0)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void frmLisans_MouseMove(object sender, MouseEventArgs e)
        {
            if (secildi)
            {
                fark_yukseklik = maus_yukselik - e.Y;
                fark_sol = maus_sol - e.X;

                this.Top = this.Top - fark_yukseklik;
                this.Left = this.Left - fark_sol;
            }
        }

        private void frmLisans_MouseUp(object sender, MouseEventArgs e)
        {
            secildi = false;
        }

        private void frmLisans_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left))
            {
                maus_yukselik = e.Y;
                maus_sol = e.X;
                secildi = true;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(label8.Text);
        }

        private void lisans_key_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            pictureBox3.Visible = true;

            if (textBox1.Text.Count() >= 29)
            {
                BtnLisansCtrl(textBox1.Text, label8.Text);
            }
        }

        private void BoyutDegistirmeEffect_Tick(object sender, EventArgs e)
        {

        }
      
    }
}