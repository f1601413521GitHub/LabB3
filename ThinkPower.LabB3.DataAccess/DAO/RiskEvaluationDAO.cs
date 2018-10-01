using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// 投資風險評估結果資料存取類別
    /// </summary>
    public class RiskEvaluationDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM RiskEvaluation", connection);
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
        /// 取得最新使用的投資風險評估結果
        /// </summary>
        /// <param name="questAnswerId">問卷答題編號</param>
        /// <returns>最新使用的投資風險評估結果</returns>
        public RiskEvaluationDO GetLatestUsedRiskEvaluation(string questAnswerId)
        {
            RiskEvaluationDO riskEvaDO = null;

            if (String.IsNullOrEmpty(questAnswerId))
            {
                throw new ArgumentNullException("questAnswerId");
            }

            try
            {
                string query = @"
SELECT TOP 1
    [Uid],[RiskEvaId],[QuestAnswerId],[CliId],[RiskResult],
    [RiskScore],[RiskAttribute],[EvaluationDate],[BusinessDate],
    [IsUsed],[CreateUserId],[CreateTime],[ModifyUserId],[ModifyTime]
FROM [RiskEvaluation]
WHERE [QuestAnswerId] = @QuestAnswerId
AND [RiskEvaId] = 'FNDINV'
AND [IsUsed] = 'Y'
ORDER BY EvaluationDate DESC;";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestAnswerId", SqlDbType.VarChar)
                    {
                        Value = questAnswerId,
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        riskEvaDO = ConvertRiskEvaluationDO(dt.Rows[0]);
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

            return riskEvaDO;
        }

        /// <summary>
        /// 轉換投資風險評估結果
        /// </summary>
        /// <param name="riskEvaluation">投資風險評估結果</param>
        /// <returns>投資風險評估結果資料</returns>
        private RiskEvaluationDO ConvertRiskEvaluationDO(DataRow riskEvaluation)
        {
            return new RiskEvaluationDO()
            {
                Uid = riskEvaluation.Field<Guid>("Uid"),
                RiskEvaId = riskEvaluation.Field<string>("RiskEvaId"),
                QuestAnswerId = riskEvaluation.Field<string>("QuestAnswerId"),
                CliId = riskEvaluation.Field<string>("CliId"),
                RiskResult = riskEvaluation.Field<string>("RiskResult"),
                RiskScore = riskEvaluation.Field<int?>("RiskScore"),
                RiskAttribute = riskEvaluation.Field<string>("RiskAttribute"),
                EvaluationDate = riskEvaluation.Field<DateTime?>("EvaluationDate"),
                BusinessDate = riskEvaluation.Field<DateTime?>("BusinessDate"),
                IsUsed = riskEvaluation.Field<string>("IsUsed"),
                CreateUserId = riskEvaluation.Field<string>("CreateUserId"),
                CreateTime = riskEvaluation.Field<DateTime?>("CreateTime"),
                ModifyUserId = riskEvaluation.Field<string>("ModifyUserId"),
                ModifyTime = riskEvaluation.Field<DateTime?>("ModifyTime"),
            };
        }


        /// <summary>
        /// 取得最新的投資風險評估結果
        /// </summary>
        /// <param name="questAnswerId">問卷答題編號</param>
        /// <returns>最新的投資風險評估結果</returns>
        public RiskEvaluationDO GetLatestRiskEvaluation(string questAnswerId)
        {
            RiskEvaluationDO riskEvaDO = null;

            if (String.IsNullOrEmpty(questAnswerId))
            {
                throw new ArgumentNullException("questAnswerId");
            }

            try
            {
                string query = @"
SELECT TOP 1
    [Uid],[RiskEvaId],[QuestAnswerId],[CliId],[RiskResult],
    [RiskScore],[RiskAttribute],[EvaluationDate],[BusinessDate],
    [IsUsed],[CreateUserId],[CreateTime],[ModifyUserId],[ModifyTime]
FROM [RiskEvaluation]
WHERE [QuestAnswerId] = @QuestAnswerId
AND [RiskEvaId] = 'FNDINV'
ORDER BY EvaluationDate DESC;";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestAnswerId", SqlDbType.VarChar)
                    {
                        Value = questAnswerId,
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        riskEvaDO = ConvertRiskEvaluationDO(dt.Rows[0]);
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

            return riskEvaDO;
        }

        /// <summary>
        /// 新增投資風險評估結果
        /// </summary>
        /// <param name="riskEvaluation">投資風險評估結果資料</param>
        public void Insert(RiskEvaluationDO riskEvaluation)
        {
            if (riskEvaluation == null)
            {
                throw new ArgumentNullException("riskEvaluation");
            }

            try
            {
                string query = @"
INSERT INTO RiskEvaluation 
    ([Uid], [RiskEvaId], [QuestAnswerId], [CliId], [RiskResult], [RiskScore], [RiskAttribute],
    [EvaluationDate], [BusinessDate], [IsUsed], [CreateUserId], [CreateTime], 
    [ModifyUserId], [ModifyTime]) 
VALUES 
    (@Uid, @RiskEvaId, @QuestAnswerId, @CliId, @RiskResult, @RiskScore, @RiskAttribute,
    @EvaluationDate, @BusinessDate, @IsUsed, @CreateUserId, @CreateTime,
    @ModifyUserId, @ModifyTime)
";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add(new SqlParameter("@Uid", SqlDbType.VarChar) { Value = riskEvaluation.Uid.ToString() });
                    command.Parameters.Add(new SqlParameter("@RiskEvaId", SqlDbType.VarChar) { Value = riskEvaluation.RiskEvaId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@QuestAnswerId", SqlDbType.VarChar) { Value = riskEvaluation.QuestAnswerId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CliId", SqlDbType.VarChar) { Value = riskEvaluation.CliId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@RiskResult", SqlDbType.VarChar) { Value = riskEvaluation.RiskResult });
                    command.Parameters.Add(new SqlParameter("@RiskScore", SqlDbType.Int) { Value = riskEvaluation.RiskScore ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@RiskAttribute", SqlDbType.VarChar) { Value = riskEvaluation.RiskAttribute ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@EvaluationDate", SqlDbType.DateTime) { Value = riskEvaluation.EvaluationDate ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@BusinessDate", SqlDbType.DateTime) { Value = riskEvaluation.BusinessDate ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@IsUsed", SqlDbType.VarChar) { Value = riskEvaluation.IsUsed ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CreateUserId", SqlDbType.VarChar) { Value = riskEvaluation.CreateUserId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = riskEvaluation.CreateTime ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ModifyUserId", SqlDbType.VarChar) { Value = riskEvaluation.ModifyUserId ?? Convert.DBNull });
                    command.Parameters.Add(new SqlParameter("@ModifyTime", SqlDbType.DateTime) { Value = riskEvaluation.ModifyTime ?? Convert.DBNull });


                    connection.Open();
                    command.ExecuteNonQuery();

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
        }
    }
}
