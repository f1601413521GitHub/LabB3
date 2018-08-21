using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Service.Interface;
using ThinkPower.LabB3.DataAccess.DAO;
using ThinkPower.LabB3.DataAccess.DO;
using NLog;
using System.Runtime.ExceptionServices;

namespace ThinkPower.LabB3.Domain.Service
{
    /// <summary>
    /// 問卷服務
    /// </summary>
    public class QuestionnaireService : IQuestionnaire
    {
        /// <summary>
        /// NLog Object
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 計算問卷填答得分
        /// </summary>
        /// <param name="answer">問卷填答資料</param>
        /// <returns></returns>
        public QuestionnaireResultEntity Calculate(QuestionnaireAnswerEntity answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得有效的問卷資料
        /// </summary>
        /// <param name="id">問卷編號</param>
        /// <returns></returns>
        public QuestionnaireEntity GetActiveQuestionaire(string id)
        {
            QuestionnaireEntity questEntity = null;

            try
            {
                if (!String.IsNullOrEmpty(id))
                {
                    QuestionnaireDAO questDAO = new QuestionnaireDAO();
                    QuestionnaireDO questDO = questDAO.Get(id);

                    if (questDO != null)
                    {
                        questEntity = new QuestionnaireEntity()
                        {
                            Uid = questDO.Uid,
                            QuestId = questDO.QuestId,
                            Version = questDO.Version,
                            Kind = questDO.Kind,
                            Name = questDO.Name,
                            Memo = questDO.Memo,
                            Ondate = questDO.Ondate,
                            Offdate = questDO.Offdate,
                            NeedScore = questDO.NeedScore,
                            QuestScore = questDO.QuestScore,
                            ScoreKind = questDO.ScoreKind,
                            HeadBackgroundImg = questDO.HeadBackgroundImg,
                            HeadDescription = questDO.HeadDescription,
                            FooterDescription = questDO.FooterDescription,
                            CreateUserId = questDO.CreateUserId,
                            CreateTime = questDO.CreateTime,
                            ModifyUserId = questDO.ModifyUserId,
                            ModifyTime = questDO.ModifyTime,
                        };
                    }
                }
                else
                {
                    questEntity = new QuestionnaireEntity();
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questEntity;
        }

        /// <summary>
        /// 取得問卷資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns></returns>
        public QuestionnaireEntity GetQuestionnaire(string uid)
        {
            throw new NotImplementedException();
        }
    }
}
