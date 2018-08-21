using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DAO
{
    /// <summary>
    /// 投資風險標的等級明細資料存取類別
    /// </summary>
    public class RiskRankDetailDAO : BaseDAO
    {
        /// <summary>
        /// 取得資料筆數
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM RiskRankDetail", connection);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
    }
}
