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
    public partial class frmMusteriBilgilendirmePaneliMail : Form
    {
        public frmMusteriBilgilendirmePaneliMail()
        {
            InitializeComponent();
        }

        string[] gln;
        public static string sefertarih = "";
        public static string sefersaat = "";
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

        private void frmMusteriBilgilendirmePaneliMail_Load(object sender, EventArgs e)
        {
            gln = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Esa.jmt");

            for (int i = 0; i < Decrypt(gln[0]).Split('-')[2].Split('+').Count(); i++)
            {
                if (Decrypt(gln[0]).Split('-')[2].Split('+')[i] != "")
                    comboBox1.Items.Add(Decrypt(gln[0]).Split('-')[2].Split('+')[i]);
            }

            comboBox1.Text = Decrypt(gln[0]).Split('-')[2];
            textBox1.Text = "Değerli yolcumuz, " + sefertarih + " tarihli,saat " + sefersaat + "' daki otobüsünüz 10 dk içinde kalkacaktır.İyi yolculuklar dileriz.";
        }

        private void button39_Click(object sender, EventArgs e)
        {
            ClsMail Fonk = new ClsMail();
            if (!Fonk.MAILGONDER(comboBox1.Text.Trim(),eposta.Text, textBox1.Text.Trim()))
                MessageBox.Show("Sms gönderme işlemi başarısız oldu.Lütfen daha sonra tekrar deneyiniz.", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Sms gönderme işlemi başarıyla gerçekleşti.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        }
    }
}
