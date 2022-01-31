using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class UserModel
    {
        public string code { get; set; }
        public string name { get; set; }
        public string mail { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public long group { get; set; }
        public long[] department { get; set; }
    }
    public class CreateUserModel : UserModel
    {

    }
    public class EditUserModel : UserModel
    {
        public long id { get; set; }
        public string department2 { get; set; }
    }
}