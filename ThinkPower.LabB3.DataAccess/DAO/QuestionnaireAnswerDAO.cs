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
    /// 問卷答題資料存取類別
    /// </summary>
    public class QuestionnaireAnswerDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM QuestionnaireAnswer", connection);
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
        /// 取得問卷答題資料
        /// </summary>
        /// <param name="uid">問卷主檔紀錄識別碼</param>
        /// <returns>問卷答題資料</returns>
        public QuestionnaireAnswerDO GetQuestionnaireAnswer(Guid uid)
        {
            QuestionnaireAnswerDO questAnswerDO = null;

            if (uid == Guid.Empty)
            {
                throw new ArgumentNullException("questAnswerId");
            }

            try
            {
                string query = @"
SELECT TOP 1
    [Uid],[QuestUid],[QuestAnswerId],[TesteeId],
    [QuestScore],[ActualScore],[TesteeSource],[CreateUserId],
    [CreateTime],[ModifyUserId],[ModifyTime]
FROM QuestionnaireAnswer
WHERE QuestUid =@QuestUid";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestUid", SqlDbType.UniqueIdentifier)
                    {
                        Value = uid,
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        questAnswerDO = ConvertQuestionnaireAnswer(dt.Rows[0]);
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

            return questAnswerDO;
        }

        /// <summary>
        /// 轉換問卷答題資料成為問卷答題資料物件
        /// </summary>
        /// <param name="questAnswer">問卷答題資料</param>
        /// <returns>問卷答題資料物件</returns>
        private QuestionnaireAnswerDO ConvertQuestionnaireAnswer(DataRow questAnswer)
        {
            return new QuestionnaireAnswerDO()
            {
                Uid = questAnswer.Field<Guid>("Uid"),
                QuestUid = questAnswer.Field<Guid>("QuestUid"),
                QuestAnswerId = questAnswer.Field<string>("QuestAnswerId"),
                TesteeId = questAnswer.Field<string>("TesteeId"),
                QuestScore = questAnswer.Field<int?>("QuestScore"),
                ActualScore = questAnswer.Field<int?>("ActualScore"),
                TesteeSource = questAnswer.Field<string>("TesteeSource"),
                CreateUserId = questAnswer.Field<string>("CreateUserId"),
                CreateTime = questAnswer.Field<DateTime?>("CreateTime"),
                ModifyUserId = questAnswer.Field<string>("ModifyUserId"),
                ModifyTime = questAnswer.Field<DateTime?>("ModifyTime"),
            };
        }

        /// <summary>
        /// 新增問卷填答結果至問卷答題主檔
        /// </summary>
        /// <param name="questAnswerDO">問卷填答資料</param>
        /// <returns></returns>
        public void Insert(QuestionnaireAnswerDO questAnswerDO)
        {
            if (questAnswerDO == null)
            {
                throw new ArgumentNullException("questAnswerDO");
            }

            try
            {
                string query = @"
INSERT INTO [QuestionnaireAnswer]
    ([Uid], [QuestUid], [QuestAnswerId], [TesteeId], [QuestScore], [ActualScore], [TesteeSource],
    [CreateUserId], [CreateTime], [ModifyUserId], [ModifyTime]) 
VALUES (@Uid, @QuestUid, @QuestAnswerId, @TesteeId, @QuestScore, @ActualScore, @TesteeSource,
    @CreateUserId, @CreateTime, @ModifyUserId, @ModifyTime);";

                using (SqlConnection connection = DbConnection)
                {

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@Uid", SqlDbType.VarChar) { Value = questAnswerDO.Uid.ToString() });
                    command.Parameters.Add(new SqlParameter("@QuestUid", SqlDbType.VarChar) { Value = questAnswerDO.QuestUid.ToString() });
                    command.Parameters.Add(new SqlParameter("@QuestAnswerId", SqlDbType.VarChar) { Value = questAnswerDO.QuestAnswerId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@TesteeId", SqlDbType.VarChar) { Value = questAnswerDO.TesteeId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@QuestScore", SqlDbType.Int) { Value = questAnswerDO.QuestScore ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ActualScore", SqlDbType.Int) { Value = questAnswerDO.ActualScore ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@TesteeSource", SqlDbType.VarChar) { Value = questAnswerDO.TesteeSource ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CreateUserId", SqlDbType.VarChar) { Value = questAnswerDO.CreateUserId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = questAnswerDO.CreateTime ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ModifyUserId", SqlDbType.VarChar) { Value = questAnswerDO.ModifyUserId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ModifyTime", SqlDbType.DateTime) { Value = questAnswerDO.ModifyTime ?? Convert.DBNull });

                    connection.Open();
                    command.ExecuteNonQuery();

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
        }
    }
}
