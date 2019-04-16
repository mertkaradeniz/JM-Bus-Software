using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Collections;
using Microsoft.Office.Interop.Excel;
namespace ULUSOY
{
    public partial class frmServis : Form
    {
        public frmServis()
        {
            InitializeComponent();
        }
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
     
        private void otobus_seferleri()
        {
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

        private void arac()
        {
            this.listView2.Items.Clear();

            SqlCommand command = new SqlCommand("select islem.ID,islem.tarih,servis_bindi,islem.saat,islem.tel,islem.koltuk_no,yolcu.cinsiyet from TumIslemler islem inner join Yolcular yolcu on (islem.ID2=yolcu.ID2) where  SID=1 and tarih='" + dateTimePicker2.Text + "' and saat='" + comboBox1.Text + "' and servis_arac='"+comboBox2.Text+"'  order by islem.koltuk_no", this.baglan);
            string cns = "";
            baglan.Open();
            SqlDataReader reader = command.ExecuteReader();
          
            while (reader.Read())
            {
                if (Convert.ToInt16(reader[6].ToString()) == 0)
                    cns = "Bayan";
                else
                    cns = "Bay";
                
                ListViewItem item = new ListViewItem(reader[0].ToString());
                item.SubItems.Add(reader[1].ToString());
                item.SubItems.Add(reader[2].ToString());
                item.SubItems.Add(reader[3].ToString());
                item.SubItems.Add(reader[4].ToString());
                item.SubItems.Add(reader[5].ToString());
                item.SubItems.Add(cns);
                this.listView2.Items.Add(item);
            }
            reader.Close();
            baglan.Close();
        }

        private void servis_Load(object sender, EventArgs e)
        {
            baglan.Open();
            SqlDataReader reader = new SqlCommand("select * from Otobusler where tur='Servis'", this.baglan).ExecuteReader();
            while (reader.Read())
            {
               comboBox2.Items.Add(reader[0]);
            }
            reader.Close();
            baglan.Close();         
        }
  
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            arac();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            arac();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
            //Kolon başlıkları sabit olduğu için listviewdeki veri aktarılmadan önce sabit verileri excel e giriyoruz.
            object hucre;
            Range rr;
            string str;

            hucre = ws.Cells[1, 1];
            rr = ws.get_Range(hucre, hucre);
            str = "ID";
            rr.Value2 = str;
            rr.Font.Bold = true;
            rr.Font.Size = 12;
            rr.Columns.ColumnWidth = 0;
            
            hucre = ws.Cells[1, 2];
            rr = ws.get_Range(hucre, hucre);
            str = "Rezervasyon T.";
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
            str = "Saat";
            rr.Value2 = str;
            rr.Font.Bold = true;
            rr.Font.Size = 12;
           
            hucre = ws.Cells[1, 5];                  
            rr = ws.get_Range(hucre, hucre);
            str = "Telefon No";
            rr.Value2 = str;
            rr.Font.Bold = true;
            rr.Font.Size = 12;
           
            hucre = ws.Cells[1, 6];
            rr = ws.get_Range(hucre, hucre);
            str = "Koltuk No";
            rr.Value2 = str;
            rr.Font.Bold = true;
            rr.Font.Size = 12;
          
            hucre = ws.Cells[1, 7];
            rr = ws.get_Range(hucre, hucre);
            str = "Cinsiyet";
            rr.Value2 = str;
            rr.Font.Bold = true;
            rr.Font.Size = 12;
            
            int dikey = 2;
            int yatay = 2;
            foreach (ListViewItem lvi in listView2.Items)
            {

              ws.Columns.AutoFit();  
                dikey = 1;
                foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                {
                    ws.Cells[yatay, dikey] = lvs.Text;
                    dikey++;  
                }
                yatay++;
            }  
        }

        public void rezerve_sil()
        {
            baglan.Open();
            SqlCommand update = new SqlCommand("update  TumIslemler set SID='0' where ID='" + listView2.SelectedItems[0].Text + "'", baglan);

            update.ExecuteNonQuery();
            baglan.Close();
        }
      
        private void button2_Click(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count !=0)
            {
                if (MessageBox.Show("Yolcuya ait işlemi silmek istediğinize emin misiniz ?", "Dikkat", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    this.rezerve_sil();
                    MessageBox.Show("Se\x00e7ili bilgi silinmiştir");
                    listView2.SelectedItems[0].Remove();
                }
            }
            else
            MessageBox.Show("Lütfen Bir Müşteri Seçiniz");   
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            otobus_seferleri();
            arac();
        }
  
    }
}