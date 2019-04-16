using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace ULUSOY
{
    public partial class frmAracEkleme : Form
    {
        public frmAracEkleme()
        {
            InitializeComponent();
        }
       
        private void arac_cek()
        {
            SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
            SqlCommand command = new SqlCommand("select * from Otobusler order by tur", connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());
                item.SubItems.Add(reader[1].ToString());
                item.SubItems.Add(reader[2].ToString());
                item.SubItems.Add(reader[3].ToString());
              
                this.listView1.Items.Add(item);
            }
            reader.Close();
            connection.Close();
        }

        public void araclari_veritabanindan_silme()
        {
            SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
            SqlCommand command = new SqlCommand("delete  from Otobusler order by tur", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
       
        public void aracları_veritabanindan_silme(string ID)
        {
            SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
            SqlCommand command = new SqlCommand("delete  from Otobusler where plaka='" + ID + "'", connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        private void aracekleme_Load(object sender, EventArgs e)
        {
              this.arac_cek();           
        }

        private void button45_Click(object sender, EventArgs e)
        {
            if (this.listView1.CheckedItems.Count > 0)
            {
                if (MessageBox.Show("Seçili Otobusleri silmek istediğinize emin misiniz ?", "Dikkat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    foreach (ListViewItem item in this.listView1.CheckedItems)
                    {
                        this.aracları_veritabanindan_silme(item.Text);
                        item.Remove();
                    }
                    MessageBox.Show("Se\x00e7ili bilgi silinmiştir");
                }
            }
            else
            {
                MessageBox.Show("L\x00fctfen bir işlem satırı se\x00e7iniz", "Uyarı ! ! !", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
   
        public static string pkala_dznle(string plaka)
        {
            string[] strArray = plaka.Split('-');
            plaka = "";
            foreach (string str in strArray)
            {
                plaka += str.Trim()+ " ";
            }
            return plaka.Trim();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                string[] plaka; string tut = "";
                ListViewItem item = listView1.SelectedItems[0];
                plaka = item.SubItems[0].Text.Trim().Split(' ');
                int a = 0;
                for (int i = 0; i < plaka.Count(); i++)
                {
                    tut += plaka[i];
                    if ((plaka[1].Length == 1) & (a == 0))
                    {
                        tut += " ";
                        a = 1;
                    }
                }
              
                maskedTextBox2.Text = tut;
                comboBox3.Text = item.SubItems[1].Text;
                textBox4.Text = item.SubItems[2].Text;
                comboBox1.Text = item.SubItems[3].Text;
         
            }    
        }

        private void aracekleme_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("Yapmış Olduğunuz Degişiklikler Program Tekrar Açıldığında Etkin Olacaktır", "Dikkat", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 58 & e.KeyChar >= 48)
                e.Handled = true;
            
            string last;
            int Upper_Character_Checking = 0;

            if (e.KeyChar == (char)32 || textBox4.Text == "") { Upper_Character_Checking = 1; } //karakter Space veya textbox bo$ ise

            if (textBox4.Text != "")
            {
                last = textBox4.Text.Substring(textBox4.Text.Length - 1);
                if (last == ((char)32).ToString() && e.KeyChar != (char)8) Upper_Character_Checking = 1;
            }

            if (e.KeyChar > (char)95 && e.KeyChar < (char)123 && Upper_Character_Checking == 1)
            {
                Upper_Character_Checking = 0; //orjinal degerin diger harfler icin geri yuklenmesi
                e.KeyChar = (char)((int)(e.KeyChar - 32)); //Buyuk harfe cevirmek icin kullanilir.
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tüm Otobusleri  silmek istediğinize emin misiniz ?", "Dikkat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                this.araclari_veritabanindan_silme();
                this.listView1.Items.Clear();
                MessageBox.Show("T\x00fcm işlem bilgileri silinmiştir");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (this.maskedTextBox2.Text.Count() < 9)
            {
                MessageBox.Show("L\x00fctfen Plakayı Boş Ge\x00e7meyiniz");
            }
            else if (this.comboBox3.Text.Trim() == "")
            {
                MessageBox.Show("L\x00fctfen Koltuk Sayısını Boş Ge\x00e7meyiniz");
            }
            else if (this.comboBox1.Text.Trim() == "")
            {
                MessageBox.Show("L\x00fctfen Araç Türünü Boş Ge\x00e7meyiniz");
            }
            else if (textBox4.Text.Trim() == "")
            {
                MessageBox.Show("L\x00fctfen Şoför Adını Boş Ge\x00e7meyiniz");
            }
            else
            {
                SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
                SqlCommand command = new SqlCommand("select count(*) from Otobusler where  plaka='" + pkala_dznle(this.maskedTextBox2.Text.Trim().ToUpper()) + "'", connection);
                connection.Open();
                int num = Convert.ToInt16(command.ExecuteScalar());
                connection.Close();
                if (num == 0)
                {
                    try
                    {
                        ClsInsert.otobus_ekle(pkala_dznle(this.maskedTextBox2.Text.Trim().ToUpper()), Convert.ToInt16(this.comboBox3.Text), this.textBox4.Text, comboBox1.Text.Trim());
                        MessageBox.Show("Kayıt İşleminiz Başarıyla Ger\x00e7ekleştirilmiştir");
                        ListViewItem item = new ListViewItem(pkala_dznle(this.maskedTextBox2.Text.Trim().ToUpper()));
                        item.SubItems.Add(this.comboBox3.Text);
                        item.SubItems.Add(this.textBox4.Text.Trim());
                        item.SubItems.Add(this.comboBox1.Text);

                        this.listView1.Items.Add(item);
                        this.maskedTextBox2.Text = "";
                        this.comboBox3.Text = "";
                        this.textBox4.Text = "";
                    }
                    catch
                    {
                        MessageBox.Show("Kayıt İşleminiz Ger\x00e7ekleştirilememiştir");
                    }
                }
                else
                {
                    connection.Open();
                    SqlCommand update = new SqlCommand("update Otobusler set koltuk_sayisi=@s ,sofor_adi=@sf,tur=@tur where  plaka='" + pkala_dznle(this.maskedTextBox2.Text.Trim()) + "'", connection);
                    update.Parameters.AddWithValue("@sf", textBox4.Text);
                    update.Parameters.AddWithValue("@s", comboBox3.Text);
                    update.Parameters.AddWithValue("@tur", comboBox1.Text);
                    update.ExecuteNonQuery();
                    connection.Close();

                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (listView1.Items[i].Text == pkala_dznle(this.maskedTextBox2.Text.Trim().ToUpper()))
                        {
                            listView1.Items[i].Remove();
                        }
                    }

                    ListViewItem item = new ListViewItem(pkala_dznle(this.maskedTextBox2.Text.Trim().ToUpper()));
                    item.SubItems.Add(this.comboBox3.Text);
                    item.SubItems.Add(this.textBox4.Text);
                    item.SubItems.Add(this.comboBox1.Text);
                    listView1.Items.Add(item);

                    MessageBox.Show("Güncelleme İşleminiz Ger\x00e7ekleştirilmiştir");
                }
            }
        }
    
    }
}
