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
    /// 問卷資料存取類別
    /// </summary>
    public class QuestionnaireDAO : BaseDAO
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
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM Questionnaire", connection);
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
        /// 取得有效的問卷資料
        /// </summary>
        /// <param name="id">問卷編號</param>
        /// <returns>有效的問卷資料</returns>
        public QuestionnaireDO Get(string id)
        {
            QuestionnaireDO questDO = null;

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                string query = @"
SELECT TOP 1 
    [Uid] ,[QuestId] ,[Version] ,[Kind] ,[Name] ,[Memo] ,[Ondate] ,
    [Offdate] ,[NeedScore] ,[QuestScore] ,[ScoreKind] ,[HeadBackgroundImg] ,[HeadDescription] ,
    [FooterDescription] ,[CreateUserId] ,[CreateTime] ,[ModifyUserId] ,[ModifyTime] 
FROM Questionnaire 
WHERE QuestId = @QuestId 
    AND Ondate < @DateTimeNow 
    AND ((Offdate > @DateTimeNow) OR (Offdate is null)) 
ORDER BY Version DESC;";

                using (SqlConnection connection = DbConnection)
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.Add(new SqlParameter("@QuestId", SqlDbType.VarChar) { Value = id });
                    command.Parameters.Add(new SqlParameter("@DateTimeNow", SqlDbType.DateTime)
                    {
                        //Value = DateTime.Parse("2018-06-01 00:00:01"),
                        Value = DateTime.Now.Date,
                    });

                    connection.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);

                    if (dt.Rows.Count == 1)
                    {
                        questDO = GetQuestionnaireDO(dt.Rows[0]);
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

            return questDO;
        }

        /// <summary>
        /// 取得問卷資料物件類別
        /// </summary>
        /// <param name="dr">資料列</param>
        /// <returns>問卷資料物件類別</returns>
        private QuestionnaireDO GetQuestionnaireDO(DataRow dr)
        {
            return new QuestionnaireDO()
            {
                //Uid = Guid.TryParse(dr.Field<string>("Uid"), out Guid tempUid) ?
                //    tempUid :
                //    throw new InvalidOperationException("Uid is invalid"),

                Uid = dr.Field<Guid>("Uid"),
                QuestId = dr.Field<string>("QuestId"),
                Version = dr.Field<string>("Version"),
                Kind = dr.Field<string>("Kind"),
                Name = dr.Field<string>("Name"),
                Memo = dr.Field<string>("Memo"),
                Ondate = dr.Field<DateTime?>("Ondate"),
                Offdate = dr.Field<DateTime?>("Offdate"),
                NeedScore = dr.Field<string>("NeedScore"),
                QuestScore = dr.Field<int?>("QuestScore"),
                ScoreKind = dr.Field<string>("ScoreKind"),
                HeadBackgroundImg = dr.Field<string>("HeadBackgroundImg"),
                HeadDescription = dr.Field<string>("HeadDescription"),
                FooterDescription = dr.Field<string>("FooterDescription"),
                CreateUserId = dr.Field<string>("CreateUserId"),
                CreateTime = dr.Field<DateTime?>("CreateTime"),
                ModifyUserId = dr.Field<string>("ModifyUserId"),
                ModifyTime = dr.Field<DateTime?>("ModifyTime"),
            };
        }
    }
}
