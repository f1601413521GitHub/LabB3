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
        /// <returns></returns>
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
        /// <returns>問卷題目資料</returns>
        public IEnumerable<QuestionDefineDO> GetQuestionDefineList(Guid uid)
        {
            List<QuestionDefineDO> questDefineList = null;

            if (uid == Guid.Empty)
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                string query = @"
SELECT 
    [Uid] ,[QuestUid] ,[QuestionId] ,[QuestionContent] ,[NeedAnswer] ,
    [AllowNaCondition] ,[AnswerType] ,[MinMultipleAnswers] ,[MaxMultipleAnswers] ,
    [SingleAnswerCondition] ,[CountScoreType] ,[Memo] ,[OrderSn] ,[CreateUserId] ,
    [CreateTime] ,[ModifyUserId] ,[ModifyTime]
FROM QuestionDefine 
WHERE QuestUid = @QuestUid 
ORDER BY OrderSn ASC;";

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
                        questDefineList = new List<QuestionDefineDO>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            questDefineList.Add(GetQuestionDefineDO(dr));
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
            }
            catch (Exception e)
            {
                logger.Error(e);
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questDefineList;
        }

        /// <summary>
        /// 轉換問卷題目資料成為問卷題目資料物件
        /// </summary>
        /// <param name="questDefine">問卷題目資料</param>
        /// <returns>問卷題目資料物件</returns>
        private QuestionDefineDO GetQuestionDefineDO(DataRow questDefine)
        {
            return new QuestionDefineDO()
            {
                Uid = questDefine.Field<Guid>("Uid"),
                QuestUid = questDefine.Field<Guid>("QuestUid"),
                QuestionId = questDefine.Field<string>("QuestionId"),
                QuestionContent = questDefine.Field<string>("QuestionContent"),
                NeedAnswer = questDefine.Field<string>("NeedAnswer"),
                AllowNaCondition = questDefine.Field<string>("AllowNaCondition"),
                AnswerType = questDefine.Field<string>("AnswerType"),
                MinMultipleAnswers = questDefine.Field<int?>("MinMultipleAnswers"),
                MaxMultipleAnswers = questDefine.Field<int?>("MaxMultipleAnswers"),
                SingleAnswerCondition = questDefine.Field<string>("SingleAnswerCondition"),
                CountScoreType = questDefine.Field<string>("CountScoreType"),
                Memo = questDefine.Field<string>("Memo"),
                OrderSn = questDefine.Field<int?>("OrderSn"),
                CreateUserId = questDefine.Field<string>("CreateUserId"),
                CreateTime = questDefine.Field<DateTime?>("CreateTime"),
                ModifyUserId = questDefine.Field<string>("ModifyUserId"),
                ModifyTime = questDefine.Field<DateTime?>("ModifyTime"),
            };
        }
    }
}
