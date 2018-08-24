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
using ThinkPower.LabB3.Domain.DTO;
using System.Configuration;

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
        /// <returns>有效的問卷資料</returns>
        public QuestionnaireEntity GetActiveQuestionnaire(string id)
        {
            QuestionnaireEntity questEntity = null;

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                QuestionnaireDO questDO = new QuestionnaireDAO().GetActiveQuestionniare(id);

                if (questDO == null)
                {
                    throw new InvalidOperationException("questDO is invalid");
                }

                RiskEvaluationDO riskEvaDO = new RiskEvaluationDAO().GetLatestRiskEvaluation(questDO.Uid.ToString());

                IEnumerable<DateTime> riskEvaCuttime = GetRiskEvaCuttime();
                DateTime timeNow = DateTime.Parse(DateTime.Now.ToString("g"));// g: 6/15/2008 9:15 PM

                int? index = null;
                foreach (DateTime riskEvaTime in riskEvaCuttime)
                {
                    if ((index == null) &&
                        (timeNow < riskEvaTime))
                    {
                        //index = riskEvaCuttime.TakeWhile(x=>x != riskEvaTime).IndexOf(riskEvaTime);
                    }
                }


                IEnumerable<QuestionDefineDO> questDefines = new QuestionDefineDAO().GetQuesyionDefineCollection(questDO.Uid.ToString());

                if ((questDefines == null) ||
                    (questDefines.Count() == 0))
                {
                    throw new InvalidOperationException("questDefines is invalid");
                }

                QuestionAnswerDefineDAO questAnsDefineDAO = new QuestionAnswerDefineDAO();
                List<QuestionAnswerDefineDO> questAnsDefines = new List<QuestionAnswerDefineDO>();

                foreach (QuestionDefineDO questDefine in questDefines)
                {
                    questAnsDefines.AddRange(questAnsDefineDAO.GetQuestionAnswerDefineCollection(questDefine.Uid.ToString()));
                }

                if ((questAnsDefines == null) ||
                    (questAnsDefines.Count == 0))
                {
                    throw new InvalidOperationException("questAnsDefines is invalid");
                }

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

                    QuestDefineEntitys = ConvertQuestDefineEntity(questDefines),
                    AnswerDefineEntitys = ConvertAnswerDefineEntity(questAnsDefines),
                };
            }
            catch (Exception e)
            {
                logger.Error(e);
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questEntity;
        }

        /// <summary>
        /// 取得 投資風險評估切點時間
        /// </summary>
        /// <returns>投資風險評估切點時間</returns>
        private IEnumerable<DateTime> GetRiskEvaCuttime()
        {
            string[] riskEvaCuttime =
                !String.IsNullOrEmpty(ConfigurationManager.AppSettings["risk.evaluation.cuttime"]) ?
                ConfigurationManager.AppSettings["risk.evaluation.cuttime"].Split(',') :
                throw new ConfigurationErrorsException("risk.evaluation.cuttime is invalid");

            List<DateTime> collection = new List<DateTime>();

            foreach (string cuttime in riskEvaCuttime)
            {
                collection.Add(DateTime.TryParse(cuttime, out DateTime tempCuttime) ?
                        tempCuttime :
                        throw new InvalidOperationException("cuttime is invalid")
                    );
            }

            return collection.OrderBy(x => x.Hour).ThenBy(x => x.Minute);
        }

        /// <summary>
        /// 取得 問卷答案定義類別
        /// </summary>
        /// <param name="questAnsDefines">問卷答案項目資料物件類別</param>
        /// <returns>問卷答案定義類別</returns>
        private IEnumerable<AnswerDefineEntity> ConvertAnswerDefineEntity(
            IEnumerable<QuestionAnswerDefineDO> questAnsDefines)
        {
            return questAnsDefines.Select(x => new AnswerDefineEntity
            {
                Uid = x.Uid,
                QuestionUid = x.QuestionUid,
                AnswerCode = x.AnswerCode,
                AnswerContent = x.AnswerContent,
                Memo = x.Memo,
                HaveOtherAnswer = x.HaveOtherAnswer,
                NeedOtherAnswer = x.NeedOtherAnswer,
                Score = x.Score,
                OrderSn = x.OrderSn,
                CreateUserId = x.CreateUserId,
                CreateTime = x.CreateTime,
                ModifyUserId = x.ModifyUserId,
                ModifyTime = x.ModifyTime,
            });
        }

        /// <summary>
        /// 取得 問卷題目定義類別
        /// </summary>
        /// <param name="questDefines">問卷題目資料物件類別</param>
        /// <returns>問卷題目定義類別</returns>
        private IEnumerable<QuestDefineEntity> ConvertQuestDefineEntity(IEnumerable<QuestionDefineDO> questDefines)
        {
            return questDefines.Select(x => new QuestDefineEntity
            {
                Uid = x.Uid,
                QuestUid = x.QuestUid,
                QuestionId = x.QuestionId,
                QuestionContent = x.QuestionContent,
                NeedAnswer = x.NeedAnswer,
                AllowNaCondition = x.AllowNaCondition,
                AnswerType = x.AnswerType,
                MinMultipleAnswers = x.MinMultipleAnswers,
                MaxMultipleAnswers = x.MaxMultipleAnswers,
                SingleAnswerCondition = x.SingleAnswerCondition,
                CountScoreType = x.CountScoreType,
                Memo = x.Memo,
                OrderSn = x.OrderSn,
                CreateUserId = x.CreateUserId,
                CreateTime = x.CreateTime,
                ModifyUserId = x.ModifyUserId,
                ModifyTime = x.ModifyTime,
            });
        }

        /// <summary>
        /// 取得問卷資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷資料</returns>
        public QuestionnaireEntity GetQuestionnaire(string uid)
        {
            QuestionnaireEntity questInfo = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                //TODO
            }
            catch (Exception e)
            {
                logger.Error(e);
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questInfo;
        }
    }
}
