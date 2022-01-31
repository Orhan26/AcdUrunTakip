using Newtonsoft.Json;
using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Helper
{
    public static class LanguageHelper
    {
        private readonly static string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        public static IEnumerable<LanguageModel> languageModel = JsonConvert.DeserializeObject<IEnumerable<LanguageModel>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Language/language.json"));

        public static void RefreshLanguageModel()
        {
            languageModel = JsonConvert.DeserializeObject<IEnumerable<LanguageModel>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Language/language.json"));
        }      

        public static string GetLangFromServer(string key)
        {
            try
            {
                string description = String.Empty;
                var currentCulture = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                switch (currentCulture)
                {
                    case "tr":
                    case "tr-TR":
                        description = languageModel.FirstOrDefault(x => x.key == key).tr;
                        break;
                    case "en":
                    case "en-US":
                        description = languageModel.FirstOrDefault(x => x.key == key).en;
                        break;
                    case "ru":
                    case "ru-RU":
                        description = languageModel.FirstOrDefault(x => x.key == key).ru;
                        break;
                    default:
                        description = languageModel.FirstOrDefault(x => x.key == key).de;
                        break;
                }
                return description;

            }
            catch (Exception ex)
            {
                InsertWords(key);
                return key;
            }

        }

        public static string GetLangWithValue(string value)
        {
            try
            {
                string description = String.Empty;
                var currentCulture = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                switch (currentCulture)
                {
                    case "tr":
                    case "tr-TR":
                        description = languageModel.FirstOrDefault(x => x.tr == value).key;
                        break;
                    case "en":
                    case "en-US":
                        description = languageModel.FirstOrDefault(x => x.en == value).key;
                        break;
                    case "ru":
                    case "ru-RU":
                        description = languageModel.FirstOrDefault(x => x.ru == value).key;
                        break;
                    default:
                        description = languageModel.FirstOrDefault(x => x.de == value).key;
                        break;
                }
                return description;
            }
            catch (Exception ex)
            {
                InsertWords(value);
                return value;
            }

        }

        public static string GetKeyFromServer(string key)
        {
            try
            {
                string description = String.Empty;
                var currentCulture = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                switch (currentCulture)
                {
                    case "tr":
                    case "tr-TR":
                        description = languageModel.FirstOrDefault(x => x.key == key).tr;
                        break;
                    case "en":
                    case "en-US":
                        description = languageModel.FirstOrDefault(x => x.key == key).en;
                        break;
                    case "ru":
                    case "ru-RU":
                        description = languageModel.FirstOrDefault(x => x.key == key).ru;
                        break;
                    default:
                        description = languageModel.FirstOrDefault(x => x.key == key).de;
                        break;
                }
                return description;

            }
            catch (Exception ex)
            {
                InsertWords(key);
                return key;
            }

        }

        public static void InsertWords(string word)
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SysLanguageSetting", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@word", word));
                    command.ExecuteScalar();
                }
            }
        }

    }
}