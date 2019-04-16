using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ULUSOY;

namespace JMOtobusYazilimi
{
    public partial class frmTopluSmsGonderme : Form
    {
        public frmTopluSmsGonderme()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;                     
        }
       
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base()); 
        ClsSms SmsGonder = new ClsSms();

        int BASARILISMS=0, BASARISIZSMS=0,TOPLAMSMS;
        string[] gln;
        bool ISLEMIPTAL = false;
     
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";

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

        private void arac()
        {  
            this.listView2.Items.Clear();
           
            SqlCommand command = new SqlCommand("select distinct tel from Yolcular", this.baglan);
            SqlDataReader reader2 = command.ExecuteReader();
            while (reader2.Read())
            {
                if (reader2[0].ToString() != "")
                {
                    ListViewItem item = new ListViewItem("Belirsiz");
                    item.SubItems.Add(reader2[0].ToString());
                    this.listView2.Items.Add(item);
                }
            }   
                reader2.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (button2.Text.Trim() == "Gönder")
                {
                    if (listView2.CheckedItems.Count > 0)
                    {
                        new frmLoading().Show();
                        button2.Text = "   İptal";
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                        MessageBox.Show("Lütfen en az 1 tane numara seçiniz.", "Numara Eksik !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ((frmLoading)Application.OpenForms["frmLoading"]).Close();
                    button2.Text = "   Gönder";
                    ISLEMIPTAL = true;
                }
            }
            catch { }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string Tarih = DateTime.Now.ToShortTimeString();
            toolStripProgressBar1.Maximum = listView2.CheckedItems.Count;
            toolStripProgressBar1.Value = 0;
            BASARILISMS = 0; BASARISIZSMS = 0;

            for (int i = 0; i < listView2.CheckedItems.Count; i++)
            {

                if (SmsGonder.SMSGONDER(comboBox2.Text, listView2.CheckedItems[i].SubItems[1].Text, textBox1.Text.Trim(), Tarih) == "-1")
                    BASARISIZSMS += 1;
                else
                    BASARILISMS += 1;
                
                toolStripProgressBar1.Value += 1;

                if (ISLEMIPTAL)
                {
                    toolStripProgressBar1.Value = 0;                
                    ISLEMIPTAL = false;
                    break;
                }

                try { System.Threading.Thread.Sleep(1000); }
                catch { }
            }

            try { ((frmLoading)Application.OpenForms["frmLoading"]).Close(); }
            catch { }

            button2.Text = "   Gönder";

            TOPLAMSMS=BASARILISMS+BASARISIZSMS;

            MessageBox.Show("Toplam " + TOPLAMSMS + " SMS'den " + BASARILISMS + " tanesi başarılı," + BASARISIZSMS + " tanesi başarısız şekilde gönderilmiştir.","Bilgi Mesajı",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void frmTopluSmsGonderme_Load(object sender, EventArgs e)
        {
            gln = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ssa.jmt");

            for (int i = 0; i < Decrypt(gln[0]).Split('-')[2].Split('+').Count(); i++)
            {
                if (Decrypt(gln[0]).Split('-')[2].Split('+')[i] != "")
                    comboBox2.Items.Add(Decrypt(gln[0]).Split('-')[2].Split('+')[i]);
            }

            comboBox2.Text = Decrypt(gln[0]).Split('-')[3];   

            baglan.Open();
            arac();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                for (int i = 0; i < listView2.Items.Count; i++)
                    listView2.Items[i].Checked = true;
            }
            else
            {
                for (int i = 0; i < listView2.Items.Count; i++)
                    listView2.Items[i].Checked = false;
            }
          
        }
    }
}
