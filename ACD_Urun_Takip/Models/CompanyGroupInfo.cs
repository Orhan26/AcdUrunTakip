using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class CompanyGroupInfo
    {
        public int HsgID { get; set; }
        public GroupInformation GroupID { get; set; }
        public CompanyInformation CompanyID { get; set; }
        public bool StartStop { get; set; }
        public bool isDeleted { get; set; }
    }
}