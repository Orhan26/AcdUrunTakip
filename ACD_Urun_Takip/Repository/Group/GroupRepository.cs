using ACD_Urun_Takip.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.Group
{
    
    public class GroupRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<GroupInformation> AllGroups()
        {
            List<GroupInformation> groups = new List<GroupInformation>();
            conn.Open();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "select*from TnmSoruGrup where Deleted=0";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.Text;
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                GroupInformation group = new GroupInformation();
                group.GroupID = int.Parse(rdr[0].ToString());
                group.GroupName = rdr[1].ToString();
                groups.Add(group);
            }
            conn.Close();
            return groups;
        }
        public static GroupInformation GetInformation(int id)
        {
            try
            {
                GroupInformation group = new GroupInformation();
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "select*from TnmSoruGrup where SG_ID=@Groupid and Deleted=0";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.Text;
                komut.Parameters.AddWithValue("@Groupid", id);
                conn.Open();
                SqlDataReader rdr = komut.ExecuteReader();
                while (rdr.Read())
                {
                    group.GroupID = int.Parse(rdr[0].ToString());
                    group.GroupName = rdr[1].ToString();
                }
                conn.Close();
                return group;
            }
            catch (Exception)
            {
                conn.Close();
                throw;
            }
           
        }
    
    }
}
