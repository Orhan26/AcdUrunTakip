using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class GlobalLogModel
    {
        public string Description { get; set; }
        public int UserId { get; set; }
        public string Method { get; set; }
        public string ErrorType { get; set; }
        public string Parameters { get; set; }
    }
}