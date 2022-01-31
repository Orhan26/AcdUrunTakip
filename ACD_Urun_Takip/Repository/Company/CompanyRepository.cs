using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Company
{
    public class CompanyRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<CompanyInformation> Companys()
        {
            List<CompanyInformation> doldur = new List<CompanyInformation>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "CompanyProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 0);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                CompanyInformation companyInformation = new CompanyInformation();
                companyInformation.CompanyId = int.Parse(rdr[0].ToString());
                companyInformation.CompanyName = rdr[1].ToString();
                companyInformation.Mail = rdr[2].ToString();
                companyInformation.Phone = rdr[3].ToString();
                companyInformation.Address = rdr[4].ToString();
                companyInformation.Authorized = rdr[5].ToString();
                companyInformation.AuthorizedPhone = rdr[6].ToString();
                companyInformation.AuthorizedMail = rdr[7].ToString();
                doldur.Add(companyInformation);
            }
            conn.Close();
            return doldur;
        }
        public static string AddCompany(CompanyInformation Add)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "CompanyProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 1);
                komut.Parameters.AddWithValue("@companyName", Add.CompanyName);
                komut.Parameters.AddWithValue("@mail", Add.Mail);
                komut.Parameters.AddWithValue("@phone", Add.Phone);
                komut.Parameters.AddWithValue("@address", Add.Address);
                komut.Parameters.AddWithValue("@authorized", Add.Authorized);
                komut.Parameters.AddWithValue("@authorizedPhone", Add.AuthorizedPhone);
                komut.Parameters.AddWithValue("@authorizedMail", Add.AuthorizedMail);
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
        public static string DeleteCompany(int[] id)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < id.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "CompanyProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 4);
                    komut.Parameters.AddWithValue("@companyId", id[i]);
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
        public static CompanyInformation GetCompany (int id)
        {
            try
            {
                CompanyInformation companyInformation = new CompanyInformation();
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "CompanyProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 2);
                komut.Parameters.AddWithValue("@companyId", id);
                conn.Open();
                SqlDataReader rdr = komut.ExecuteReader();
                while (rdr.Read())
                {
                    companyInformation.CompanyName = rdr["SirketAdi"].ToString();
                    companyInformation.Mail = rdr["Mail"].ToString();
                    companyInformation.Phone = rdr["Telefon"].ToString();
                    companyInformation.Address = rdr["Adres"].ToString();
                    companyInformation.Authorized = rdr["YetkiliKisi"].ToString();
                    companyInformation.AuthorizedPhone = rdr["YetkiliTel"].ToString();
                    companyInformation.AuthorizedMail = rdr["YetkiliMail"].ToString();
                }
                companyInformation.CompanyId = id;
                conn.Close();
                return companyInformation;
            }
            catch (Exception)
            {
                conn.Close();
                throw;
            }
            
        }
        public static string UpdateCompany(CompanyInformation guncel)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "CompanyProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 3);
                komut.Parameters.AddWithValue("@companyId", guncel.CompanyId);
                komut.Parameters.AddWithValue("@companyName", guncel.CompanyName);
                komut.Parameters.AddWithValue("@mail", guncel.Mail);
                komut.Parameters.AddWithValue("@phone", guncel.Phone);
                komut.Parameters.AddWithValue("@address", guncel.Address);
                komut.Parameters.AddWithValue("@authorized", guncel.Authorized);
                komut.Parameters.AddWithValue("@authorizedPhone", guncel.AuthorizedPhone);
                komut.Parameters.AddWithValue("@authorizedMail", guncel.AuthorizedMail);
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