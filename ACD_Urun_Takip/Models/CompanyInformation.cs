using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class CompanyInformation
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Authorized { get; set; }
        public string AuthorizedPhone { get; set; }
        public string AuthorizedMail { get; set; }

    }
}