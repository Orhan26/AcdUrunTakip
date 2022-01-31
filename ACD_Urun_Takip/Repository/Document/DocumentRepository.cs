using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Mime;
using System.Web.UI.WebControls;
using ACD_Urun_Takip.Repository.Company;

namespace ACD_Urun_Takip.Repository.Document
{
    public class DocumentRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<DocumentInfo> GetDocumentInformation()
        {
            List<DocumentInfo> result = new List<DocumentInfo>();
            try
            {
                List<DocumentInfo> dosyalar = new List<DocumentInfo>();
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "DocumentProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 0);
                conn.Open();
                SqlDataReader rdr = komut.ExecuteReader();
                while (rdr.Read())
                {
                    DocumentInfo d = new DocumentInfo();
                    d.DocumentId = int.Parse(rdr[0].ToString());
                    d.DocumentName = rdr[1].ToString();
                    d.DocumentPath = rdr[2].ToString();
                    d.Company = CompanyRepository.GetCompany(int.Parse(rdr[3].ToString()));
                    DateTime dt1 = new DateTime();
                    dt1 = DateTime.Parse(rdr[4].ToString());
                    d.Date = dt1;
                    d.Version = rdr["VersionNo"].ToString();
                    if (d.DocumentName.EndsWith("txt") || d.DocumentName.EndsWith("jpeg") || d.DocumentName.EndsWith("pdf") || d.DocumentName.EndsWith("jpg") || d.DocumentName.EndsWith("png"))
                    {
                        d.IsPreview = true;
                    }
                    else
                    {
                        d.IsPreview = false;
                    }
                    dosyalar.Add(d);
                }
                conn.Close();
                result = dosyalar;
            }
            catch (Exception ex)
            {
                conn.Close();
                
            }
            return result;
        }
        public static void AddDocument(int id, string fileName, string path,string version)
        {
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "DocumentProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 2);
            komut.Parameters.AddWithValue("@companyId", id);
            komut.Parameters.AddWithValue("@fileName", fileName);
            komut.Parameters.AddWithValue("@path", path);
            komut.Parameters.AddWithValue("@version", version);
            komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
            conn.Open();
            komut.ExecuteNonQuery();
            conn.Close();
        }
        public static void UpdateDocument(int documentId, string documentName, string path, string version)
        {
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "DocumentProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 4);
            komut.Parameters.AddWithValue("@documentId", documentId);
            komut.Parameters.AddWithValue("@documentName", documentName);
            komut.Parameters.AddWithValue("@path", path);
            komut.Parameters.AddWithValue("@version", version);
            komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
            conn.Open();
            komut.ExecuteNonQuery();
            conn.Close();
        }
        public static string DocumentDelete(int[] id)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < id.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "DocumentProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 1);
                    komut.Parameters.AddWithValue("@documentId", id[i]);
                    komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                    komut.ExecuteNonQuery();
                }
                conn.Close();
                return "Seçili kayıtlar silindi...";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
                throw;
            }

        }
        public static DocumentInfo DosyaGetir(int id)
        {
            DocumentInfo dokuman = new DocumentInfo();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "DocumentProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 3);
            komut.Parameters.AddWithValue("@documentId", id);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                dokuman.DocumentName = rdr["DokumanAdi"].ToString();
                dokuman.DocumentPath = rdr["DokumanYolu"].ToString();
                dokuman.Company = CompanyRepository.GetCompany(int.Parse(rdr["SirketID"].ToString()));
                DateTime dt1 = new DateTime();
                dt1 = DateTime.Parse(rdr["Tarih"].ToString());
                dokuman.Date = dt1;
                dokuman.Version = rdr["VersionNo"].ToString();
            }
            dokuman.DocumentId = id;
            conn.Close();
            return dokuman;
        }



    }
}
        
        
    

