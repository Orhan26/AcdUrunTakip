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
        public static SqlConnection baglanti = new SqlConnection(SqlConn.connectionString);
        public static List<DokumanBilgi> GetDocumentInformation()
        {
            List<DokumanBilgi> result = new List<DokumanBilgi>();
            try
            {
                List<DokumanBilgi> dosyalar = new List<DokumanBilgi>();
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "DokumanIslem";
                komut.Connection = baglanti;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@islemid", 0);
                baglanti.Open();
                SqlDataReader rdr = komut.ExecuteReader();
                while (rdr.Read())
                {
                    DokumanBilgi d = new DokumanBilgi();
                    d.DokumanID = int.Parse(rdr[0].ToString());
                    d.DokumanAdi = rdr[1].ToString();
                    d.DokumanYolu = rdr[2].ToString();
                    d.SirketID = CompanyRepository.GetCompany(int.Parse(rdr[3].ToString()));
                    DateTime dt1 = new DateTime();
                    dt1 = DateTime.Parse(rdr[4].ToString());
                    d.Tarih = dt1;
                    d.Version = rdr["VersionNo"].ToString();
                    if (d.DokumanAdi.EndsWith("txt") || d.DokumanAdi.EndsWith("jpeg") || d.DokumanAdi.EndsWith("pdf") || d.DokumanAdi.EndsWith("jpg") || d.DokumanAdi.EndsWith("png"))
                    {
                        d.IsPreview = true;
                    }
                    else
                    {
                        d.IsPreview = false;
                    }
                    dosyalar.Add(d);
                }
                baglanti.Close();
                result = dosyalar;
            }
            catch (Exception ex)
            {
                baglanti.Close();
                throw;
            }
            return result;
        }
        public static void DokumanEkle(int id, string fileName, string path, DateTime Tarih, string Version)
        {
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "DokumanIslem";
            komut.Connection = baglanti;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@islemid", 2);
            komut.Parameters.AddWithValue("@Sirketid", id);
            komut.Parameters.AddWithValue("@DosyaAdi", fileName);
            komut.Parameters.AddWithValue("@Uzanti", path);
            komut.Parameters.AddWithValue("@Tarih", Tarih);
            komut.Parameters.AddWithValue("@Version", Version);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        public static void DokumanGuncelle(int dokumanid, string adi, string yolu, int sirketid, DateTime Tarih, string Version)
        {
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "DokumanIslem";
            komut.Connection = baglanti;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@islemid", 4);
            komut.Parameters.AddWithValue("@Dokumanid", dokumanid);
            komut.Parameters.AddWithValue("@DokumanAdi", adi);
            komut.Parameters.AddWithValue("@Uzanti", yolu);
            komut.Parameters.AddWithValue("@Tarih", Tarih);
            komut.Parameters.AddWithValue("@Version", Version);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        public static string DocumentDelete(int[] id)
        {
            try
            {
                baglanti.Open();
                for (int i = 0; i < id.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "DokumanIslem";
                    komut.Connection = baglanti;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@islemid", 1);
                    komut.Parameters.AddWithValue("@Dokumanid", id[i]);
                    komut.ExecuteNonQuery();
                }
                baglanti.Close();
                return "Seçili kayıtlar silindi...";
            }
            catch (Exception ex)
            {
                baglanti.Close();
                return ex.Message;
                throw;
            }

        }
        public static DokumanBilgi DosyaGetir(int id)
        {
            DokumanBilgi dokuman = new DokumanBilgi();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "DokumanIslem";
            komut.Connection = baglanti;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@islemid", 3);
            komut.Parameters.AddWithValue("@Dokumanid", id);
            baglanti.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                dokuman.DokumanAdi = rdr["DokumanAdi"].ToString();
                dokuman.DokumanYolu = rdr["DokumanYolu"].ToString();
                dokuman.SirketID = CompanyRepository.GetCompany(int.Parse(rdr["SirketID"].ToString()));
                DateTime dt1 = new DateTime();
                dt1 = DateTime.Parse(rdr["Tarih"].ToString());
                dokuman.Tarih = dt1;
                dokuman.Version = rdr["VersionNo"].ToString();
            }
            dokuman.DokumanID = id;
            baglanti.Close();
            return dokuman;
        }



    }
}
        
        
    

