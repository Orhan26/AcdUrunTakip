using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class DocumentInfo
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public CompanyInformation Company { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }
        public bool IsPreview { get; set; }
    }
}