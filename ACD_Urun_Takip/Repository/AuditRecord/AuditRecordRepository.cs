using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Group;
using ACD_Urun_Takip.Repository.Questions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.AuditRecord
{
    public class AuditRecordRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static string AddAuditRecord(int[] groupId,int companyId,int controlId)
        {
            int sonuc;
            string msj;
            try
            {
                for (int i = 0; i < groupId.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "HrkCompanyGroup";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 2);
                    komut.Parameters.AddWithValue("@groupId", groupId[i]);
                    komut.Parameters.AddWithValue("@companyId", companyId);
                    komut.Parameters.AddWithValue("@controlId", controlId);
                    komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                    conn.Open();
                    sonuc = int.Parse(komut.ExecuteScalar().ToString());
                    conn.Close();
                    if (sonuc == 1)
                    {
                        return msj = "Bu kayıt daha önce eklendi";
                    }
                }
                return msj = "Başarıyla kaydedildi";
            }
            catch (Exception ex)
            {
                conn.Close();
                return msj = ex.Message;
                throw;
            }
            finally
            {
                if (controlId == 1)
                {
                    
                    for (int i = 0; i < groupId.Length; i++)
                    {
                        List<QuestionInformation> questionInformations = new List<QuestionInformation>();
                        conn.Open();
                        SqlCommand km = new SqlCommand();
                        km.CommandText = "select*from TnmSoru where SR_GrupID=@groupid and Deleted=0";
                        km.Connection = conn;
                        km.CommandType = System.Data.CommandType.Text;
                        km.Parameters.AddWithValue("@groupid", groupId[i]);
                        SqlDataReader rdr = km.ExecuteReader();
                        while (rdr.Read())
                        {
                            QuestionInformation questionInformation = new QuestionInformation();
                            questionInformation.QuestionID = int.Parse(rdr[0].ToString());
                            questionInformation.Group = GroupRepository.GetInformation(int.Parse(rdr[2].ToString()));
                            questionInformations.Add(questionInformation);
                        }
                        for (int j = 0; j < questionInformations.Count; j++)
                        {
                            SqlCommand kmt = new SqlCommand();
                            kmt.CommandText = "ControlStatusProcess";
                            kmt.Connection = conn;
                            kmt.CommandType = System.Data.CommandType.StoredProcedure;
                            kmt.Parameters.AddWithValue("@processId", 4);
                            kmt.Parameters.AddWithValue("@companyId", companyId);
                            kmt.Parameters.AddWithValue("@groupId", questionInformations[j].Group.GroupID);
                            kmt.Parameters.AddWithValue("@questionId", questionInformations[j].QuestionID);
                            kmt.Parameters.AddWithValue("@reply", 0);
                            kmt.Parameters.AddWithValue("@explanation", "");
                            kmt.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                            kmt.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                }
            }
   
            }
        }
    }
