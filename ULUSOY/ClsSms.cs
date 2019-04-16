using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Services.Description;
using System.Windows.Forms;

namespace JMOtobusYazilimi
{
   public class ClsSms
    {
     
       private const string AesIV = @"!QAZ2WSX#EDC4RFV";
       private string AesKey = @"5TGB&YHN7UJM(IK<";
       string[] gln;
       string KullaniciAdi = "", Sifre = "";

       private string XMLPOST(string PostAddress, string xmlData)
        {
            try
            {
                var res = "";
                byte[] bytes = Encoding.UTF8.GetBytes(xmlData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PostAddress);

                request.Method = "POST";
                request.ContentLength = bytes.Length;
                request.ContentType = "text/xml";
                request.Timeout = 300000000;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
               
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string message = String.Format(
                        "POST failed. Received HTTP {0}",
                        response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    Stream responseStream = response.GetResponseStream();
                    using (StreamReader rdr = new StreamReader(responseStream))
                    {
                        res = rdr.ReadToEnd();
                    }
                    return res;
                }
            }
            catch
            {
                return "-1";
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

        public string SMSGONDER(string GonderenFirma,string Alici,string Mesaj,string Tarih)
        {

            gln = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ssa.jmt");

            KullaniciAdi= Decrypt(gln[0]).Split('-')[0];
            Sifre = Decrypt(gln[0]).Split('-')[1];       
            
            String testXml = "<request>";
            testXml += "<authentication>";
            testXml += "<username>" + KullaniciAdi + "</username>";
            testXml += "<password>" + Sifre + "</password>";
            testXml += "</authentication>";
            testXml += "<order>";
            testXml += "<sender>" + GonderenFirma + "</sender>";
            testXml += "<sendDateTime>"+""+"</sendDateTime>";
            testXml += "<message>";
            testXml += "<text>" + Mesaj + "</text>";
            testXml += "<receipents>";
            testXml += "<number>" + Alici + "</number>";
            testXml += "</receipents>";
            testXml += "</message>";
            testXml += "</order>";
            testXml += "</request>";

            if (this.XMLPOST("http://api.iletimerkezi.com/v1/send-sms", testXml) == "-1")
                return "-1";
            else
                return "1";
     
       
       }
   
   }
}
