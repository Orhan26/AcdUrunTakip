using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Modul;
using ACD_Urun_Takip.Repository.Program;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.License
{
    public class LicenceRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<ModuleInformation> Modul(int prg)
        {
            List<ModuleInformation> moduller = new List<ModuleInformation>();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "select modulıd,moduladi from moduller where @id=programıd and deleted=0";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.Text;
            komut.Parameters.AddWithValue("@id", prg);
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                ModuleInformation b = new ModuleInformation();
                b.ModuleId = int.Parse(rdr[0].ToString());
                b.ModuleName = rdr["moduladi"].ToString();
                moduller.Add(b);
            }
            conn.Close();
            return moduller;
        }
        public static List<LicenceInfo> AllLicenses()
        {
            List<LicenceInfo> doldur = new List<LicenceInfo>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "LicenceProcess";
            komut.Connection = conn;
            komut.Parameters.AddWithValue("@processId", 0);
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                LicenceInfo lisans = new LicenceInfo();
                lisans.LicenceId = int.Parse(rdr[0].ToString());
                lisans.LicenceKey = rdr[4].ToString();

                lisans.Module=ModulRepository.GetModul(int.Parse(rdr[3].ToString()));
                lisans.Program=ProgramRepository.GetProgram(int.Parse(rdr[2].ToString()));
                lisans.Company= CompanyRepository.GetCompany(int.Parse(rdr[1].ToString()));
                doldur.Add(lisans);
            }
            conn.Close();
            return doldur;
        }
        public static string AddLicense(LicenceInfo newLicence)
        {
            try
            {
           SqlCommand komut = new SqlCommand();
            komut.CommandText = "LicenceProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 1);
            komut.Parameters.AddWithValue("@companyId", newLicence.Company.CompanyId);
            komut.Parameters.AddWithValue("@programId", newLicence.Program.ProgramId);
            komut.Parameters.AddWithValue("@licenceKey", newLicence.LicenceKey);
            komut.Parameters.AddWithValue("@moduleId", newLicence.Module.ModuleId);
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
        public static string DeleteLicense(int[] licenceId)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < licenceId.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "LicenceProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 4);
                    komut.Parameters.AddWithValue("@licenceId", licenceId[i]);
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
        public static LicenceInfo GetLicense(int licenceId)
        {
            LicenceInfo lisans = new LicenceInfo();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "LicenceProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 2);
            komut.Parameters.AddWithValue("@licenceId", licenceId);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                lisans.LicenceKey = rdr[4].ToString();
                lisans.Module = ModulRepository.GetModul(int.Parse(rdr[3].ToString()));
                lisans.Program = ProgramRepository.GetProgram(int.Parse(rdr[2].ToString()));
                lisans.Company = CompanyRepository.GetCompany(int.Parse(rdr[1].ToString()));
            }
            lisans.LicenceId = licenceId;
            conn.Close();
            return lisans;
        }
        public static string UpdateLicanse(LicenceInfo guncel)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "LicenceProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 3);
                komut.Parameters.AddWithValue("@licenceId", guncel.LicenceId);
                komut.Parameters.AddWithValue("@companyId", guncel.Company.CompanyId);
                komut.Parameters.AddWithValue("@programId", guncel.Program.ProgramId);
                komut.Parameters.AddWithValue("@moduleId", guncel.Module.ModuleId);
                komut.Parameters.AddWithValue("@licenceKey", guncel.LicenceKey);
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
    }
}