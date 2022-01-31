using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Group;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.CompanyGroup
{
    public class CompanyGroupRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<CompanyGroupInfo> CompanyGroups()
        {
            List<CompanyGroupInfo> groups = new List<CompanyGroupInfo>();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "select*from HrkSirketGrup ";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.Text;
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                CompanyGroupInfo companyGroup = new CompanyGroupInfo();
                companyGroup.HsgID = int.Parse(rdr[0].ToString());
                companyGroup.GroupID = GroupRepository.GetInformation(int.Parse(rdr[1].ToString()));
                companyGroup.CompanyID = CompanyRepository.GetCompany(int.Parse(rdr[2].ToString()));
                companyGroup.StartStop = bool.Parse(rdr[3].ToString());
                groups.Add(companyGroup);
            }
            conn.Close();
            return groups;
        }
        public static List<CompanyGroupInfo> CompanyGroupsFalse()
        {
            List<CompanyGroupInfo> groups = new List<CompanyGroupInfo>();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "select*from HrkSirketGrup where StartStopID=0";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.Text;
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                CompanyGroupInfo companyGroup = new CompanyGroupInfo();
                companyGroup.HsgID = int.Parse(rdr[0].ToString());
                companyGroup.GroupID = GroupRepository.GetInformation(int.Parse(rdr[1].ToString()));
                companyGroup.CompanyID = CompanyRepository.GetCompany(int.Parse(rdr[2].ToString()));
                companyGroup.StartStop = bool.Parse(rdr[3].ToString());
                groups.Add(companyGroup);
            }
            conn.Close();
            return groups;
        }
        public static string DeleteRecord(int[] groupId,int companyId)
        {          
            try
            {
                conn.Open();
                for (int i = 0; i < groupId.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "HrkCompanyGroup";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 1);
                    komut.Parameters.AddWithValue("@groupId", groupId[i]);
                    komut.Parameters.AddWithValue("@companyId", companyId);
                    komut.ExecuteNonQuery();
                }
                conn.Close();
                return "Şirket Grup Kaydı silindi";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
        }
    }
}