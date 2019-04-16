using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using ULUSOY;

namespace Yazılım_Güncelleme
{
    public partial class frmAna : Form
    {
        public frmAna()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        SqlConnection OnlineConnection = new SqlConnection(ClsDatabase.OnlineConnection);
        string ftpdown = "ftp://ftp.jmteknoloji.com/";
        string kullanici = "jm_ftp123";
        string sifre = "Ye4NAM1f";
        long size = 0;
        
        void DownLoadProtocol()
        {
            try
            {
                int totalReadBytesCount = 0;

                string[] DosyaListesi;
                StringBuilder result = new StringBuilder();
                FtpWebRequest FTP;
                FTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpdown));
                // Dosya tranferinin Binary türden yapılacağını belirtiyoruz
                FTP.UseBinary = true;
                // Ftp bağlantısı için UserName ve Şifremizi belirtiyoruz
                FTP.Credentials = new NetworkCredential(kullanici, sifre);
                // Bu kısımda hangi işlemi yapacağımızı belirtiyoruz
                FTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FTP.Method = WebRequestMethods.Ftp.ListDirectory;

                // Dosya listesini alıyoruz
                WebResponse response = FTP.GetResponse();
                // Aldığımız listeyi StreamReader ile her satırını okuyup dosya isimlerini ayırıyoruz
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();

                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }

                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                DosyaListesi = result.ToString().Split('\n');

