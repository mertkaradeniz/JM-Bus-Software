using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace JMOtobusYazilimi
{
    public partial class frmMusteriBilgilendirmePaneli : Form
    {
        public frmMusteriBilgilendirmePaneli()
        {
            InitializeComponent();
        }

        string[] gln;
        public static string sefertarih = "";
        public static string sefersaat = "";
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";
      
        public static string NoDuzenle(string no)
        {
            string[] strArray = no.Split(new char[] { '-' });
            no = "";

            foreach (string str in strArray)
            {
                no = no + str.Trim();
            }

            string[] strArray2 = no.Split(new char[] { '(' });
            no = "";

            foreach (string str2 in strArray2)
            {
                no = no + str2.Trim();
            }

            string[] strArray3 = no.Split(new char[] { ')' });
            no = "";

            foreach (string str3 in strArray3)
            {
                no = no + str3.Trim();
            }

            string[] strArray4 = no.Split(new char[] { ')' });
            no = "";

            foreach (string str4 in strArray4)
            {
                no = no + str4.Trim();
            }
            return no.Trim();
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

        private void frmMusteriBilgilendirmePaneli_Load(object sender, EventArgs e)
        {
            gln = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ssa.jmt");

            for (int i = 0; i < Decrypt(gln[0]).Split('-')[2].Split('+').Count(); i++)
            {
                if (Decrypt(gln[0]).Split('-')[2].Split('+')[i] != "")
                    comboBox1.Items.Add(Decrypt(gln[0]).Split('-')[2].Split('+')[i]);
            }
           
            comboBox1.Text = Decrypt(gln[0]).Split('-')[3];
            textBox1.Text = "Değerli yolcumuz, " + sefertarih + " tarihli,saat " +sefersaat + "' daki otobüsünüz 10 dk içinde kalkacaktır.İyi yolculuklar dileriz.";
        }

        private void button39_Click(object sender, EventArgs e)
        {
            ClsSms Fonk = new ClsSms();
            if (Fonk.SMSGONDER(comboBox1.Text.Trim(), NoDuzenle(Cihaz_tel.Text), textBox1.Text.Trim(), "") == "-1")
                MessageBox.Show("Sms gönderme işlemi başarısız oldu.Lütfen daha sonra tekrar deneyiniz.", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Sms gönderme işlemi başarıyla gerçekleşti.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        }
    }
}
