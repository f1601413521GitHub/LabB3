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
        /// <returns>資料筆數</returns>
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
        /// 取得問卷答案定義資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷答案定義資料</returns>
        public IEnumerable<QuestionAnswerDefineDO> GetQuestionAnswerDefineList(Guid uid)
        {
            List<QuestionAnswerDefineDO> answerDefineDOList = null;

            if (uid == Guid.Empty)
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
ORDER BY OrderSn ASC";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestionUid",
                        SqlDbType.UniqueIdentifier)
                        {
                            Value = uid,
                        });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        answerDefineDOList = new List<QuestionAnswerDefineDO>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            answerDefineDOList.Add(ConvertQuestionAnswerDefine(dr));
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
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return answerDefineDOList;
        }

        /// <summary>
        /// 轉換問卷答案定義資料為問卷答案定義資料物件
        /// </summary>
        /// <param name="questionAnswerDefine">問卷答案定義資料</param>
        /// <returns>問卷答案定義資料物件</returns>
        private QuestionAnswerDefineDO ConvertQuestionAnswerDefine(
            DataRow questionAnswerDefine)
        {
            return new QuestionAnswerDefineDO()
            {
                Uid = questionAnswerDefine.Field<Guid>("Uid"),
                QuestionUid = questionAnswerDefine.Field<Guid>("QuestionUid"),
                AnswerCode = questionAnswerDefine.Field<string>("AnswerCode"),
                AnswerContent = questionAnswerDefine.Field<string>("AnswerContent"),
                Memo = questionAnswerDefine.Field<string>("Memo"),
                HaveOtherAnswer = questionAnswerDefine.Field<string>("HaveOtherAnswer"),
                NeedOtherAnswer = questionAnswerDefine.Field<string>("NeedOtherAnswer"),
                Score = questionAnswerDefine.Field<int?>("Score"),
                OrderSn = questionAnswerDefine.Field<int?>("OrderSn"),
                CreateUserId = questionAnswerDefine.Field<string>("CreateUserId"),
                CreateTime = questionAnswerDefine.Field<DateTime?>("CreateTime"),
                ModifyUserId = questionAnswerDefine.Field<string>("ModifyUserId"),
                ModifyTime = questionAnswerDefine.Field<DateTime?>("ModifyTime")
            };
        }
    }
}