                FtpWebResponse response5 = null;
                for (int x = 0; x < DosyaListesi.Count(); x++)
                {
                    FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpdown + DosyaListesi[x].ToString()));
                    request.Proxy = null;
                    request.Credentials = new NetworkCredential(kullanici, sifre);
                    request.Method = WebRequestMethods.Ftp.GetFileSize;

                    response5 = (FtpWebResponse)request.GetResponse();
                    size += response5.ContentLength;

                }

                response5.Close();
                progressBar1.Maximum = Convert.ToInt32(size);

                label2.Text = "Dosyalar İndiriliyor...";
                for (int x = 0; x < DosyaListesi.Count(); x++)
                {
                    int kntrl = 0;

                    for (int i = 0; i < DosyaListesi[x].Length; i++)
                    {
                        if (DosyaListesi[x][i].ToString() == ".")
                        {
                            kntrl = 1;
                        }
                    }

                    if (kntrl == 1)
                    {
                        FileStream SR = new FileStream(Application.StartupPath + "\\" + DosyaListesi[x].ToString(), FileMode.Create);
                        FtpWebRequest FTPi0;
                        FTPi0 = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpdown + DosyaListesi[x].ToString()));
                        FTPi0.Credentials = new NetworkCredential(kullanici, sifre);
                        FTPi0.Method = WebRequestMethods.Ftp.DownloadFile;
                        FTPi0.UseBinary = true;
                        FtpWebResponse response2 = (FtpWebResponse)FTPi0.GetResponse();
                        Stream ftpStream = response2.GetResponseStream();
                        long cl = response2.ContentLength;
                        int bufferSize = 1024;
                        int readCount;
                        byte[] buffer = new byte[bufferSize];
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                        progressBar1.Value += readCount;
                        label1.Text = "Dosya İndiriliyor...: " + DosyaListesi[x].ToString();

                        while (readCount > 0)
                        {
                            SR.Write(buffer, 0, readCount);
                            readCount = ftpStream.Read(buffer, 0, bufferSize);
                            progressBar1.Value += readCount;
                        }
                        ftpStream.Close();
                        SR.Close();
                        response2.Close();


                        label1.Text = "Dosya İndi...: " + DosyaListesi[x].ToString();

                    }
                    else
                    {
                        FtpWebRequest FTP2;
                        Directory.CreateDirectory(Application.StartupPath + "\\" + DosyaListesi[x]);
                        string[] DosyaListesi2;
                        FTP2 = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpdown + DosyaListesi[x]));
                        FTP2.UseBinary = true;
                        FTP2.Credentials = new NetworkCredential(kullanici, sifre);
                        StringBuilder result2 = new StringBuilder();
                        FTP2.Method = WebRequestMethods.Ftp.ListDirectory;
                        WebResponse response3 = FTP2.GetResponse();
                        StreamReader reader3 = new StreamReader(response3.GetResponseStream());
                        string line3 = reader3.ReadLine();

                        while (line3 != null)
                        {
                            result2.Append(line3);
                            result2.Append("\n");
                            line3 = reader3.ReadLine();
                        }

                        result2.Remove(result2.ToString().LastIndexOf('\n'), 1);
                        reader3.Close();
                        response3.Close();
                        DosyaListesi2 = result2.ToString().Split('\n');

                        for (int y = 0; y < DosyaListesi2.Length; y++)
                        {
                            kntrl = 0;
                            for (int i = 0; i < DosyaListesi2[y].Length; i++)
                            {
                                if (DosyaListesi2[y][i].ToString() == ".")
                                {
                                    kntrl = 1;
                                }
                            }

                            if (kntrl == 1)
                            {
                                try
                                {

                                    FileStream SR = new FileStream(Application.StartupPath + "\\" + DosyaListesi[x].ToString() + "\\" + DosyaListesi2[y].ToString(), FileMode.Create);
                                    FtpWebRequest FTPi0;
                                    FTPi0 = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpdown + DosyaListesi[x].ToString() + @"/" + DosyaListesi2[y].ToString()));
                                    FTPi0.Credentials = new NetworkCredential(kullanici, sifre);
                                    FTPi0.Method = WebRequestMethods.Ftp.DownloadFile;
                                    FTPi0.UseBinary = true;
                                    FTPi0.Method = WebRequestMethods.Ftp.DownloadFile;
                                    FTPi0.UseBinary = true;
                                    FtpWebResponse response2 = (FtpWebResponse)FTPi0.GetResponse();
                                    Stream ftpStream2 = response2.GetResponseStream();
                                    long cl = response2.ContentLength;
                                    int bufferSize = 1024;
                                    int readCount;
                                    byte[] buffer = new byte[bufferSize];
                                    readCount = ftpStream2.Read(buffer, 0, bufferSize);
                                    progressBar1.Value += readCount;
                                    label1.Text = "Dosya İndiriliyor...: " + DosyaListesi[x].ToString() + "\\" + DosyaListesi2[y].ToString();


                                    while (readCount > 0)
                                    {
                                        SR.Write(buffer, 0, readCount);
                                        readCount = ftpStream2.Read(buffer, 0, bufferSize);
                                        progressBar1.Value += readCount;
                                    }

                                    ftpStream2.Close();
                                    SR.Close();
                                    response2.Close();
                                }
                                catch { ;}


                                label1.Text = "Dosya İndi...: " + DosyaListesi[x].ToString() + "\\" + DosyaListesi2[y].ToString();


                                response3.Close();
                                response.Close();

                            }
                        }
                    }
                }

                System.Threading.Thread.Sleep(2000);
            }
            catch { MessageBox.Show("Sunucuya şuan bağlantı yapılamadı.Lütfen daha sonra tekrar deneyiniz", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }

            if (MessageBox.Show("Sisteminiz Güncellendi.Sisteminiz yeniden başlatılacaktır.Şimdi Başlatmak istiyor musunuz ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "\\JM Otobüs Yazılımı.exe");
                Application.Exit();
            }
            else
            { Application.Exit(); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string[] dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\Srm.jmt");

                OnlineConnection.Open();
                string durum = "";
                SqlDataReader reader0 = new SqlCommand("select surum from Program  where adi='JM Otobüs Yazilimi'", this.OnlineConnection).ExecuteReader();
                while (reader0.Read())
                {
                    durum = reader0[0].ToString();
                }

                if (durum == dizi[0].ToString())
                {
                    MessageBox.Show("Güncelleme Bulunmamaktadır.", "Güncelleme Durumu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
                else
                {  backgroundWorker1.RunWorkerAsync(); }
            }
            catch { MessageBox.Show("Sisteme ait olan bir dosya silinmiştir.Hatayı düzelmek için yazılımı tekrar kurunuz.Hala hata alırsanız yazılım sahibi ile iletişime geçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); Application.Exit(); }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DownLoadProtocol();
        }
    }
}
