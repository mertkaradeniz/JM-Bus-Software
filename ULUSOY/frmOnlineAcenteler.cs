using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ULUSOY;

namespace JMOtobusYazilimi
{
    public partial class frmOnlineAcenteler : Form
    {
        public frmOnlineAcenteler()
        {
            InitializeComponent();
        }
       
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        StreamWriter islem; 
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";
        string[] gln_sts, gln_krip;
        string[] dizi;
        public static int mesaj_sayisi; 
        public static string[] MesajID;
        string Alıcı; 
        static int mesaj = 0;   

        [DllImport("kernel32.dll")]
        static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll")]
        static extern ushort GlobalFindAtom(string lpString);

        [DllImport("kernel32.dll")]
        static extern ushort GlobalDeleteAtom(ushort atom);

        void AyarlariKaydet()
        {
            try
            {
                islem = new StreamWriter(Application.StartupPath + "\\Setting\\Ma.jmt");
                islem.WriteLine(checkBox3.Checked);
                islem.WriteLine(checkBox4.Checked);
            }
            finally
            {
                islem.Flush();
                islem.Close();
            }
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

        private void ResimEkle(string dosya)
        {
            // parametre olarak gelen adresten bitmap(resim) nesnesi oluştur
            Bitmap resim = new Bitmap(dosya);
            // oluşturulan resmi hafızaya kopyala
            Clipboard.SetDataObject(resim);
            // resim formatını al
            DataFormats.Format format = DataFormats.GetFormat(DataFormats.Bitmap);
            // resim yapıştırılabilir mi diye kontrol et
            if (richTextBox1.CanPaste(format))
            {
                // hafızadaki resmi richTextBox1 içine yapıştır
                richTextBox1.Paste(format);
            }
        }
      
        private void frmOnlineAcenteler_Load(object sender, EventArgs e)
        {
            baglan.Open();

            dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ma.jmt");
            
            if (dizi[0] == "True")
                checkBox3.Checked = true;
            if (dizi[1] == "True")
                checkBox4.Checked = true;


            gln_sts = System.IO.File.ReadAllLines(Application.StartupPath + "\\Security\\Sct.jmt");

            gln_krip = Decrypt(gln_sts[0]).Split('-');
            label18.Text = "Acente Adı: " + gln_krip[2] + "       Bölge: " + gln_krip[3];
            label9.Text = "Sistem Adı: " + gln_krip[0] + "       PC Adı: " + System.Environment.MachineName;

            treeView1.ImageList = imageList1;
            TreeNode node = new TreeNode("Online Acenteler");
            
            SqlDataReader reader = new SqlCommand("select acente_adi,sistem_adi,ID from OnlineAcenteler order by acente_adi", baglan).ExecuteReader();
            int bag = 0, dal = 0;
            while (reader.Read())
            {
                if (node.Nodes.Count > 0)
                {
                    if (node.Nodes[bag - 1].Text == reader[0].ToString())
                    {
                        node.Nodes[bag - 1].Nodes.Add(reader[1].ToString() + " | " + reader[2] + " |");
                        node.Nodes[bag - 1].Nodes[dal].ImageIndex = 2;
                        node.Nodes[bag - 1].Nodes[dal].SelectedImageIndex = 3;
                        dal++;
                    }
                    else
                    {
                        dal = 0;
                        node.Nodes.Add(reader[0].ToString());
                        node.Nodes[bag].ImageIndex = 1;
                        node.Nodes[bag].SelectedImageIndex = 1;
                        node.Nodes[bag].Nodes.Add(reader[1].ToString() + " | " + reader[2] + " |");
                        node.Nodes[bag].Nodes[dal].ImageIndex = 2;
                        node.Nodes[bag].Nodes[dal].SelectedImageIndex = 3;
                        dal++;
                        bag++;
                    }
                }
                else
                {
                    node.Nodes.Add(reader[0].ToString());
                    node.Nodes[bag].ImageIndex = 1;
                    node.Nodes[bag].SelectedImageIndex = 1;
                    node.Nodes[bag].Nodes.Add(reader[1].ToString() + " | " + reader[2] + " |");
                    node.Nodes[bag].Nodes[dal].ImageIndex = 2;
                    node.Nodes[bag].Nodes[dal].SelectedImageIndex = 3;
                    dal++;
                    bag++;
                }
            }
            reader.Close();
                     
            treeView1.Nodes.Add(node);
        }
         
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.SelectedImageIndex == 3)
            {
                richTextBox1.Clear();

                Alıcı = treeView1.SelectedNode.Text;
                string[] select_tutucu = Alıcı.Split('|');

                Alıcı = select_tutucu[1];

                SqlDataReader reader = new SqlCommand("select acente_adi,bolge,sistem_adi,pc_adi from OnlineAcenteler where ID='" + Alıcı.Trim() + "'", baglan).ExecuteReader();
                while (reader.Read())
                {
                    label5.Text = reader[0].ToString();
                    label6.Text = reader[1].ToString();
                    label7.Text = reader[2].ToString();
                    label8.Text = reader[3].ToString();

                }
                reader.Close();

                richTextBox1.AppendText(label5.Text + " Acentesinin, " + label7.Text + " Sistem kullanıcısyla Sohbet Etmeye Başlayabilirsiniz..." + Environment.NewLine);
                richTextBox1.AppendText("" + Environment.NewLine);
                richTextBox1.AppendText("" + Environment.NewLine);
            
                mesaj_sayisi = 0; mesaj = 0;
                mesaj_sayisi =Convert.ToInt32( new SqlCommand("select count(S_ID)  from OnlineMesajlasma where gonderen='" + frmYazilimGüvenlik.OnlineID.Trim() + "' or alici='" + frmYazilimGüvenlik.OnlineID.Trim() + "'", baglan).ExecuteScalar());
                MesajID=new string[mesaj_sayisi+1];
               
                SqlDataReader reader2 = new SqlCommand("select mesaj,gonderen,S_ID  from OnlineMesajlasma where gonderen='" + frmYazilimGüvenlik.OnlineID.Trim() + "' or alici='" + frmYazilimGüvenlik.OnlineID.Trim() + "'", baglan).ExecuteReader();

                while (reader2.Read())
                {
                    MesajID[mesaj] = reader2[2].ToString();

                    if (reader2[1].ToString() == frmYazilimGüvenlik.OnlineID.Trim())
                        richTextBox1.AppendText("Ben: " + reader2[0] + Environment.NewLine);
                    else
                        richTextBox1.AppendText("O : " + reader2[0] + Environment.NewLine);

                    richTextBox1.AppendText("" + Environment.NewLine);

                    mesaj++;
                }
                reader2.Close();
            }
        }  
     
