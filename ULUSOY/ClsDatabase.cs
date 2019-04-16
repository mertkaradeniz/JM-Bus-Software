using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ULUSOY
{
    class ClsDatabase
    {
        public static string aktif_veritabani_adi = "";
        
        public static string  data_base()
        {
          string[] dizi = System.IO.File.ReadAllLines(Application.StartupPath + "\\DataBase\\Server.jmt");

            if (dizi[0] == "True")
            {
                aktif_veritabani_adi = dizi[2].ToString();             
                return "server='" + dizi[1] + "'; Password='" + dizi[4] + "';Persist Security Info=True;User ID='" + dizi[3] + "'; Database=" + dizi[2] + ";";
            }
            else
            {
                aktif_veritabani_adi = "Data";              
                return "Data Source=" + System.Environment.MachineName + "\\SQLEXPRESS; Initial Catalog='Data'; Integrated security=true";
            }
        }
        public static string anaserver = "server='93.89.230.234'; Password='jmt66382183670!';Persist Security Info=True;User ID='jmtech'; Database=jmt_db";
    }
}
