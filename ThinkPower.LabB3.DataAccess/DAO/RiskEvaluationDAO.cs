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
        /// <returns></returns>
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
        /// 取得最新的 投資風險評估結果資料物件類別
        /// </summary>
        /// <param name="uid">問卷主檔 紀錄識別碼</param>
        /// <returns>投資風險評估結果資料物件類別</returns>
        public RiskEvaluationDO GetLatestRiskEvaluation(string uid)
        {
            RiskEvaluationDO riskEvaDO = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                string query = @"
WITH Quest AS
(
    SELECT Uid
    FROM Questionnaire A
    WHERE A.Uid = @Uid
),
QuestAns AS
(
    SELECT B.QuestUid,B.QuestAnswerId FROM Quest A
    JOIN QuestionnaireAnswer B 
    ON A.Uid = B.QuestUid
),
RiskEva AS
(
    SELECT TOP 1 
        B.[Uid],B.[RiskEvaId],B.[QuestAnswerId],B.[CliId],B.[RiskResult],
        B.[RiskScore],B.[RiskAttribute],B.[EvaluationDate],B.[BusinessDate],
        B.[IsUsed],B.[CreateUserId],B.[CreateTime],B.[ModifyUserId],B.[ModifyTime] 
    FROM QuestAns A
    JOIN  [dbo].[RiskEvaluation] B 
    ON A.QuestAnswerId = B.QuestAnswerId
    WHERE B.IsUsed = 'Y'
    ORDER BY B.EvaluationDate DESC
)

SELECT * FROM RiskEva
GO";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@Uid", SqlDbType.UniqueIdentifier)
                    {
                        Value = Guid.TryParse(uid, out Guid tempUid) ?
                            tempUid :
                            throw new InvalidOperationException("Uid is invalid"),
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
                logger.Error(e);
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return riskEvaDO;
        }

        /// <summary>
        /// 取得 投資風險評估結果資料物件類別
        /// </summary>
        /// <param name="dr">資料列</param>
        /// <returns>投資風險評估結果資料物件類別</returns>
        private RiskEvaluationDO ConvertRiskEvaluationDO(DataRow dr)
        {
            return new RiskEvaluationDO()
            {
                Uid = dr.Field<Guid>("Uid"),
                RiskEvaId = dr.Field<string>("RiskEvaId"),
                QuestAnswerId = dr.Field<string>("QuestAnswerId"),
                CliId = dr.Field<string>("CliId"),
                RiskResult = dr.Field<string>("RiskResult"),
                RiskScore = dr.Field<int?>("RiskScore"),
                RiskAttribute = dr.Field<string>("RiskAttribute"),
                EvaluationDate = dr.Field<DateTime?>("EvaluationDate"),
                BusinessDate = dr.Field<DateTime?>("BusinessDate"),
                IsUsed = dr.Field<string>("IsUsed"),
                CreateUserId = dr.Field<string>("CreateUserId"),
                CreateTime = dr.Field<DateTime?>("CreateTime"),
                ModifyUserId = dr.Field<string>("ModifyUserId"),
                ModifyTime = dr.Field<DateTime?>("ModifyTime"),
            };
        }
    }
}
