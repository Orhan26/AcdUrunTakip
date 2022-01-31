using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Menu = ACD_Urun_Takip.Models.Menu;

namespace ACD_Urun_Takip.Repository.Extension
{
    public class ExtensionRepository
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        /// <summary>
        /// return menu list
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public static List<Menu> GetMenu(int userId)
        {
            List<Menu> menus = new List<Menu>();
            FormCollection fm = new FormCollection();
            fm.Add("tablename", "SysMenu");
            fm.Add("UserID", userId.ToString());
            //TODO:async yapılmalı !!
            DataTable dt = DbOperation.FillDataTable(fm, "SysFillMenu");
            bool pref = GetUserNewTab(userId);
            menus = dt.AsEnumerable().Select(item => new Menu
            {
                Action = (string)item["Mnu_Action"],
                Controller = (string)item["Mnu_Controller"],
                Description = (string)item["Mnu_Description"],
                Icon = (string)item["Mnu_Icon"],
                Id = (int)item["Mnu_ID"],
                Order = (int)item["Mnu_Order"],
                Parameter = item["Mnu_Parameter"].ToString() ?? (string)item["Mnu_Parameter"],
                Title = (string)item["Mnu_Title"],
                UpperId = (int)item["Mnu_UpperID"],
                Search = (string)item["Mnu_Search"],
                IsReport = (bool)item["Mnu_IsReport"],
                HasChild = (bool)item["Mnu_HasChild"],
                IsNewTab = pref
            }).ToList();
            return menus;
        }

