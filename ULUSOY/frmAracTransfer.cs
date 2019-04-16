using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ULUSOY
{
    public partial class frmYolcuHaritasi : Form
    {
        public frmYolcuHaritasi()
        {
            InitializeComponent();
        }
     
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        private int[] yolcu_ucret_int;
        private string[] yolcu_ucret;
        bool uyumluluk;

        #region OZEL FONKSIYONLAR

        private void otobus_seferleri()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add("07:00");
            comboBox1.Items.Add("07:30");
            comboBox1.Items.Add("08:00");
            comboBox1.Items.Add("08:30");
            comboBox1.Items.Add("09:00");
            comboBox1.Items.Add("09:30");
         
            for (int a = 10; a <= 21; a++)
            {
                comboBox1.Items.Add(a + ":00");
                comboBox1.Items.Add(a + ":30");
            }
            
        }
     
        private void otobus_seferleri2()
        {
            comboBox4.Items.Clear();         
            comboBox4.Items.Add("07:00");
            comboBox4.Items.Add("07:30");
            comboBox4.Items.Add("08:00");
            comboBox4.Items.Add("08:30");
            comboBox4.Items.Add("09:00");
            comboBox4.Items.Add("09:30");
       
            for (int a = 10; a <= 21; a++)
            {
                comboBox4.Items.Add(a + ":00");
                comboBox4.Items.Add(a + ":30");
            }
        }
     
        private void yolcu_sayisi()
        {
            SqlCommand sy = new SqlCommand("select count(*) from TumIslemler where tarih='" + dateTimePicker1.Text + "' and saat='" + comboBox1.Text + "' and plaka='" + comboBox2.Text + "'", baglan);

            this.label18.Text = Convert.ToInt16(Convert.ToInt16(sy.ExecuteScalar())).ToString();
           
            if (this.comboBox2.Text.Trim() != "")
            {
                SqlCommand command = new SqlCommand("select * from Otobusler where plaka=@comboplaka and tur='Ana Hat'", this.baglan);
                command.Parameters.AddWithValue("@comboplaka", this.comboBox2.Text.Trim());
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    this.label34.Text = reader[1].ToString();
                }
                reader.Close();
                this.label21.Text = (Convert.ToInt16(this.label34.Text) - Convert.ToInt16(this.label18.Text)).ToString();
            }
           
            uyumluluk_kontrolu();
        }
      
        private void yolcu_sayisi2()
        {

            SqlCommand sy = new SqlCommand("select count(*) from TumIslemler where tarih='" + dateTimePicker2.Text + "' and saat='" + comboBox4.Text + "' and plaka='" + comboBox3.Text + "'", baglan);

            this.label42.Text = Convert.ToInt16(Convert.ToInt16(sy.ExecuteScalar())).ToString();
           
            if (this.comboBox3.Text.Trim() != "")
            {
                SqlCommand command = new SqlCommand("select * from Otobusler where plaka=@comboplaka and tur='Ana Hat'", this.baglan);
                command.Parameters.AddWithValue("@comboplaka", this.comboBox3.Text.Trim());
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    this.label7.Text = reader[1].ToString();
                }
                reader.Close();
                this.label41.Text = (Convert.ToInt16(this.label7.Text) - Convert.ToInt16(this.label42.Text)).ToString();
            }
         
            uyumluluk_kontrolu();
        }
      
        private void plaka_donen()
        {
            SqlDataReader reader = new SqlCommand("select * from Otobusler where tur='Ana Hat'", this.baglan).ExecuteReader();
            while (reader.Read())
            {
                this.comboBox2.Items.Add(reader[0].ToString());
                this.comboBox3.Items.Add(reader[0].ToString());
          
            }
            reader.Close();
        }
       
        public void uyumluluk_kontrolu()
        {
            pictureBox1.BackgroundImage = new Bitmap(Application.StartupPath + @"\Pictures\Belirsiz.png");
           
            if ((comboBox1.Text != "") & (comboBox2.Text != "") & (comboBox3.Text != "") & (comboBox4.Text != "")&(Convert.ToInt16 (label34.Text)<=Convert.ToInt16(label7.Text)) )
            {

                if ((Convert.ToInt16(label18.Text) <= Convert.ToInt16(label41.Text))&&(label42.Text=="0"))
                {
                    pictureBox1.BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\Okey.png");
                    uyumluluk = true;
                }
                else
                {
                    pictureBox1.BackgroundImage = new Bitmap(Application.StartupPath + @"\\Pictures\Close 2.png");
                    MessageBox.Show("Dolu Koltuk Sayısı Seçilmiş Olan Aracın Kapasitesinin Üstünde Olduğu yada Taşımak istediğiniz Araç Üzerinde Yolcu Olduğu İçin Çakışmaya Sebep Olacağından Boş Bir Araç Seçiniz.","Uyuşmazlık ! ! !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    uyumluluk = false;
                
                }
            }
        }

        #endregion

        #region TUMLESIK FONKSIYONLAR

        public void otobusler_arası_tasımayı_gerceklestir()
        {
            int a=0; 
            string[] strArray =new string [44];
           
            SqlCommand cek_ID = new SqlCommand("select islem.ID from TumIslemler islem where tarih=@tarih and plaka=@comboplaka and saat=@combosaat ", this.baglan);
            cek_ID.Parameters.AddWithValue("@tarih", this.dateTimePicker1.Text);
            cek_ID.Parameters.AddWithValue("@comboplaka", this.comboBox2.Text);
            cek_ID.Parameters.AddWithValue("@combosaat", comboBox1.Text);
            SqlDataReader reader = cek_ID.ExecuteReader();
         
            while (reader.Read())
            {
                strArray[a] = reader[0].ToString();  
                a++;             
            }
            reader.Close();        
             
            for(int i=0;i<strArray.Count();i++)
            {      
                if (strArray[i] != null)
                {
                    SqlCommand command = new SqlCommand("update  TumIslemler set tarih=@tarih2 , plaka=@comboplaka2 , saat=@combosaat2 where ID=@islemID2", this.baglan);
                    command.Parameters.AddWithValue("@tarih2", this.dateTimePicker2.Text);
                    command.Parameters.AddWithValue("@comboplaka2", this.comboBox3.Text);
                    command.Parameters.AddWithValue("@combosaat2", comboBox4.Text);
                    command.Parameters.AddWithValue("@islemID2", strArray[i]);
                    command.ExecuteNonQuery();
                } 
            }
        
            MessageBox.Show("İşlem Başarıyla Gerçekleştirilmiştir");
        }
      
        private void yolcularitasi_Load(object sender, EventArgs e)
        {
            this.yolcu_ucret = new string[0x98967f];
            this.yolcu_ucret_int = new int[0x98967f];
            this.baglan = new SqlConnection(ClsDatabase.data_base());
            this.baglan.Open();
            this.plaka_donen();
        }
      
        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            this.otobus_seferleri2();
            this.yolcu_sayisi2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (uyumluluk == true)
            {
                otobusler_arası_tasımayı_gerceklestir();
                uyumluluk = false;
                Close();
            }
        }  

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            otobus_seferleri2();
            this.yolcu_sayisi2();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            otobus_seferleri();
            this.yolcu_sayisi();
        }

        private void yolcularitasi_FormClosing(object sender, FormClosingEventArgs e)
        {
            frmYolcuKoltuguBelirle.yenile = "ok";
        }
      
        private void comboBox3_SelectedValueChanged_1(object sender, EventArgs e)
        {
            this.otobus_seferleri2();
            this.yolcu_sayisi2();
        }

        private void comboBox4_SelectedValueChanged_1(object sender, EventArgs e)
        {
            this.yolcu_sayisi2();
        }

        private void comboBox2_SelectedValueChanged_1(object sender, EventArgs e)
        {
            this.otobus_seferleri();
            this.yolcu_sayisi();
        }

        private void comboBox1_SelectedValueChanged_1(object sender, EventArgs e)
        {
            this.yolcu_sayisi();
        }
    
        #endregion

    }
}
