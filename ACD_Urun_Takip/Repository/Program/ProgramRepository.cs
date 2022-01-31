using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Program
{
    public class ProgramRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<ProgramInfo> AllPrograms()
        {
            List<ProgramInfo> doldur = new List<ProgramInfo>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "ProgramProcess";
            komut.Connection = conn;
            komut.Parameters.AddWithValue("@processId", 0);
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                ProgramInfo program = new ProgramInfo();
                program.ProgramId = int.Parse(rdr[0].ToString());
                program.ProgramName = rdr[1].ToString();
                doldur.Add(program);
            }
            conn.Close();
            return doldur;
        }
        public static string AddProgram(ProgramInfo newProgram)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "ProgramProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 1);
                komut.Parameters.AddWithValue("@programName", newProgram.ProgramName);
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
        public static string DeleteProgram(int[] ProgramId)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < ProgramId.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "ProgramProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 4);
                    komut.Parameters.AddWithValue("@programId", ProgramId[i]);
                    komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                    komut.ExecuteNonQuery();
                }
                conn.Close();
                return "Seçili kayıtlar silindi";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
                throw;
            }
           
        }
        public static string UpdateProgram(ProgramInfo update)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "ProgramProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 3);
                komut.Parameters.AddWithValue("@programId", update.ProgramId);
                komut.Parameters.AddWithValue("@programName", update.ProgramName);
                komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                conn.Open();
                komut.ExecuteNonQuery();
                conn.Close();
                return "Düzenleme kaydedildi";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
                throw;
            }
            
        }
        public static ProgramInfo GetProgram(int id)
        {
            ProgramInfo program = new ProgramInfo();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "ProgramProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 2);
            komut.Parameters.AddWithValue("@programId", id);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                program.ProgramName = rdr["ProgramAdi"].ToString();
            }
            program.ProgramId = id;
            conn.Close();
            return program;
        }
    }
}