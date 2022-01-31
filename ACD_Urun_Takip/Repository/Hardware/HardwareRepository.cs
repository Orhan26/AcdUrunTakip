using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Hardware
{
    public class HardwareRepository
    {

        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);

        public static List<HardwareInfo> AllHardWares()
        {
            List<HardwareInfo> donanim = new List<HardwareInfo>();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "HardwareProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 0);

            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                HardwareInfo d = new HardwareInfo();
                d.HardwareId = int.Parse(rdr[0].ToString());
                d.HardwareName = rdr[1].ToString();
                d.HardwareType = rdr[2].ToString();
                d.DocumentName = rdr[3].ToString();
                if(d.DocumentName=="")
                {
                    d.iconHide = true;
                }
                else
                {
                    d.iconHide = false;
                }
                donanim.Add(d);
            }
            conn.Close();
            return donanim;
        }
        public static string AddHardware(HardwareInfo kayit)
        {
            try
            {
                conn.Open();
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "HardwareProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 1);
                komut.Parameters.AddWithValue("@hardwareName", kayit.HardwareName);
                komut.Parameters.AddWithValue("@hardwareType", kayit.HardwareType);
                komut.Parameters.AddWithValue("@documentName", kayit.DocumentName);
                komut.Parameters.AddWithValue("@documentPath", kayit.DocumentPath);
                komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                komut.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception hata)
            {
                conn.Close();
                return hata.ToString();
            }
            return "Başarıyla kaydedildi";
        }
        public static string DeleteHardware(int[] id)
        {
            conn.Open();
            for (int i = 0; i < id.Length; i++)
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "HardwareProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processId", 4);
                komut.Parameters.AddWithValue("@hardwareId", id[i]);
                komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                komut.ExecuteNonQuery();
            }
            conn.Close();
            return "Seçili kayıtlar silindi";
        }
        public static HardwareInfo GetHardware(int id)
        {
            HardwareInfo donanim = new HardwareInfo();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "HardwareProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 2);
            komut.Parameters.AddWithValue("@hardwareId", id);
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                donanim.HardwareName = rdr[1].ToString();
                donanim.HardwareType = rdr[2].ToString();
                donanim.DocumentName = rdr[3].ToString();
                donanim.DocumentPath = rdr[4].ToString();
            }
            donanim.HardwareId = id;
            conn.Close();
            return donanim;
        }
        public static string UpdateHardware(HardwareInfo guncel)
        {
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "HardwareProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 3);
            komut.Parameters.AddWithValue("@hardwareId", guncel.HardwareId);
            komut.Parameters.AddWithValue("@hardwareName", guncel.HardwareName);
            komut.Parameters.AddWithValue("@hardwareType", guncel.HardwareType);
            komut.Parameters.AddWithValue("@documentName", guncel.DocumentName);
            komut.Parameters.AddWithValue("@documentPath", guncel.DocumentPath);
            komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
            komut.ExecuteNonQuery();
            conn.Close();
            return "Düzenleme kaydedildi";
        }
    }
}
