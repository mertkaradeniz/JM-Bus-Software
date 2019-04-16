using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Data.SqlClient;
using System.Reflection;
				
namespace ULUSOY
{
    public partial class frmWordAktar : Form
    {
        public frmWordAktar()
        {
            InitializeComponent();
        }

        Word._Application uygulama = new Word.Application();
        Word._Document belge;
        int toplamAlan = 0;
        string[] tut = frmAna.dısa_aktar_parametre.Split('+');
        string[] ARACBILGILERI = frmAna.ARACBILGILERI.Split('~');
        SqlConnection baglan = new SqlConnection(ClsDatabase.data_base());
     
        private void worda_aktar_Load(object sender, EventArgs e)
        {
            listView3.Items.Clear();
            baglan.Open();
            SqlDataReader reader = new SqlCommand("select islem.koltuk_no,yolcu.ad,yolcu.soyad,islem.tel,islem.bindi,islem.indi,islem.bilet_islem,islem.ucret from TumIslemler islem inner join Yolcular yolcu on (islem.ID2=yolcu.ID2)  where tarih='" + tut[0] + "' and plaka='" + tut[1] + "' and saat='" + tut[2] + "' order by islem.koltuk_no asc", this.baglan).ExecuteReader();
            while (reader.Read())
            {
                ListViewItem item = new ListViewItem(reader[0].ToString());
                item.SubItems.Add(reader[1].ToString() + " " + reader[2].ToString());
                item.SubItems.Add(reader[3].ToString());
                item.SubItems.Add(reader[4].ToString() + "/" + reader[5].ToString());
                item.SubItems.Add(reader[6].ToString() + " / " + reader[7].ToString());              
                listView3.Items.Add(item);          
            }
            reader.Close();
            baglan.Close();

            plaka.Text = tut[1].ToString();
            zaman.Text = tut[0]+"/"+tut[2];
            kaptan.Text = ARACBILGILERI[0];

            VerileriOlustur();
            this.Close();
        }
       
        public DataTable VerileriTabloyaYukle(string sorgucumle)
        {
            SqlConnection selectConnection = this.VeriTabaniBaglanti();
            SqlDataAdapter adapter = new SqlDataAdapter(sorgucumle, selectConnection);
            DataTable dataTable = new DataTable();
            try
            {
                adapter.Fill(dataTable);
            }
            catch (SqlException exception)
            {
                throw new Exception(exception.Message + "(" + sorgucumle + ")");
            }
            adapter.Dispose();
            selectConnection.Close();
            selectConnection.Dispose();
            return dataTable;
        }

        public SqlConnection VeriTabaniBaglanti()
        {
            SqlConnection connection = new SqlConnection(ClsDatabase.data_base());
            connection.Open();
            return connection;
        }

        private void VerileriWordeAktar(Word.Field alan, string text)
        {     
            alan.Select();     
            uygulama.Selection.TypeText(text);
        }

        void VerileriOlustur()
        {
           
        uygulama.Visible = true;
        object missing = Missing.Value;
        Object sablonYolu = System.Windows.Forms.Application.StartupPath + "\\DataBase\\ULUSOY.docx";
        belge = uygulama.Documents.Add(ref sablonYolu, ref missing, ref missing, ref missing);
        
        foreach (Word.Field alanAdi in belge.Fields)
        {
            toplamAlan++;
            Word.Range rngAlan = alanAdi.Code;
            string fieldText = rngAlan.Text;
 
            if (fieldText.StartsWith(" MERGEFIELD"))
            {
                  
                int bitis = fieldText.IndexOf("\\");
                int uzunluk = fieldText.Length - bitis;
                string fieldName = fieldText.Substring(11, bitis - 11);
                fieldName = fieldName.Trim();
   
                switch (fieldName)
                {
                    case "kaptan": VerileriWordeAktar(alanAdi, kaptan.Text); break;
                    case "saat": VerileriWordeAktar(alanAdi, zaman.Text); break;
                    case "plaka": VerileriWordeAktar(alanAdi, plaka.Text); break;
                    case "toplamtutar": VerileriWordeAktar(alanAdi, ARACBILGILERI[1] + " TL"); break;
                    case "servis": VerileriWordeAktar(alanAdi, ARACBILGILERI[2] + " TL"); break;
                    case "reklam": VerileriWordeAktar(alanAdi, ARACBILGILERI[3] + " TL"); break;
                    case "genel": VerileriWordeAktar(alanAdi, ARACBILGILERI[4] + " TL"); break;
                    case "komisyon": VerileriWordeAktar(alanAdi, ARACBILGILERI[5] + " TL"); break;
                    default: break;
                }

                  #region YOLCULARI AKTARMA 

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {               
                        if (listView3.Items[i].Text == "1")
                        {
                            switch (fieldName)
                            {
                                case "Belirsiz1": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                                default: break;
                            }
                        }                  
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "2")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz2": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "3")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz3": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "4")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz4": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "5")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz5": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "6")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz6": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "7")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz7": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "8")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz8": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "9")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz9": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
                 
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "10")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz10": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
            
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "11")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz11": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
                 
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "12")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz12": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
               
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "13")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz13": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
                 
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "14")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz14": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "15")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz15": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "16")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz16": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "17")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz17": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "18")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz18": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
              
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "19")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz19": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "20")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz20": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "21")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz21": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "22")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz22": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "23")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz23": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "24")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz24": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "25")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz25": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "26")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz26": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "27")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz27": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "28")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz28": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "29")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz29": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
     
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "30")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz30": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "31")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz31": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "32")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz32": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
                  
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "33")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz33": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
             
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "34")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz34": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }
          
                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "35")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz35": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  for (int i = 0; i < listView3.Items.Count; i++)
                  {
                      if (listView3.Items[i].Text == "36")
                      {
                          switch (fieldName)
                          {
                              case "Belirsiz36": VerileriWordeAktar(alanAdi, "Ad:" + listView3.Items[i].SubItems[1].Text + " * Tel:" + listView3.Items[i].SubItems[2].Text + " * " + listView3.Items[i].SubItems[3].Text + " - (" + listView3.Items[i].SubItems[4].Text + " TL)"); break;
                              default: break;
                          }
                      }
                  }

                  #endregion

                }
            }
        }
       
    }
}
