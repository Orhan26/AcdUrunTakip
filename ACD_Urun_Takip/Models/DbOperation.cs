using ACD_Urun_Takip.Attribute;
using ACD_Urun_Takip.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACD_Urun_Takip.Models
{
    //Bu class içerisinde generic olarak AJAX sorgusu ile db üzerinden işlem yapan methodlar birleştirilmiştir.Bunun amacı view içerisinde yazılan aynı mantığa sahip Ajax sorgularını kısaltmaktır.
    //DB üzerinde kullanılacak olaran Stored Procedure ve buna ait parametreler view üzerinde encrypt edilmiş şekilde tutulur ve direk buraya yollanır.
    //Burada da bu parametreler ve sp nin adı decrpyt edilir ve gerekli işlem yapılır.Methodlar birkaç tipe ayrılmıştır.
    //Sadece işlemi yapanlar ve bunun sonunda oldu olmadı diye sonuç dönenler ve ya işlemi yapıp sonrasında liste dönenler bunun örneklerindendir.
    public class DbOperation
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        public static string ExecuteScalar(FormCollection fm)
        {
            ICrypt aes = new CryptAdapter(new AES());
            string message = String.Empty;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(aes.Decrypt(fm["sproc"]), con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in fm.AllKeys.Where(x => x != "sproc"))
                    {
                        command.Parameters.AddWithValue(key, key == "userid" ? aes.Decrypt(fm[key]) : key == "key" ? "ACDmailKey" : fm[key]);
                    }
                    object result = command.ExecuteScalar();
                    message = (result.GetType() == typeof(DBNull) || Object.ReferenceEquals(null, result)) ? "" : result.ToString();
                }
            }
            return message;
        }
        public static DataTable FillDataTable(FormCollection formCollection, string procedure)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in formCollection.AllKeys)
                    {
                        command.Parameters.AddWithValue(key, formCollection[key]);
                    }
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(table);
                    }
                }
            }
            return table;
        }
        public static DataTable FillDataTableForReportFunctions(FormCollection fm, string procedure)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in fm.AllKeys)
                    {
                        if (fm[key].Contains(','))
                        {
                            string columnnames = "";
                            foreach (string col in fm[key].Split(','))
                            {
                                if (col.Trim() != "")
                                    columnnames += LanguageHelper.GetKeyFromServer(col) + ",";
                            }
                            command.Parameters.AddWithValue(key, columnnames);
                        }
                        else
                            command.Parameters.AddWithValue(key, fm[key]);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }
        [LogElapsedTime]
        public static string FillListFromDataTable(FormCollection fm)
        {
            DataTable dt = new DataTable();
            ICrypt aes = new CryptAdapter(new AES());
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(aes.Decrypt(fm["sproc"]), connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in fm.AllKeys)
                    {
                        if (key != "sproc")
                        {
                            command.Parameters.AddWithValue(key, key == "user" ? aes.Decrypt(fm[key]) : fm[key]);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return JsonConvert.SerializeObject(dt);
        }
        [LogElapsedTime]
        public static string FillListFromDataSet(FormCollection fm)
        {
            DataSet table = new DataSet();
            ICrypt aes = new CryptAdapter(new AES());
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(aes.Decrypt(fm["sproc"]), con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in fm.AllKeys)
                    {
                        if (key != "sproc")
                        {
                            command.Parameters.AddWithValue(key, key == "user" ? aes.Decrypt(fm[key]) : fm[key]);
                        }
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return JsonConvert.SerializeObject(table);
        }
        public static DataSet FillDataSet(FormCollection formCollection, string procedure)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(procedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in formCollection.AllKeys)
                    {
                        command.Parameters.AddWithValue(key, formCollection[key]);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataSet);
                    }
                }
            }
            return dataSet;
        }
        public static DataTable ExecQueryReturnDt(string sql)
        {
            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                }
            }
            return table;
        }
        public static string ExecQueryReturnMessage(string sql)
        {
            string message = String.Empty;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    message = (string)command.ExecuteScalar();
                }
            }
            return message;
        }
        public static void ExecQuery(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                    command.ExecuteScalar();
            }
        }
        public static string ReturnDataSetAsJson(FormCollection formCollection)
        {
            DataSet dataSet = new DataSet();
            ICrypt aes = new CryptAdapter(new AES());
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(aes.Decrypt(formCollection["sproc"]), connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (var key in formCollection.AllKeys)
                    {
                        if (key != "sproc")
                            command.Parameters.AddWithValue(key, formCollection[key]);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataSet);
                    }
                }
            }
            return JsonConvert.SerializeObject(dataSet);
        }


        /// <summary>
        /// ExecuteScalar ASYNC
        /// </summary>
        /// <param name="sSQL"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<object> ExecuteScalarAsync(string sSQL, params SqlParameter[] parameters)
        {
            return Task.Run(() =>
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand(sSQL, connection))
                {
                    command.CommandType = CommandType.Text;
                    if (parameters != null) command.Parameters.AddRange(parameters);

                    connection.Open();
                    return command.ExecuteScalar();
                }
            });
        }

        public static Task<DataTable> GetDataTableAsync(string storedProcedure, params SqlParameter[] parameters)
        {
            return Task.Run(() =>
            {
                using (var connection = new SqlConnection(connectionString))
                using (var adapter = new SqlDataAdapter(storedProcedure, connection))
                {
                    adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        adapter.SelectCommand.Parameters.AddRange(parameters);

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            });
        }
    }
}