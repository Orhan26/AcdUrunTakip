using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class LicenceInfo
    {
        public int LicenceId { get; set; }

        public string LicenceKey { get; set; }

        public CompanyInformation Company { get; set; }
        public ProgramInfo Program { get; set; }
        public ModuleInformation Module { get; set; }
        
    }
}