using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Hardware;
using ACD_Urun_Takip.Repository.Modul;
using ACD_Urun_Takip.Repository.Program;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Satis
{
    public class SalesRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<SalesInfo> AllSales()
        {
            List<SalesInfo> doldur = new List<SalesInfo>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "SalesProcess";
            komut.Connection = conn;
            komut.Parameters.AddWithValue("@processId", 0);
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                SalesInfo satislar = new SalesInfo();
                satislar.SalesId = int.Parse(rdr[0].ToString());
                satislar.Company = CompanyRepository.GetCompany(int.Parse(rdr[1].ToString()));
                satislar.Program = ProgramRepository.GetProgram(int.Parse(rdr[2].ToString()));
                satislar.Module = rdr[3].ToString() != ""? ModulRepository.GetModul(int.Parse(rdr[3].ToString())):null;
                satislar.Hardware = rdr[4].ToString() !=""?HardwareRepository.GetHardware(int.Parse(rdr[4].ToString())):null;
                satislar.HardwareQty = rdr[5].ToString();
                DateTime dt1 = new DateTime();
                satislar.Date = DateTime.Parse(rdr["Tarih"].ToString());
                //satislar.Date = dt1;
                satislar.UserId = rdr["Usr_Code"].ToString();
                doldur.Add(satislar);
            }
            conn.Close();
            return doldur;
        }
        public static string AddSales(SalesInfo newSales)
        {

            try
            {
                int? nullInt = null;
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "SalesProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 1);
                komut.Parameters.AddWithValue("@companyId", newSales.Company.CompanyId);
                komut.Parameters.AddWithValue("@programId", newSales.Program.ProgramId);
                komut.Parameters.AddWithValue("@modulId", newSales.Module.ModuleId == 0 ? nullInt : newSales.Module.ModuleId);
                komut.Parameters.AddWithValue("@hardwareQty", newSales.HardwareQty);
                komut.Parameters.AddWithValue("@hardwareId", newSales.Hardware.HardwareId == 0 ? nullInt : newSales.Hardware.HardwareId);
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
        public static SalesInfo GetSales(int salesId)
        {
            SalesInfo satis = new SalesInfo();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "SalesProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 4);
            komut.Parameters.AddWithValue("@salesId", salesId);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                satis.SalesId = int.Parse(rdr[0].ToString());
                satis.Company = CompanyRepository.GetCompany(int.Parse(rdr[1].ToString()));
                satis.Program = ProgramRepository.GetProgram(int.Parse(rdr[2].ToString()));
                DateTime dt1 = new DateTime();
                dt1 = DateTime.Parse(rdr["Tarih"].ToString());
                satis.Hardware = rdr[4].ToString() == "" ? null: HardwareRepository.GetHardware(int.Parse(rdr[4].ToString()));
                satis.Module = rdr[3].ToString() == "" ? null : ModulRepository.GetModul(int.Parse(rdr[3].ToString()));
                satis.HardwareQty = rdr[5].ToString();
                satis.Date = dt1;
            }
            conn.Close();
            return satis;
        }
        public static string UpdateSales(SalesInfo update)
        {
            int? nullInt = null;
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "SalesProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 2);
            komut.Parameters.AddWithValue("@salesId", update.SalesId);
            komut.Parameters.AddWithValue("@companyId", update.Company.CompanyId);
            komut.Parameters.AddWithValue("@programId", update.Program.ProgramId);
            komut.Parameters.AddWithValue("@modulId", update.Module.ModuleId == 0 ? nullInt : update.Module.ModuleId);
            komut.Parameters.AddWithValue("@hardwareId", update.Hardware.HardwareId == 0 ? nullInt : update.Hardware.HardwareId);
            komut.Parameters.AddWithValue("@hardwareQty", update.HardwareQty);
            komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
            conn.Open();
            komut.ExecuteNonQuery();
            conn.Close();
            return "Düzenleme kaydedildi";
        }
        public static string DeleteSales(int[] salesId)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < salesId.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "SalesProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 5);
                    komut.Parameters.AddWithValue("@salesId", salesId[i]);
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
    }
}