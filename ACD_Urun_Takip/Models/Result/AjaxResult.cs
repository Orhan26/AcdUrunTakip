using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models.Result
{
    public class AjaxResult
    {

        public bool Success { get; set; }
        public int? Code { get; set; }
        public string Message { get; set; }
        public Guid Guid { get; set; }
        public List<MultipleResult> Result { get; set; }
        public string Help { get; set; }
        public string Lang { get; set; }
        public DataTable Table { get; set; }
        public long? Identity { get; set; }
    }
}