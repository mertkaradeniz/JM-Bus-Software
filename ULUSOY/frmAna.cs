using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using ULUSOY;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SpeechLib;
using System.Net;
using JMOtobusYazilimi;
using Excel=Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Security.Cryptography;
using Lisans;
namespace ULUSOY
{
    public partial class frmAna : Form
    {
        public frmAna()
        {
            CheckForIllegalCrossThreadCalls = false;          
            InitializeComponent();
        }

        #region DEGISKENLER

        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        SqlConnection onlinebaglan = new SqlConnection(ClsDatabase.anaserver);
       
        frmGuncellemeKontrolu _FrmGuncellemePaneli;
        frmOnlineAcenteler _frmOnlineAcenteler;

        List<TumIslemler> TumIslemlerPaket = new List<TumIslemler>();
        ClsSms SmsGonder = new ClsSms();

        Button[] Koltuklar = new Button[44];
        TextWriter dosya;      
        public static StreamReader dosyaAkimi;
      
        Color Transfer_eden_renk, Transfer_edilen_renk;
      
        bool UpdateKontrolTrueFalse = false;

        public const short FILE_ATTRIBUTE_NORMAL = 0x80;
        public const short INVALID_HANDLE_VALUE = -1;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint CREATE_NEW = 1;
        public const uint CREATE_ALWAYS = 2;
        public const uint OPEN_EXISTING = 3;
        
        public static string YeniVersiyon = "";
        public static string ID = "", top_koltuk = "", dolu_koltuk = "";
        public static string ARACBILGILERI = "";
       
        private string[] yolcu_ucret;
        string[] islem; 
        string[] isimler, soyadları, tcler;
        string[] gln_sts, gln_krip;
        string[] islem_ayar;
        string[] gln;
        string[] SecilenKoltuklar = new string[50];

        private const string AesIV = @"!QAZ2WSX#EDC4RFV";
        private string AesKey = @"5TGB&YHN7UJM(IK<";
        public static string yolcu_sayısını_yolla = "";
        public static string dısa_aktar_parametre;        
        public static string GelenMesajID = "";
        public string ad_k, bindi_k, indi_k, plaka_k, saat_k, ucret_k, tarih_k, koltuk_k, tc_g, syd;
        
        string kaptan = "";
        string isim_k, soyad_k, tc_k;      
        string poliengel = "";
        string son_kayıt = "";
        string butondan_sender;
        string yenile = "";
        string kntrol_noktasi = "";
        string KAPTAN = "";
        string MusteriTelefonNo = "";

        private int effect = 1;       
        int Transfer_eden = 0, Transfer_edilen = 0;
        private int[] yolcu_ucret_int;
        double Total_Ucret = 0, Araca_Teslim_Edilecek_Tutar = 0, Servis_Ucreti = 0, Reklam_F_Ucreti = 0, Komisyon, Rezerve_Ucret_Total, Fiks;
        int DoluKoltukSayisi = 0;
        int KoltukSayisi = 0;
        int BosKoltukSayisi = 0;
      
        #endregion
  
        #region JMT.dll  
       
        [DllImport("kernel32.dll")]
        static extern ushort GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll")]
        static extern ushort GlobalFindAtom(string lpString);

        [DllImport("kernel32.dll")]
        static extern ushort GlobalDeleteAtom(ushort atom);

        [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

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
        
        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static Boolean demo_k();

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

        #endregion

        #region BILET KESIM FONKSIYONU

        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false;
            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }

            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;

            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            bytes = br.ReadBytes(nLength);
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;

            dwCount = (szString.Length + 1) * Marshal.SystemMaxDBCSCharSize;
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        #endregion

        #region OZEL FONKSIYONLAR

        private void AracVerileriYukleme()
        {
            TumIslemler Deg=new TumIslemler();

            TumIslemlerPaket.Clear();

            string[] strArray = this.arac_saat.Text.Split(new char[] { '-' });
            string[] strArray2 = this.label20.Text.Split(new char[] { '-' });
            string[] tut = label20.Text.Trim().Split(' ');
            string tut2 = tut[2].ToString();
            string[] tut3 = tut2.Split('-');

            SqlCommand command = new SqlCommand("select islem.ID,islem.tel,islem.tarih,islem.bindi,islem.indi,islem.koltuk_no,islem.plaka,islem.saat,islem.bilet_islem,islem.ucret,yolcu.ad,yolcu.soyad,yolcu.cinsiyet,yolcu.ID2,islem.servis_bindi from TumIslemler islem inner join Yolcular yolcu on (islem.ID2=yolcu.ID2) where tarih=@tarih and plaka=@comboplaka and saat=@combosaat ", this.baglan);
            command.Parameters.AddWithValue("@tarih", this.rezer_tarih.Text);
            command.Parameters.AddWithValue("@comboplaka", this.arac_plaka.Text);
            command.Parameters.AddWithValue("@combosaat", tut3[1].Trim());
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string[] strArray3 = reader[9].ToString().Split(new char[] { ',' });
                Deg.ID=Convert.ToInt32(reader[0]);
                Deg.Telefon=reader[1].ToString();
                Deg.Tarih=reader[2].ToString();
                Deg.Bindi=reader[3].ToString();
                Deg.Indi=reader[4].ToString();
                Deg.KoltukNo=reader[5].ToString();
                Deg.Plaka=reader[6].ToString();
                Deg.Saat=reader[7].ToString();
                Deg.BiletIslem=reader[8].ToString();
                Deg.Ucret=reader[9].ToString();
                Deg.Ad = reader[10].ToString();
                Deg.Soyad=reader[11].ToString();
                Deg.Cinsiyet=reader[12].ToString();
                Deg.ID2=Convert.ToInt32(reader[13].ToString());
                Deg.ServisBindi=reader[14].ToString();

                TumIslemlerPaket.Add(Deg);
            }
            reader.Close();
        }

        private void AracToplamTutar()
        {
            if (TumIslemlerPaket.Count > 0)
            {
                int[] numArray = new int[50];
                int num = 0;
                int i = 0;

                foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
                {
                    if (BiletIslemler.BiletIslem != "Ücretsiz")
                        numArray[i] = Convert.ToInt16(BiletIslemler.Ucret.Split(',')[0]);
                    i++;
                }             
                
                for (int j = 0; j < TumIslemlerPaket.Count; j++)
                {
                    num += numArray[j];
                }
                this.lblToplam_tutar.Text = num.ToString() + ",00";
            }
            else
            {
                this.lblToplam_tutar.Text = "00,00";
            }
        }

        private void KoltukKontrol()
        {
            YolcuSayisi();

            for (int j = 0; j < 44; j++)
            {             
               this.Koltuklar[j].Enabled = true;
            }

            int num = Convert.ToInt16(KoltukSayisi);
            for (int i = 1; i < 44; i++)
            {
                for (int j = 0; j < 44; j++)
                {
                    if (this.Koltuklar[j].Name == Convert.ToString((int)(num + i)).ToString())
                    {
                        this.Koltuklar[j].Enabled = false;
                    }
                }
            }
        }

        public string VerileriListeyeYukle(string tel)
        {
            this.listView1.Items.Clear();

            SqlDataReader reader = new SqlCommand("select * from TumIslemler where tel='" + tel + "'", this.baglan).ExecuteReader();
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());
                item.SubItems.Add(reader[4].ToString());
                item.SubItems.Add(reader[5].ToString());
                item.SubItems.Add(reader[7].ToString());
                item.SubItems.Add(reader[6].ToString());
                item.SubItems.Add(reader[3].ToString() + " / " + reader[8].ToString());
                item.SubItems.Add(reader[10].ToString());

