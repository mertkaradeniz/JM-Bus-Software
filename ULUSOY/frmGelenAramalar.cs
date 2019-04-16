using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using Excel=Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace ULUSOY
{
    public partial class frmGelenAramalar : Form
    {
        public frmGelenAramalar()
        {
            InitializeComponent();
        }
   
        private SqlConnection baglan;
        private DateTime kucukTarih;
        private DateTime buyukTarih;

        bool TamOrAralik = true;

        public void tarih_farkı()
        {
            this.buyukTarih = new DateTime(this.dateTimePicker2.Value.Year, this.dateTimePicker2.Value.Month, this.dateTimePicker2.Value.Day);
            this.kucukTarih = new DateTime(this.dateTimePicker1.Value.Year, this.dateTimePicker1.Value.Month, this.dateTimePicker1.Value.Day);
        }

        public void verileri_listele()
        {
            this.baglan = new SqlConnection(ClsDatabase.data_base());
            this.baglan.Open();
           
            string[] reader = System.IO.File.ReadAllLines(Application.StartupPath + "\\Setting\\Ga.jmt");
            string tutucu;                        
            string[] reader_parca=new string[3]; this.tarih_farkı();
          
            this.listView2.Items.Clear();
            this.listView1.Items.Clear();          
           
            for(int i=0;i<reader.Count();i++)
            {
                tutucu = reader[i].ToString();
                reader_parca = tutucu.Split(',');
                string[] strArray = reader_parca[1].ToString().Split(new char[] { '.' });

                DateTime time = new DateTime(Convert.ToInt16(strArray[2]), Convert.ToInt16(strArray[1]), Convert.ToInt16(strArray[0]));

                if ((time >= this.kucukTarih) && (time <= this.buyukTarih))
                {
                    ListViewItem item = new ListViewItem(reader_parca[0]);
                    item.SubItems.Add(reader_parca[1]);
                    item.SubItems.Add(reader_parca[2]);                   
                    listView2.Items.Add(item);
                }
            }

            for (int i = 0; i < reader.Count(); i++)
            {
                tutucu = reader[i].ToString();
                reader_parca = tutucu.Split(',');
                string[] strArray = reader_parca[1].ToString().Split(new char[] { '.' });
               
                ListViewItem item = new ListViewItem(reader_parca[0]);
                item.SubItems.Add(reader_parca[1]);
                item.SubItems.Add(reader_parca[2]);
                listView1.Items.Add(item);              
            }      
        }

        public void ExcelVerileriAktar()
        {
            try
            {
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

                        hucre = ws.Cells[1, 1];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Numaralar";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 2];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Tarih";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        hucre = ws.Cells[1, 3];
                        rr = ws.get_Range(hucre, hucre);
                        str = "Saat";
                        rr.Value2 = str;
                        rr.Font.Bold = true;
                        rr.Font.Size = 12;

                        int dikey = 2;
                        int yatay = 2;

                        if (!TamOrAralik)
                        {
                            foreach (ListViewItem lvi in listView2.Items)
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

                                    dikey++;
                                }
                                yatay++;
                            }
                        }
                        else
                        {
                            foreach (ListViewItem lvi in listView1.Items)
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

                                    dikey++;
                                }
                                yatay++;
                            }
                        }
                        kitap.Close();
              
               
            }
            catch { MessageBox.Show("Bir sorun oluştu.Lütfen tekrar deneyiniz.", "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void gelenaramalar_Load(object sender, EventArgs e)
        {
            this.tarih_farkı();
            this.verileri_listele();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            this.tarih_farkı();
            this.verileri_listele();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            this.tarih_farkı();
            this.verileri_listele();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TamOrAralik = true;
            ExcelVerileriAktar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TamOrAralik = false;
            ExcelVerileriAktar();
        }
  
    }
}
