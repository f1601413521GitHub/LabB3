using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.DataAccess.DO;

namespace ThinkPower.LabB3.DataAccess.DAO
{
    /// <summary>
    /// 問卷資料存取類別
    /// </summary>
    public class QuestionnaireDAO : BaseDAO
    {
        /// <summary>
        /// 取得資料筆數
        /// </summary>
        /// <returns>資料筆數</returns>
        public override int Count()
        {
            int count;

            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM Questionnaire", connection);
                connection.Open();

                count = (int)command.ExecuteScalar();

                command = null;

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return count;
        }

        /// <summary>
        /// 取得有效的問卷資料
        /// </summary>
        /// <param name="id">問卷編號</param>
        /// <returns>有效的問卷資料</returns>
        public QuestionnaireDO GetActiveQuestionniare(string id)
        {
            QuestionnaireDO questDO = null;

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                string query = @"
SELECT TOP 1 
    [Uid] ,[QuestId] ,[Version] ,[Kind] ,[Name] ,[Memo] ,[Ondate] ,
    [Offdate] ,[NeedScore] ,[QuestScore] ,[ScoreKind] ,[HeadBackgroundImg] ,[HeadDescription] ,
    [FooterDescription] ,[CreateUserId] ,[CreateTime] ,[ModifyUserId] ,[ModifyTime] 
FROM Questionnaire 
WHERE QuestId = @QuestId 
    AND Ondate < @DateTimeNow 
    AND ((Offdate > @DateTimeNow) OR (Offdate is null)) 
ORDER BY Version DESC";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestId", SqlDbType.VarChar) { Value = id });
                    command.Parameters.Add(new SqlParameter("@DateTimeNow", SqlDbType.DateTime)
                    {
                        Value = DateTime.Now.Date,
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        questDO = ConvertQuestionnaireDO(dt.Rows[0]);
                    }

                    adapter = null;
                    dt = null;
                    command = null;

                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questDO;
        }

        /// <summary>
        /// 轉換問卷資料成為問卷資料物件
        /// </summary>
        /// <param name="questionnaire">問卷資料</param>
        /// <returns>問卷資料物件</returns>
        private QuestionnaireDO ConvertQuestionnaireDO(DataRow questionnaire)
        {
            return new QuestionnaireDO()
            {
                Uid = questionnaire.Field<Guid>("Uid"),
                QuestId = questionnaire.Field<string>("QuestId"),
                Version = questionnaire.Field<string>("Version"),
                Kind = questionnaire.Field<string>("Kind"),
                Name = questionnaire.Field<string>("Name"),
                Memo = questionnaire.Field<string>("Memo"),
                Ondate = questionnaire.Field<DateTime?>("Ondate"),
                Offdate = questionnaire.Field<DateTime?>("Offdate"),
                NeedScore = questionnaire.Field<string>("NeedScore"),
                QuestScore = questionnaire.Field<int?>("QuestScore"),
                ScoreKind = questionnaire.Field<string>("ScoreKind"),
                HeadBackgroundImg = questionnaire.Field<string>("HeadBackgroundImg"),
                HeadDescription = questionnaire.Field<string>("HeadDescription"),
                FooterDescription = questionnaire.Field<string>("FooterDescription"),
                CreateUserId = questionnaire.Field<string>("CreateUserId"),
                CreateTime = questionnaire.Field<DateTime?>("CreateTime"),
                ModifyUserId = questionnaire.Field<string>("ModifyUserId"),
                ModifyTime = questionnaire.Field<DateTime?>("ModifyTime"),
            };
        }

        /// <summary>
        /// 取得問卷資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷資料</returns>
        public QuestionnaireDO GetQuestionnaire(string uid)
        {
            QuestionnaireDO questDO = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                string query = @"
SELECT 
    [Uid] ,[QuestId] ,[Version] ,[Kind] ,[Name] ,[Memo] ,[Ondate] ,
    [Offdate] ,[NeedScore] ,[QuestScore] ,[ScoreKind] ,[HeadBackgroundImg] ,[HeadDescription] ,
    [FooterDescription] ,[CreateUserId] ,[CreateTime] ,[ModifyUserId] ,[ModifyTime] 
FROM Questionnaire 
WHERE Uid = @Uid
    AND Ondate < @DateTimeNow 
    AND ((Offdate > @DateTimeNow) OR (Offdate is null)) ";


                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@Uid", SqlDbType.VarChar) { Value = uid });
                    command.Parameters.Add(new SqlParameter("@DateTimeNow", SqlDbType.DateTime)
                    {
                        Value = DateTime.Now.Date,
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        questDO = ConvertQuestionnaireDO(dt.Rows[0]);
                    }

                    adapter = null;
                    dt = null;
                    command = null;

                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questDO;
        }
    }
}
