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
        public List<QuestionDefineDO> GetQuesyionDefineCollection(string uid)
        {
            List<QuestionDefineDO> questDefines = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                string query = @"
SELECT [Uid] ,[QuestUid] ,[QuestionId] ,[QuestionContent] ,[NeedAnswer] ,
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
                        Value = Guid.TryParse(uid, out Guid tempUid) ?
                            tempUid :
                            throw new InvalidOperationException("Uid is invalid"),
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        throw new InvalidOperationException("dt.Rows is invalid");
                    }

                    questDefines = new List<QuestionDefineDO>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        questDefines.Add(GetQuesyionDefineDO(dr));
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

            return questDefines;
        }

        /// <summary>
        /// 取得問卷題目資料物件類別
        /// </summary>
        /// <param name="dr">資料列</param>
        /// <returns>問卷題目資料物件類別</returns>
        private QuestionDefineDO GetQuesyionDefineDO(DataRow dr)
        {
            return new QuestionDefineDO()
            {
                Uid = dr.Field<Guid>("Uid"),
                QuestUid = dr.Field<Guid>("QuestUid"),
                QuestionId = dr.Field<string>("QuestionId"),
                QuestionContent = dr.Field<string>("QuestionContent"),
                NeedAnswer = dr.Field<string>("NeedAnswer"),
                AllowNaCondition = dr.Field<string>("AllowNaCondition"),
                AnswerType = dr.Field<string>("AnswerType"),
                MinMultipleAnswers = dr.Field<int?>("MinMultipleAnswers"),
                MaxMultipleAnswers = dr.Field<int?>("MaxMultipleAnswers"),
                SingleAnswerCondition = dr.Field<string>("SingleAnswerCondition"),
                CountScoreType = dr.Field<string>("CountScoreType"),
                Memo = dr.Field<string>("Memo"),
                OrderSn = dr.Field<int?>("OrderSn"),
                CreateUserId = dr.Field<string>("CreateUserId"),
                CreateTime = dr.Field<DateTime?>("CreateTime"),
                ModifyUserId = dr.Field<string>("ModifyUserId"),
                ModifyTime = dr.Field<DateTime?>("ModifyTime"),
            };
        }
    }
}
