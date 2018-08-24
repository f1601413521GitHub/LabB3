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
    /// 問卷答案項目資料存取類別
    /// </summary>
    public class QuestionAnswerDefineDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM QuestionAnswerDefine", connection);
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
        /// 取得問卷答案項目
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷答案項目</returns>
        public List<QuestionAnswerDefineDO> GetQuestionAnswerDefineCollection(string uid)
        {
            List<QuestionAnswerDefineDO> questAnsDefines = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                string query = @"
SELECT [Uid],[QuestionUid],[AnswerCode],[AnswerContent],[Memo],
    [HaveOtherAnswer],[NeedOtherAnswer],[Score],[OrderSn],[CreateUserId],
    [CreateTime],[ModifyUserId],[ModifyTime] 
FROM QuestionAnswerDefine 
WHERE QuestionUid = @QuestionUid 
ORDER BY OrderSn ASC;";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestionUid", SqlDbType.UniqueIdentifier)
                    {
                        Value = Guid.TryParse(uid, out Guid tempQuestionUid) ?
                            tempQuestionUid :
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

                    questAnsDefines = new List<QuestionAnswerDefineDO>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        questAnsDefines.Add(GetQuestionAnswerDefineDO(dr));
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

            return questAnsDefines;
        }

        /// <summary>
        /// 取得 問卷答案項目資料物件類別
        /// </summary>
        /// <param name="dr">資料列</param>
        /// <returns>問卷答案項目資料物件類別</returns>
        private QuestionAnswerDefineDO GetQuestionAnswerDefineDO(DataRow dr)
        {
            return new QuestionAnswerDefineDO()
            {
                Uid = dr.Field<Guid>("Uid"),
                QuestionUid = dr.Field<Guid>("QuestionUid"),
                AnswerCode = dr.Field<string>("AnswerCode"),
                AnswerContent = dr.Field<string>("AnswerContent"),
                Memo = dr.Field<string>("Memo"),
                HaveOtherAnswer = dr.Field<string>("HaveOtherAnswer"),
                NeedOtherAnswer = dr.Field<string>("NeedOtherAnswer"),
                Score = dr.Field<int?>("Score"),
                OrderSn = dr.Field<int?>("OrderSn"),
                CreateUserId = dr.Field<string>("CreateUserId"),
                CreateTime = dr.Field<DateTime?>("CreateTime"),
                ModifyUserId = dr.Field<string>("ModifyUserId"),
                ModifyTime = dr.Field<DateTime?>("ModifyTime")
            };
        }
    }
}
