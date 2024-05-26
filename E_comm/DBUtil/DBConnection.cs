using System;
using E_comm.entity;
using E_comm.DBUtil;
using E_comm.exception;
using E_comm.Main;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace E_comm.DBUtil
{
    public static class DBConnection
    {
        
        private static string connstring = "Data Source=LAPTOP-8NH5PHBS;Initial Catalog=Ecommerce;Integrated Security=True";

        public static SqlConnection GetConnectionString()
        {
            return new SqlConnection(connstring);
        }
    }
}
