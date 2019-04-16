using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using JMOtobusYazilimi;
namespace ULUSOY
{
    public partial class frmYolcuKoltuguBelirle : Form
    {
        public frmYolcuKoltuguBelirle()
        {
            InitializeComponent();
        }
        
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        int arac_koltuk;
        public static string yenile = "";
        TextWriter dosya;
        StreamReader dosyaAkimi;
        string[] islem_ayar;
        public string ad_k, bindi_k, indi_k, plaka_k, saat_k, ucret_k, tarih_k, koltuk_k, tc_g, syd;

        #region YAZICI DLL ve FONKSIYONLAR

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.
            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            try
            {
                // Open the printer.
                if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    // Start a document.
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        // Start a page.
                        if (StartPagePrinter(hPrinter))
                        {
                            // Write your bytes.
                            bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }

            }
            catch
            {
                MessageBox.Show("Bu Yazıcı uygun değildir", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;

            // How many characters are in the string?
            // Fix from Nicholas Piasecki:
            // dwCount = szString.Length;
            dwCount = (szString.Length + 1) * Marshal.SystemMaxDBCSCharSize;

            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
      
        #endregion

        #region OZEL FONKSIYONLAR

        public void yolcu_bilgilerini_yukle()
        {
            SqlCommand plak = new SqlCommand("select islem.ID,islem.tel,islem.tarih,islem.bindi,islem.indi,islem.koltuk_no,islem.plaka,islem.saat,islem.bilet_islem,islem.ucret,yolcu.ad,yolcu.soyad,yolcu.cinsiyet,yolcu.ID2,islem.servis_bindi,yolcu.tc,yolcu.eposta from TumIslemler islem inner join Yolcular yolcu on (islem.ID2=yolcu.ID2)    where islem.ID='" + frmAna.ID + "'", baglan);        
            SqlDataReader oku = plak.ExecuteReader();

            if (oku.Read())
            {
                if (Convert.ToInt16(oku[12].ToString()) == 0)
                    cinsiyet.Text = "Bayan";
                else
                    cinsiyet.Text = "Bay";
  
                oto_plaka.Text= oku[6].ToString();
                label8. Text = oku[5].ToString()+". Koltuk";
                koltuk_no.Text = oku[5].ToString();
                rezervasyon_t.Text = oku[2].ToString();
                kalkıs_saati.Text = oku[7].ToString();
                binis.Text = oku[3].ToString();
                inis.Text = oku[4].ToString();
                tc_no.Text = oku[15].ToString();
                telefon.Text = oku[1].ToString();
                isim.Text = oku[10].ToString();
                soyadı.Text = oku[11].ToString();
                label11.Text = oku[13].ToString();
                servis_binis.Text = oku[14].ToString();
                bilet_islem.Text = oku[8].ToString();
                fiyat.Text = oku[9].ToString();
                label16.Text = oku[9].ToString();
                eposta.Text = oku[16].ToString();
            }
            oku.Close();
        }

        public void koltuk_bloke_etme()
        {
            string kendinde_olan = koltuk_no.Text;

            int num;
            this.koltuk_no.Items.Clear();
          
            for (num = 1; num <= Convert.ToInt16(arac_koltuk); num++)
            {
                this.koltuk_no.Items.Add(num);
            }
            
            string[] strArray = new string[44];
            num = 0;
           
            while (num < this.listView3.Items.Count)
            {
                strArray[num] = this.listView3.Items[num].SubItems[5].Text;
                num++;
            }
            int num2 = 1;
          
            for (int i = 1; i < 44; i++)
            {
                for (num = 0; num < this.listView3.Items.Count; num++)
                {
                    if (strArray[num] == i.ToString())
                    {
                        this.koltuk_no.Items.RemoveAt(i - num2);
                        num2++;
                    }
                }
            }

            koltuk_no.Items.Insert(0, kendinde_olan);
        }
    
        private void arac()
        {
            this.listView3.Items.Clear();
            SqlCommand command = new SqlCommand("select islem.ID,islem.tel,islem.tarih,islem.bindi,islem.indi,islem.koltuk_no,islem.plaka,islem.saat,islem.bilet_islem,islem.ucret,yolcu.ad,yolcu.soyad,yolcu.cinsiyet from TumIslemler islem inner join Yolcular yolcu on (islem.ID2=yolcu.ID2)  where tarih=@tarih and plaka=@comboplaka and saat=@combosaat", this.baglan);
            command.Parameters.AddWithValue("@tarih", this.rezervasyon_t.Text);
            command.Parameters.AddWithValue("@comboplaka", oto_plaka.Text.Trim());
            command.Parameters.AddWithValue("@combosaat", this.kalkıs_saati.Text);
        
            SqlDataReader reader2 = command.ExecuteReader();
            while (reader2.Read())
            {
                string[] strArray = reader2[9].ToString().Split(new char[] { ',' });
                ListViewItem item = new ListViewItem(reader2[0].ToString());
                item.SubItems.Add(reader2[1].ToString());
                item.SubItems.Add(reader2[2].ToString());
                item.SubItems.Add(reader2[3].ToString());
                item.SubItems.Add(reader2[4].ToString());
                item.SubItems.Add(reader2[5].ToString());
                item.SubItems.Add(reader2[6].ToString());
                item.SubItems.Add(reader2[7].ToString());
                item.SubItems.Add(reader2[8].ToString());
                item.SubItems.Add(strArray[0].ToString());
                item.SubItems.Add(reader2[10].ToString());
                item.SubItems.Add(reader2[11].ToString());
                item.SubItems.Add(reader2[12].ToString());
                
                listView3.Items.Add(item);
            }
            reader2.Close();
        }
       
        private void yolcu_sayisi()
        {
            SqlCommand command = new SqlCommand("select * from Otobusler where plaka=@comboplaka", this.baglan);
            command.Parameters.AddWithValue("@comboplaka", oto_plaka.Text.Trim());
             
         
            SqlDataReader reader3 = command.ExecuteReader();
            
            if (reader3.Read())            
            {
               arac_koltuk=Convert.ToInt16( reader3[1].ToString());
            }
            reader3.Close();
        }
     
        private void otobus_seferleri()
        {
            kalkıs_saati.Items.Add("07:00");
            kalkıs_saati.Items.Add("07:30");
            kalkıs_saati.Items.Add("08:00");
            kalkıs_saati.Items.Add("08:30");
            kalkıs_saati.Items.Add("09:00");
            kalkıs_saati.Items.Add("09:30");
         
            for (int a = 10; a <= 21; a++)
            {
                kalkıs_saati.Items.Add(a + ":00");
                kalkıs_saati.Items.Add(a + ":30");
            }
        }
    
        public void otobus_doldu()
        {
            string[] tut = kalkıs_saati.Text.Trim().Split('-');
          
            if (Convert.ToInt16(frmAna.dolu_koltuk) == Convert.ToInt16(frmAna.top_koltuk))
            {
                kalkıs_saati.Items.Remove(tut[0].Trim());
                kalkıs_saati.Items.Remove(tut[0].Trim() + " - DOLDU");
                kalkıs_saati.Items.Insert(0, tut[0].Trim() + " - DOLDU");
            }
        }
       
        private void plaka_donen()
        {  
            SqlDataReader reader6 = new SqlCommand("select * from Otobusler where tur='Ana Hat'", this.baglan).ExecuteReader();
            while (reader6.Read())
            {
                this.oto_plaka.Items.Add(reader6[0].ToString());
            }
            reader6.Close();      
        }
     
        private  void OnPrintDocument(object sender, PrintPageEventArgs e)
        {
            string[] satirlar;
            satirlar = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ya.jmt");

            float leftMargin, topMargin;

            Font font = new Font("Verdana", 11);
            float yPozisyon = 0; int LineCount = 0;

            if (satirlar[3] == "true")
            {
                leftMargin = Convert.ToInt16(satirlar[5]);
                topMargin = Convert.ToInt16(satirlar[4]);
            }
            else
            {
                leftMargin = 250;
                topMargin = 125;
            }
        
            string line = null;
            float SayfaBasinaDusenSatir = e.MarginBounds.Height / font.GetHeight();

            while (((line = dosyaAkimi.ReadLine()) != null) && LineCount < SayfaBasinaDusenSatir)
            {
                yPozisyon = topMargin + (LineCount * font.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, font, Brushes.Red, leftMargin, yPozisyon);
                LineCount++;
            }

            if (line == null)
                e.HasMorePages = false;
            else
                e.HasMorePages = true;
        }
    
        void ucret_cek()
        {         
            fiyat.Items.Clear();
            fiyat.Items.Add("");
        
            SqlDataReader reader = new SqlCommand("select * from Ucretler order by ucret", baglan).ExecuteReader();
            while (reader.Read())
            {
                fiyat.Items.Add(reader[0].ToString());
            }
            reader.Close();
        }
   
        void binis_cek()  
        {
           binis.Items.Clear();
           binis.Items.Add("");
          
            SqlDataReader reader2 = new SqlCommand("select * from BinisNoktalari", baglan).ExecuteReader();
           
            while (reader2.Read())        
            {     
               binis.Items.Add(reader2[0].ToString());
            }      
            reader2.Close();
       }
    
        void inis_cek()
        {
           inis.Items.Clear();

           SqlDataReader reader3 = new SqlCommand("select * from InisNoktalari", baglan).ExecuteReader();
           inis.Items.Add("");
           while (reader3.Read())
           {
               inis.Items.Add(reader3[0].ToString());
           }
           reader3.Close();
        }
     
        public static string no_dznle(string no)
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
       
        public static string turkce_cevir(string gln)
        {
            string yazi = gln;
            yazi = yazi.Replace('ö', 'o');
            yazi = yazi.Replace('ü', 'u');
            yazi = yazi.Replace('ğ', 'g');
            yazi = yazi.Replace('ş', 's');
            yazi = yazi.Replace('ı', 'i');
            yazi = yazi.Replace('İ', 'I');
            yazi = yazi.Replace('Ğ', 'G');
            yazi = yazi.Replace('Ö', 'O');
            yazi = yazi.Replace('Ü', 'U');
            yazi = yazi.Replace('Ş', 'S');
            return yazi;
        }

        static string bilet_tc_ayari(string gelen2)
        {
            string bsk = "";
            string gndr = "";

            if (gelen2.Length < 31)
            {

                for (int i2 = 0; i2 < 31 - gelen2.Length; i2++)
                {
                    bsk += " ";
                }

                gndr += gelen2.Trim() + bsk;
            }
            return gndr;
        }

        static string bilet_yazi_ayari(string gelen)
        {
            string bsk = "";
            string gndr = "";
            if (gelen.Length >= 24)
            {
                gndr = gelen.Trim().Substring(0, 24);
                gndr += ".";
            }
            if (gelen.Length < 24)
            {
                for (int i = 0; i < 24 - gelen.Length; i++)
                {
                    bsk += " ";
                }
                gndr += gelen.Trim() + bsk;
            }
            return gndr;
        }

        public void yolcuları_veritabanindan_silme()
        {
            SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
            SqlCommand command = new SqlCommand("delete  from TumIslemler where ID='" + frmAna.ID + "'", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        public static string bosluk_olustur(int b)
        {
            string bosluk = "";
            for (int i = 0; i < b; i++)
            { bosluk += " "; }
            return bosluk;
        }

        #endregion

        #region TUMLESIK FONKSIYONLAR

        private void yolcukoltugubelirle_Load(object sender, EventArgs e)
        {
            baglan.Open();

            foreach (String yazici in PrinterSettings.InstalledPrinters)
            {
                comboBox10.Items.Add(yazici);
                PrintDocument vs = new PrintDocument();
                comboBox10.Text = vs.PrinterSettings.PrinterName;
            }
            string[] satirlar;

            satirlar = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ya.jmt");

            comboBox10.Text = satirlar[0];
            yolcu_bilgilerini_yukle();
            arac(); 
            otobus_seferleri();         
            yolcu_sayisi();
            koltuk_bloke_etme();
            otobus_doldu(); plaka_donen(); ucret_cek(); binis_cek(); inis_cek();        
        }

        private void button2_Click(object sender, EventArgs e)
        {
             if (this.binis.Text == "")
            {
                MessageBox.Show("L\x00fctfen Biniş Noktasını Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if (this.inis.Text == "")
            {
                MessageBox.Show("L\x00fctfen İniş Noktasını Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if (this.oto_plaka.Text == "")
            {
                MessageBox.Show("L\x00fctfen Ara\x00e7 Plakasını Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            } 
            else if (this.kalkıs_saati.Text == "")
            {
                MessageBox.Show("L\x00fctfen Ara\x00e7 Kalkış Saatini Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if (this.koltuk_no.Text == "")
            {
                MessageBox.Show("L\x00fctfen Koltuk Numarasını Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if (this.bilet_islem.Text == "")
            {
                 MessageBox.Show("L\x00fctfen Bilet İşlem B\x00f6l\x00fcm\x00fcn\x00fc Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if ((this.fiyat.Text == "") && (this.bilet_islem.Text.Trim() != "Ücretsiz") && (this.bilet_islem.Text.Trim() != "Rezerve Bilet"))
            {
                 MessageBox.Show("L\x00fctfen Bilet Fiyatını Belirleyiniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if ((no_dznle(telefon.Text).Count() >= 1)&(no_dznle(telefon.Text).Count() <= 9))
            {
                 MessageBox.Show("L\x00fctfen Telefon Numarasını eksiksiz giriniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else if ((tc_no.Text.Count() >= 1)&(tc_no.Text.Count() <= 10))
            {
                 MessageBox.Show("L\x00fctfen TC Kimlik Numarasını eksiksiz giriniz","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            else
            {
                int num2;
                if (this.cinsiyet.Text == "Bay")
                {
                    num2 = 1;
                }
                else
                {
                    num2 = 0;
                }

                ClsInsert.yolcu_ekle(label11.Text, no_dznle(this.telefon.Text.Trim().ToUpper()), num2, this.isim.Text.Trim(), this.soyadı.Text.Trim(),tc_no.Text,eposta.Text.Trim());

                SqlCommand command = new SqlCommand("update  TumIslemler set  bilet_islem=@islem,ucret=@ucret, koltuk_no=@koltuk ,saat=@saat,bindi=@binis,indi=@inis,plaka=@plaka,servis_bindi=@servis,tel=@tel where ID=@islemID", this.baglan);
                command.Parameters.AddWithValue("@koltuk", koltuk_no.Text.Trim());
                command.Parameters.AddWithValue("@binis", binis.Text.Trim());
                command.Parameters.AddWithValue("@inis", inis.Text.Trim());
                command.Parameters.AddWithValue("@tarih", this.rezervasyon_t.Text);
                command.Parameters.AddWithValue("@saat", kalkıs_saati.Text.Trim());
                command.Parameters.AddWithValue("@plaka", oto_plaka.Text.Trim());
                command.Parameters.AddWithValue("@tel", no_dznle(telefon.Text));
                command.Parameters.AddWithValue("@islem",bilet_islem.Text);
                command.Parameters.AddWithValue("@ucret",label16.Text);
                command.Parameters.AddWithValue("@comboplaka", oto_plaka.Text.Trim());
                command.Parameters.AddWithValue("@combosaat", this.kalkıs_saati.Text);
                command.Parameters.AddWithValue("@servis", servis_binis.Text.Trim());   
                command.Parameters.AddWithValue("@islemID", frmAna.ID);

                SqlCommand command03 = new SqlCommand("update  ServerClientBaglanti set durum='True'  where durum='False'", this.baglan);        
                command03.ExecuteNonQuery();
                command.ExecuteNonQuery();
              
                MessageBox.Show("İşleminiz Başarıyla Ger\x00e7ekleştirilmiştir");
            }
        }

        private void comboBox5_SelectedValueChanged(object sender, EventArgs e)
        {
            koltuk_no.Text = "";
            koltuk_no.Items.Clear();
            this.arac();
            this.yolcu_sayisi();
            otobus_doldu();
            koltuk_bloke_etme();  
        }

        private void yolcukoltugubelirle_FormClosed(object sender, FormClosedEventArgs e)
        {
            yenile = "ok";
        }
      
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {     
            kalkıs_saati.Text = "";
            koltuk_no.Items.Clear();
            koltuk_no.Text = "";
            arac();
            this.yolcu_sayisi();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yolcuya ait işlemi silmek istediğinize emin misiniz ?", "Dikkat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                SqlCommand command03 = new SqlCommand("update  ServerClientBaglanti set durum='True'  where durum='False'", this.baglan);
                command03.ExecuteNonQuery();

                yolcuları_veritabanindan_silme();
                yenile = "ok";
              
                MessageBox.Show("Se\x00e7ili bilgi silinmiştir");
                Close();            
            }
        }

        private void comboBox7_SelectedValueChanged(object sender, EventArgs e)
        {
            string[] strArray = this.fiyat.Text.Trim().Split(new char[] { ' ' });

            this.label16.Text = strArray[0].Trim();

            if (fiyat.Text == "")
                this.label16.Text = "00,00";
        }

        private void button39_Click(object sender, EventArgs e)
        {
            if (((bilet_islem.Text.Trim() == "Ücret Alindi") & (fiyat.Text.Trim() == "")) || (fiyat.Text.Trim() == "00,00"))
            {
                MessageBox.Show("Lütfen Ücreti Belirleyiniz", "Uyarı !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            islem_ayar = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Sa.jmt");
           
            if (islem_ayar[4] == "True")
            {
                if ((tc_no.Text.Trim() == "") || (tc_no.Text.Length < 11))
                {
                    MessageBox.Show("Lütfen TC Kimlik Nosunu Doğru Giriniz", "Uyarı !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
               
            }
         
            if ((tc_no.Text.Trim() != "") && (tc_no.Text.Length < 11))
            {
                MessageBox.Show("Lütfen TC Kimlik Nosunu Doğru Giriniz", "Uyarı !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (isim.Text.Trim() == "")
            {
                MessageBox.Show("Lütfen Yolcu Adını Giriniz", "Uyarı !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (soyadı.Text.Trim() == "")
            {
                MessageBox.Show("Lütfen Yolcu Soyadını Giriniz", "Uyarı !", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if ((bilet_islem.Text != "Rezerve Bilet"))
            {
                if (MessageBox.Show("Bilet Kesme İşlemini Onaylıyormusunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {    
                    string[] ucrt;
                    ad_k = isim.Text.Trim().ToUpper();
                    syd = soyadı.Text.Trim().ToUpper();
                    bindi_k = binis.Text.Trim().ToUpper();
                    indi_k = inis.Text.Trim().ToUpper();
                    tc_g=tc_no.Text.Trim();
                    saat_k = kalkıs_saati.Text.Trim().ToUpper();
                    ucrt = fiyat.Text.Trim().ToUpper().Split(' ');
                    string[] g_fiyat = ucrt[0].Split(',');
                    string s_fiyat = g_fiyat[0].ToString().Trim();
                    ucret_k = s_fiyat.ToString();                   
                    tarih_k = rezervasyon_t.Text.Trim().ToUpper();
                    koltuk_k = koltuk_no.Text.Trim().ToUpper();

                    try
                    {
                        dosya = new StreamWriter(Application.StartupPath + "\\Setting\\Ct.jmt");

                        for (int i = 0; i < 9; i++)
                        {
                            dosya.WriteLine("");
                        }

                        dosya.WriteLine("       " + bilet_yazi_ayari(turkce_cevir( bindi_k.Trim()) + " - " +turkce_cevir( indi_k.Trim())) + "  " + tarih_k.Trim() + "       " + saat_k.Trim() + "          " + koltuk_k.Trim());
                        for (int i = 0; i < 2; i++)
                        {
                            dosya.WriteLine("");
                        }

                        dosya.WriteLine("                      " + bilet_tc_ayari(turkce_cevir(ad_k.Trim().ToUpper())) + tc_g);                   
                        dosya.WriteLine("                      " + turkce_cevir(syd.Trim().ToUpper()));
                    
                        for (int i = 0; i < 2; i++)
                        {
                            dosya.WriteLine("");
                        }

                        dosya.WriteLine("                      " + ucret_k.Trim() + "  TL");
                     
                        for (int i = 0; i < 6; i++)
                        {
                            dosya.WriteLine("");
                        }
                    
                        dosya.WriteLine(".");
                    }
                    finally
                    {
                        dosya.Flush();
                        dosya.Close();
                    }

                    string dados = "";

                    StreamReader sr = new StreamReader(Application.StartupPath + "\\Setting\\Ct.jmt");
                    dados = sr.ReadToEnd();
                    sr.Close();

                    string printer = comboBox10.Text.Trim();
                    SendStringToPrinter(printer, dados);
                    button2_Click(sender, new EventArgs());       
                }
            }
            else
                MessageBox.Show("Yolcu Rezerve Olduğu için Bilet Kesilemez", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            frmMusteriBilgilendirmePaneli.sefertarih = rezervasyon_t.Text;
            frmMusteriBilgilendirmePaneli.sefersaat = kalkıs_saati.Text;
            new frmMusteriBilgilendirmePaneli().Show();                     
            ((frmMusteriBilgilendirmePaneli)Application.OpenForms["frmMusteriBilgilendirmePaneli"]).Cihaz_tel.Text = telefon.Text;
            
     
        
        }

        private void button4_Click(object sender, EventArgs e)
        {
           frmMusteriBilgilendirmePaneliMail.sefertarih = rezervasyon_t.Text;
           frmMusteriBilgilendirmePaneliMail.sefersaat = kalkıs_saati.Text;
           new frmMusteriBilgilendirmePaneliMail().Show();
           ((frmMusteriBilgilendirmePaneliMail)Application.OpenForms["frmMusteriBilgilendirmePaneliMail"]).eposta.Text = eposta.Text;
        }

    }
}

