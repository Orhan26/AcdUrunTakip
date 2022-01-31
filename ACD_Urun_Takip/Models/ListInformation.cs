using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class ListInformation
    {
        public List<CompanyInformation> Companies { get; set; }
        public List<GroupInformation> Groups { get; set; }
        public List<CompanyGroupInfo> CompanyGroups { get; set; }
        public DocumentInfo  DocumentInfo { get; set; }
        public List<ProgramInfo>  ProgramInfos { get; set; }
        public LicenceInfo  LicenceInfo { get; set; }
        public List<HardwareInfo>  Hardwares { get; set; }
        public SalesInfo SalesInfo { get; set; }
        public List<ModuleInformation> Modules { get; set; }
        public ModuleInformation ModuleInformation { get; set; }
    }
}