using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Extension;
using ACD_Urun_Takip.Repository.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Extensions
{
    public static class WebExtension
    {
        public static IEnumerable<LanguageModel> languageModel = JsonConvert.DeserializeObject<IEnumerable<LanguageModel>>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Language/language.json"));

        public static IHtmlString GetLang(this HtmlHelper helper, string key)
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
                return new HtmlString(description);

            }
            catch (Exception ex)
            {
                LanguageHelper.InsertWords(key);
                return new HtmlString(key);
            }
        }


        public static IHtmlString CreateMenu(this HtmlHelper helper, int userId)
        {
            string html = ExtensionRepository.CreateMenu(userId);

            return new HtmlString(html);

        }

        public static IHtmlString CreateForm(this HtmlHelper helper, List<SqlDataType> content, string tablename)
        {
            string formHtml = ExtensionRepository.CreateForm(content, tablename);
            return new HtmlString(formHtml);
        }
        public static IHtmlString EncryptData(this HtmlHelper helper, string data)
        {
            ICrypt aes = new CryptAdapter(new AES());
            return new HtmlString(aes.Encrypt(data));
        }

    }
}