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
    /// 投資風險等級資料存取類別
    /// </summary>
    public class RiskRankDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM RiskRank", connection);
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
        /// 取得投資風險等級
        /// </summary>
        /// <param name="actualScore">問卷得分</param>
        /// <returns>投資風險等級資料物件</returns>
        public RiskRankDO GetRiskRank(int? actualScore)
        {
            RiskRankDO riskRankDO = null;

            if (actualScore == null)
            {
                throw new ArgumentNullException("actualScore");
            }

            try
            {
                string query = @"
SELECT 
    [Uid], [RiskEvaId], [MinScore], [MaxScore], [RiskRankKind], [CreateUserId], [CreateTime], 
    [ModifyUserId], [ModifyTime] 
FROM [RiskRank] 
WHERE RiskEvaId = 'FNDINV' 
    AND @ActualScore >= MinScore 
    AND(@ActualScore <= MaxScore OR MaxScore IS NUll);";


                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@ActualScore", SqlDbType.Int) { Value = (int)actualScore });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        riskRankDO = ConvertRiskRankDO(dt.Rows[0]);
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

            return riskRankDO;
        }

        /// <summary>
        /// 轉換投資風險等級資料成投資風險等級資料物件
        /// </summary>
        /// <param name="riskRank">投資風險等級資料</param>
        /// <returns>投資風險等級資料物件</returns>
        private RiskRankDO ConvertRiskRankDO(DataRow riskRank)
        {
            return new RiskRankDO()
            {
                Uid = riskRank.Field<Guid>("Uid"),
                RiskEvaId = riskRank.Field<string>("RiskEvaId"),
                MinScore = riskRank.Field<int?>("MinScore"),
                MaxScore = riskRank.Field<int?>("MaxScore"),
                RiskRankKind = riskRank.Field<string>("RiskRankKind"),
                CreateUserId = riskRank.Field<string>("CreateUserId"),
                CreateTime = riskRank.Field<DateTime?>("CreateTime"),
                ModifyUserId = riskRank.Field<string>("ModifyUserId"),
                ModifyTime = riskRank.Field<DateTime?>("ModifyTime"),
            };
        }

        /// <summary>
        /// 取得所有投資風險等級
        /// </summary>
        /// <param name="riskEvaId">風險評估項目代號</param>
        /// <returns>投資風險等級資料物件的集合</returns>
        public List<RiskRankDO> ReadAll(string riskEvaId)
        {
            List<RiskRankDO> riskRankDOList = new List<RiskRankDO>();

            if (String.IsNullOrEmpty(riskEvaId))
            {
                throw new ArgumentNullException("riskEvaId");
            }

            try
            {
                string query = @"
SELECT 
    [Uid], [RiskEvaId], [MinScore], [MaxScore], [RiskRankKind], [CreateUserId], [CreateTime], 
    [ModifyUserId], [ModifyTime] 
FROM [RiskRank] 
WHERE RiskEvaId = @RiskEvaId;";


                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@RiskEvaId", SqlDbType.VarChar) { Value = riskEvaId });


                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        riskRankDOList.Add(ConvertRiskRankDO(dr));
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

            return riskRankDOList;
        }
    }
}
