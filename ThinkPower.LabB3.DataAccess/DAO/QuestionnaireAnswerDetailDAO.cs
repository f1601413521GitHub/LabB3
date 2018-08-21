using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM QuestionnaireAnswerDetail", connection);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
    }
}
