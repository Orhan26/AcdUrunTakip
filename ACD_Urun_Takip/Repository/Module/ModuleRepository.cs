using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Program;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Modul
{
    public class ModulRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<ModuleInformation> Modules()
        {
            List<ModuleInformation> Moduller = new List<ModuleInformation>();
            SqlCommand command = new SqlCommand();
            command.CommandText = "ModuleProcess";
            command.Connection = conn;
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@processId", 0);
            conn.Open();
            SqlDataReader rdr = command.ExecuteReader();
            while (rdr.Read())
            {
                ModuleInformation moduleInfo = new ModuleInformation();
                moduleInfo.ModuleId = int.Parse(rdr[0].ToString());
                moduleInfo.Program = ProgramRepository.GetProgram(int.Parse(rdr[1].ToString()));
                moduleInfo.ModuleName = rdr[2].ToString();
                Moduller.Add(moduleInfo);
            }
            conn.Close();
            return (Moduller);
        }
        public static ModuleInformation ModulGetir(int id)
        {
            ModuleInformation b = new ModuleInformation();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "ModuleProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 1);
            komut.Parameters.AddWithValue("@moduleId", id);
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                b.Program = ProgramRepository.GetProgram(int.Parse(rdr[1].ToString()));
                b.ModuleName = rdr["ModulAdi"].ToString();
            }

            b.ModuleId = id;
            conn.Close();
            return (b);
        }
        public static string AddModule(int programId, string moduleName)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "ModuleProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("processId", 2);
                komut.Parameters.AddWithValue("@moduleName", moduleName);
                komut.Parameters.AddWithValue("@programId", programId);
                komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                conn.Open();
                komut.ExecuteNonQuery();
                conn.Close();
                return "Başarıyla kaydedildi";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
                throw;
            } 
        }
        public static string DeleteModul(int[] moduleId)
        {
            conn.Open();
            for (int i = 0; i < moduleId.Length; i++)
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "ModuleProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 3);
                komut.Parameters.AddWithValue("@moduleId", moduleId[i]);
                komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                komut.ExecuteNonQuery();
            }
            conn.Close();
            return "Seçili kayıtlar silindi";
        }
        public static ModuleInformation GetModul(int moduleId)
        {
            ModuleInformation b = new ModuleInformation();
            conn.Open();
      
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "ModuleProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 1);
                komut.Parameters.AddWithValue("@moduleId", moduleId);
                SqlDataReader rdr = komut.ExecuteReader();
                while (rdr.Read())
                {
                    b.Program = ProgramRepository.GetProgram(int.Parse(rdr[1].ToString()));
                    b.ModuleName = rdr["ModulAdi"].ToString();
                }
            b.ModuleId = moduleId;
            conn.Close();
            return b;
        }
        public static string UpdateModul(int programId, string moduleName, int moduleId)
        {
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "ModuleProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 4);
            komut.Parameters.AddWithValue("@moduleId", moduleId);
            komut.Parameters.AddWithValue("@programId", programId);
            komut.Parameters.AddWithValue("@moduleName", moduleName);
            komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
            komut.ExecuteNonQuery();
            conn.Close();
            return "Düzenleme kaydedildi";
        }
    }
}