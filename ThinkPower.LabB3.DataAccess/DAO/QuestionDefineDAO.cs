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
    /// 問卷題目資料存取類別
    /// </summary>
    public class QuestionDefineDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM QuestionDefine", connection);
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
        /// 取得問卷題目資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷題目資料集合</returns>
        public IEnumerable<QuestionDefineDO> GetQuestionDefineList(Guid uid)
        {
            List<QuestionDefineDO> questionDefineList = null;

            if (uid == Guid.Empty)
            {
                throw new ArgumentNullException("uid");
            }

            string query = @"
SELECT 
    [Uid],[QuestUid],[QuestionId],[QuestionContent],[NeedAnswer],
    [AllowNaCondition],[AnswerType],[MinMultipleAnswers],[MaxMultipleAnswers],
    [SingleAnswerCondition],[CountScoreType],[Memo],[OrderSn],[CreateUserId],
    [CreateTime],[ModifyUserId],[ModifyTime]
FROM QuestionDefine 
WHERE QuestUid = @QuestUid 
ORDER BY OrderSn ASC";

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

                if (dt.Rows.Count > 0)
                {
                    questionDefineList = new List<QuestionDefineDO>();

                    QuestionDefineDO questionDefineDO = null;

                    foreach (DataRow dr in dt.Rows)
                    {
                        questionDefineDO = null;
                        questionDefineDO = ConvertQuestionDefineDO(dr);
                        questionDefineList.Add(questionDefineDO);
                    }
                }

                adapter = null;
                dt = null;
                command = null;

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return questionDefineList;
        }

        /// <summary>
        /// 轉換問卷題目資料
        /// </summary>
        /// <param name="questionDefine">問卷題目資料</param>
        /// <returns>問卷題目資料</returns>
        private QuestionDefineDO ConvertQuestionDefineDO(DataRow questionDefine)
        {
            return new QuestionDefineDO()
            {
                Uid = questionDefine.Field<Guid>("Uid"),
                QuestUid = questionDefine.Field<Guid>("QuestUid"),
                QuestionId = questionDefine.Field<string>("QuestionId"),
                QuestionContent = questionDefine.Field<string>("QuestionContent"),
                NeedAnswer = questionDefine.Field<string>("NeedAnswer"),
                AllowNaCondition = questionDefine.Field<string>("AllowNaCondition"),
                AnswerType = questionDefine.Field<string>("AnswerType"),
                MinMultipleAnswers = questionDefine.Field<int?>("MinMultipleAnswers"),
                MaxMultipleAnswers = questionDefine.Field<int?>("MaxMultipleAnswers"),
                SingleAnswerCondition = questionDefine.Field<string>("SingleAnswerCondition"),
                CountScoreType = questionDefine.Field<string>("CountScoreType"),
                Memo = questionDefine.Field<string>("Memo"),
                OrderSn = questionDefine.Field<int?>("OrderSn"),
                CreateUserId = questionDefine.Field<string>("CreateUserId"),
                CreateTime = questionDefine.Field<DateTime?>("CreateTime"),
                ModifyUserId = questionDefine.Field<string>("ModifyUserId"),
                ModifyTime = questionDefine.Field<DateTime?>("ModifyTime"),
            };
        }
    }
}
