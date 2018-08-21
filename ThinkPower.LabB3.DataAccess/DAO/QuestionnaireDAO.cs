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
        /// <returns></returns>
        public override int Count()
        {
            int count;

            using (SqlConnection connection = DbConnection)
            {
                SqlCommand command = new SqlCommand("SELECT Count(1) FROM Questionnaire", connection);
                connection.Open();

                count = (int)command.ExecuteScalar();
            }

            return count;
        }

        /// <summary>
        /// 取得有效的問卷資料
        /// </summary>
        /// <param name="id">問卷編號</param>
        /// <returns></returns>
        public QuestionnaireDO Get(string id)
        {
            QuestionnaireDO result = null;

            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    using (SqlConnection connection = DbConnection)
                    {
                        string query = @"SELECT TOP 1 
[Uid] ,[QuestId] ,[Version] ,[Kind] ,[Name] ,[Memo] ,[Ondate] ,
[Offdate] ,[NeedScore] ,[QuestScore] ,[ScoreKind] ,[HeadBackgroundImg] ,[HeadDescription] ,[FooterDescription] ,
[CreateUserId] ,[CreateTime] ,[ModifyUserId] ,[ModifyTime] 
FROM Questionnaire 
WHERE 1=1 
AND QuestId = @QuestId 
AND Ondate < GETDATE() 
AND((Offdate > GETDATE()) OR (Offdate is null)) 
ORDER BY Version DESC";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            DataTable dt = new DataTable();
                            command.Parameters.AddWithValue("@QuestId", id);
                            connection.Open();

                            using (SqlDataAdapter da = new SqlDataAdapter(command))
                            {
                                da.Fill(dt);
                            }

                            foreach (DataRow dr in dt.Rows)
                            {
                                result = new QuestionnaireDO()
                                {
                                    Uid = Guid.TryParse(dr.Field<string>("Uid"), out Guid tempUid) ?
                                    tempUid :
                                    Guid.Empty,

                                    QuestId = dr.Field<string>("QuestId"),
                                    Version = dr.Field<string>("Version"),
                                    Kind = dr.Field<string>("Kind"),
                                    Name = dr.Field<string>("Name"),
                                    Memo = dr.Field<string>("Memo"),

                                    Ondate = DateTime.TryParse(dr.Field<string>("Ondate"), out DateTime tempOndate) ?
                                    (DateTime?)tempOndate :
                                    null,

                                    Offdate = DateTime.TryParse(dr.Field<string>("Offdate"), out DateTime tempOffdate) ?
                                    (DateTime?)tempOffdate :
                                    null,

                                    NeedScore = dr.Field<string>("NeedScore"),

                                    QuestScore = Int32.TryParse(dr.Field<string>("QuestScore"), out int tempQuestScore) ?
                                    (int?)tempQuestScore :
                                    null,

                                    ScoreKind = dr.Field<string>("ScoreKind"),
                                    HeadBackgroundImg = dr.Field<string>("HeadBackgroundImg"),
                                    HeadDescription = dr.Field<string>("HeadDescription"),
                                    FooterDescription = dr.Field<string>("FooterDescription"),
                                    CreateUserId = dr.Field<string>("CreateUserId"),

                                    CreateTime = DateTime.TryParse(dr.Field<string>("CreateTime"), out DateTime tempCreateTime) ?
                                    (DateTime?)tempCreateTime :
                                    null,
                                    ModifyUserId = dr.Field<string>("ModifyUserId"),

                                    ModifyTime = DateTime.TryParse(dr.Field<string>("ModifyTime"), out DateTime tempModifyTime) ?
                                    (DateTime?)tempModifyTime :
                                    null,
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
        }
    }
}
