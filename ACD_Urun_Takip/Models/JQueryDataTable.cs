using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class JQueryDataTable
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<string> data { get; set; }
    }
    public class JQueryDataTableModel
    {
        public float KpiId { get; set; }
        public string more_data { get; set; }
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
        public string oSearch { get; set; }
        public string tablename { get; set; }
        public string[] orders { get; set; }
        public string columnsearch { get; set; }
        public string date { get; set; }
    }
}