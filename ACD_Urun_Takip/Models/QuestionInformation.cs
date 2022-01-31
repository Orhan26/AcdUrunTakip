using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class QuestionInformation
    {
        public int QuestionID { get; set; }
        public GroupInformation Group { get; set; }
        public string Question { get; set; }
        public string isDeleted { get; set; }

    }
}