                this.listView1.Items.Add(item);
                ListViewItem item2 = new ListViewItem(reader[3].ToString() + " / " + reader[8].ToString());
                item2.SubItems.Add(reader[10].ToString() + " TL");
            }
            reader.Close();
            return "";

        }

        public void IslemSilme(string ID)
        {
            new SqlCommand("delete  from islem where tel='" + ID + "'", this.baglan).ExecuteNonQuery();
        }

        private void OtobusSeferleri()
        {
            arac_saat.Items.Clear();

            arac_saat.Items.Add("07:00");
            arac_saat.Items.Add("07:30");
            arac_saat.Items.Add("08:00");
            arac_saat.Items.Add("08:30");
            arac_saat.Items.Add("09:00");
            arac_saat.Items.Add("09:30");
          
            for (int a = 10; a <= 21; a++)
            {
                arac_saat.Items.Add(a + ":00");
                arac_saat.Items.Add(a + ":30");
            }
        }

        public void OtobusDoldu()
        {
            string[] tut = label20.Text.Trim().Split(' ');
            string tut2 = tut[2].ToString();
            string[] tut3 = tut2.Split('-');
         
            if (DoluKoltukSayisi == KoltukSayisi)
            {
                arac_saat.Items.Remove(tut3[1].Trim());
                arac_saat.Items.Remove(tut3[1] + " - DOLDU");
                arac_saat.Items.Insert(0, tut3[1] + " - DOLDU");
            }
          
            if ((arac_saat.Text == tut3[1] + " - DOLDU") && (DoluKoltukSayisi != KoltukSayisi))
            {
                arac_saat.Items.Remove(tut3[1] + " - DOLDU");
                arac_saat.Items.Insert(0, tut3[1]);

            }

        }

        public void OtobusYolcuResimleriniYukle()
        {
            int index = 0;
            int i = 0;

            while (index < 44)
            {
                Koltuklar[index].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[index].BackColor = Color.Snow;
                index++;
            }

            string[] source = new string[44];
            string[] strArray2 = new string[44];
            string[] strArray3 = new string[44];
            string[] strArray4 = new string[44];
            string[] strArray5 = new string[44];
            string[] strArray6 = new string[44];

         
            foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
            {
                source[i] = BiletIslemler.KoltukNo;
                strArray2[i] = BiletIslemler.Cinsiyet;
                strArray3[i] = BiletIslemler.BiletIslem;
                strArray4[i] = BiletIslemler.KoltukNo;
                strArray5[i] = BiletIslemler.Ad;
                strArray6[i] = BiletIslemler.Soyad;
                i++;
            }
            
            if (TumIslemlerPaket.Count > 0)
            {
                for (int j = 0; j < source.Count<string>(); j++)
                {
                    for (index = 0; index < 44; index++)
                    {
                        if (Koltuklar[index].Name == source[j])
                        {
                            if (strArray2[j] == "1")
                            {
                                if (strArray3[j] == "Rezerve Bilet")
                                {
                                    Koltuklar[index].BackColor = Color.Red;
                                }
                                else if (strArray3[j] == "Ücretsiz")
                                {
                                    Koltuklar[index].BackColor = Color.Yellow;
                                }
                                else
                                {
                                    Koltuklar[index].BackColor = Color.Green;
                                }

                                if (Koltuklar[index].Name == strArray4[j].ToString())
                                {
                                    ToolTip tip = new ToolTip();
                                    tip.SetToolTip(Koltuklar[index], strArray5[j] + " " + strArray6[j]);
                                }
                                Koltuklar[index].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Mr.png");
                            }
                           
                            if (strArray2[j] == "0")
                            {
                                if (strArray3[j] == "Rezerve Bilet")
                                {
                                    Koltuklar[index].BackColor = Color.Red;
                                }
                                else if (strArray3[j] == "Ücretsiz")
                                {
                                    Koltuklar[index].BackColor = Color.Yellow;
                                }
                                else
                                {
                                    Koltuklar[index].BackColor = Color.Green;
                                }

                                if (Koltuklar[index].Name == strArray4[j].ToString())
                                {
                                    ToolTip tip = new ToolTip();
                                    tip.SetToolTip(Koltuklar[index], strArray5[j] + " " + strArray6[j]);
                                }

                                Koltuklar[index].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Mrs.png");
                            }
                      
                        }
                    }
                }
            }
        }

        private void PlakaDonen()
        {
            SqlDataReader reader = new SqlCommand("select * from Otobusler where tur='Ana Hat'", this.baglan).ExecuteReader();
            while (reader.Read())
            {
                this.arac_plaka.Items.Add(reader[0].ToString());
            }
            reader.Close();
        }

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

        private void YolcuSayisi()
        {
            DoluKoltukSayisi =TumIslemlerPaket.Count();
           
            if (this.arac_plaka.Text.Trim() != "")
            {
                SqlCommand command = new SqlCommand("select * from Otobusler where plaka=@comboplaka", this.baglan);
                command.Parameters.AddWithValue("@comboplaka", this.arac_plaka.Text.Trim());
                SqlDataReader reader = command.ExecuteReader();
              
                if (reader.Read())
                {
                    KoltukSayisi =Convert.ToInt32(reader[1]);
                    kaptan = reader[2].ToString();
                }
               reader.Close();
               BosKoltukSayisi = KoltukSayisi - DoluKoltukSayisi;
            }
        }

        public string YolcuKoltukBilgisi(string koltuk)
        {
            string str = "";
            
            foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
            {
                if (BiletIslemler.KoltukNo == koltuk)
                {
                    ID = BiletIslemler.ID.ToString();
                    top_koltuk = DoluKoltukSayisi.ToString();
                    dolu_koltuk = KoltukSayisi.ToString();
                    new frmYolcuKoltuguBelirle().ShowDialog();
                    str = "v";
                    break;
                }
            }
            
            if (str != "v")
            {
                for (int i = 0; i < 44; i++)
                {
                    if (Koltuklar[i].Name == koltuk)
                    {

                        if (Koltuklar[i].BackColor == Color.Gray)
                        {
                            Koltuklar[i].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                            Koltuklar[i].BackgroundImageLayout = ImageLayout.Stretch;
                            Koltuklar[i].BackColor = Color.Snow;
                        }
                        else
                        {
                            Koltuklar[i].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Select.png");
                            Koltuklar[i].BackgroundImageLayout = ImageLayout.Stretch;
                            Koltuklar[i].BackColor = Color.Gray;
                        }

                    }
                }        
            }
            return "";
        }

        private void SoforBilgileri(object sender, EventArgs e)
        {
            if (this.arac_plaka.Text.Trim() == "")
            {
                MessageBox.Show("Her Hangi Bir Ara\x00e7 Se\x00e7mediniz.L\x00fctfen Bir Ara\x00e7 Se\x00e7erek işleminizi Ger\x00e7ekleştiriniz");
            }
            else
            {
                SqlCommand command = new SqlCommand("select * from Otobusler  where plaka='" + this.arac_plaka.Text.Trim() + "'", this.baglan);             
                SqlDataReader reader = command.ExecuteReader();
             
                if (reader.Read())
                {
                    KAPTAN = reader[2].ToString();
                }
                reader.Close();
              
                MessageBox.Show("Bu Ara\x00e7 " + KAPTAN + " Kontrol\x00fcndedir");
            }
        }

        public void OtobusSekilleriniYukle()
        {
            int num3;
            Button s = new Button
            {
                BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Direksiyon.png"),
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(0x16, 0xbd),
                Size = new Size(60, 0x31),
                Font = new Font("Microsoft Sans Serif", 12f),      
                TextAlign = ContentAlignment.TopRight,
                FlatStyle = FlatStyle.Flat,
            };

            s.Click += new EventHandler(this.SoforBilgileri);
            this.panel1.Controls.Add(s);
            Button m = new Button
            {
                BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png"),
                Text = "M",
                BackgroundImageLayout = ImageLayout.Stretch,
                Location = new Point(0x16, 9),
                Size = new Size(60, 0x31),
                Font = new Font("Microsoft Sans Serif", 12f),
                Enabled = false,             
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                FlatStyle = FlatStyle.Flat,

            };
            this.panel1.Controls.Add(m);
            int num = 1;
            int num2 = 0;

            for (num3 = 0; num3 < 5; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Location = new Point(0x9d + num2, 0xbd);
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
            }
            num2 = 0;
            num = 2;

            for (num3 = 5; num3 < 10; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].Location = new Point(0x9d + num2, 0x86);
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);

                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
            }
            num2 = 0;
            num = 3;

            for (num3 = 10; num3 < 15; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].Location = new Point(0x9d + num2, 0x40);
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
              
            }
            num2 = 0;
            num = 4;

            for (num3 = 15; num3 < 20; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Location = new Point(0x9d + num2, 9);
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
              
            }
            num2 = 0;
            num = 21;
          
            for (num3 = 20; num3 < 26; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].Location = new Point(0x22a + num2, 0xbc);
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
              
            }
            num2 = 0;
            num = 22;

            for (num3 = 26; num3 < 32; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Location = new Point(0x22a + num2, 0x85);
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
               
            }
            num2 = 0;
            num = 23;

            for (num3 = 32; num3 < 38; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Location = new Point(0x22a + num2, 0x40);
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
             
            }
            num2 = 0;
            num = 24;

            for (num3 = 38; num3 < 44; num3++)
            {
                Koltuklar[num3] = new Button();
                Koltuklar[num3].Name = num.ToString();
                Koltuklar[num3].Text = num.ToString();
                Koltuklar[num3].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Null.png");
                Koltuklar[num3].BackgroundImageLayout = ImageLayout.Stretch;
                Koltuklar[num3].ForeColor = Color.White;
                Koltuklar[num3].BackColor = Color.Snow;
                Koltuklar[num3].Location = new Point(0x22a + num2, 9);
                Koltuklar[num3].Size = new Size(60, 0x31);
                Koltuklar[num3].FlatStyle = FlatStyle.Flat;
                Koltuklar[num3].FlatAppearance.BorderColor = Color.DodgerBlue;
                Koltuklar[num3].FlatAppearance.MouseDownBackColor = Color.Turquoise;
                Koltuklar[num3].FlatAppearance.MouseOverBackColor = Color.Aquamarine;
                Koltuklar[num3].Font = new Font("Microsoft Sans Serif", 12f);
                Koltuklar[num3].TextAlign = ContentAlignment.TopRight;
                Koltuklar[num3].MouseDown += new MouseEventHandler(KoltukIslem);
                Koltuklar[num3].Click += new EventHandler(this.YolcuKoltuk);
                this.panel1.Controls.Add(Koltuklar[num3]);
                num += 4;
                num2 += 0x42;
              
            }
        }

        private void YolcuKoltuk(object sender, EventArgs e)
        {
            Button button = sender as Button;
            this.YolcuKoltukBilgisi(button.Name);
        }

        private void KoltukIslem(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button btnSender = (Button)sender;
                butondan_sender = btnSender.Name;
                Point ptLowerLeft = new Point(0, btnSender.Height);

                ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
                contextMenuYolcuSecenekleri.Show(ptLowerLeft);
            }

        }

        public void YolcuUcretleri()
        {
        //    if (this.listView2.Items.Count > 0)
        //    {
        //        for (int i = 0; i < this.listView2.Items.Count; i++)
        //        {

        //            this.yolcu_ucret = this.listView2.Items[i].SubItems[1].Text.Split(new char[] { ',' });
        //            this.toplam += Convert.ToInt16(this.yolcu_ucret[0]);
        //        }
        //        this.label38.Text = this.toplam.ToString() + ",00";
        //        this.toplam = 0;
        //    }
        //    else
        //    {
        //        this.toplam = 0;
        //        this.label38.Text = "00,00";
        //    }
        }

        public string TelSorgula(string tel)
        {
            int num = Convert.ToInt16(new SqlCommand("select count(*) from Yolcular  where tel='" + tel + "'", this.baglan).ExecuteScalar());
            SqlDataReader reader = new SqlCommand("select * from Yolcular  where tel='" + tel + "'", this.baglan).ExecuteReader();
            
            while (reader.Read())
            {
                if (Convert.ToInt16(reader[2].ToString()) == 0)
                {
                    this.cinsiyet.Text = "Bayan";
                }
                else
                {
                    this.cinsiyet.Text = "Bay";
                }

                this.musteri_adı.Text = reader[3].ToString();
                this.musteri_soyadı.Text = reader[4].ToString();
                tc.Text = reader[5].ToString();
            }
            reader.Close();
          
            if (num == 0)
            {
                this.cinsiyet.Text = "";
                this.musteri_adı.Text = "";
                this.musteri_soyadı.Text = "";
            }

            return "";
        }

        public void TeslimEdilecekParaninHesaplanmasi()
        {
            islem = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Sa.jmt");

            int[] numArray = new int[50];
            int num = 0;
            int i = 0;

            foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
            {
                if ((BiletIslemler.BiletIslem == "Rezerve Bilet"))
                    numArray[i] = Convert.ToInt16(BiletIslemler.Ucret.Split(',')[0]);
                i++;
            }
            
            for (int j = 0; j < TumIslemlerPaket.Count; j++)
            {
                num += numArray[j];
            }

            Fiks = Convert.ToDouble(islem[5]);

            ////////////////////////////////Genel Matematiksel sonuçların hesaplanması//////////////////////////

            Total_Ucret = Convert.ToDouble(this.lblToplam_tutar.Text.Split(new char[] { ',' })[0]);

            Komisyon = Convert.ToDouble((Total_Ucret * 20) / 100); //Toplam Ücretten %20 lik komisyon alınıyor.
            Rezerve_Ucret_Total = Convert.ToDouble(num);

            Servis_Ucreti = Convert.ToDouble(islem[2]);
            Reklam_F_Ucreti = Convert.ToDouble(Convert.ToDouble(islem[3]));

            Araca_Teslim_Edilecek_Tutar = Total_Ucret - (Komisyon + Servis_Ucreti + Reklam_F_Ucreti + Rezerve_Ucret_Total) + Fiks;

            ///////////////////////////////////////////////////////////////////////////////////////////////////

            lblServis_ucret.Text = Servis_Ucreti.ToString();
            lblReklam_Fon.Text = Reklam_F_Ucreti.ToString();
            lblSonuc.Text = Araca_Teslim_Edilecek_Tutar.ToString();

        }

        void VeriTabaniOlusturma()
        {
            string[] yuklusqller = (string[])Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Microsoft").OpenSubKey("Microsoft SQL Server").GetValue("InstalledInstances");
            //Eğer kullanıcının bilgisayarında SQLExpress yüklü değilse
            var yukluozellikler = (from s in yuklusqller where s.Contains("SQLEXPRESS") select s).FirstOrDefault();
            if (yukluozellikler == null)
            {
                DialogResult sonuc = MessageBox.Show("Programı kullanabilmek için SQLEXPRESS yüklenmelidir. Yüklemek istiyor musunuz?", "UYARI", MessageBoxButtons.YesNo);
                if (sonuc == DialogResult.Yes)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SQLEXPR.exe";
                    Process p = new Process();
                    p.StartInfo.FileName = path;
                    //Aşağıdaki argumanları (parametreleri) SQLEXPRESS setup dosyama göndererek kurulumu başlatırsam kullanıcıya kurulum yeri v.s gibi bilgileri sormayacak ve doğrudan kuruluma geçecektir.
                    p.StartInfo.Arguments = "/qb INSTANCENAME=\"SQLEXPRESS\" INSTALLSQLDIR=\"C:\\Program Files\\Microsoft SQL Server\" INSTALLSQLSHAREDDIR=\"C:\\Program Files\\Microsoft SQL Server\" INSTALLSQLDATADIR=\"C:\\Program Files\\Microsoft SQL Server\" ADDLOCAL=\"All\" SQLAUTOSTART=1 SQLBROWSERAUTOSTART=0 SQLBROWSERACCOUNT=\"NT AUTHORITY\\SYSTEM\" SQLACCOUNT=\"NT AUTHORITY\\SYSTEM\" SECURITYMODE=SQL SAPWD=\"\" SQLCOLLATION=\"SQL_Latin1_General_Cp1_CS_AS\" DISABLENETWORKPROTOCOLS=0 ERRORREPORTING=1 ENABLERANU=0";
                    //Process için pencere oluşturma.
                    p.StartInfo.CreateNoWindow = true;
                    //Process gizli çalışsın.
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.Start();
                    //İşlem bitene kadar bekle
                    p.WaitForExit();
                }
                else
                {
                    //Eğer SQLEXPRESS'i kurmak istemiyorsa programı sonlandırıyorum.
                    this.Close();
                }
            }

        }

        void UcretVerileriniYukle()
        {
            //bilet_fiyat.Items.Clear();

            //SqlDataReader reader = new SqlCommand("select * from Ucretler order by ucret", baglan).ExecuteReader();
            //bilet_fiyat.Items.Add("");
            //while (reader.Read())
            //{
            //    bilet_fiyat.Items.Add(reader[0].ToString());
            //}
            //reader.Close();
        }

        void BiniVerileriniYukle()
        {
            binis_yeri.Items.Clear();

            SqlDataReader reader2 = new SqlCommand("select * from BinisNoktalari order by binis", baglan).ExecuteReader();
            binis_yeri.Items.Add("");
         
            while (reader2.Read())
            {
                binis_yeri.Items.Add(reader2[0].ToString());
            }
            reader2.Close();
        }

        void InisVerileriniYukle()
        {
            inis_yeri.Items.Clear();

            SqlDataReader reader3 = new SqlCommand("select * from InisNoktalari order by inis", baglan).ExecuteReader();
            inis_yeri.Items.Add("");
            while (reader3.Read())
            {
                inis_yeri.Items.Add(reader3[0].ToString());
            }
         
            reader3.Close();
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

        private static void OnPrintDocument(object sender, PrintPageEventArgs e)
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

        public static string TurkceCevir(string gln)
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

        private static byte[] GetDocument()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write('*');
                bw.Write((byte)33);
                bw.Write((byte)3);
                bw.Flush();
                return ms.ToArray();
            }
        }

        static string BiletTcSecenek(string gelen2)
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

        static string BiletYaziAyari(string gelen)
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

        public static string BoslukOlustur(int b)
        {
            string bosluk = "";
            for (int i = 0; i < b; i++)
            { bosluk += " "; }
            return bosluk;
        }

        private void SaatTarih()
        {
            string durum = "";
            string[] dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\DataBase\\Server.jmt");
            if (dizi[0] == "True")
                durum = "Server App";
            else
                durum = "Local App";
            label41.Text = DateTime.Now.ToLongDateString() + " / " + DateTime.Now.ToLongTimeString() + "  " + durum;
        }

        private void KontrolTick(object sender, EventArgs e)
        {
            if ((frmYolcuKoltuguBelirle.yenile == "ok") || (yenile == "ok"))
            {
                try
                {
                    Transfer_edilen = 0;
                    this.AracVerileriYukleme();
                    OtobusDoldu();
                    this.OtobusYolcuResimleriniYukle();
                    this.AracToplamTutar();
                    OtobusDoldu();
                    this.TeslimEdilecekParaninHesaplanmasi();
                   
                    frmYolcuKoltuguBelirle.yenile = "";
                    yenile = "";
                }
                catch
                {
                    return;
                }
            }
        }

        private void UpdateVeriTabaniTick(object sender, EventArgs e)
        {
            string durum_kontrol = "";

            SqlDataReader reader3 = new SqlCommand("select durum from ServerClientBaglanti", baglan).ExecuteReader();
            while (reader3.Read())
            {
                durum_kontrol = reader3[0].ToString();
            }
            reader3.Close();

            if (poliengel != "basladi")
            {
                if (durum_kontrol.Trim() == "True")
                {
                    poliengel = "basladi";
                    ServerClientUpdate2.Enabled = true;
                    this.AracVerileriYukleme();
                    this.YolcuSayisi();
                    this.OtobusYolcuResimleriniYukle();
                    this.AracToplamTutar();
                    this.TeslimEdilecekParaninHesaplanmasi();
                    OtobusDoldu();

                }
            }

        }

        public void YolculariVeriTabanindanSilme()
        {
          
            foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
            {
                if (BiletIslemler.KoltukNo == butondan_sender)
                {
                    SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
                    SqlCommand command = new SqlCommand("delete  from TumIslemler where ID='" + BiletIslemler.ID + "'", connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    butondan_sender = "";
                    SqlCommand command03 = new SqlCommand("update  ServerClientBaglanti set durum=@durum  where durum='False'", this.baglan);
                    command03.Parameters.AddWithValue("@durum", "True");
                    command03.ExecuteNonQuery();
                }
            }
            
        }

        public void BiletKesmeIslemi()
        {
            islem_ayar = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Sa.jmt");

            if (this.binis_yeri.Text == "")
            {
                MessageBox.Show("L\x00fctfen Biniş Noktasını Belirleyiniz");
            }
            else if (this.inis_yeri.Text == "")
            {
                MessageBox.Show("L\x00fctfen İniş Noktasını Belirleyiniz");
            }

            else if (this.arac_plaka.Text == "")
            {
                MessageBox.Show("L\x00fctfen Ara\x00e7 Plakasını Belirleyiniz");
            }
            else if (this.arac_saat.Text == "")
            {
                MessageBox.Show("L\x00fctfen Ara\x00e7 Kalkış Saatini Belirleyiniz");
            }
            else if (this.bilet_islem.Text == "")
            {
                MessageBox.Show("L\x00fctfen Bilet İşlem B\x00f6l\x00fcm\x00fcn\x00fc Belirleyiniz");
            }
            else if ((this.bilet_fiyat.Text == "") && (this.bilet_islem.Text.Trim() != "Ücretsiz") && (this.bilet_islem.Text.Trim() != "Rezerve Bilet"))
            {
                MessageBox.Show("L\x00fctfen Bilet Fiyatını Belirleyiniz");
            }
            else if (this.cinsiyet.Text == "")
            {
                MessageBox.Show("L\x00fctfen Yolcu Cinsiyetini Giriniz");
            }
            else
            {
                int dongu = 0;
                string[] koltuk_tut = new string[44];
                for (int say = 0; say < 44; say++)
                {
                    if (Koltuklar[say].BackColor == Color.Gray)
                    {
                        koltuk_tut[dongu] = Koltuklar[say].Name;
                        dongu++;
                    }
                }

                if (dongu == 0)
                {
                    MessageBox.Show("Lütfen Koltuk Belirleyiniz");
                    return;
                }

                tcler = tc.Text.Split(',');
                int count = 0, count2, count3;

                isimler = musteri_adı.Text.Trim().Split(',');
                soyadları = musteri_soyadı.Text.Trim().Split(',');

                if ((bilet_islem.Text == "Ücret Alindi") || (bilet_islem.Text == "Ücretsiz"))
                {

                    if (isimler[0].Trim() == "")
                        count = 0;
                    else
                        count = isimler.Count();
                    if (soyadları[0].Trim() == "")
                        count2 = 0;
                    else
                        count2 = soyadları.Count();
                    if (tcler[0].Trim() == "")
                        count3 = 0;
                    else
                        count3 = tcler.Count();

                    if (((count < dongu) || (count2 < dongu)))
                    {
                        MessageBox.Show("Lütfen Koltuk Sayıyı kadar Ad/Soyad Giriniz");
                        return;
                    }

                    if (islem_ayar[4] == "True")
                    {
                        if (count3 < dongu)
                        {

                            MessageBox.Show("Lütfen Koltuk Sayıyı kadar TC No Giriniz");
                            return;
                        }
                    }

                }

                Array.Clear(SecilenKoltuklar, 0, SecilenKoltuklar.Count());

                /////////////////////////Tekrarlama olayı begin////////////////////////////
                for (int bilet_kes = 0; bilet_kes < dongu; bilet_kes++)
                {
                    if (islem_ayar[4] == "True")
                    {
                        if (tcler[bilet_kes].Length < 11)
                        {
                            MessageBox.Show("Lütfen  TC Kimlik Nosunu Eksiksiz Giriniz");
                            return;
                        }
                    }
                    else
                        tc_k = "";

                    int num2;

                    for (int i = 0; i < 44; i++)
                    {
                        if (Koltuklar[i].Name == koltuk_tut[bilet_kes])
                        {
                            SecilenKoltuklar[i] = Koltuklar[i].Name;
                            
                            if (this.cinsiyet.Text == "Bay")
                            {
                                if (this.bilet_islem.Text == "Rezerve Bilet")
                                {
                                    Koltuklar[i].BackColor = Color.Red;
                                }
                                else if (this.bilet_islem.Text == "Ücretsiz")
                                {
                                    Koltuklar[i].BackColor = Color.Yellow;
                                }
                                else
                                {
                                    Koltuklar[i].BackColor = Color.Green;
                                }
                                Koltuklar[i].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Mr.png");
                            }

                            if (this.cinsiyet.Text == "Bayan")
                            {
                                if (this.bilet_islem.Text == "Rezerve Bilet")
                                {
                                    Koltuklar[i].BackColor = Color.Red;
                                }
                                else if (this.bilet_islem.Text == "Ücretsiz")
                                {
                                    Koltuklar[i].BackColor = Color.Yellow;
                                }
                                else
                                {
                                    Koltuklar[i].BackColor = Color.Green;
                                }

                                Koltuklar[i].BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\\Mrs.png");
                            }
                        }
                    }

                    if (this.cinsiyet.Text == "Bay")
                    {
                        num2 = 1;
                    }
                    else
                    {
                        num2 = 0;
                    }

                    if (radioButton1.Checked == true)
                        MusteriTelefonNo = Telefon_no.Text;
                    if (radioButton2.Checked == true)
                        MusteriTelefonNo = Cihaz_tel.Text;
                    string byk = "";

                    if (bilet_islem.Text == "Rezerve Bilet")
                    {
                        isim_k = "";
                        soyad_k = "";
                        tc_k = "";
                    }
                    else
                    {
                        isim_k = isimler[bilet_kes];
                        soyad_k = soyadları[bilet_kes];

                        if (islem_ayar[4] == "True")
                        {
                            tc_k = tcler[bilet_kes];
                        }
                    }

                    ClsInsert.kayıt("", NoDuzenle(MusteriTelefonNo.Trim()), num2, isim_k, soyad_k, tc_k);
                    SqlCommand command = new SqlCommand("select max(ID2) from Yolcular", baglan);
                    byk = Convert.ToString(command.ExecuteScalar());
                    string[] tut = this.arac_saat.Text.Split('-');
                    ClsInsert.bilet_kes(NoDuzenle(MusteriTelefonNo.Trim()), this.rezer_tarih.Text, this.binis_yeri.Text, this.inis_yeri.Text, koltuk_tut[bilet_kes], this.arac_plaka.Text.Trim(), tut[0], this.bilet_islem.Text, this.toplam_fiyat.Text, byk, servis_binis.Text);

                    ////////////////////////////////BİLET KESME İŞLEMİ//////////////////////////////
                    if (bilet_islem.Text != "Rezerve Bilet")
                    {
                        if (MessageBox.Show("Bilet Kesme İşlemini Onaylıyormusunuz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                        {

                            ad_k = isim_k;
                            syd = soyad_k;
                            bindi_k = binis_yeri.Text.ToUpper();
                            indi_k = inis_yeri.Text.ToUpper();
                            plaka_k = arac_plaka.Text;
                            saat_k = arac_saat.Text;
                            tc_g = tc_k; ;
                            string[] g_fiyat = toplam_fiyat.Text.Split(',');
                            string s_fiyat = g_fiyat[0].ToString().Trim();
                            ucret_k = s_fiyat.ToString();
                            tarih_k = rezer_tarih.Text;
                            koltuk_k = koltuk_tut[bilet_kes];

                            try
                            {
                                dosya = new StreamWriter(Application.StartupPath + "\\Setting\\Ct.jmt");

                                for (int i = 0; i < 9; i++)
                                {
                                    dosya.WriteLine("");
                                }

                                dosya.WriteLine("       " + BiletYaziAyari(TurkceCevir(bindi_k.Trim()) + " - " + TurkceCevir(indi_k.Trim())) + "  " + tarih_k.Trim() + "       " + saat_k.Trim() + "          " + koltuk_k.Trim());
                                for (int i = 0; i < 2; i++)
                                {
                                    dosya.WriteLine("");
                                }

                                dosya.WriteLine("                      " + BiletTcSecenek(TurkceCevir(ad_k.Trim().ToUpper())) + tc_g);
                                dosya.WriteLine("                      " + TurkceCevir(syd.Trim().ToUpper()));

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
                        }
                    }

                    //////////////tekrarlama olayı////////////////end//////////
                    SqlCommand command03 = new SqlCommand("update  ServerClientBaglanti set durum='True'  where durum='False'", this.baglan);
                    command03.ExecuteNonQuery();
                }

                if (MusteriTelefonNo != "")
                {
                    if (DialogResult.Yes == MessageBox.Show("Sms gönderme işlemini onaylıyor musunuz ?", "Sms Gönderme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                    {
                        SmsGonderme.RunWorkerAsync();
                    }
                }

                if (MessageBox.Show("Formu Temizlemek ister misiniz ?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    this.listView1.Items.Clear();
                    this.arac_saat.Text = "";
                    this.arac_plaka.ValueMember = "";
                    this.inis_yeri.Text = "";
                    this.binis_yeri.Text = "";
                    this.cinsiyet.Text = "";
                    this.bilet_fiyat.Text = "56";
                    this.bilet_islem.Text = "";
                    this.musteri_adı.Text = "";
                    this.musteri_soyadı.Text = "";
                    servis_binis.Text = "";
                    tc.Text = "";
                    this.AracVerileriYukleme();
                    this.OtobusYolcuResimleriniYukle();
                    this.YolcuSayisi();
                    AracToplamTutar();
                    this.TeslimEdilecekParaninHesaplanmasi();

                    SqlCommand command04 = new SqlCommand("update  ServerClientBaglanti set durum='True'  where durum='False'", this.baglan);
                    command04.ExecuteNonQuery();
                }
                else
                {
                    this.musteri_adı.Text = "";
                    this.musteri_soyadı.Text = "";
                    this.AracVerileriYukleme();
                    this.OtobusYolcuResimleriniYukle();
                    this.YolcuSayisi();
                    AracToplamTutar();
                    this.TeslimEdilecekParaninHesaplanmasi();

                    SqlCommand command04 = new SqlCommand("update  ServerClientBaglanti set durum='True'  where durum='False'", this.baglan);
                    command04.ExecuteNonQuery();
                }
            }
        }

        public void VeriYuklemeKalip()
        {
            if ((arac_plaka.Text != "")&&(arac_saat.Text !=""))
            {
                AracVerileriYukleme();
                YolcuSayisi();
                AracToplamTutar();
                TeslimEdilecekParaninHesaplanmasi();
                OtobusYolcuResimleriniYukle();
                ServerClientUpdate.Start();
                KoltukKontrol();
                OtobusDoldu();                
                Transfer_edilen = 0;
            }           
        }

        public void FrmLoadYukle()
        {
            //UrunKontrol.Start();
           
            int DateDifference = Convert.ToInt16((new LiBackCount().LiDateMinutes() - new LiBackCount().BackCountRemaining()) / 60 / 24);       
          
            Text = "JM Otobüs Programı v" + Application.ProductVersion + " - JM Technology [Kalan Kullanım: "+DateDifference.ToString()+" Gün]";

            

            islem = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Sa.jmt");
            lblServis_ucret.Text = Convert.ToDouble(islem[3]).ToString();
            gln_sts = System.IO.File.ReadAllLines(Application.StartupPath + "\\Security\\Sct.jmt");
            gln_krip = Decrypt(gln_sts[0]).Split('-');

            label18.Text = "Acente Adı : " + gln_krip[2] + "              Bölge : " + gln_krip[3] + "              Sistem Adı : " + gln_krip[0] + "              PC Adı : " + System.Environment.MachineName + "              Sunucu : ";

            string[] dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\DataBase\\Server.jmt");
            if (dizi[1] == "Null")
                dizi[1] = "LocalHost";

            label18.Text += dizi[1];

            foreach (String yazici in PrinterSettings.InstalledPrinters)
            {
                comboBox10.Items.Add(yazici);
                PrintDocument vs = new PrintDocument();
                comboBox10.Text = vs.PrinterSettings.PrinterName;
            }

            string[] satirlar;
            satirlar = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ya.jmt");

            comboBox10.Text = satirlar[0];

            axCIDv51.Hide();
            axCIDv51.Start();
            yolcu_ucret = new string[0x98967f];
            yolcu_ucret_int = new int[0x98967f];
            panel1.Visible = false;           
            
            baglan = new SqlConnection(ClsDatabase.data_base());
            baglan.Open(); 
        
            OtobusSekilleriniYukle();
            PlakaDonen();
           
            UcretVerileriniYukle(); BiniVerileriniYukle(); InisVerileriniYukle();
            panel8.Visible = true;
        }

        public void ExcelVerileriAktar()
        {
            try
            {
                if (label20.Text.Length > 5)
                {
                    string[] s1 = label20.Text.Trim().Split(' ');
                    string s2 = s1[2].ToString();
                    string[] s3 = s2.Split('-');
                    string[] s4 = label20.Text.Split('-');
                    if ((s4[1] != "") && (arac_plaka.Text != ""))
                    {

                        this.listView4.Items.Clear();
                        string[] strArray = this.arac_saat.Text.Split(new char[] { '-' });
                        string[] strArray2 = this.label20.Text.Split(new char[] { '-' });
                        string[] tut = label20.Text.Trim().Split(' ');
                        string tut2 = tut[2].ToString();
                        string[] tut3 = tut2.Split('-');
                        string cins = "";
                        SqlCommand command = new SqlCommand("select islem.ID,yolcu.ad,yolcu.soyad,islem.tel,islem.bindi,islem.indi,islem.koltuk_no,yolcu.cinsiyet,islem.bilet_islem,islem.ucret from TumIslemler islem inner join Yolcular yolcu on (islem.ID2=yolcu.ID2) where tarih=@tarih and plaka=@comboplaka and saat=@combosaat Order by islem.koltuk_no", this.baglan);
                        command.Parameters.AddWithValue("@tarih", this.rezer_tarih.Text);
                        command.Parameters.AddWithValue("@comboplaka", this.arac_plaka.Text);
                        command.Parameters.AddWithValue("@combosaat", tut3[1].Trim());
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {

                            ListViewItem item = new ListViewItem(reader[1].ToString() + " " + reader[2].ToString());
                            item.SubItems.Add(reader[3].ToString());
                            item.SubItems.Add(reader[4].ToString());
                            item.SubItems.Add(reader[5].ToString());
                            item.SubItems.Add(reader[6].ToString());

                            if (reader[7].ToString() == "1")
                                cins = "Bay";
                            else
                                cins = "Bayan";

                            item.SubItems.Add(cins);
                            item.SubItems.Add(reader[8].ToString());
                            item.SubItems.Add(Convert.ToDouble(reader[9]) + ",00");

                            this.listView4.Items.Add(item);
                        }
                        reader.Close();

                        Excel._Workbook kitap;
                        Excel._Worksheet ws;
                        Excel.Range rng;
                        Excel._Application app = new Excel.Application();
                        app.Visible = true;
                        kitap = (Excel._Workbook)(app.Workbooks.Add(Missing.Value));
                        ws = (Microsoft.Office.Interop.Excel.Worksheet)kitap.ActiveSheet;
                        //Kolon başlıkları sabit olduğu için listviewdeki veri aktarılmadan önce sabit verileri excel e giriyoruz.
                        object hucre;
                        Microsoft.Office.Interop.Excel.Range rr;
                        string str;

                        //ws.PageSetup.PrintGridlines = true;
                        //ws.PageSetup.CenterHorizontally = true;//sayfayi yatayda ortala
                        //ws.PageSetup.CenterVertically = false;
                        //ws.PageSetup.PaperSize = Excel.XlPaperSize.xlPaperTabloid;

                        rng = ws.get_Range("A46", "H46");
                        rng.Cells.get_Offset();

                        ws.get_Range("A46", "A46").Value2 = "Kaptan:";
                        ws.get_Range("B46", "B46").Value2 = kaptan;
                        ws.get_Range("C46", "C46").Value2 = "Plaka:";
                        ws.get_Range("D46", "D46").Value2 = arac_plaka.Text;
                        ws.get_Range("E46", "E46").Value2 = "Tarih:";
                        ws.get_Range("F46", "F46").Value2 = rezer_tarih.Text;
                        ws.get_Range("A47", "A47").Value2 = "Toplam:";
                        ws.get_Range("B47", "B47").Value2 = lblToplam_tutar.Text;
                        ws.get_Range("C47", "C47").Value2 = "Komisyon:";
                        ws.get_Range("D47", "D47").Value2 = Komisyon;
                        ws.get_Range("E47", "E47").Value2 = "Teslim Tutar:";
                        ws.get_Range("F47", "F47").Value2 = lblSonuc.Text;
                        ws.get_Range("A46", "H46").Font.Color = Color.White;
                        ws.get_Range("A47", "H47").Font.Color = Color.White;
                        ws.get_Range("A46", "H46").Font.Bold = true;
                        ws.get_Range("A46", "H46").Font.Size = 10;
                        ws.get_Range("A47", "H47").Font.Bold = true;
                        ws.get_Range("A47", "H47").Font.Size = 10;

                        for (int x = 46; x < 48; x++)
                        {
                            for (int y = 1; y < 9; y++)
                            {
                                ws.Cells[x, y].Interior.Color = Color.Gray;
                            }
                        }

                        hucre = ws.Cells[1, 1];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Yolcu Adı";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 2];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Telefon";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 3];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Bindiği Yer";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 4];
                        rr = ws.get_Range(hucre, hucre);
                        str = "İndiği Yer";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 5];
                        rr = ws.get_Range(hucre, hucre);
                        str = "K.No";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 6];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Cinsiyet";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 7];
                        rr = ws.get_Range(hucre, hucre);
                        str = "B.İşlem";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 8];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Ücret";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        int dikey = 2;
                        int yatay = 2;

                        foreach (ListViewItem lvi in listView4.Items)
                        {
                            ws.Columns.AutoFit();
                            dikey = 1;
                            foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                            {
                                ws.Cells[yatay, dikey] = lvs.Text;

                                if ((yatay % 2) == 0)
                                    ws.Cells[yatay, dikey].Interior.Color = Color.SkyBlue;

                                else
                                    ws.Cells[yatay, dikey].Interior.Color = Color.MistyRose;

                                if (lvs.Text == "Ücret Alindi")
                                    ws.Cells[yatay, dikey].Interior.Color = Color.Green;

                                if (lvs.Text == "Ücretsiz")
                                    ws.Cells[yatay, dikey].Interior.Color = Color.Yellow;

                                if (lvs.Text == "Rezerve Bilet")
                                    ws.Cells[yatay, dikey].Interior.Color = Color.Red;

                                dikey++;
                            }
                            yatay++;
                        }
                        kitap.Close();
                    }
                    else
                        MessageBox.Show("Lütfen Plaka veya Saati Boş Geçmeyiniz");
                }
            }
            catch { MessageBox.Show("Bir sorun oluştu.Lütfen tekrar deneyiniz.", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public void WordeVerileriAktar()
        {
            if (label20.Text != "")
            {
                string[] tut = label20.Text.Split('-');
                if ((tut[1] != "") && (arac_plaka.Text != ""))
                {
                    SqlCommand command = new SqlCommand("select * from Otobusler  where plaka='" + this.arac_plaka.Text.Trim() + "'", this.baglan);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        KAPTAN = reader[2].ToString();
                    }
                    reader.Close();

                    ARACBILGILERI = KAPTAN + "~" + lblToplam_tutar.Text + "~" + lblServis_ucret.Text + "~" + lblReklam_Fon.Text + "~" + lblSonuc.Text + "~" + Komisyon;
                    dısa_aktar_parametre = rezer_tarih.Text + "+" + arac_plaka.Text + "+" + tut[1].Trim();
                    new frmWordAktar().ShowDialog();
                }
            }
            else
                MessageBox.Show("Lütfen Plaka veya Saati Boş Geçmeyiniz");
        }

        public void ServerClientUpdatePaket()
        {
            string durum_kontrol = "";

            SqlDataReader reader3 = new SqlCommand("select durum from ServerClientBaglanti", baglan).ExecuteReader();
            while (reader3.Read())
            {
                durum_kontrol = reader3[0].ToString();
            }

            reader3.Close();

            if (poliengel != "basladi")
            {
                if (durum_kontrol.Trim() == "True")
                {
                    poliengel = "basladi";
                    ServerClientUpdate2.Enabled = true;
                    this.AracVerileriYukleme();
                    this.YolcuSayisi();
                    this.OtobusYolcuResimleriniYukle();
                    this.AracToplamTutar();
                    this.TeslimEdilecekParaninHesaplanmasi();
                    OtobusDoldu();
                }
            }
        }

        public void FormGecisUpdatePaket()
        {
            if ((frmYolcuKoltuguBelirle.yenile == "ok") || (yenile == "ok"))
            {
                try
                {
                    Transfer_edilen = 0;
                    AracVerileriYukleme();
                    OtobusDoldu();
                    this.OtobusYolcuResimleriniYukle();
                    this.AracToplamTutar();
                    OtobusDoldu();
                    TeslimEdilecekParaninHesaplanmasi();
                    frmYolcuKoltuguBelirle.yenile = "";
                    yenile = "";
                }
                catch
                {
                    return;
                }
            }
        }

        #endregion

        #region TUMLESIK FONKSİYONLAR

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new frmAracEkleme().ShowDialog();
        }
    
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            VeriYuklemeKalip();
        }
      
        private void frmana_Load(object sender, EventArgs e)
        {
            FrmLoadYukle();
        }
    
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            baglan.Close();
            Application.Exit();
        }      
      
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string[] tut = arac_saat.Text.Trim().Split('-');
            label20.Text = arac_plaka.Text + "-" + tut[0].Trim();
          
            panel6.Visible = false;
            panel6.Visible = true;
            panel8.Visible = false;
            panel1.Visible = true;
            label20.Visible = true;

            VeriYuklemeKalip();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.TelSorgula(NoDuzenle(this.Telefon_no.Text.Trim().ToUpper()));
            this.VerileriListeyeYukle(NoDuzenle(this.Telefon_no.Text.Trim().ToUpper()));         
        }
   
        private void button39_Click(object sender, EventArgs e)
        {
            BiletKesmeIslemi();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            base.WindowState = FormWindowState.Maximized;
        }
  
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            base.WindowState = FormWindowState.Minimized;
        }
     
        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            new frmAracEkleme().ShowDialog();
        }

        private void comboBox7_SelectedValueChanged(object sender, EventArgs e)
        {

            string[] strArray = this.bilet_fiyat.Text.Trim().Split(new char[] { ' ' });
            this.toplam_fiyat.Text = strArray[0].Trim();
            if (bilet_fiyat.Text == "")
                this.toplam_fiyat.Text = "00,00";
        }
      
        private void toolStripButton6_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            baglan.Close();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (label20.Text.Length > 5)
            {
                string[] tut = label20.Text.Split('-');
                if ((tut[1] != "") && (arac_plaka.Text != ""))
                {
                    dısa_aktar_parametre = rezer_tarih.Text + "+" + arac_plaka.Text + "+" + arac_saat.Text;
                    new frmWordAktar().ShowDialog();
                }
                else
                    MessageBox.Show("Lütfen Plaka veya Saati Boş Geçmeyiniz");
            }
        }
      
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (label20.Text.Length > 5)
            {
                string[] tut0 = label20.Text.Trim().Split(' ');
                string tut2 = tut0[2].ToString();
                string[] tut3 = tut2.Split('-');
                string[] tut = label20.Text.Split('-');
                if ((tut[1] != "") && (arac_plaka.Text != ""))
                {
                    this.WindowState = FormWindowState.Minimized;
                    dısa_aktar_parametre = rezer_tarih.Text + "+" + arac_plaka.Text + "+" + tut3[1] + "+" + kaptan + "+" + lblToplam_tutar.Text + "+" + lblServis_ucret.Text + "+" + lblReklam_Fon.Text + "+" + lblSonuc.Text + "+" + Komisyon;
                    new frmWordAktar().ShowDialog();
                }
                else
                    MessageBox.Show("Lütfen Plaka veya Saati Boş Geçmeyiniz");
            }
        }
      
        private void timer3_Tick_1(object sender, EventArgs e)
        {
            label19.Text = "Cihaz : " + axCIDv51.Command("Devicemodel") + "     " + axCIDv51.Command("Serial");
        }
        
        private void axCIDv51_OnCallerID_1(object sender, Axcidv5callerid.ICIDv5Events_OnCallerIDEvent e)
        {
            string tut = e.phoneNumber.Substring(1, 10).ToString();
            Cihaz_tel.Text = tut + "0";
        }
    
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 58 & e.KeyChar >= 48)
                e.Handled = true;
          
            string last;
            int Upper_Character_Checking = 0;
          
            if (e.KeyChar == (char)32 || musteri_adı.Text == "") { Upper_Character_Checking = 1; }
           
            if (musteri_adı.Text != "")
            {
                last = musteri_adı.Text.Substring(musteri_adı.Text.Length - 1);
                if (last == ((char)32).ToString() && e.KeyChar != (char)8) Upper_Character_Checking = 1;
            }
           
            if (e.KeyChar > (char)95 && e.KeyChar < (char)123 && Upper_Character_Checking == 1)
            {
                Upper_Character_Checking = 0;
                e.KeyChar = (char)((int)(e.KeyChar - 32));
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 58 & e.KeyChar >= 48)
                e.Handled = true;
            string last;
            int Upper_Character_Checking = 0;
            if (e.KeyChar == (char)32 || musteri_soyadı.Text == "") { Upper_Character_Checking = 1; }
            if (musteri_soyadı.Text != "")
            {
                last = musteri_soyadı.Text.Substring(musteri_soyadı.Text.Length - 1);
                if (last == ((char)32).ToString() && e.KeyChar != (char)8) Upper_Character_Checking = 1;
            }
            if (e.KeyChar > (char)95 && e.KeyChar < (char)123 && Upper_Character_Checking == 1)
            {
                Upper_Character_Checking = 0;
                e.KeyChar = (char)((int)(e.KeyChar - 32));
            }
        }
    
        private void button2_Click_2(object sender, EventArgs e)
        {
            string[] tut = arac_saat.Text.Trim().Split('-');
            this.label20.Text = this.arac_plaka.Text + "-" + tut[0].Trim();
            this.panel6.Visible = false;
            this.panel6.Visible = true;
            this.panel8.Visible = false;
            this.panel1.Visible = true;
            this.AracVerileriYukleme();
            this.YolcuSayisi();
            this.OtobusYolcuResimleriniYukle();
            this.AracToplamTutar();
            this.TeslimEdilecekParaninHesaplanmasi();
            OtobusDoldu();
        }
           
        private void button2_Click_3(object sender, EventArgs e)
        {
            musteri_soyadı.Text = "";
            musteri_adı.Text = "";
            cinsiyet.Text = "";
            if (this.Telefon_no.Text.Count<char>() == 14)
            {
                this.TelSorgula(NoDuzenle(this.Telefon_no.Text.Trim().ToUpper()));
                this.VerileriListeyeYukle(NoDuzenle(this.Telefon_no.Text.Trim().ToUpper()));
            }           
        }
       
        private void button3_Click(object sender, EventArgs e)
        {
            musteri_soyadı.Text = "";
            musteri_adı.Text = "";
            cinsiyet.Text = "";
            if (this.Cihaz_tel.Text.Count<char>() == 14)
            {
                this.TelSorgula(NoDuzenle(this.Cihaz_tel.Text.Trim().ToUpper()));
                this.VerileriListeyeYukle(NoDuzenle(this.Cihaz_tel.Text.Trim().ToUpper()));
            }
        }
      
        private void frmana_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (MessageBox.Show("Programdan çıkmak üzeresiniz,çıkmak istediğinize emin misiniz ?", "Uyarı", MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes)
            //{
                try
                {
                    SqlCommand command = new SqlCommand("delete  from OnlineAcenteler where ID='" + frmYazilimGüvenlik.OnlineID + "'", baglan);
                    command.ExecuteNonQuery();
                }
                catch { }
                finally { Application.Exit(); }
            //}
            //else
            //    e.Cancel = true;
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if ((Cihaz_tel.Text != "") && (Cihaz_tel.Text.Count() == 14))
            {
                StreamWriter yaz;
 
                if (son_kayıt.Trim() != NoDuzenle(Cihaz_tel.Text.Trim()))
                {              
                    yaz = File.AppendText(Application.StartupPath+@"\Setting\Ga.jmt");
                    yaz.WriteLine(NoDuzenle(Cihaz_tel.Text.Trim()) + "," + DateTime.Now.ToShortDateString() + "," + DateTime.Now.ToShortTimeString());
                    yaz.Close();
                    son_kayıt = NoDuzenle(Cihaz_tel.Text.Trim());
                }
            }
        }

        private void frmana_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }
         
        private void müşteriyiSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                if (Koltuklar[i].Name == butondan_sender)
                {
                    if ((Koltuklar[i].BackColor != Color.Snow) & (Koltuklar[i].BackColor != Color.Gray))
                    {
                        if (MessageBox.Show("Yolcuya ait işlemi silmek istediğinize emin misiniz ?", "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                        {
                            this.YolculariVeriTabanindanSilme();
                            yenile = "ok";
                            Koltuklar[i].BackColor = Color.LavenderBlush;
                        }
                    }
                }
            }
        }
    
        private void bilgileriGösterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                if (Koltuklar[i].Name == butondan_sender)
                {
                    Button button = sender as Button;
                    this.YolcuKoltukBilgisi(Koltuklar[i].Name);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                if (Koltuklar[i].Name == butondan_sender)
                {
                    if ((Koltuklar[i].BackColor == Color.Red))
                    {                   
                        foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
                        {
                            if (BiletIslemler.KoltukNo == Koltuklar[i].Name)
                            {
                                if (MessageBox.Show("Müşteriyi Servisden Silmek İstiyormusunuz ?", "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                                {
                                    SqlCommand update = new SqlCommand("update  TumIslemler set SID='0' where ID='" + BiletIslemler.ID+ "'", baglan);
                                    update.ExecuteNonQuery();
                                }
                            }
                        }                   
                    }
                    else
                        MessageBox.Show("Sadece Rezervasyon Yaptıran Yolcular Servisten Silinebilinir !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void DemoKontrolTick(object sender, EventArgs e)
        {
           
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            toolStripComboBox1.ComboBox.Items.Clear();

            SqlDataReader reader = new SqlCommand("select * from Otobusler where tur='Servis'", this.baglan).ExecuteReader();
            while (reader.Read())
            {
                toolStripComboBox1.ComboBox.Items.Add(reader[0]);
            }
            reader.Close();
        }

        private void TmrYazilimGüncellemeKontrol_Tick(object sender, EventArgs e)
        {
            if (!UpdateKontrolTrueFalse)
            {
                try { JMOtobusUpdateKontrol.RunWorkerAsync(); }
                catch { return; }

            }     
        }
      
        private void yolcuTransferOnaylaSeçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                if (Koltuklar[i].Name == butondan_sender)
                {
                    if ((Koltuklar[i].BackColor != Color.Snow) & (Koltuklar[i].BackColor != Color.Gray))
                    {
                        if (Transfer_edilen == 0)
                        {
                            Transfer_edilen = Convert.ToInt16(Koltuklar[i].Name);
                            Transfer_edilen_renk = Koltuklar[i].BackColor;
                            Koltuklar[i].BackColor = Color.DarkOrange;
                        }

                        else if (Transfer_edilen.ToString() == Koltuklar[i].Name)
                        {
                        MessageBox.Show("Burası aynı Koltuk.Lütfen Başka bir koltuğa transfer yapınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                        }
                        else   
                        {
                            Transfer_eden = Convert.ToInt16(Koltuklar[i].Name);
                            Transfer_eden_renk = Koltuklar[i].BackColor;
                            if (DialogResult.No == MessageBox.Show("'" + Transfer_edilen + "' Numarada oturan yolcuyu, '" + Transfer_eden + "' Numaralı yolcu ile yer değişikliğini onaylıyormusunuz ?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                            { return; }
           
                            foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
                            {
                                if (BiletIslemler.KoltukNo == Transfer_eden.ToString())
                                {
                                    ID = BiletIslemler.ID.ToString();

                                    SqlCommand command = new SqlCommand("update  TumIslemler set koltuk_no=@koltuk  where ID=" + ID, this.baglan);
                                    command.Parameters.AddWithValue("@koltuk", Transfer_edilen);
                                    command.ExecuteNonQuery();

                                    Koltuklar[i].BackColor = Transfer_edilen_renk;
                                }
                            }
                            
                            foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
                            {
                                if (BiletIslemler.KoltukNo == Transfer_edilen.ToString())
                                {
                                    ID = BiletIslemler.ID.ToString();

                                    SqlCommand command = new SqlCommand("update  TumIslemler set koltuk_no=@koltuk  where ID=" + ID, this.baglan);
                                    command.Parameters.AddWithValue("@koltuk", Transfer_eden);
                                    command.ExecuteNonQuery();
                                    Koltuklar[i].BackColor = Transfer_eden_renk;
                                }
                            }

                            SqlCommand command04 = new SqlCommand("update  ServerClientBaglanti set durum='True'  where durum='False'", this.baglan);
                            command04.ExecuteNonQuery();
                       
                            MessageBox.Show("'" + Transfer_edilen + "' Numarada oturan yolcu, '" + Transfer_eden + "' Numaralı yolcu ile yer değişikliği başarıyla gerçekleştirildi.", "Transfer Başarılı", MessageBoxButtons.OK);                           
                             
                            Transfer_edilen = 0;                            
                            yenile = "ok";         
                        }
                    }
                    else
                    {
                        MessageBox.Show("Bu koltuk boş olduğu için transfer işlemi yapamazsınız.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
     
        private void transferİptalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                if (Koltuklar[i].Name == butondan_sender)
                {
                    if ((Koltuklar[i].BackColor != Color.Snow) & (Koltuklar[i].BackColor != Color.Gray))
                    {

                        if (Koltuklar[i].BackColor == Color.DarkOrange)
                        {
                            Transfer_edilen = Convert.ToInt16(0);
                            Koltuklar[i].BackColor = Transfer_edilen_renk;
                        }
                    }
                }
            }
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            Cihaz_tel.Text = Telefon_no.Text;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
             for (int i = 0; i < 36; i++)
            {
                if (Koltuklar[i].Name == butondan_sender)
                {
                    if ((Koltuklar[i].BackColor == Color.Red))
                    {
                        foreach (TumIslemler BiletIslemler in TumIslemlerPaket)
                        {
                            if (BiletIslemler.KoltukNo == Koltuklar[i].Name)
                            {

                                if (MessageBox.Show("Rezervazyon yaptırmış yolcuyu servise aktarmak istiyormusunuz ?", "Dikkat", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                                {
                                    SqlCommand update = new SqlCommand("update  TumIslemler set SID='1', servis_arac='" + toolStripComboBox1.ComboBox.SelectedItem + "' where ID='" + BiletIslemler.ID+ "'", baglan);
                                    update.ExecuteNonQuery();

                                    contextMenuYolcuSecenekleri.Close();
                                    MessageBox.Show("Yolcu Servise Başarıyla Aktarıldı", "İşlem Başarılı");
                                }
                            }
                        }     
                    }
                    else
                        MessageBox.Show("Sadece Rezervasyon Yaptıran Yolcular Servise Aktarılabilinir !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        
        }
        
        private void button5_Click_3(object sender, EventArgs e)
        {
            new JMOtobusYazilimi.frmMesajGelmeKontrol().Show();
        }

        private void timer1_Tick_2(object sender, EventArgs e)
        {
            try
            {
                SqlDataReader reader2 = new SqlCommand("select S_ID  from OnlineMesajlasma where (gonderen='" + frmYazilimGüvenlik.OnlineID.Trim() + "' or alici='" + frmYazilimGüvenlik.OnlineID.Trim() + "') and m_gosterilme='0'", baglan).ExecuteReader();

                while (reader2.Read())
                {
                    GelenMesajID = reader2[0].ToString();
                }
                reader2.Close();

                SqlCommand update = new SqlCommand("update  OnlineMesajlasma set m_gosterilme='1' where S_ID='" + GelenMesajID + "'", baglan);
                update.ExecuteNonQuery();

                if (GelenMesajID != "")
                {
                    new frmMesajGelmeKontrol().Show();
                }
            }
            catch { }
        }

        private void Server_Client_kontrol2_Tick(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("update  ServerClientBaglanti set durum='False'  where durum='True'", this.baglan);
            command.ExecuteNonQuery();
            poliengel = "bitti";     
            ServerClientUpdate2.Enabled=false;
        }

        private void button5_Click_4(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("delete  from ServerClientBaglanti", baglan);       
            command.ExecuteNonQuery();
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            new frmAracEkleme().ShowDialog();
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            new frmServis().ShowDialog();
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {
            yolcu_sayısını_yolla = TumIslemlerPaket.Count().ToString();
            new frmYolcuHaritasi().ShowDialog();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            new frmGelenAramalar().ShowDialog();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            if (_frmOnlineAcenteler == null || _frmOnlineAcenteler.IsDisposed)
            {
                _frmOnlineAcenteler = new frmOnlineAcenteler();
                _frmOnlineAcenteler.Show();
            }
            else { _frmOnlineAcenteler.Activate(); }
          
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            new frmGenelAyarlar().ShowDialog();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            if (_FrmGuncellemePaneli == null || _FrmGuncellemePaneli.IsDisposed)
            {
                _FrmGuncellemePaneli = new frmGuncellemeKontrolu();
                _FrmGuncellemePaneli.Show();
            }
            else { _FrmGuncellemePaneli.Activate();}
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            frmIletisim gec = new frmIletisim();
            gec.ShowDialog();
        }

        private void arac_plaka_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.label42.Text = "Bir Saat Belirleyiniz";

            VeriYuklemeKalip();
            OtobusSeferleri();
        }

        private void Tarih_Tick(object sender, EventArgs e)
        {
            SaatTarih();
        }

        private void musteri_adı_Click(object sender, EventArgs e)
        {
            musteri_adı.Text = "";
        }

        private void musteri_soyadı_Click(object sender, EventArgs e)
        {
            musteri_soyadı.Text = "";
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            new frmTopluSmsGonderme().ShowDialog();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Toplu mail özelliğini kullanabilmek için program sahibinden yeni kurulum dosyalarını isteyiniz.", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);           
            new frmTopluEpostaGonderme().ShowDialog();
        }

        private void FormGecisUpdate_Tick(object sender, EventArgs e)
        {
            FormGecisUpdatePaket();
        }

        private void LabelEffect_Tick(object sender, EventArgs e)
        {   
            if (effect == 1)
            {
                this.label20.BackColor = Color.OrangeRed;
                this.label20.ForeColor = Color.White;        
            }
         
            if (effect == 2)
            {
                this.label20.BackColor = Color.OrangeRed;
                this.label20.ForeColor = Color.White;  
               
            }  
           
            if (effect == 3)
            {
                this.label20.BackColor = Color.DeepSkyBlue;
                this.label20.ForeColor = Color.Crimson; 
                this.effect = 1;
            }
            this.effect++;
        }

        private void ServerClientUpdate_Tick(object sender, EventArgs e)
        {
            ServerClientUpdatePaket(); 
        }

        private void UrunKontrol_Tick(object sender, EventArgs e)
        {
          
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            WordeVerileriAktar();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExcelVerileriAktar();
        }

        #endregion

        private void SmsGonderme_DoWork(object sender, DoWorkEventArgs e)
        {
            string EkTaki = "";
            string SmsVarsayilanFirma = "";
            string SecilenKoltuklarToplam = "";
            int SecilenTotal = 0;

            for (int i = 0; i < SecilenKoltuklar.Count(); i++)
            {
                if (SecilenKoltuklar[i] != null)
                {
                    SecilenKoltuklarToplam += SecilenKoltuklar[i] + ",";
                    SecilenTotal++;
                }
            }
            SecilenKoltuklarToplam = SecilenKoltuklarToplam.Substring(0, SecilenKoltuklarToplam.Length - 1);

            if (SecilenTotal > 1) EkTaki = "koltuklara"; else EkTaki = "koltuğa";

            gln = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ssa.jmt");
            SmsVarsayilanFirma = Decrypt(gln[0]).Split('-')[3];
            
            if (bilet_islem.Text.Trim() == "Rezerve Bilet")
                SmsGonder.SMSGONDER(SmsVarsayilanFirma, MusteriTelefonNo, "Değerli yolcumuz, " + rezer_tarih.Text + " , saat " + arac_saat.Text + "'daki otobüs seferi için " + SecilenKoltuklarToplam + " numaralı "+EkTaki+" rezervasyon yapılmıştır.İyi yolculuklar dileriz.", "");

            if (bilet_islem.Text.Trim() == "Ücret Alindi")
                SmsGonder.SMSGONDER(SmsVarsayilanFirma, MusteriTelefonNo, "Değerli yolcumuz, " + rezer_tarih.Text + " , saat " + arac_saat.Text + "'daki otobüs seferi için " + SecilenKoltuklarToplam + " numaralı " + EkTaki + " biletiniz kesilmiştir.İyi yolculuklar dileriz.", "");

            if (bilet_islem.Text.Trim() == "Ücretsiz")
                SmsGonder.SMSGONDER(SmsVarsayilanFirma, MusteriTelefonNo, "Değerli yolcumuz, " + rezer_tarih.Text + " , saat " + arac_saat.Text + "'daki otobüs seferi için " + SecilenKoltuklarToplam + " numaralı " + EkTaki + " biletiniz kesilmiştir.İyi yolculuklar dileriz.", "");
        }

        private void FlowTurkUpdateKontrol_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                onlinebaglan.Open();

                SqlDataReader reader = new SqlCommand("select surum from Program  where adi='JM Otobüs Yazilimi'", onlinebaglan).ExecuteReader();
                while (reader.Read())
                {
                    YeniVersiyon = reader[0].ToString();
                }
                reader.Close();

                onlinebaglan.Close();

                if (YeniVersiyon.Trim() != "")
                {
                    if (YeniVersiyon.Trim() != Application.ProductVersion)
                    {
                        UpdateKontrolTrueFalse = true;
                        BtnUpdate.Visible = true;
                        YazilimUpdateKontrol.Stop();
                        new frmGuncellemeKontrolu().ShowDialog();
                    }
                }
            }
            catch { return; }
        }

        private void arac_saat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

}
    
    
    
    
    
    
    
    
