using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;

namespace ACD_Urun_Takip.Repository.User
{
    public class UserRepository
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        private static readonly string lang = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];


        public static ProfileModel GetProfile(int userId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysUserProfileSettings", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GETUSER");
                    command.Parameters.AddWithValue("@userid", userId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            ProfileModel profile = new ProfileModel();
            foreach (DataRow item in dt.Rows)
            {
                profile.Code = (string)item["Usr_Code"];
                profile.Mail = (string)item["Usr_eMail"];
                profile.Name = (string)item["Usr_NameSurname"];
                profile.Phone = (string)item["Usr_Phone"];
                profile.Image = item["Usr_ImageID"].ToString();
            }
            return profile;

        }

        public static EditUserModel GetUser(long userid)
        {
            DataTable table = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GETONEUSER");
                    command.Parameters.AddWithValue("@userid", userid);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(table);
                    }
                }
            }
            EditUserModel user = new EditUserModel
            {
                code = table.Rows[0]["Usr_Code"].ToString(),
                group = Convert.ToInt64(table.Rows[0]["Usr_GroupID"]),
                id = Convert.ToInt64(table.Rows[0]["ID"]),
                mail = table.Rows[0]["Usr_eMail"].ToString(),
                name = table.Rows[0]["Usr_NameSurname"].ToString(),
                phone = table.Rows[0]["Usr_Phone"].ToString(),
                department2 = table.Rows[0]["Departman"].ToString()
            };
            return user;
        }

        public static AjaxResult CreateUserGroup(string usgname, int userId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@mod", "CREATEGROUP");
                        command.Parameters.AddWithValue("@UsgName", usgname);
                        command.Parameters.AddWithValue("@userid", userId);
                        command.ExecuteScalar();
                    }
                }
                return new AjaxResult { Success = true, Message = "Kayıt edildi." };
            }
            catch (Exception ex)
            {

                return new AjaxResult { Success = false, Message = "Kayıt edilirken bir hata ile karşılaşıldı. Error:" + ex.Message };
            }
        }

        public static AjaxResult UpdateMenuGroup(SysMenu sysMenu, int userId)
        {
            int insert = -1;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sysMenu.IsReport ? "[SysReportUserSettings]" : "[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "UPDATEMENU");
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@GroupID", sysMenu.UserGroupId);
                    command.Parameters.AddWithValue("@MenuID", sysMenu.MenuId);
                    command.Parameters.AddWithValue("@CHECK", sysMenu.IsRead);
                    if (!sysMenu.IsReport)
                    {
                        command.Parameters.AddWithValue("@IsCreate", sysMenu.IsCreate);
                        command.Parameters.AddWithValue("@IsUpdate", sysMenu.IsUpdate);
                        command.Parameters.AddWithValue("@IsDelete", sysMenu.IsDelete);
                    }

                    insert = Convert.ToInt16(command.ExecuteScalar());
                }
            }
            return new AjaxResult { Success = true, Code = insert, Message = "Güncelleme Tamamlandı!" };
        }

        public static AjaxResult DeleteUser(long userid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@mod", "DELETEUSER");
                        command.Parameters.AddWithValue("@userid", userid);
                        command.ExecuteScalar();
                    }
                }

                return new AjaxResult { Success = true, Message = "Kullanıcı silindi." };
            }
            catch (Exception ex)
            {

                return new AjaxResult { Success = false, Message = "Kullanıcı silinirken bir hata ile karşılasıldı.Error:" + ex.Message };

            }
        }

        public static MenuProfileGroupModel GetMenuGroupById(long groupid, int userId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GetMenuByGroup");
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@GroupID", groupid);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(ds);
                    }
                }
            }

            MenuProfileGroupModel menuProfileGroup = new MenuProfileGroupModel();
            menuProfileGroup.AllMenu = ds.Tables[0].AsEnumerable().Select(item => new AllMenu
            {
                MenuId = Convert.ToInt64(item["Mnu_ID"]),
                MenuUpperId = Convert.ToInt64(item["Mnu_UpperID"]),
                Title = (string)item["Mnu_Title"],
                HasChild = (bool)item["Mnu_HasChild"],
                IsReport = Convert.ToBoolean(item["Menu"]),
                IsCreate = Convert.ToBoolean(item["Smp_Create"]),
                IsRead = Convert.ToBoolean(item["Smp_Read"]),
                IsUpdate = Convert.ToBoolean(item["Smp_Update"]),
                IsDelete = Convert.ToBoolean(item["Smp_Delete"])


            }).ToList();
            return menuProfileGroup;
        }

        public static DataTable GetUserMenu(int userId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GETGROUP");
                    command.Parameters.AddWithValue("@userid", userId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static AjaxResult UpdateUser(EditUserModel model, int userId)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@mod", "UPDATEUSER");
                        command.Parameters.AddWithValue("@userid", model.id);
                        command.Parameters.AddWithValue("@GroupID", model.group);
                        command.Parameters.AddWithValue("@name", model.code);
                        command.Parameters.AddWithValue("@NameSurname", model.name);
                        command.Parameters.AddWithValue("@mail", model.mail);
                        command.Parameters.AddWithValue("@number", model.phone);
                        command.ExecuteScalar();
                    }
                }
                for (int i = 0; i < model.department.Length; i++)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@mod", "INSERTRELATION");
                            command.Parameters.AddWithValue("@userid", model.id);
                            command.Parameters.AddWithValue("@depID", model.department[i]);
                            command.ExecuteScalar();
                        }
                    }
                }
                return new AjaxResult { Success = true, Message = "Kullanıcı başarıyla güncellendi." };
            }
            catch (Exception ex)
            {

                return new AjaxResult { Success = false, Message = "Kullanıcı güncellenirken hata ile karşılaşıldı.Error:" + ex.Message };

            }
        }

        public static AjaxResult CreateUser(CreateUserModel model, int userId)
        {
            float scopeidentity = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "CREATENEWUSER");
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@GroupID", model.group);
                    command.Parameters.AddWithValue("@name", model.code);
                    command.Parameters.AddWithValue("@newpass", model.password);
                    command.Parameters.AddWithValue("@NameSurname", model.name);
                    command.Parameters.AddWithValue("@mail", model.mail);
                    command.Parameters.AddWithValue("@number", model.phone);
                    scopeidentity = Convert.ToInt64(command.ExecuteScalar());
                }
            }
            try
            {

                for (int i = 0; i < model.department.Length; i++)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@mod", "INSERTRELATION");
                            command.Parameters.AddWithValue("@userid", scopeidentity);
                            command.Parameters.AddWithValue("@depID", model.department[i]);
                            command.ExecuteScalar();
                        }
                    }
                }
                return new AjaxResult { Success = true, Message = "Kullanıcı başarıyla eklendi." };
            }
            catch (Exception ex)
            {

                return new AjaxResult { Success = false, Message = "Kullanıcı eklenirken hata ile karşılaştı.Error:" + ex.Message };

            }
        }

        public static DataSet GetUsers(int userId)
        {
            DataSet dataset = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("[SysUserProfileSettings]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@mod", "GETUSERGROUP");
                    command.Parameters.AddWithValue("@userid", userId);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(dataset);
                    }
                }
            }
            return dataset;
        }
    }
}