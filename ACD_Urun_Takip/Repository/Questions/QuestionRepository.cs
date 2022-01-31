using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ACD_Urun_Takip.Models;
using ACD_Urun_Takip.Repository.Group;

namespace ACD_Urun_Takip.Repository.Questions
{
    public class QuestionRepository
    {
        public static SqlConnection conn = new SqlConnection(SqlConn.connectionString);
        public static List<QuestionInformation> AllQuestion()
        {
            List<QuestionInformation> doldur = new List<QuestionInformation>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "TnmQuestionProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processid", 0);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                QuestionInformation questionInformation = new QuestionInformation();
                questionInformation.QuestionID = int.Parse(rdr[0].ToString());
                questionInformation.Question = rdr[1].ToString();
                doldur.Add(questionInformation);
            }
            conn.Close();
            return doldur;
        }
        public static QuestionInformation GetQuestions(int id)
        {
            QuestionInformation S = new QuestionInformation();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "TnmQuestionProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processid", 1);
            komut.Parameters.AddWithValue("@QuestionID", id);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                S.Question = rdr[1].ToString();
                S.isDeleted = rdr["Deleted"].ToString() == "True" ? "disabled" : "";
            }
            S.QuestionID = id;
            conn.Close();
            return S;
        }
        public static string AddQuestion(string quest)
        {
            try
            {
                SqlCommand komut = new SqlCommand();
                komut.CommandText = "TnmQuestionProcess";
                komut.Connection = conn;
                komut.CommandType = System.Data.CommandType.StoredProcedure;
                komut.Parameters.AddWithValue("@processid", 2);
                komut.Parameters.AddWithValue("@question",quest);
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
        public static List<QuestionInformation> GetQuestion(int groupid)
        {
            List<QuestionInformation> questionInformations = new List<QuestionInformation>();
            SqlCommand komut = new SqlCommand();
            komut.CommandText = "TnmQuestionProcess";
            komut.Connection = conn;
            komut.CommandType = System.Data.CommandType.StoredProcedure;
            komut.Parameters.AddWithValue("@processid", 3);
            komut.Parameters.AddWithValue("@qroupID", groupid);
            conn.Open();
            SqlDataReader rdr = komut.ExecuteReader();
            while (rdr.Read())
            {
                QuestionInformation S = new QuestionInformation();
                S.QuestionID = int.Parse(rdr[0].ToString());
                S.Question = rdr[1].ToString();
                S.Group = GroupRepository.GetInformation(int.Parse(rdr[2].ToString()));
                questionInformations.Add(S);
            }
            conn.Close();
            return questionInformations;
        }


    }
}