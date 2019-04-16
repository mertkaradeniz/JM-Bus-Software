using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Data.SqlClient;
using JMOtobusYazilimi;
namespace ULUSOY
{
    public partial class frmGenelAyarlar : Form
    {
        public frmGenelAyarlar()
        {
            InitializeComponent();
        }
       
        #region DEGISKENLER

        static SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        ClsSms SmsGonder = new ClsSms();

        string[] gln_krip, gln_sts, gln_ssa, gln_krip_ssa, gln_krip_esa, gln_esa;        
        string[] islem_sistem;
        string baglanti_dene; 
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";
        StreamWriter dosya2,dosya3,dosya4;
        StreamWriter dosya;
        StreamWriter islem;
        StreamWriter server;
        RegistryKey key;
        
        #endregion

        #region OZEL FONKSIYONLAR

        public static int AltUst(int x)
        {
            int gonder = 0;

            for (int i = 0; i < x; i++)
            {
                gonder += 20;
            }
            return gonder;
        }

        private string AnahtarCalistirma()
        {
            DESCryptoServiceProvider kripto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            return ASCIIEncoding.ASCII.GetString(kripto.Key);
        }

        public static class myPrinters
        {
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDefaultPrinter(string Name);
        }

        private string Encrypt(string text)
        {
            // AesCryptoServiceProvider
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;//blok blok şifreleme yapıldığı için 
            //nekadarlık bloklar halinde şifreleneceği tanımlanıyor.
            aes.KeySize = 128;//anahtar ile şifreleme yapılıyo. 
            //anahtar boyutları, 128-192 ve 256 olabilir.
            aes.IV = Encoding.UTF8.GetBytes(AesIV);//bunu anlamadım :=)
            aes.Key = Encoding.UTF8.GetBytes(AesKey);//anahtar bytea çevriliyor. 
            //Böylece evrensel olarak bütün dosya, resim, metin vs 
            //bytelara çevrilerek şifreleme yapılabilir.
            aes.Mode = CipherMode.CBC;//Şifreleme modu seçiliyo genelde cbc olur 
            aes.Padding = PaddingMode.PKCS7;

            // Convert string to byte array
            byte[] src = Encoding.Unicode.GetBytes(text);//aynı şekilde 
            //metin de bytelara çevrilir

            //şifreleme burda gerçekleşiyor
            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                //bloklar alınır şifrelenir
                byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                // //daha sonra şifreli byte blokları stringe çevrilir.
                return Convert.ToBase64String(dest);
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

        private void Sifreleme(string adres, string yeniadres, string StrKarma)
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

            CryptoStream kriptoStream = new CryptoStream(YazmaStream, TDCS.CreateEncryptor(), CryptoStreamMode.Write);
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
        }

        void GenelAyarlarKaydet()
        {
            Double servis, reklam, fix;

            if (checkBox6.Checked)
            {
                servis = 20; reklam = 1; fix = 5;
            }
            else
            {
                servis = Convert.ToDouble(textBox4.Text);
                reklam = Convert.ToDouble(textBox3.Text);
                fix = Convert.ToDouble(textBox8.Text);
            }

            try
            {

                islem = new StreamWriter(Application.StartupPath + "\\Setting\\Sa.jmt");
                islem.WriteLine(checkBox4.Checked);
                islem.WriteLine(checkBox6.Checked);
                islem.WriteLine(servis);
                islem.WriteLine(reklam);
                islem.WriteLine(checkBox2.Checked);
                islem.WriteLine(fix);
            }
            finally
            {
                islem.Flush();
                islem.Close();

            }
        }

        void BinisVerileriniYukle()
        {
            this.listView2.Items.Clear();
            baglan.Open();

            SqlDataReader reader = new SqlCommand("select * from BinisNoktalari order by binis", baglan).ExecuteReader();

            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());


