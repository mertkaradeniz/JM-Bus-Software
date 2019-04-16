using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace JMOtobusYazilimi
{
    class ClsMail
    {
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";
        string[] gln_krip_esa, gln_esa;        

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

        public bool MAILGONDER(string GonderenFirma, string Alici, string Mesaj)
        {
            try
            {
                SmtpClient sc = new SmtpClient();
                sc.Port = 587;
                sc.Host = "smtp.live.com";
                sc.EnableSsl = true;
                sc.Timeout = 50000;

                gln_esa = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Esa.jmt");
                gln_krip_esa = Decrypt(gln_esa[0]).Split('-');

                sc.Credentials = new NetworkCredential(gln_krip_esa[0], gln_krip_esa[1]);

                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(gln_krip_esa[0],GonderenFirma );

                mail.To.Add(Alici);
                mail.Subject = gln_krip_esa[3];
                mail.IsBodyHtml = true;
                mail.Body = Mesaj;

                sc.Send(mail);
                return true;
            }
            catch { return false; }
           
        }
    }
}
