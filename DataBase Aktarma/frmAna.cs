using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataBase_Aktarma
{
    public partial class frmAna : Form
    {
        public frmAna()
        {
            InitializeComponent();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (textBox7.Text != "")
            {
                DialogResult sr = MessageBox.Show("Veri Tabanını aktarmak istediğinize emin misiniz,aksi takdirde şimdiki verileriniz silinecektir.", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (DialogResult.OK == sr)
                {
                    string kopyalanacakDosya = textBox7.Text; ;
                    string dosyanınKopyanacagiKlasor = "C:\\Program Files\\Microsoft SQL Server\\MSSQL.1\\MSSQL\\Backup";
                    string kopyalanacakDosyaIsmi = "VeriTabani.bak";

                    if (File.Exists(dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi) == true)
                    {
                        File.Delete(dosyanınKopyanacagiKlasor + kopyalanacakDosyaIsmi);                                        
                    }

                    if (File.Exists(dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi) == false)
                    {
                        File.Copy(kopyalanacakDosya, dosyanınKopyanacagiKlasor + "\\" + kopyalanacakDosyaIsmi);
                    }

                    SqlConnection myConnection = new SqlConnection("Data Source=" + System.Environment.MachineName + "\\SqlExpress; Initial Catalog='master'; Integrated security=true");
                    myConnection.Open();
                 
                    try
                    {
                        string cmdText2 = "DROP DATABASE [Data]";
                        SqlCommand komut2 = new SqlCommand(cmdText2, myConnection);
                        komut2.ExecuteNonQuery();
                    }
                    catch { }
                    finally
                    {
                        string cmdText = "restore database Data from disk = 'VeriTabani.bak' with file=2";
                        SqlCommand komut = new SqlCommand(cmdText, myConnection);

                        komut.ExecuteNonQuery();
                        myConnection.Close();

                        MessageBox.Show("İşleminiz başarıyla gerçekleşti.Sistem yeniden başlatılıyor...", "İşlem Başarılı");
                        System.Diagnostics.Process.Start(Application.StartupPath + "\\JM Otobüs Yazılımı.exe");
                        Application.Exit();
                    }
                }
            }
            else
                MessageBox.Show("Lütfen Dosyayı aktarmak için dosyayı belirleyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Title = "Veri Tabanı dosyasını Seçtiniz";
            file.Filter = "Veri Tabanı Dosyası |*.bak";
            file.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            file.RestoreDirectory = true;
            file.CheckFileExists = false;

            if (file.ShowDialog() == DialogResult.OK)
            {
                textBox7.Text = file.FileName;
            }  
        }

    }
}
