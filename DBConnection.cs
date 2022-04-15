using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace pos_and_inventory_csharp
{
    internal class DBConnection
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public string MyConnection()
        {
            string con = @"Data Source=ICM-LUCAS;Initial Catalog=POS_DEMO_DB;Integrated Security=True";
            return con;
        }

        public double GetVal()
        {
            double vat=0;
            cn.ConnectionString = MyConnection();
            cn.Open();
            cm =    new SqlCommand("select * from tblvat ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                vat = double.Parse(dr["vat"].ToString());
            }
            dr.Close();
            cn.Close();
            return vat;
        }
    }
}
