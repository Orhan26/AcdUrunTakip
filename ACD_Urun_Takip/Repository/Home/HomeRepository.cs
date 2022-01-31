using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Repository.Home
{

    public static class HomeRepository
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        /// <summary>
        /// Önemli! Language.json dosyasının güncel olması için dil tablosunda yapılan değişiklik sonrası burası çalıştırılmalıdır.
        /// </summary>
        /// <returns></returns>
        public static string CreateLanguage()
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/Language/language.json", String.Empty);
                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("[SysLanguageFile]", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            sda.Fill(table);
                        }
                    }
                }
                string json = JsonConvert.SerializeObject(table);
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/Language/language.json", json);
                LanguageHelper.RefreshLanguageModel();
                sw.Stop();
                return sw.ElapsedMilliseconds.ToString();
            }
            catch (Exception ex)
            {
                sw.Stop();
                return ex.Message;
            }

        }

        public static string SaveGlobalLog(GlobalLogModel model)
        {
            string result = String.Empty;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SystemLog", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userid", model.UserId);
                    command.Parameters.AddWithValue("@description", model.Description);
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                    command.Parameters.AddWithValue("@method", model.Method);
                    command.Parameters.AddWithValue("@type", model.ErrorType);
                    command.Parameters.AddWithValue("@param", model.Parameters);

                    result = (string)command.ExecuteScalar();
                }
            }
            return result;
        }

    


     


    }
}