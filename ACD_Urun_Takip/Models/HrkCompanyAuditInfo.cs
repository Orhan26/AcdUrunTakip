using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class HrkCompanyAuditInfo
    {
        public int DenetimID { get; set; }
        public CompanyInformation SirketID { get; set; }
        public GroupInformation GrupID { get; set; }
        public QuestionInformation SoruID { get; set; }
        public bool Cevap { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
    }
}