        /// <summary>
        /// return new tab option for menu 
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public static bool GetUserNewTab(int userId)
        {
            bool pref = false;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysUserProfileSettings", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@newTab", pref);
                    command.Parameters.AddWithValue("@mod", "GETUSERNEWTAB");
                    pref = Convert.ToBoolean(command.ExecuteScalar());
                }
            }
            return pref;
        }

        public static string CreateForm(List<SqlDataType> content, string tablename)
        {
            StringBuilder sb = new StringBuilder();
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            int sayac = 0;
            sb.Append("<form id='defineForm'  method='post' action='" + url.Action("Insert", "Define", new { tablename = tablename }) + "'>");
            foreach (var item in content)
            {
                sb.Append("<div class=\"form-group\">");
                if (item.DisplayName != "None")
                {
                    if (item.IsList != "0")
                    {
                        if (item.Name.EndsWith("ImageID"))
                        {
                            sb.Append("</br><img id='tempimage' data-id='-1'  src='http://www.placehold.it/200x150/EFEFEF/AAAAAA&text=no+image'  /></br>");
                            sb.Append(String.Format("<input type='number' id='hiddenimage'  min='0' class='form-control {2}' name={0} placeholder = '{1}' style='display:none;'/>", item.Name + "/" + item.Type.Split('_')[0], item.DisplayName, item.Unique));
                            sb.Append(String.Format("</br><button type='button' name={0}  class='btn btn-default' id='uploadImage' style='float:right;'>" + LanguageHelper.GetLangFromServer("ResimYükle") + "</button></br>", item.Name));
                        }

                        else
                        {
                            sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                            sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                            sb.Append(String.Format("<select name='{0}' class='EmptyCheck form-control'>", item.Name + "/" + item.Type.Split('_')[0]));
                            if (item.Name != "Ord_CustomerID")
                            {
                                sb.Append(String.Format("<option value='{0}'>{1}</option>", 0, "Seçiniz..."));
                            }
                            using (SqlConnection con = new SqlConnection(connectionString))
                            {
                                con.Open();
                                using (SqlCommand command = new SqlCommand("SysGenerateCombo", con))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.Add(new SqlParameter("@tableName", tablename));
                                    command.Parameters.Add(new SqlParameter("@colname", item.Name));
                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            string str1 = "";
                                            string str2 = " - ";
                                            str1 = reader.GetValue(1).ToString();
                                            try
                                            {
                                                str2 = str2 + reader.GetValue(2).ToString();
                                            }
                                            catch (Exception)
                                            {
                                                str2 = "";

                                            }

                                            sb.Append(String.Format($"<option value={reader.GetValue(0).ToString()} {(item.Name.EndsWith("ColorID") ? "style='background-color:" + reader.GetValue(1).ToString() + "'" : "")}>{reader.GetValue(1).ToString() + str2}</option>"));
                                        }
                                    }
                                }
                            }
                            sb.Append(String.Format("</select> <br />"));
                        }
                    }
                    else
                    {
                        if (item.Type.Split('_')[0] == "int" || item.Type.Split('_')[0] == "int?" || item.Type.Split('_')[0] == "float" || item.Type.Split('_')[0] == "float?" || item.Type.Split('_')[0] == "decimal" || item.Type.Split('_')[0] == "decimal?")
                        {
                            sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                            sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                            sb.Append(String.Format("<input type='number' min='0' class='EmptyCheck form-control {2}' name={0} placeholder = '{1}' />", item.Name + "/" + item.Type.Split('_')[0], item.DisplayName.Replace(' ', '_').Equals(item.Name) ? LanguageHelper.GetLangFromServer(item.Name) : item.DisplayName, item.Unique));
                        }
                        else if (item.Type == "bool" || item.Type == "bool?")
                        {
                            sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                            sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                            sb.Append(String.Format("<select name='{0}' class='EmptyCheck form-control'><option value='1'>Evet</option><option value='0'>Hayır</option>", item.Name + "/" + item.Type.Split('_')[0]));
                        }
                        else if ((item.Type.Split('_')[0] == "string" || item.Type.Split('_')[0] == "string?") && !item.Name.EndsWith("Color"))
                        {
                            if (item.Name != "Ord_Code")
                            {
                                sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                                sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                                sb.Append(String.Format("<input type='text'  class='EmptyCheck form-control {3}' name={0} placeholder = '{1}' maxlength='{2}'/>", item.Name + "/" + item.Type.Split('_')[0], item.DisplayName.Replace(' ', '_').Equals(item.Name) ? LanguageHelper.GetLangFromServer(item.Name) : item.DisplayName, item.Type.Split('_')[1], item.Unique));
                            }
                            else
                            {
                                sb.Append(String.Format("<input type='text' style='display:none'  class='EmptyCheck form-control {3}' name={0} placeholder = '{1}' maxlength='{2}'/>", item.Name + "/" + item.Type.Split('_')[0], item.DisplayName.Replace(' ', '_').Equals(item.Name) ? LanguageHelper.GetLangFromServer(item.Name) : item.DisplayName, item.Type.Split('_')[1], item.Unique));
                            }
                        }
                        else if (item.Name.EndsWith("Color"))
                        {
                            // <input type="text" id="wheel-demo" class="form-control demo" data-control="wheel" value="#000000">
                            sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                            sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                            sb.Append(String.Format("<input  type='text' class='EmptyCheck form-control demo {3}' name='{0}' data-control='wheel'  id='wheel-demo'/>", item.Name + "/" + item.Type.Split('_')[0], item.DisplayName.Replace(' ', '_').Equals(item.Name) ? LanguageHelper.GetLangFromServer(item.Name) : item.DisplayName, item.Type.Split('_')[1], item.Unique));

                        }
                        else if ((item.Type.Split('_')[0] == "TimeSpan" || item.Type.Split('_')[0] == "TimeSpan?"))//&& !item.Name.EndsWith("Color")
                        {
                            sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                            sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                            sb.Append(String.Format("<input type='text'  class='EmptyCheck form-control {3}' name='{0}' placeholder = '{1}' maxlength='{2}'/>", item.Name + "/" + "string", item.DisplayName.Replace(' ', '_').Equals(item.Name) ? LanguageHelper.GetLangFromServer(item.Name) : item.DisplayName, "string", item.Unique));
                        }
                        else if ((item.Type.Split('_')[0] == "DateTime" || item.Type.Split('_')[0] == "DateTime?"))
                        {
                            sayac++;
                            sb.Append(String.Format("<label for='{0}'><input type='checkbox' class='icheck enable'></label>", item.Name));
                            sb.Append(String.Format("<label for='{0}'>{1}</label>", item.Name, LanguageHelper.GetLangFromServer(item.Name)));
                            sb.Append(String.Format("<div class='form-group'>" +
                                "<div id='datetime" + sayac.ToString() + "'" + "class='input-group datetimepicker date'  data-target-input='nearest'  onclick='CalendarClick($(this))'>" +
                                "<input type='text' class='EmptyCheck form-control {3}  datetimepicker-input'  name='{0}' placeholder = '{1}' maxlength='{2}' data-target='#datetime" + sayac.ToString() + "'/>" +
                                " <div class=\"input-group-append\" data-target='#datetime" + sayac.ToString() + "' data-toggle=\"datetimepicker\">" +
                                "<div class=\"input-group-text\"><i class=\"fa fa-calendar\"></i></div>" +
                                "</div>" +
                                "</div>" +
                                "</div>", item.Name + "/" + "datetime", item.DisplayName.Replace(' ', '_').Equals(item.Name) ? LanguageHelper.GetLangFromServer(item.Name) : item.DisplayName, "datetime", item.Unique));
                        }
                    }
                }
                sb.Append("</div>");
            }
            sb.Append("<div class=\"form-group\"><input type='button' id='insert' value='" + LanguageHelper.GetLangFromServer("Kaydet") + "' class='btn btn-primary btn-insert'/></div>");
            sb.Append("</form>");
            return sb.ToString();
        }

        [LogElapsedTime]
        public static string CreateMenu(int userId)
        {
            StringBuilder builder = new StringBuilder();
            List<Menu> menus = GetMenu(userId);
            foreach (var menu in menus.Where(x => x.HasChild).OrderBy(x => x.Order))
            {
                if (menu.UpperId == 0)
                {
                    builder.Append(String.Format("<li class=\"nav-item\">" +
                        "<a href='javascript:;' class='nav-link'>" +
                        "<i class='{1} nav-icon'></i>" +
                        "<p class='title'   title='{2}' search='{3}'>{0}</p>" +
                        "<i class='right fas fa-angle-left'></i></a><ul class='nav nav-treeview'>",
                        LanguageHelper.GetLangFromServer(Regex.Replace(menu.Title, @"\s+", "")),
                        menu.Icon,
                        LanguageHelper.GetLangFromServer(Regex.Replace(menu.Description, @"\s+", "")),
                        menu.Search));
                    CreateChildMenu(menus, menu.Id, builder);
                }
            }
            return builder.ToString();
        }

        public static StringBuilder CreateChildMenu(List<Menu> menus, int parentMenuId, StringBuilder builder)
        {
            UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
            foreach (var item in menus.Where(menu => menu.UpperId == parentMenuId).OrderBy(x => x.Order))
            {
                if (item.HasChild)
                {
                    builder.Append(String.Format("<li  class=\"nav-item\">" +
                        "<a href='javascript:;' class=\"nav-link\">" +
                        "<i class='{1} nav-icon'></i>" +
                        "<p class='title'  title='{2}' search='{3}'>{0}</p>" +
                        "<i class='right fas fa-angle-left'></i>" +
                        "</a>" +
                        "<ul class='nav nav-treeview'>",//sub-menu 
                        LanguageHelper.GetLangFromServer(Regex.Replace(item.Title, @"\s+", "")),
                        item.Icon,
                        LanguageHelper.GetLangFromServer(Regex.Replace(item.Description, @"\s+", "")),
                        item.Search));
                    CreateChildMenu(menus, item.Id, builder);
                }
                else
                {
                    if (item.Parameter == "-")
                    {
                        builder.Append(String.Format("<li  class=\"nav-item\">" +
                            "<a href='{0}' class=\"nav-link\"  target='{4}'>" +
                            "<i class='{2} nav-icon'></i><p  title='{3}'  search='{5}' >{1}</p>" +
                            "</a>" +
                            "</li>",
                            url.Action(item.Action, item.Controller),
                            LanguageHelper.GetLangFromServer(Regex.Replace(item.Title, @"\s+", "")),
                            item.Icon,
                            LanguageHelper.GetLangFromServer(Regex.Replace(item.Description, @"\s+", "")),
                            item.IsNewTab == true ? "_blank" : "_self",
                            item.Search));
                    }
                    else if (item.IsReport == true)
                    {
                        builder.Append(String.Format("<li  class=\"nav-item\">" +
                            "<a href='{0}' class=\"nav-link\"  target='{4}'>" +
                            "<i class='{2} nav-icon'></i><p  title='{3}' search='{5}' >{1}</p>" +
                            "</a>" +
                            "</li>",
                            url.Action(item.Action, item.Controller, new { reportname = item.Parameter }),
                            LanguageHelper.GetLangFromServer(Regex.Replace(item.Title, @"\s+", "")),
                            item.Icon,
                            LanguageHelper.GetLangFromServer(Regex.Replace(item.Description, @"\s+", "")),
                            item.IsNewTab == true ? "_blank" : "_self",
                            item.Search));
                    }
                    else
                    {
                        builder.Append(String.Format("<li  class=\"nav-item\">" +
                            "<a href='{0}' class=\"nav-link\"  target='{4}'>" +
                            "<i class='{2} nav-icon'></i><p   title='{3}' search='{5}'>{1}</p>" +
                            "</a>" +
                            "</li>",
                            url.Action(item.Action, item.Controller, new { tablename = item.Parameter }),
                            LanguageHelper.GetLangFromServer(Regex.Replace(item.Title, @"\s+", "")),
                            item.Icon,
                            LanguageHelper.GetLangFromServer(Regex.Replace(item.Description, @"\s+", "")),
                            item.IsNewTab == true ? "_blank" : "_self",
                            item.Search));
                    }
                }
            }
            builder.Append(String.Format("</ul></li>"));

            return builder;
        }
    }
}