        private void button3_Click(object sender, EventArgs e)
        {
            if (label5.Text != "")
            {
                ResimEkle(Application.StartupPath + "\\Pictures\\Online.png");
                richTextBox1.AppendText("Ben: " + textBox1.Text.Trim() + Environment.NewLine);
                richTextBox1.AppendText("" + Environment.NewLine);
              
                MesajID = new string[mesaj_sayisi+1];
 
                ClsInsert.OnlineMesajGonder(textBox1.Text.Trim(), Alıcı.Trim());

                textBox1.Text = "";
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.Focus();
            }
            else
                MessageBox.Show("Lüften ilk önce bir sistem belirleyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (label5.Text != "")
            {
                try
                {
                    SqlDataReader reader2 = new SqlCommand("select mesaj,gonderen,S_ID  from OnlineMesajlasma where gonderen='" + frmYazilimGüvenlik.OnlineID.Trim() + "' or alici='" + frmYazilimGüvenlik.OnlineID.Trim() + "'", baglan).ExecuteReader();
                    while (reader2.Read())
                    {
                        MesajID[mesaj] = reader2[2].ToString();
                    }
                    reader2.Close();
                   
                    ClsInsert.OnlineMesajSilme(MesajID);
                    richTextBox1.Clear();
                    richTextBox1.AppendText(label5.Text + " Acentesinin, " + label7.Text + " Sistem kullanıcısyla Sohbet Etmeye Başlayabilirsiniz..." + Environment.NewLine);
                    richTextBox1.AppendText("" + Environment.NewLine);
                    richTextBox1.AppendText("" + Environment.NewLine);
                    
                    MessageBox.Show("Mesajlarınız başarıyla silindi", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
               
                }
                catch
                { MessageBox.Show("Mesajlarınız silinedi !", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {   
            AyarlariKaydet();
        }
  
        private void frmOnlineAcenteler_FormClosed(object sender, FormClosedEventArgs e)
        {
            string str = Application.ProductName + Application.ProductVersion;
            ushort atom = GlobalFindAtom(str);
            GlobalDeleteAtom(atom);
            
            if (checkBox4.Checked)
            {
                SqlDataReader reader2 = new SqlCommand("select mesaj,gonderen,S_ID  from OnlineMesajlasma where gonderen='" + frmYazilimGüvenlik.OnlineID.Trim() + "' or alici='" + frmYazilimGüvenlik.OnlineID.Trim() + "'", baglan).ExecuteReader();
                while (reader2.Read())
                {
                    MesajID[mesaj] = reader2[2].ToString();
                }
                reader2.Close();            
                ClsInsert.OnlineMesajSilme(MesajID);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (checkBox3.Checked)
            { 
                if (e.KeyCode == Keys.Enter)
                {
                    button3_Click(sender,new EventArgs());
                    textBox1.Clear();
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            treeView1.ImageList = imageList1;
            TreeNode node = new TreeNode("Online Acenteler");
            
            SqlDataReader reader = new SqlCommand("select acente_adi,sistem_adi,ID from OnlineAcenteler order by acente_adi", baglan).ExecuteReader();
            int bag = 0, dal = 0;
            while (reader.Read())
            {
                if (node.Nodes.Count > 0)
                {
                    if (node.Nodes[bag - 1].Text == reader[0].ToString())
                    {
                        node.Nodes[bag - 1].Nodes.Add(reader[1].ToString() + " | " + reader[2] + " |");
                        node.Nodes[bag - 1].Nodes[dal].ImageIndex = 2;
                        node.Nodes[bag - 1].Nodes[dal].SelectedImageIndex = 3;
                        dal++;
                    }
                    else
                    {
                        dal = 0;
                        node.Nodes.Add(reader[0].ToString());
                        node.Nodes[bag].ImageIndex = 1;
                        node.Nodes[bag].SelectedImageIndex = 1;
                        node.Nodes[bag].Nodes.Add(reader[1].ToString() + " | " + reader[2] + " |");
                        node.Nodes[bag].Nodes[dal].ImageIndex = 2;
                        node.Nodes[bag].Nodes[dal].SelectedImageIndex = 3;
                        dal++;
                        bag++;
                    }
                }
                else
                {
                    node.Nodes.Add(reader[0].ToString());
                    node.Nodes[bag].ImageIndex = 1;
                    node.Nodes[bag].SelectedImageIndex = 1;
                    node.Nodes[bag].Nodes.Add(reader[1].ToString() + " | " + reader[2] + " |");
                    node.Nodes[bag].Nodes[dal].ImageIndex = 2;
                    node.Nodes[bag].Nodes[dal].SelectedImageIndex = 3;
                    dal++;
                    bag++;
                }
            }
            reader.Close();
            treeView1.Nodes.Add(node);
        }

    }
}
