using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ULUSOY;

namespace JMOtobusYazilimi
{
    public partial class frmHarici : Form
    {
        public frmHarici()
        {
            InitializeComponent();
        }
        void DegiskenVerileriniYukle()
        {
            this.listView5.Items.Clear();
            baglan.Open();
            SqlDataReader reader = new SqlCommand("select * from Degiskenler order by degisken_adi", baglan).ExecuteReader();

            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());
                item.SubItems.Add(reader[1].ToString());
                item.SubItems.Add(reader[2].ToString());
                item.SubItems.Add(reader[3].ToString());

                this.listView5.Items.Add(item);
            }
            baglan.Close();
        }
        private void button18_Click(object sender, EventArgs e)
        {
            if (txtDegiskenAdi.Text.Trim() == "") { return; }
            if (txtDegeri.Text.Trim() == "") { return; }
            if (txtEtiketAdi.Text.Trim() == "") { return; }

            SqlCommand ekle = new SqlCommand("insert into  Degiskenler(degisken_adi,etiket_adi,degeri)values("
        + "'" + txtDegiskenAdi.Text.Trim() + "' ,"
        + "'" + txtEtiketAdi.Text.Trim() + "' ,"

        + "'" + Convert.ToDouble(txtDegeri.Text.Trim()).ToString() + "')"
       , baglan);
            baglan.Open();
            ekle.ExecuteNonQuery();
            baglan.Close();
            listView5.Items.Add(txtDegiskenAdi.Text.Trim());
            listView5.Items[0].SubItems.Add(txtDegeri.Text.Trim());
            listView5.Items[0].SubItems.Add(txtEtiketAdi.Text.Trim());

            DegiskenVerileriniYukle();
            txtDegiskenAdi.Text = "";
            txtDegeri.Text = "";


        }
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
        private void button17_Click(object sender, EventArgs e)
        {

            if (listView5.SelectedItems.Count == 0) { return; }

            string[] sil;
            sil = listView5.SelectedItems[0].Text.Trim().Split(',');
            if (this.listView5.SelectedItems[0].Selected)
            {

                SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
                SqlCommand command = new SqlCommand("delete  from Degiskenler where ID='" + sil[0] + "'", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                listView5.SelectedItems[0].Remove();

            }
        }
        private void frmHarici_Load(object sender, EventArgs e)
        {

        }
    }
}
