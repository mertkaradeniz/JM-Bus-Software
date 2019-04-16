using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows.Forms;
using  System.Management;
using JMOtobusYazilimi;
namespace ULUSOY
{
    class ClsInsert
    {
        static SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private const string AesKey = @"5TGB&YHN7UJM(IK<";
     
        private static string Decrypt(string text)
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

        public static void SistemeOnlineOl()
        {
            string[] gln_sts, gln_krip;
            gln_sts = System.IO.File.ReadAllLines(Application.StartupPath + "\\Security\\Sct.jmt");
            gln_krip = Decrypt(gln_sts[0]).Split('-');

            SqlCommand ekle = new SqlCommand("insert into  OnlineAcenteler(ID,acente_adi,bolge,sistem_adi,pc_adi)values("
             + "'" + frmYazilimGüvenlik.OnlineID + "' , '"
             + gln_krip[2] + "' , '"
             + gln_krip[3] + "' , '"
             + gln_krip[0] + "' , '"
             + System.Environment.MachineName + "')", baglan);
            baglan.Open();
            ekle.ExecuteNonQuery();
            baglan.Close();
        }

        public static void OnlineMesajGonder(string mesaj,string alıcı)
        {
             baglan.Open();

             SqlCommand ekle = new SqlCommand("insert into  OnlineMesajlasma(gonderen,alici,mesaj,tarih,m_gosterilme)values("
             + "'" + frmYazilimGüvenlik.OnlineID + "' , '"
             +alıcı+ "' , '"
             + mesaj+ "' , '"
             + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "' , '"
             +"0" + "')", baglan);
             ekle.ExecuteNonQuery();
          
            SqlDataReader reader = new SqlCommand("select S_ID from OnlineMesajlasma", baglan).ExecuteReader();
            while (reader.Read())
            {
                frmOnlineAcenteler.MesajID[frmOnlineAcenteler.mesaj_sayisi] = reader[0].ToString();
            }
            reader.Close();
            baglan.Close();
        }

        public static void bilet_kes(string tel, string tarih, string bindi, string indi, string no, string plaka, string saat, string bilet_islem, string ucret,string byk,string servis_binis)
        {
            SqlCommand ekle = new SqlCommand("insert into  TumIslemler(tel,tarih,bindi,indi,koltuk_no,plaka,saat,bilet_islem,ucret,ID2,servis_bindi)values("
             + "'" + tel + "' , '"
             + tarih + "' , '"
             + bindi + "' , '"
             + indi + "' , '"
             + no + "' , '"
             + plaka + "' , '"
             + saat + "' , '"
             + bilet_islem + "' , '"
             + ucret + "' , '"
             + byk + "' , '"        
             + servis_binis + "')", baglan);
            baglan.Open();
            ekle.ExecuteNonQuery();
            baglan.Close();
        }
   
        public static string yolcu_ekle(string ID, string tel, int cinsiyet, string ad, string soyad,string tc,string eposta)
        {
            baglan.Open();

            SqlCommand update = new SqlCommand("update  Yolcular set tel='" + tel + "' , cinsiyet='" + cinsiyet + "', ad= '" + ad + "' ,soyad= '" + soyad + "' , tc='" + tc+ "' , eposta='" + eposta + "' where ID2='" + ID + "'", baglan);
            update.ExecuteNonQuery();
            baglan.Close();

            return "Aynı Numarada Bulunduğu için Güncelleştirme İşlemi Yapılmıştır";
        }

        public static string kayıt(string ID, string tel, int cinsiyet, string ad, string soyad,string tc)
        {     
            baglan.Open();

            SqlCommand ekle = new SqlCommand("insert into  Yolcular(tel,cinsiyet,ad,tc,soyad)values("
            + "'" + tel + "' , '"
            + cinsiyet + "' , '"
            + ad + "' , '"
            + tc + "' , '"
            + soyad + "')", baglan);

            ekle.ExecuteNonQuery();
            baglan.Close();
            return "Kayıt Başarıyla Gerçekleşmiştir";  
       }

        public static void OnlineMesajSilme(string[] mesajlar)
        {
            for (int i = 0; i < mesajlar.Count(); i++)
            {
                if (mesajlar[i] != "")
                {
                    SqlCommand command = new SqlCommand("delete  from OnlineMesajlasma where S_ID='" + mesajlar[i] + "'", baglan);
                    baglan.Open();
                    command.ExecuteNonQuery();
                    baglan.Close();
                }
            }
        }

        public static void otobus_ekle(string plaka, int koltuk, string sofor,string tur)
        {
            SqlCommand ekle = new SqlCommand("insert into  Otobusler(plaka,koltuk_sayisi,sofor_adi,tur)values('"
           + plaka + "' , '"
           + koltuk + "' ,'"
           + sofor + "' ,'"
           + tur + "')" , baglan);
         
            baglan.Open();     
            ekle.ExecuteNonQuery();
            baglan.Close();
        }
   
    }
}

