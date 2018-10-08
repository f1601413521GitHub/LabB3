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
        /// <param name="uid">問卷題目識別碼</param>
        /// <returns>問卷答案定義資料集合</returns>
        public IEnumerable<QuestionAnswerDefineDO> GetAnswerDefineList(Guid uid)
        {
            List<QuestionAnswerDefineDO> answerDefineList = null;

            if (uid == Guid.Empty)
            {
                throw new ArgumentNullException("uid");
            }

            string query = @"
SELECT 
    [Uid],[QuestionUid],[AnswerCode],[AnswerContent],[Memo],
    [HaveOtherAnswer],[NeedOtherAnswer],[Score],[OrderSn],[CreateUserId],
    [CreateTime],[ModifyUserId],[ModifyTime] 
FROM QuestionAnswerDefine 
WHERE QuestionUid = @QuestionUid 
ORDER BY OrderSn ASC";

            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(new SqlParameter("@QuestionUid", SqlDbType.UniqueIdentifier)
                {
                    Value = uid,
                });

                connection.Open();

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    answerDefineList = new List<QuestionAnswerDefineDO>();

                    QuestionAnswerDefineDO answerDefineDO = null;

                    foreach (DataRow dr in dt.Rows)
                    {
                        answerDefineDO = null;
                        answerDefineDO = ConvertAnswerDefineDO(dr);
                        answerDefineList.Add(answerDefineDO);
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

            return answerDefineList;
        }

        /// <summary>
        /// 轉換問卷答案定義資料
        /// </summary>
        /// <param name="answerDefine">問卷答案定義資料</param>
        /// <returns>問卷答案定義資料</returns>
        private QuestionAnswerDefineDO ConvertAnswerDefineDO(DataRow answerDefine)
        {
            return new QuestionAnswerDefineDO()
            {
                Uid = answerDefine.Field<Guid>("Uid"),
                QuestionUid = answerDefine.Field<Guid>("QuestionUid"),
                AnswerCode = answerDefine.Field<string>("AnswerCode"),
                AnswerContent = answerDefine.Field<string>("AnswerContent"),
                Memo = answerDefine.Field<string>("Memo"),
                HaveOtherAnswer = answerDefine.Field<string>("HaveOtherAnswer"),
                NeedOtherAnswer = answerDefine.Field<string>("NeedOtherAnswer"),
                Score = answerDefine.Field<int?>("Score"),
                OrderSn = answerDefine.Field<int?>("OrderSn"),
                CreateUserId = answerDefine.Field<string>("CreateUserId"),
                CreateTime = answerDefine.Field<DateTime?>("CreateTime"),
                ModifyUserId = answerDefine.Field<string>("ModifyUserId"),
                ModifyTime = answerDefine.Field<DateTime?>("ModifyTime")
            };
        }
    }
}