                this.listView2.Items.Add(item);
            }
            baglan.Close();
        }

        void InisVerileriniYukle()
        {
            this.listView1.Items.Clear();
            baglan.Open();
            SqlDataReader reader = new SqlCommand("select * from InisNoktalari order by inis", baglan).ExecuteReader();

            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());


                this.listView1.Items.Add(item);
            }
            baglan.Close();
        }

        void UcretVerileriniYukle()
        {

            baglan.Open();
            SqlDataReader reader = new SqlCommand("select * from Ucretler order by ucret", baglan).ExecuteReader();

            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());
            }

            baglan.Close();
        }

        string BaglantiKontrol()
        {
            if (rdbsunucu.Checked)
            {
                baglanti_dene = "server='" + txtserver.Text + "'; Password='" + txtsifre.Text + "';Persist Security Info=True;User ID='" + txtkullaniciadi.Text + "'; Database=" + txtdatabase.Text + ";";
            }

            else
            {
                baglanti_dene = "Data Source=" + System.Environment.MachineName + "\\SqlExpress; Initial Catalog='Data'; Integrated security=true";
            }

            SqlConnection baglanti = new SqlConnection(baglanti_dene);

            try
            {
                using (baglanti)
                {
                    return "true";
                }
            }
            catch { return "false"; }
        }

        #endregion

        #region TUMLESIK FONKSİYONLAR
 
        private void genel_ayarlar_Load(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {
                checkBox3.Text = "Off";
                txtdatabase.ReadOnly = true;
                txtserver.ReadOnly = true;
                txtkullaniciadi.ReadOnly = true;
                txtsifre.ReadOnly = true;
            }
            else
            {
                checkBox3.Text = "On";
                txtdatabase.ReadOnly = false;
                txtserver.ReadOnly = false;
                txtkullaniciadi.ReadOnly = false;
                txtsifre.ReadOnly = false;
            }

            string[] dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\DataBase\\Server.jmt");

            if (dizi[0] == "True")
            {
                txtdatabase.Enabled = true;
                txtkullaniciadi.Enabled = true;
                txtserver.Enabled = true;
                txtsifre.Enabled = true;
                rdbsunucu.Checked = Convert.ToBoolean(dizi[0].ToString());
            }

            txtdatabase.Text = dizi[2];
            txtkullaniciadi.Text = dizi[3];
            txtserver.Text = dizi[1];
            txtsifre.Text = dizi[4];

            UcretVerileriniYukle();
            BinisVerileriniYukle();
            InisVerileriniYukle();

            islem_sistem = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Sa.jmt");

            checkBox4.Checked = Convert.ToBoolean(islem_sistem[0]);
            checkBox6.Checked = Convert.ToBoolean(islem_sistem[1]);

            textBox3.Text = islem_sistem[3];
            textBox4.Text = islem_sistem[2];
            textBox8.Text = islem_sistem[5];

            checkBox2.Checked = Convert.ToBoolean(islem_sistem[4]);

            gln_sts = System.IO.File.ReadAllLines(Application.StartupPath + "\\Security\\Sct.jmt");

            gln_krip = Decrypt(gln_sts[0]).Split('-');
            textBox6.Text = gln_krip[0];
            textBox5.Text = gln_krip[1];
            textBox7.Text = gln_krip[2];
            textBox1.Text = gln_krip[3];

            gln_ssa = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ssa.jmt");

            gln_krip_ssa = Decrypt(gln_ssa[0]).Split('-');
            textBox12.Text = gln_krip_ssa[0];
            textBox11.Text = gln_krip_ssa[1];

            for (int i = 0; i < gln_krip_ssa[2].Split('+').Count(); i++)
            {
              if(gln_krip_ssa[2].Split('+')[i] !="")
                comboBox1.Items.Add(gln_krip_ssa[2].Split('+')[i]);
            }
        
            comboBox1.Text = gln_krip_ssa[3];

            gln_esa = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Esa.jmt");

            gln_krip_esa = Decrypt(gln_esa[0]).Split('-');
            textBox14.Text = gln_krip_esa[0];
            textBox13.Text = gln_krip_esa[1];
            textBox10.Text = gln_krip_esa[2];
            textBox15.Text = gln_krip_esa[3];

            foreach (String yazici in PrinterSettings.InstalledPrinters)
            {
                comboBox10.Items.Add(yazici);
                PrintDocument vs = new PrintDocument();
                comboBox10.Text = vs.PrinterSettings.PrinterName;
            }

            string[] sat;

            sat = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ya.jmt");

            comboBox10.Text = sat[0];
            checkBox1.Checked = Convert.ToBoolean(sat[1]);
        }
    
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages[3].SendToBack();
        }   
        
        private void button2_Click_1(object sender, EventArgs e)
        {
            string gonder = Encrypt(textBox6.Text+"-"+textBox5.Text+"-"+textBox7.Text.Trim()+"-"+textBox1.Text.Trim());
            try
            {
                dosya2 = new StreamWriter(Application.StartupPath + "\\Security\\Sct.jmt");
                dosya2.WriteLine(gonder);
            }
            finally
            {
                dosya2.Flush();
                dosya2.Close();
            }

            MessageBox.Show("İşleminiz Başarıyla Gerçekleştirilmiştir");
            textBox1.Enabled = false;
            textBox7.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            textBox7.Enabled = true;
            textBox1.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
        }      
       
        private void button10_Click(object sender, EventArgs e)
        {
            GenelAyarlarKaydet();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

            try
            {
                key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (checkBox4.Checked)
                {
                    key.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
                }
                else
                    key.DeleteValue(Application.ProductName);
            }
            catch { return; }
            finally
            {
                GenelAyarlarKaydet();
                MessageBox.Show("İşleminiz Başarıyla Kaydedilmiştir.");
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)

                checkBox4.Text = "Bilgisayar açıldığında program otomatik olarak çalıştır";
            else
                checkBox4.Text = "Bilgisayar açıldığında program otomatik olarak çalıştırma";
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsNumber((char)e.KeyChar) || e.KeyChar == (char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)44)
            {

                e.Handled = false;
            }
        
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DialogResult mes = MessageBox.Show("Varsayılan ayarlara dönmek istiyormusunuz ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (mes == DialogResult.No)
            { return; }
            try
            {
                islem = new StreamWriter(Application.StartupPath + "\\Setting\\Sa.jmt");

                islem.WriteLine(true);
                islem.WriteLine(true);                          
                islem.WriteLine("20");
                islem.WriteLine("20");
                islem.WriteLine(false);
             }
            finally
            {
                islem.Flush();
                islem.Close();

            }
            try
            {
             key.DeleteValue("ULUSOY  v2.1 JMTech");
            }
            catch { return; }
            finally{
            try
            {
                dosya = new StreamWriter(Application.StartupPath + "\\Setting\\Bk.jmt");

                dosya.WriteLine("Tarih-" + 7 + "-" + 2);
                dosya.WriteLine("Saat-" + 7 + "-" + 2);
                dosya.WriteLine("Koltuk No-" + 5 + "-" + 2);
                dosya.WriteLine("Ad-" + 0 + "-" + 1);
                dosya.WriteLine("Soyad-" + 0 + "-" + 0);
                dosya.WriteLine("Bindi-" + 14 + "-" + 1);
                dosya.WriteLine("İndi-" + 5 + "-" + 0);
                dosya.WriteLine("TC-" + 14 + "-" + 1);
                dosya.WriteLine("Ücret-" + 0 + "-" + 2);
                dosya.WriteLine("yk-" + 0);
                dosya.WriteLine("sg-" + 0);
            }
            finally
            {
                dosya.Flush();
                dosya.Close();
            }
           TextWriter dosyaya = new StreamWriter(Application.StartupPath + "\\Setting\\Ya.jmt");
            try
            {
                dosyaya.WriteLine();
                dosyaya.WriteLine(false);
                dosyaya.WriteLine(true);
                dosyaya.WriteLine(false);
                dosyaya.WriteLine(125);
                dosyaya.WriteLine(250);
            }
            finally
            {
                dosyaya.Flush();
                dosyaya.Close();
            }
            string gonder = Encrypt(textBox6.Text + "-" );

            TextWriter dosya_sct = new StreamWriter(Application.StartupPath + "\\Security\\Sct.jmt");
            try
            {            
                dosya_sct.WriteLine(gonder);
            }
            finally
            {
                dosya_sct.Flush();
                dosya_sct.Close();
            }

            MessageBox.Show("İşleminiz Başarıyla Gerçekleştirildi.");
            Close();
        }
        }
      
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {              
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox8.Enabled = false;         
            }
            else         
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                textBox8.Enabled = true;              
            }
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox2.Checked)

                checkBox2.Text = "TC No Zorunlu olma durumu Aktif";
            else
                checkBox2.Text = "TC No Zorunlu olma durumu Pasif";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (binis_a.Text.Trim() != "")
            {
                SqlCommand ekle = new SqlCommand("insert into  BinisNoktalari(binis)values(" + "'"+ binis_a.Text.Trim() + "')",baglan);
                baglan.Open();
                ekle.ExecuteNonQuery();
                baglan.Close();
              
                listView2.Items.Add(binis_a.Text.Trim());
                BinisVerileriniYukle();
                binis_a.Text = "";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (inis_a.Text.Trim() != "")
            {
                SqlCommand ekle = new SqlCommand("insert into  InisNoktalari(inis)values("+ "'" + inis_a.Text.Trim() + "')", baglan);baglan.Open();
                ekle.ExecuteNonQuery();
                baglan.Close();

                listView1.Items.Add(inis_a.Text.Trim());
                InisVerileriniYukle();

                inis_a.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.listView2.SelectedItems.Count!=0)
            {
                SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
                SqlCommand command = new SqlCommand("delete  from BinisNoktalari where binis='" + listView2.SelectedItems[0].Text + "'", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                listView2.SelectedItems[0].Remove();          
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count != 0)
            {
                SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
                SqlCommand command = new SqlCommand("delete  from InisNoktalari where inis='" + listView1.SelectedItems[0].Text + "'", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                listView1.SelectedItems[0].Remove();
            }
        }
       
        private void button15_Click(object sender, EventArgs e)
        {
            UcretVerileriniYukle();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                SqlConnection myConnection = new SqlConnection(ClsDatabase.data_base());
                myConnection.Open();
                string cmdText = "backup database " + ClsDatabase.aktif_veritabani_adi + " to disk = 'VeriTabani.bak'";
                SqlCommand komut = new SqlCommand(cmdText, myConnection);
                komut.ExecuteNonQuery();
                string kopyalanacakDosya = "C:\\Program Files\\Microsoft SQL Server\\MSSQL.1\\MSSQL\\Backup\\VeriTabani.bak";
                string dosyanınKopyanacagiKlasor = textBox2.Text;
                string kopyalanacakDosyaIsmi = "VeriTabani.bak";
              
                if (File.Exists(dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi) == false)
                {
                    File.Copy(kopyalanacakDosya, dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi);
                    MessageBox.Show("Yedekleme İşlemi Başarıyla gerçekleştirilmiştir.Dosya adı , 'VeriTabani.bak' sistemin tekrar yüklenmesi açısından saklayınız.", "İşlem Başarılı");

                }
                else
                {
                    MessageBox.Show("Belirtilen konumda zaten aynı isimde dosya bulunmaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }                
            }  
            else              
                MessageBox.Show("Lütfen Dosyayı kaydetmek için konum belirleyiniz.", "Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Dosyayı Nereye Kaydetmek İstersiniz?";
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
            }
        
        }  

        private void button9_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + "\\DataBase Aktarma.exe");
            Application.Exit();
        
        }
       
        private void button13_Click_1(object sender, EventArgs e)
        {
           
            if (MessageBox.Show("Ayarlarınızı işleme alınmasını istiyormusunuz ? Devam Ederseniz program yeniden başlatılacaktır.", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (rdbsunucu.Checked)
                {
                    if (txtserver.Text.Trim() == "") { MessageBox.Show("Lütfen Server bölümünü boş geçmeyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (txtdatabase.Text.Trim() == "") { MessageBox.Show("Lütfen DataBase bölümünü boş geçmeyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (txtkullaniciadi.Text.Trim() == "") { MessageBox.Show("Lütfen Kullanıcı Adı bölümünü boş geçmeyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                    if (txtsifre.Text.Trim() == "") { MessageBox.Show("Lütfen Şifre bölümünü boş geçmeyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                }
                try
                {
                    server = new StreamWriter(Application.StartupPath + "\\DataBase\\Server.jmt");
                    server.WriteLine(rdbsunucu.Checked);
                    server.WriteLine(txtserver.Text);
                    server.WriteLine(txtdatabase.Text);
                    server.WriteLine(txtkullaniciadi.Text);
                    server.WriteLine(txtsifre.Text);
                }
                finally
                {
                    server.Flush();
                    server.Close();

                }
                 
                MessageBox.Show("İşleminiz Başarıyla Gerçekleştirilmiştir");
                Application.Restart();
            }
          
        }
     
        private void rdbsunucu_CheckedChanged(object sender, EventArgs e)
        {
            string[]   dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\DataBase\\Server.jmt");

            txtdatabase.Text = dizi[2];
            txtkullaniciadi.Text = dizi[3];
            txtserver.Text = dizi[1];
            txtsifre.Text = dizi[4];
            
            txtdatabase.Enabled = true;
            txtkullaniciadi.Enabled = true;
            txtserver.Enabled = true;
            txtsifre.Enabled = true;
        }

        private void rdblocal_CheckedChanged(object sender, EventArgs e)
        {    
            txtdatabase.Enabled = false;
            txtkullaniciadi.Enabled = false;
            txtserver.Enabled = false;
            txtsifre.Enabled = false;      
        }

        private void checkBox3_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {              
                checkBox3.Text = "Off";
                txtdatabase.ReadOnly = true;
                txtserver.ReadOnly = true;
                txtkullaniciadi.ReadOnly = true;
                txtsifre.ReadOnly = true;
            }
            else
            {    
                checkBox3.Text = "On";
                txtdatabase.ReadOnly = false;
                txtserver.ReadOnly = false;
                txtkullaniciadi.ReadOnly = false;
                txtsifre.ReadOnly = false;
            } 
        }
 
        private void button16_Click_1(object sender, EventArgs e)
        {
         if (BaglantiKontrol() == "true") 
             MessageBox.Show("Bağlantınız başarıyla gerçekleştirildi.Bu bilgileri sistem üzerinde kullanabilir,işlem yapabilirsiniz.Lütfen Kaydetmeyi unutmayınız.","Sunucu Bağlantısı Onaylandı",MessageBoxButtons.OK,MessageBoxIcon.Information);
            else
             MessageBox.Show("Bağlantı hatası alındı.Lütfen bilgilerinizi tekrar kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            ArrayList elemanlar = new ArrayList();

            if (checkBox1.Checked)
            {
                string pname = comboBox10.Text;
                myPrinters.SetDefaultPrinter(pname);
            }

            TextWriter dosya = new StreamWriter(Application.StartupPath + "\\Setting\\Ya.jmt");
            try
            {
                dosya.WriteLine(comboBox10.Text);
                dosya.WriteLine(checkBox1.Checked);
            }
            finally
            {
                dosya.Flush();
                dosya.Close();
            }

            MessageBox.Show("İşleminiz Başarıyla gerçekleştirilmiştir");
        }   

        private void button17_Click(object sender, EventArgs e)
        {
            string ComboBasliklar = "";

            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                ComboBasliklar += comboBox1.Items[i] + "+";
            }

            string gonder2 = Encrypt(textBox12.Text + "-" + textBox11.Text + "-" + ComboBasliklar + "-" + comboBox1.Text);
            try
            {
                dosya3 = new StreamWriter(Application.StartupPath + "\\Setting\\Ssa.jmt");
                dosya3.WriteLine(gonder2);
            }
            finally
            {
                dosya3.Flush();
                dosya3.Close();
            }

            MessageBox.Show("İşleminiz Başarıyla Gerçekleştirilmiştir");
            textBox12.Enabled = false;
            textBox11.Enabled = false;
            textBox9.Enabled = false;
            comboBox1.Enabled = false;
            button20.Enabled = false;
            button21.Enabled = false;
       
        }

        private void button18_Click(object sender, EventArgs e)
        {
            textBox11.Enabled = true;
            textBox12.Enabled = true;
            textBox9.Enabled = true;
            comboBox1.Enabled = true;
            button20.Enabled = true;
            button21.Enabled = true;  
        }

        private void button19_Click(object sender, EventArgs e)
        {
            textBox14.Enabled = true;
            textBox13.Enabled = true;
            textBox10.Enabled = true;
            textBox15.Enabled = true;
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //if (tabControl1.Controls[0] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 360);
            //}

            //else if (tabControl1.Controls[1] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 305);
            //}

            //else if (tabControl1.Controls[2] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 455);
            //}

            //else if (tabControl1.Controls[3] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 345);
            //}

            //else  if (tabControl1.Controls[4] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 361);
            //}

            //else  if (tabControl1.Controls[5] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 320);
            //}

            //else  if (tabControl1.Controls[6] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 280);
            //}

            //else  if (tabControl1.Controls[7] == tabControl1.SelectedTab)
            //{
            //    this.Size = new Size(795, 400);
            //}
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (textBox9.Text != "")
            {              
                comboBox1.Items.Add(textBox9.Text.Trim());
            }
            textBox9.Text = "";
        }

        private void button21_Click(object sender, EventArgs e)
        {
            comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            string gonder = Encrypt(textBox14.Text + "-" + textBox13.Text + "-" + textBox10.Text.Trim() + "-" + textBox15.Text.Trim());
            try
            {
                dosya4 = new StreamWriter(Application.StartupPath + "\\Setting\\Esa.jmt");
                dosya4.WriteLine(gonder);
            }
            finally
            {
                dosya4.Flush();
                dosya4.Close();
            }

            MessageBox.Show("İşleminiz Başarıyla Gerçekleştirilmiştir");

            textBox14.Enabled = false;
            textBox13.Enabled = false;
            textBox10.Enabled = false;
            textBox15.Enabled = false;      
        }   

        private void button22_Click(object sender, EventArgs e)
        {
           if(SmsGonder.SMSGONDER(comboBox1.Text, textBox12.Text.Trim(), "Bu bir test mesajıdır.Sistem düzgün bir şekilde çalışmaktadır.", "")=="-1")
               MessageBox.Show("Sms gönderme işlemi başarısız oldu.Lütfen daha sonra tekrar deneyiniz.", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           else
               MessageBox.Show("Sms gönderme işlemi başarıyla gerçekleşti.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
     
        #endregion
    }
   
}

