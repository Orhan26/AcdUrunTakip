using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Models
{
    public class LoggedIn
    {
        public LoggedIn()
        {
            Date = DateTime.Now;
            Username = HttpContext.Current.User.Identity.Name;
        }
        public string Username { get; set; }
        public string Page { get; set; }
        public DateTime Date { get; set; }
        public bool Active { get; set; }

    }
}