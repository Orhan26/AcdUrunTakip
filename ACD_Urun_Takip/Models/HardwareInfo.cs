using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class HardwareInfo
    {
        public int HardwareId { get; set; }
        public string HardwareName { get; set; }
        public string HardwareType { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public bool iconHide { get; set; }
    }
}