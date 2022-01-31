using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Attribute
{
    public static class AttributeRepository
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        public static bool SaveLogElapsedTime(string action, string controller, string param, float elapsed, string userid)
        {
            int time = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysLogElapsedTime", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GETTIME");
                    time = (int)command.ExecuteScalar();
                }
                if (elapsed > (time * 3000))
                {
                    using (SqlCommand command = new SqlCommand("SysLogElapsedTime", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@mod", "SET");
                        command.Parameters.AddWithValue("@action", action);
                        command.Parameters.AddWithValue("@controller", controller);
                        command.Parameters.AddWithValue("@param", param);
                        command.Parameters.AddWithValue("@elapsed", elapsed);
                        command.Parameters.AddWithValue("@userid", userid);
                        command.ExecuteScalar();
                    }
                }
            }
            return true;
        }
        public static string GetPage(string cont, string act, string param)
        {
            string sql = String.Empty;
            string result = String.Empty;
            if (param == "") param = "-";
            if (cont == "Home" && act == "Index") return "Anasayfa";
            if (param.Contains("reportname"))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    sql = "Select RepMenu_Title from SysReportMenu where RepMenu_Controller='" + cont + "' AND RepMenu_Action='" + act + "' AND RepMenu_Parameter ='" + param.Split('=')[1] + "'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        result = (string)command.ExecuteScalar();
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (param != "-")
                    {
                        sql = "Select Mnu_Title from SysMenu where Mnu_Controller='" + cont + "' AND Mnu_Action='" + act + "' AND Mnu_Parameter ='" + param.Split('=')[1] + "'";
                    }
                    else
                    {
                        sql = "Select Mnu_Title from SysMenu where Mnu_Controller='" + cont + "' AND Mnu_Action='" + act + "'";
                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        result = (string)command.ExecuteScalar();
                    }
                }
            }
            return result;
        }
        public static List<AllMenu> GetMenuWithPermission(string userName)
        {
            long usergroupId;
            List<AllMenu> menuList = new List<AllMenu>();
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("Select Usr_GroupID from SysUser where Usr_Code='" + userName.Split('_')[4] + "' AND Usr_Active=1", con))
                {
                    usergroupId = Convert.ToInt32(command.ExecuteScalar());
                }

                using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GetMenuByGroup");
                    command.Parameters.AddWithValue("@userid", Convert.ToInt32(userName.Split('_')[1]));
                    command.Parameters.AddWithValue("@GroupID", usergroupId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(ds);
                    }
                }
            }
            foreach (var item in ds.Tables[0].AsEnumerable())
            {
                AllMenu menu = new AllMenu
                {
                    MenuId = Convert.ToInt64(item["Mnu_ID"]),
                    MenuUpperId = Convert.ToInt64(item["Mnu_UpperID"]),
                    Title = (string)item["Mnu_Title"],
                    HasChild = (bool)item["Mnu_HasChild"],
                    IsReport = Convert.ToBoolean(item["Menu"]),
                    IsCreate = Convert.ToBoolean(item["Smp_Create"]),
                    IsRead = Convert.ToBoolean(item["Smp_Read"]),
                    IsUpdate = Convert.ToBoolean(item["Smp_Update"]),
                    IsDelete = Convert.ToBoolean(item["Smp_Delete"]),
                    Path = (string)item["UrlPath"]


                };
                if (menu != null)
                    menuList.Add(menu);
            }
            return menuList;
        }
    }
}