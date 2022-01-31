using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class SqlConn
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
    }
}