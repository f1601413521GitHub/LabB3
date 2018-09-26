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
            int count;

            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM RiskRankDetail", connection);
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
        /// 取得投資風險標的等級明細
        /// </summary>
        /// <param name="uid">投資風險等級紀錄識別碼</param>
        /// <returns>投資風險標的等級明細資料物件集合</returns>
        public List<RiskRankDetailDO> GetRiskRankDetail(Guid uid)
        {
            List<RiskRankDetailDO> riskRankDetailDOList = null;

            if (uid == Guid.Empty)
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                string query = @"
SELECT 
    [Uid], [RiskRankUid], [ProfitRiskRank], [IsEffective], [CreateUserId], [CreateTime], 
    [ModifyUserId], [ModifyTime] 
FROM [RiskRankDetail] 
WHERE RiskRankUid = @RiskRankUid;";


                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@RiskRankUid", SqlDbType.VarChar) { Value = uid.ToString() });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);


                    riskRankDetailDOList = new List<RiskRankDetailDO>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        riskRankDetailDOList.Add(ConvertRiskRankDetailDO(dr));
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

            return riskRankDetailDOList;
        }


        /// <summary>
        /// 轉換投資風險標的等級明細資料，成為投資風險標的等級明細資料物件
        /// </summary>
        /// <param name="riskRankDetail">投資風險標的等級明細資料</param>
        /// <returns>投資風險標的等級明細資料物件</returns>
        private RiskRankDetailDO ConvertRiskRankDetailDO(DataRow riskRankDetail)
        {
            return new RiskRankDetailDO()
            {
                Uid = riskRankDetail.Field<Guid>("Uid"),
                RiskRankUid = riskRankDetail.Field<Guid>("RiskRankUid"),
                ProfitRiskRank = riskRankDetail.Field<string>("ProfitRiskRank"),
                IsEffective = riskRankDetail.Field<string>("IsEffective"),
                CreateUserId = riskRankDetail.Field<string>("CreateUserId"),
                CreateTime = riskRankDetail.Field<DateTime?>("CreateTime"),
                ModifyUserId = riskRankDetail.Field<string>("ModifyUserId"),
                ModifyTime = riskRankDetail.Field<DateTime?>("ModifyTime"),
            };
        }
    }
}
