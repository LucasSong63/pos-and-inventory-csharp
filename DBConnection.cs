using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos_and_inventory_csharp
{
    internal class DBConnection
    {
        public string MyConnection()
        {
            string con = @"Data Source=ICM-LUCAS;Initial Catalog=POS_DEMO_DB;Integrated Security=True";
            return con;
        }
    }
}
