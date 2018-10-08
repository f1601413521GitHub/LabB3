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
    /// 問卷答題明細資料存取類別
    /// </summary>
    public class QuestionnaireAnswerDetailDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM QuestionnaireAnswerDetail", connection);
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
        /// 新增多筆問卷填答資料至問卷答題明細
        /// </summary>
        /// <param name="questAnswerDetailList">問卷填答資料</param>
        /// <returns></returns>
        public void Insert(IEnumerable<QuestionnaireAnswerDetailDO> questAnswerDetailList)
        {
            if (questAnswerDetailList == null)
            {
                throw new ArgumentNullException("questAnswerDetailDO");
            }

            string query = @"
INSERT INTO [QuestionnaireAnswerDetail]
    ([Uid],[AnswerUid],[QuestionUid],[AnswerCode],[OtherAnswer],[Score],[CreateUserId],
    [CreateTime],[ModifyUserId],[ModifyTime]) 
VALUES (@Uid, @AnswerUid, @QuestionUid, @AnswerCode, @OtherAnswer, @Score, @CreateUserId,
    @CreateTime, @ModifyUserId, @ModifyTime)";

            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                foreach (QuestionnaireAnswerDetailDO questAnswerDetail in questAnswerDetailList)
                {
                    command.Parameters.Add(new SqlParameter("@Uid", SqlDbType.VarChar) { Value = questAnswerDetail.Uid.ToString() });
                    command.Parameters.Add(new SqlParameter("@AnswerUid", SqlDbType.VarChar) { Value = questAnswerDetail.AnswerUid.ToString() });
                    command.Parameters.Add(new SqlParameter("@QuestionUid", SqlDbType.VarChar) { Value = questAnswerDetail.QuestionUid.ToString() });
                    command.Parameters.Add(new SqlParameter("@AnswerCode", SqlDbType.VarChar) { Value = questAnswerDetail.AnswerCode ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@OtherAnswer", SqlDbType.VarChar) { Value = questAnswerDetail.OtherAnswer ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@Score", SqlDbType.Int) { Value = questAnswerDetail.Score ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CreateUserId", SqlDbType.VarChar) { Value = questAnswerDetail.CreateUserId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = questAnswerDetail.CreateTime ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ModifyUserId", SqlDbType.VarChar) { Value = questAnswerDetail.ModifyUserId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ModifyTime", SqlDbType.DateTime) { Value = questAnswerDetail.ModifyTime ?? Convert.DBNull });

                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                command = null;

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
