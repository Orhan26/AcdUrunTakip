using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Company;
using ACD_Urun_Takip.Repository.Group;
using ACD_Urun_Takip.Repository.Questions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ACD_Urun_Takip.Repository.ControlStatus
{
    public class ControlStatusRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<HrkCompanyAuditInfo> AllControls()
        {
            List<HrkCompanyAuditInfo> doldur = new List<HrkCompanyAuditInfo>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "select*from HRK_SirketDenetim where Deleted=0";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.Text;
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                HrkCompanyAuditInfo k = new HrkCompanyAuditInfo();
                k.DenetimID = int.Parse(rdr[0].ToString());
                k.SirketID = CompanyRepository.GetCompany(int.Parse(rdr[1].ToString()));
                k.GrupID = GroupRepository.GetInformation(int.Parse(rdr[2].ToString()));
                k.SoruID = QuestionRepository.GetQuestions(int.Parse(rdr[3].ToString()));
                k.Cevap = bool.Parse(rdr[4].ToString());
                k.Aciklama = rdr[5].ToString();
                DateTime dt1 = new DateTime();
                dt1 = DateTime.Parse(rdr[6].ToString());
                k.Tarih = dt1;

                doldur.Add(k);
            }
            conn.Close();
            return doldur;
        }
        public static string DeleteControl(int[] id)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < id.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "ControlStatusProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 2);
                    komut.Parameters.AddWithValue("@id", id[i]);
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
        public static List<HrkCompanyAuditInfo> GetControlStatus(int id)
        {
            List<HrkCompanyAuditInfo> hrkSirketDenetimInformations = new List<HrkCompanyAuditInfo>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "ControlStatusProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 3);
            komut.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                HrkCompanyAuditInfo hrkSirketDenetimInformation = new HrkCompanyAuditInfo();
                hrkSirketDenetimInformation.DenetimID = int.Parse(rdr[0].ToString());
                hrkSirketDenetimInformation.SirketID = CompanyRepository.GetCompany(int.Parse(rdr[1].ToString()));
                hrkSirketDenetimInformation.GrupID = GroupRepository.GetInformation(int.Parse(rdr[2].ToString()));
                hrkSirketDenetimInformation.SoruID = QuestionRepository.GetQuestions(int.Parse(rdr[3].ToString()));
                hrkSirketDenetimInformation.Cevap = bool.Parse(rdr[4].ToString());
                hrkSirketDenetimInformation.Aciklama = rdr[5].ToString();
                hrkSirketDenetimInformations.Add(hrkSirketDenetimInformation);
            }
            conn.Close();
            return hrkSirketDenetimInformations;
        }
        public static List<HrkCompanyAuditInfo> RecordControl(int companyId,int grupId)
        {
            List<HrkCompanyAuditInfo> hrkSirketDenetimInformations = new List<HrkCompanyAuditInfo>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "ControlStatusProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processId", 6);
            komut.Parameters.AddWithValue("@companyId", companyId);
            komut.Parameters.AddWithValue("@groupId", grupId);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                HrkCompanyAuditInfo hrkSirketDenetimInformation = new HrkCompanyAuditInfo();
                hrkSirketDenetimInformation.SirketID = CompanyRepository.GetCompany(int.Parse(rdr[0].ToString()));
                hrkSirketDenetimInformation.GrupID = GroupRepository.GetInformation(int.Parse(rdr[1].ToString()));
                hrkSirketDenetimInformation.SoruID = QuestionRepository.GetQuestions(int.Parse(rdr[2].ToString()));
                hrkSirketDenetimInformations.Add(hrkSirketDenetimInformation);
            }
            conn.Close();
            return hrkSirketDenetimInformations;
        }
        public static string AllDelete(int companyId)
        {
            try
            {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "ControlStatusProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 5);
                    komut.Parameters.AddWithValue("@companyId",companyId);
                    komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                    conn.Open();
                    komut.ExecuteNonQuery();
                    conn.Close();
                return "Kayıtlar silindi";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
            }
        }
        public static string AddAuditRecord(int[] groupId,int companyId,int controlId)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < groupId.Length; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "ControlStatusProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 0);
                    komut.Parameters.AddWithValue("@groupId", groupId[i]);
                    komut.Parameters.AddWithValue("@companyId", companyId);
                    komut.Parameters.AddWithValue("@controlId", controlId);
                    komut.ExecuteNonQuery();
                }
                conn.Close();
                return "Kayıt Başarılı";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
                throw;
            }
            finally
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
                            QuestionInformation hrkQuestionGroup = new QuestionInformation();
                            hrkQuestionGroup.QuestionID = int.Parse(rdr[0].ToString());
                            hrkQuestionGroup.Group = GroupRepository.GetInformation(int.Parse(rdr[2].ToString()));
                            questionInformations.Add(hrkQuestionGroup);
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
        public static string UpdateStatus(List<HrkCompanyAuditInfo> hrkSirketDenetims)
        {
            try
            {
                conn.Open();
                for (int i = 0; i < hrkSirketDenetims.Count; i++)
                {
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "ControlStatusProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 1);
                    komut.Parameters.AddWithValue("@id", hrkSirketDenetims[i].DenetimID);
                    komut.Parameters.AddWithValue("@reply",hrkSirketDenetims[i].Cevap);
                    komut.Parameters.AddWithValue("@explanation",hrkSirketDenetims[i].Aciklama);
                    komut.Parameters.AddWithValue("@userId", @HttpContext.Current.User.Identity.Name.Split('_')[1]);
                    komut.ExecuteNonQuery();
                }
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
        public static string AddQuestion(int groupid,int companyId,int[] questionId)
        {
            try
            {
                conn.Open();
                    SqlCommand komut = new SqlCommand();
                    komut.CommandText = "ControlStatusProcess";
                    komut.Connection = conn;
                    komut.CommandType = System.Data.CommandType.StoredProcedure;
                    komut.Parameters.AddWithValue("@processId", 0);
                    komut.Parameters.AddWithValue("@groupId", groupid);
                    komut.Parameters.AddWithValue("@companyId", companyId);
                    komut.Parameters.AddWithValue("@controlId", 1);
                    komut.ExecuteNonQuery();               
                conn.Close();
                return "Kayıt Başarılı";
            }
            catch (Exception ex)
            {
                conn.Close();
                return ex.Message;
                throw;
            }
            finally
            {
                conn.Open();
                for (int j = 0; j < questionId.Length; j++)
                {
                    SqlCommand kmt = new SqlCommand();
                    kmt.CommandText = "ControlStatusProcess";
                    kmt.Connection = conn;
                    kmt.CommandType = System.Data.CommandType.StoredProcedure;
                    kmt.Parameters.AddWithValue("@processId", 4);
                    kmt.Parameters.AddWithValue("@companyId", companyId);
                    kmt.Parameters.AddWithValue("@groupId", groupid);
                    kmt.Parameters.AddWithValue("@questionId", questionId[j]);
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