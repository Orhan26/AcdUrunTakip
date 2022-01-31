using ACD_Urun_Takip.Controllers;
using ACD_Urun_Takip.Helper;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Models.Result;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ACD_Urun_Takip.Repository.Define
{
    public class DefineRepository
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        private static readonly string lang = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
        public static DefineTable GetDefineTbl(string tableName, int userId)
        {
            DefineTable define = new DefineTable(tableName);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select TOP 1 Mnu_Title,Mnu_Search from SysMenu where Mnu_Parameter='" + tableName + "'", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            define.Caption = LanguageHelper.GetLangFromServer(Regex.Replace(reader.GetValue(0).ToString(), @"\s+", ""));
                            define.Menu = (string)reader.GetValue(1);
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand("Select Deleted from SysBookMark where Bkm_UserID=" + userId + " AND Bkm_MenuNumber='" + define.Menu + "'", connection))
                {
                    define.IsBookMark = (bool?)command.ExecuteScalar();
                }
            }
            define.IsBookMark = (define.IsBookMark == null || define.IsBookMark == true) ? true : false;
            return define;
        }

        public async static Task<DefineTable> GetDefineTblBody(string tablename)
        {
            DefineTable defineTable = new DefineTable(tablename);
            DataTable dt = new DataTable();
            var tableColumn = new List<dynamic>();

            Task<DataTable> tbl = DbOperation.GetDataTableAsync("SysSelectGenerate", new SqlParameter[] { new SqlParameter("@tablename", tablename), new SqlParameter("@action", "LIST") });

            (string menu, string caption) data = GetMenuCaption(tablename);
            defineTable.Menu = data.menu;
            defineTable.Caption = data.caption;

            await Task.WhenAll(tbl);
            dt = tbl.Result;

            foreach (var item in dt.AsEnumerable().Skip(0).Take(50))
            {
                IDictionary<string, object> dn = new ExpandoObject();
                foreach (var column in dt.Columns.Cast<DataColumn>())
                {
                    dn[column.ColumnName] = item[column];
                }
                tableColumn.Add(dn);
            }
            if (tablename.Length > 0)
                defineTable.Body = tableColumn;
            return defineTable;
        }

        public static byte[] GetImage(int id)
        {
            byte[] image = null;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("GetImage", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            image = (byte[])reader.GetValue(0);
                        }
                    }
                }
            }
            return image;
        }

        public static List<SqlDataType> GetRowTypes(string tablename, string GuID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            List<SqlDataType> typeList = new List<SqlDataType>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysGetTableType", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@tableName", tablename));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetValue(0).ToString() != "")
                            {
                                string data = (string)reader.GetValue(0);
                                string type = data.Split(' ')[0];
                                string name = data.Split(' ')[1];
                                string display = data.Split(' ')[2];
                                string isunique = data.Split(' ')[3];
                                string islist = data.Split(' ')[4];
                                typeList.Add(new SqlDataType { Name = name, Type = type, DisplayName = display.Replace('_', ' '), Unique = isunique, IsList = islist });
                            }
                        }
                    }
                }
            }
            return typeList;
        }

        public static (string, string) GetMenuCaption(string tableName)
        {
            string caption = "", menu = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select TOP 1 Mnu_Title,Mnu_Search from SysMenu where Mnu_Parameter='" + tableName + "'", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            caption = LanguageHelper.GetLangFromServer(Regex.Replace(reader.GetValue(0).ToString(), @"\s+", ""));
                            menu = (string)reader.GetValue(1);
                        }
                    }
                }
            }

            return (menu, caption);
        }

        public static AjaxResult UpdateDefineTbl(FormCollection fm, string tblName, int userId)
        {
            string Message = "";
            bool Error = false;
            string tableName = tblName.Split('_')[0];
            Guid Guid = new Guid(tblName.Split('_')[1]);
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                foreach (var key in fm.AllKeys)
                {
                    using (SqlCommand command = new SqlCommand("SysDefinitionPopulate", con))
                    {
                        string datetimeType = key.Split('/')[1];
                        string ordCode = key.Split('/')[0];

                        if (datetimeType == "datetime")
                        {
                            if (fm[key] != "")
                            {
                                DateTime value = Convert.ToDateTime(fm[key]);
                                string setValue = value.ToString("yyyy-MM-dd HH':'mm':'ss");
                                command.Parameters.AddWithValue("@value", setValue);
                            }
                            else
                            {
                                goto Jump;
                            }
                        }
                        //TODO IPM5 Geçirilmesi durumu için kaldırılmadı
                        else if (ordCode == "Ord_Code")
                        {
                            command.Parameters.AddWithValue("@value", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@value", fm[key]);
                        }

                        if (key.Split('/')[0] == "Wrs_PeriodPcs" || key.Split('/')[1] == "Wrs_PeriodPcs")
                        {
                            ;
                        }

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@key", key.Split('/')[0]);
                        command.Parameters.AddWithValue("@type", key.Split('/')[1]);
                        command.Parameters.AddWithValue("@guid", Guid);
                        command.Parameters.AddWithValue("@len", fm[key].Length);
                        command.Parameters.AddWithValue("@tablename", tableName);
                        command.ExecuteScalar();

                    Jump:;
                    }
                }
                using (SqlCommand command = new SqlCommand("SysDefinitionGenerate", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "UPDATE");
                    command.Parameters.AddWithValue("@Guid", Guid);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@tablename", tableName);
                    string lang = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                    command.Parameters.AddWithValue("@lang", lang);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Message = (string)reader.GetValue(0);
                            Error = (bool)reader.GetValue(1);
                        }
                    }
                }
            }
            return new AjaxResult { Success = Error, Message = Message, Guid = Guid };
        }

        public static AjaxResult UpdateSelectedRow(FormCollection fm, int userId)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            DefineMultiRowModel multiRow = new DefineMultiRowModel();
            dynamic output = serializer.DeserializeObject(fm["rest"]);
            multiRow.Ids = output["ids"];
            multiRow.TableName = output["tbl"];
            Guid guid = Guid.Empty;
            string Message = String.Empty;
            bool Error = false;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    foreach (var item in multiRow.Ids.ToList())
                    {
                        guid = new Guid(item.ToString());
                        foreach (var key in fm.AllKeys)
                        {
                            if (key != "rest")
                            {
                                using (SqlCommand command = new SqlCommand("SysDefinitionPopulate", con))
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Parameters.AddWithValue("@key", key.Split('/')[0]);
                                    command.Parameters.AddWithValue("@value", fm[key]);
                                    command.Parameters.AddWithValue("@type", key.Split('/')[1]);
                                    command.Parameters.AddWithValue("@guid", guid);
                                    command.Parameters.AddWithValue("@len", fm[key].Length);
                                    command.Parameters.AddWithValue("@tablename", multiRow.TableName.ToString());
                                    command.ExecuteScalar();
                                }
                            }
                        }
                        using (SqlCommand command = new SqlCommand("SysDefinitionGenerate", con))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Action", "UPDATE");
                            command.Parameters.AddWithValue("@Guid", guid);
                            command.Parameters.AddWithValue("@UserID", userId);
                            command.Parameters.AddWithValue("@tablename", multiRow.TableName.ToString());
                            string lang = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                            command.Parameters.AddWithValue("@lang", lang);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Message = (string)reader.GetValue(0);
                                    Error = (bool)reader.GetValue(1);
                                }
                            }
                        }
                    }
                }
                return new AjaxResult { Success = Error, Message = Message };
            }
            catch (NullReferenceException ex)
            {
                return new AjaxResult { Success = false, Message = "Kayıt Sırasında Hata Oluştu. Error" + ex.Message };
            }
        }

        public static AjaxResult DeleteAllDefineTbl(DefineDeleteModel model, int userId)
        {
            string Message = String.Empty;
            bool Error = false;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    foreach (var item in model.Ids)
                    {
                        using (SqlCommand command = new SqlCommand("SysDefinitionGenerate", con))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Action", "DELETE");
                            command.Parameters.AddWithValue("@Guid", item);
                            command.Parameters.AddWithValue("@UserID", userId);
                            command.Parameters.AddWithValue("@tablename", model.TableName);
                            command.Parameters.AddWithValue("@lang", lang);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Message = (string)reader.GetValue(0);
                                    Error = (bool)reader.GetValue(1);
                                }
                            }
                        }
                    }
                }
                return new AjaxResult { Success = Error, Message = Message };
            }
            catch (NullReferenceException ex)
            {
                return new AjaxResult { Success = false, Message = "Kayıt Sırasında Hata Oluştu. Error:" + ex.Message };
            }

        }

        public static JQueryDataTable GetDefineTblData(JQueryDataTableModel model)
        {
            JQueryDataTable table = new JQueryDataTable();

            int total = 0;
            string ordercol = model.orders[0].Split(',')[0];
            string order = model.orders[0].Split(',')[1];
            string colnumber = model.iColumns.ToString();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysSelectGenerate", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tablename", model.tablename);
                    command.Parameters.AddWithValue("@action", "SEARCH");
                    command.Parameters.AddWithValue("@search", model.sSearch);
                    command.Parameters.AddWithValue("@ordercol", ordercol);
                    command.Parameters.AddWithValue("@order", order);
                    command.Parameters.AddWithValue("@page", model.iDisplayStart);
                    command.Parameters.AddWithValue("@display", model.iDisplayLength);
                    command.Parameters.AddWithValue("@innersearch", model.columnsearch);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                        reader.NextResult();
                        while (reader.Read())
                        {
                            total = (int)reader.GetValue(0);
                        }
                    }
                }
            }
            if (model.iDisplayLength == -1)
            {
                model.iDisplayLength = total;
            }
            var result = JustBody(model.tablename, model.iDisplayStart, model.iDisplayLength, model.sSearch, ordercol, order, colnumber, model.columnsearch);
            // note: we only sort one column at a time
            List<string> list = new List<string>();
            list.Add(result);
            table = new JQueryDataTable();
            table.draw = Convert.ToInt32(model.sEcho);
            table.recordsTotal = total;
            //int recordsFiltered = total;
            table.data = list;
            table.recordsFiltered = total;

            return table;
        }

        /// <summary>
        /// String olarak HTML birleştirilir ve sayfa içerisindeki tabloda listelenen HTML kodu oluşturulur.
        /// </summary>
        /// <param name="tablename">Tablo Adı</param>
        /// <param name="start">Başlangıç</param>
        /// <param name="length">Kayıt Sayısı</param>
        /// <param name="search">Yapılan arama sözcüğü</param>
        /// <param name="ordercol">Herhangi bir sütuna göre sıralama var mı onu alır</param>
        /// <param name="order"></param>
        /// <param name="colnumber"></param>
        /// <param name="innersearch"></param>
        /// <returns></returns>
        public static string JustBody(string tablename, int start, int length, string search, string ordercol, string order, string colnumber, string innersearch)
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysSelectGenerate", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@tablename", tablename);
                    command.Parameters.AddWithValue("@action", "SEARCH");
                    command.Parameters.AddWithValue("@search", search);
                    command.Parameters.AddWithValue("@ordercol", ordercol);
                    command.Parameters.AddWithValue("@order", order);
                    command.Parameters.AddWithValue("@page", start);
                    command.Parameters.AddWithValue("@display", length);
                    command.Parameters.AddWithValue("@innersearch", innersearch);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(dt);
                    }
                }
            }
            var dns = new List<dynamic>();
            foreach (var item in dt.AsEnumerable())
            {
                IDictionary<string, object> dn = new ExpandoObject();
                foreach (var column in dt.Columns.Cast<DataColumn>())
                {
                    dn[column.ColumnName] = item[column];
                }
                dns.Add(dn);
            }
            StringBuilder sb = new StringBuilder();
            if (dns.Count == 0)
            {
                sb.Append(String.Format("<tr><td colspan='{0}'>{1}</td></tr>", colnumber, LanguageHelper.GetLangFromServer("zeroRecords")));
            }
            else
            {
                foreach (dynamic item in dns)
                {
                    //TODO:CheckedChanged method kaldırılmalı UI //?method bulunamadı
                    //TODO:imageModal a-image ile değişti
                    sb.Append("<tr><td><div class=\"icheck-primary d-inline\"><input type='checkbox' onclick='CheckedChanged($(this))' class='icheck'><label></label></div></td>");
                    foreach (var prop in item as IDictionary<string, object>)
                    {
                        if (prop.Key.EndsWith("_ImageID"))
                        {
                            if (prop.Value.ToString() != "" && Convert.ToInt32(prop.Value) == 0)
                            {
                                sb.Append("<td>###</td>");
                            }
                            else
                            {
                                sb.Append(String.Format("<td><a href='../Define/GetImage/{0}' target = '_blank' class='btn default btn-xs blue a-image' data-id={1} ><i class='fa fa-image' ></i>RESİM</a></td>", prop.Value, prop.Value));
                            }
                        }
                        else if (prop.Key.EndsWith("Color") || prop.Key.Contains("Clr_ID"))
                        {
                            sb.Append(String.Format("<td {0}>{1}</td>", "style='background-color:" + prop.Value + "'", prop.Value));
                        }
                        else
                        {
                            sb.Append(String.Format("<td {0} >{1}</td>", prop.Key.EndsWith("GuID") ? "style='display:none'" : "", prop.Value));
                        }
                    }
                }
            }
            return sb.ToString();
        }

        public static AjaxResult SaveDefineTbl(FormCollection fm, string tablename, int userId)
        {
            bool Error = false;
            string Message = String.Empty;
            Guid Guid = Guid.Empty;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysNewGuid", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Guid = (Guid)reader.GetValue(0);
                        }
                    }
                }

                foreach (var key in fm.AllKeys)
                {

                    using (SqlCommand command = new SqlCommand("SysDefinitionPopulate", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        string datetim = key.Split('/')[1];
                        string ordCode = key.Split('/')[0];

                        if (datetim == "datetime")
                        {
                            DateTime value = Convert.ToDateTime(fm[key]);
                            string setValue = value.ToString("yyyy-MM-dd HH':'mm':'ss");
                            command.Parameters.AddWithValue("@value", setValue);
                        }
                        else if (ordCode == "Ord_Code")
                        {
                            command.Parameters.AddWithValue("@value", "1");
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@value", fm[key]);
                        }

                        command.Parameters.AddWithValue("@key", key.Split('/')[0]);
                        command.Parameters.AddWithValue("@type", key.Split('/')[1]);
                        command.Parameters.AddWithValue("@guid", Guid);
                        command.Parameters.AddWithValue("@len", fm[key].Length);
                        command.Parameters.AddWithValue("@tablename", tablename);
                        command.ExecuteScalar();
                    }
                }
                using (SqlCommand command = new SqlCommand("SysDefinitionGenerate", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Action", "INSERT");
                    command.Parameters.AddWithValue("@Guid", Guid);
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@tablename", tablename);
                    string lang = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                    command.Parameters.AddWithValue("@lang", lang);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!string.IsNullOrEmpty(reader.GetName(0)))
                            {
                                Message = (string)reader.GetValue(0);
                                Error = (bool)reader.GetValue(1);
                            }
                        }
                    }
                }
            }
            return new AjaxResult { Message = Message, Guid = Guid, Success = Error };
        }

        public static List<dynamic> GetRow(DefineRowModel model, int userId)
        {
            DataTable table = new DataTable();
            var row = new List<dynamic>();
            string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SysDefinitionGenerate", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@Action", "GETROW"));
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@guid", model.GID);
                    command.Parameters.AddWithValue("@tablename", model.TableName);
                    string lang = Thread.CurrentThread.CurrentUICulture.ToString().Split('-')[0];
                    command.Parameters.AddWithValue("@lang", lang);
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(table);
                    }
                }
            }
            foreach (var item in table.AsEnumerable())
            {
                IDictionary<string, object> dn = new ExpandoObject();
                foreach (var column in table.Columns.Cast<DataColumn>())
                {
                    dn[column.ColumnName] = item[column];
                }
                row.Add(dn);
            }
            return row;
        }

        public static AjaxResult UploadImage(HttpPostedFileBase upload)
        {
            int lastId = 0;
            if (upload != null && upload.ContentLength > 0)
            {
                try
                {
                    byte[] data = ReduceSize(upload.InputStream, 400, 400);
                    string mime = upload.ContentType;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand command = new SqlCommand("SysSaveImage", con))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@image", data);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    lastId = Convert.ToInt32(reader.GetValue(0));
                                }
                            }
                        }
                    }
                    return new AjaxResult { Code = lastId };
                }
                catch (Exception ex)
                {
                    return new AjaxResult { Success = false, Message = "Yükleme Sırasında Bir Hata oluştu. Error :" + ex.Message };
                }
            }
            else
                return new AjaxResult { Success = false, Message = "Lütfen Dosya Yükleyin" };


        }

        public static byte[] ReduceSize(Stream stream, int maxWidth, int maxHeight)
        {
            System.Drawing.Image source = System.Drawing.Image.FromStream(stream);
            double widthRatio = ((double)maxWidth) / source.Width;
            double heightRatio = ((double)maxHeight) / source.Height;
            double ratio = (widthRatio < heightRatio) ? widthRatio : heightRatio;
            System.Drawing.Image thumbnail = source.GetThumbnailImage((int)(source.Width * ratio), (int)(source.Height * ratio), null, IntPtr.Zero);
            using (var memory = new MemoryStream())
            {
                thumbnail.Save(memory, source.RawFormat);
                return memory.ToArray();
            }
        }
    }
}