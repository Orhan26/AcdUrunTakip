
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ACD_Urun_Takip.Controllers;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;

namespace ACD_Urun_Takip.Repository.Login
{
    public class LoginRepository
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        public static (string, Version) GetDbVersion()
        {
            Assembly assembly = typeof(HomeController).Assembly;
            AssemblyName assemblyName = assembly.GetName();
            Version version = assemblyName.Version;
            string dbVersiyon = "";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("[SysGetDbVersion]", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    dbVersiyon = command.ExecuteScalar().ToString();
                }
            }
            return (dbVersiyon, version);
        }

        /// <summary>
        /// HttpContext.User.Identity.Name format userName, userId, imageId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async static Task<AjaxResult> LoginAsync(LoginModel model)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            object password = null;
            int userId = 0, userGroup = 0;
            string userGrupname = "", imageId = String.Empty, lang = String.Empty;

            try
            {
                Task<object> passwordT = DbOperation.ExecuteScalarAsync("Select Usr_Password from SysUser where Usr_Code='" + model.UserName + "' AND Usr_Active=1");
                Task<object> userIdT = DbOperation.ExecuteScalarAsync("Select ID from SysUser where Usr_Code='" + model.UserName + "' AND Usr_Active=1");
                Task<object> userGroupT = DbOperation.ExecuteScalarAsync("Select Usr_GroupID from SysUser where Usr_Code='" + model.UserName + "' AND Usr_Active=1");
                Task<object> imageIdT = DbOperation.ExecuteScalarAsync("Select Usr_ImageID from SysUser where Usr_Code='" + model.UserName + "' AND Usr_Active=1");

                await Task.WhenAll(passwordT, userGroupT, userIdT, imageIdT);
                userId = Convert.ToInt32(userIdT.Result);
                password = passwordT.Result;
                userGroup = Convert.ToInt32(userGroupT.Result);
                imageId = Convert.ToString(imageIdT.Result);

                //Task<object> themeT = DbOperation.ExecuteScalarAsync("Select Usr_Theme from SysUser where ID=" + userId + "");//TODO:theme yapısı UI kısmına entegre edilmedi
                //Session["Theme"] = (string)command.ExecuteScalar();
                Task<object> userGroupNameT = DbOperation.ExecuteScalarAsync("SELECT UsG_Name FROM SysUser AS su INNER JOIN SysUserGroup AS sug ON su.Usr_GroupID=sug.UsG_ID WHERE su.ID=" + userId);
                Task<object> languageT = DbOperation.ExecuteScalarAsync("Select Usr_Language from SysUser where ID=" + userId + "");
                await Task.WhenAll(userGroupNameT, languageT);
                userGrupname = Convert.ToString(userGroupNameT.Result);
                lang = Convert.ToString(languageT.Result);

                bool isLogin = CompareHashValue(model.Password, password);
                if (isLogin)
                {
                    string username = String.Format("{0}_{1}_{2}_{3}_{4}", model.UserName, userId, imageId == "" ? "None" : imageId, userGroup, userGrupname);
                    SignInRemember(username, model.Remember);
                    stopwatch.Stop();
                    return new AjaxResult { Success = true, Code = 0, Lang = lang, Message = "Giriş Yapıldı" + stopwatch.ElapsedMilliseconds };
                }
                else
                {
                    stopwatch.Stop();
                    return new AjaxResult { Success = false, Code = 2, Message = "Girdiğiniz kullanıcı adı ve şifre eşleşmemektedir." + stopwatch.Elapsed };

                }


            }
            catch (NullReferenceException nullexception)
            {
                stopwatch.Stop();
                return new AjaxResult { Success = false, Code = 1, Message = "Girilen Bilgilere Kayıtlı Kullanıcı Bulunmamaktadır." + stopwatch.ElapsedMilliseconds };
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                return new AjaxResult { Success = false, Code = 1000, Message = exception.Message + stopwatch.ElapsedMilliseconds };
            }

        }
        private static void SignInRemember(string userName, bool isPersistent = false)
        {
            if (HttpContext.Current.User.Identity.Name == userName)
            {
                FormsAuthentication.SignOut();
            }
            // Write the authentication cookie    
            FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }
        public static bool CompareHashValue(string pass, object password)
        {
            bool match = true;
            byte[] hash;
            try
            {
                MD5 md5 = MD5.Create();
                hash = md5.ComputeHash(Encoding.Unicode.GetBytes(pass));

                if (password == null)
                    return false;

                for (int i = 0; i < hash.Length; i++)
                {
                    if (((byte[])password)[i] != hash[i])
                    {
                        match = false;
                        break;
                    }
                }
                return match;
            }
            catch
            {
                return false;
            }
        }
    }

}