using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class SalesInfo
    {
        public int SalesId { get; set; }
        public CompanyInformation Company { get; set; }
        public ProgramInfo Program { get; set; }
        public ModuleInformation Module { get; set; }
        public HardwareInfo Hardware { get; set; }
        public string HardwareQty { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
    }
}