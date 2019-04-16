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

namespace ULUSOY
{
    public partial class frmSunucuAyarlari : Form
    {
        public frmSunucuAyarlari()
        {
            InitializeComponent();
        }

        StreamWriter server;
        string baglanti_dene;
 
        string baglanti_kontrol()
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
                    baglanti.Open();
                    return "true";
                }
            }
            catch { return "false"; }
            finally { baglanti.Close(); }
        }
   
        private void button16_Click(object sender, EventArgs e)
        {
            if (baglanti_kontrol() == "true")

                MessageBox.Show("Bağlantınız başarıyla gerçekleştirildi.Bu bilgileri sistem üzerinde kullanabilir,işlem yapabilirsiniz.Lütfen Kaydetmeyi unutmayınız.", "Sunucu Bağlantısı Onaylandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Bağlantı hatası alındı.Lütfen bilgilerinizi tekrar kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      
        private void button13_Click(object sender, EventArgs e)
        {
              if (baglanti_kontrol() == "true")
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
            else
              { MessageBox.Show("Bağlantı hatası alındı.Lütfen bilgilerinizi tekrar kontrol ediniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    
        private void rdblocal_CheckedChanged(object sender, EventArgs e)
        {
            txtdatabase.Enabled = false;
            txtkullaniciadi.Enabled = false;
            txtserver.Enabled = false;
            txtsifre.Enabled = false;
        }

        private void rdbsunucu_CheckedChanged(object sender, EventArgs e)
        {
            string[] dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\DataBase\\Server.jmt");

            txtdatabase.Text = dizi[2];
            txtkullaniciadi.Text = dizi[3];
            txtserver.Text = dizi[1];
            txtsifre.Text = dizi[4];
            txtdatabase.Enabled = true;
            txtkullaniciadi.Enabled = true;
            txtserver.Enabled = true;
            txtsifre.Enabled = true;
        }

        private void frmSunucuAyarlari_Load(object sender, EventArgs e)
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

    }
}