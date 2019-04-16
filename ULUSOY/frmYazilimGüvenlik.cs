using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Data.SqlClient;
using System.Management;
using Lisans;
namespace ULUSOY
{
    public partial class frmYazilimGüvenlik : Form
    {
        public frmYazilimGüvenlik()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;        
        }

        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());       
        public static string OnlineID = "";
        string[] satirlar, parca;
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";
        bool TimerAktif = true;

        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static Boolean dosya_sil();

        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static int l();
      
        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static Boolean tarih_k_geri_donen();

        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static Boolean demo_k();

        private void sifreCozme(string adres, string yeniadres, string StrKarma)
        {
            TripleDESCryptoServiceProvider TDCS = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            FileStream OkuStream = new FileStream(adres, FileMode.Open, FileAccess.Read);
            FileStream YazmaStream = new FileStream(yeniadres, FileMode.OpenOrCreate, FileAccess.Write);
            byte[] Karma = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(StrKarma));
            byte[] Metin_Oku = File.ReadAllBytes(adres);

            md5.Clear();

            TDCS.Key = Karma;
            TDCS.Mode = CipherMode.ECB;

            CryptoStream kriptoStream = new CryptoStream(YazmaStream, TDCS.CreateDecryptor(), CryptoStreamMode.Write);
            int depo;
            long position = 0;
            while (position < OkuStream.Length)
            {
                depo = OkuStream.Read(Metin_Oku, 0, Metin_Oku.Length);
                position += depo;

                kriptoStream.Write(Metin_Oku, 0, depo);
            }

            OkuStream.Close();
            YazmaStream.Close();
            anahtar_çalıştırma();
        }  
       
        private string Decrypt(string text)
        {
            // AesCryptoServiceProvider
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 128;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Convert Base64 strings to byte array
            byte[] src = System.Convert.FromBase64String(text);

            // decryption
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                return Encoding.Unicode.GetString(dest);
            }
        }
    
        public static string MAC()
        {
            string Mac = "";
            ManagementClass manager = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (ManagementObject obj in manager.GetInstances())
            {
                if ((bool)obj["IPEnabled"])
                {
                    Mac = obj["MacAddress"].ToString();
                }
            }
            return Mac;
        }
  
        private void button3_Click(object sender, EventArgs e)   
        {
            try
            {
                if ((textBox1.Text == parca[0]) && (textBox2.Text == parca[1]))
                {
                    frmYazilimGüvenlik.OnlineID = MAC();
                    baglan.Open();

                    SqlCommand command = new SqlCommand("delete  from OnlineAcenteler where ID='" + frmYazilimGüvenlik.OnlineID + "'", baglan);
                    command.ExecuteNonQuery();
                    baglan.Close();
                    ClsInsert.SistemeOnlineOl();

                    new frmAna().ShowDialog();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Lütfen Kullanıcı Adı/Şifreyi kontrol ediniz !!!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { MessageBox.Show("Veritabanı bağlantı hatası oluştu.Lütfen daha sonra tekrar deneyiniz.", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
      
        public void DosyayiSil()
        {
            File.Delete("C:\\Windows\\System32\\wdi\\LogFiles\\Boot.dll");
        }

        private string anahtar_çalıştırma()
        {
            DESCryptoServiceProvider kripto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(kripto.Key);
        }
  
        private void yazilimgüvenlik_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();

            if (!new LiDataCtrl().IfDateFileNull(Application.StartupPath + "\\LiDaP.dll"))
            {
                MessageBox.Show("Sistem Dosyası eksiktir.Lütfen programı tekrar kurunuz ya da yazılım sahibine başvurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            try
            {
                if (new LiBackCount().LiDateDifference())
                {
                    TimerAktif = false;
                    Hide();
                    new frmLisans().ShowDialog();
                }
            }
            catch
            {
                TimerAktif = false;
                Hide(); new frmLisans().ShowDialog();
            }

            anahtar_çalıştırma();
            sifreCozme(Application.StartupPath + "\\Security\\Sct.jmt", Application.StartupPath + "\\Security\\Sct.jm", "123");
            satirlar = System.IO.File.ReadAllLines(Application.StartupPath + "\\Security\\Sct.jmt");

            parca = Decrypt(satirlar[0]).Split('-');
            textBox1.Text = parca[0];
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if ((textBox1.Text == parca[0]) && (textBox2.Text == parca[1]))
                {
                    new frmAna().Show();
                    Hide();
                }
                else
                {
                    MessageBox.Show("Lütfen Kullanıcı Adı/Şifreyi kontrol ediniz !!!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void yazilimgüvenlik_FormClosed(object sender, FormClosedEventArgs e)
        {
           Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmSunucuAyarlari().ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //new LiDateCtrl().NetSaati();

        }

        private void LisansCtrl_Tick(object sender, EventArgs e)
        {
            if (!TimerAktif) return;
            try
            {
                if (new LiBackCount().LiDateDifference())
                {

                    LisansCtrl.Stop();
                    Hide();
                    new frmLisans().ShowDialog();
                }
            }
            catch { }
        }

        private void NetDateUpdate_Tick(object sender, EventArgs e)
        {
            //backgroundWorker1.RunWorkerAsync();

        }
   
    }
}
