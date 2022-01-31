using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class ModuleInformation
    {
        public int ModuleId { get; set; }
        public ProgramInfo Program { get; set; }
        public string ModuleName { get; set; }
    }
}