using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Helper
{
    public class SecurityHelper
    {
        public static List<LoggedIn> GetLoggedInUsers()
        {
            List<LoggedIn> loggedInUsers = new List<LoggedIn>();

            if (HttpContext.Current != null)
            {
                loggedInUsers = (List<LoggedIn>)HttpContext.Current.Application["loggedinusers"];

                if (loggedInUsers == null)
                {
                    loggedInUsers = new List<LoggedIn>();
                    HttpContext.Current.Application["loggedinusers"] = loggedInUsers;
                }

            }
            return loggedInUsers;

        }
    }
}