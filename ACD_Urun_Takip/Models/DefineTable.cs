using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class DefineTable
    {
        public DefineTable(string tblName)
        {
            this.Name = HttpUtility.UrlDecode(tblName);
            this.Body = new List<dynamic>();
        }
        public string Caption { get; set; }
        /// <summary>
        ///Table name in SQL
        /// </summary>
        public string Name { get; set; }
        public string Menu { get; set; }
        public bool? IsBookMark { get; set; } = false;
        public List<dynamic> Body { get; set; }
    }

    public class DefineRowModel
    {
        public int TableID { get; set; }
        public string TableName { get; set; }
        public Guid GID { get; set; }
        public string uniqueId { get; set; }
    }

    public class DefineDeleteModel
    {
        public List<System.Guid> Ids { get; set; }
        public string TableName { get; set; }
    }
    public class DefineMultiRowModel
    {
        public object[] Ids { get; set; }
        public string fm { get; set; }
        public string TableName { get; set; }
    }


}