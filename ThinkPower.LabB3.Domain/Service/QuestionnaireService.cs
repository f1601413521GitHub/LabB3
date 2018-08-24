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
                QuestionnaireDO questionnaireDO =
                    new QuestionnaireDAO().GetActiveQuestionniare(id);

                if (questionnaireDO == null)
                {
                    throw new InvalidOperationException("questDO not found");
                }

                QuestionnaireAnswerDO questAnswerDO =
                    new QuestionnaireAnswerDAO().GetQuestionnaireAnswer(questionnaireDO.Uid);

                if (questAnswerDO == null)
                {
                    throw new InvalidOperationException("questAnswerDO not found");
                }

                RiskEvaluationDO riskEvaDO =
                    new RiskEvaluationDAO().GetLatestRiskEvaluation(questAnswerDO.QuestAnswerId);

                bool riskEvaluationInCuttimeRange = false;

                if (riskEvaDO != null)
                {
                    IEnumerable<DateTime> cuttimeRange = GetRiskEvaCuttime();

                    if ((riskEvaDO.EvaluationDate < cuttimeRange.Max()) &&
                        (riskEvaDO.EvaluationDate > cuttimeRange.Min()))
                    {
                        riskEvaluationInCuttimeRange = true;
                    }
                }

                if (!riskEvaluationInCuttimeRange)
                {
                    IEnumerable<QuestionDefineDO> questDefineList = 
                        new QuestionDefineDAO().GetQuestionDefineList(questionnaireDO.Uid);

                    if ((questDefineList == null) ||
                        (questDefineList.Count() == 0))
                    {
                        throw new InvalidOperationException("questDefines not found");
                    }

                    QuestionAnswerDefineDAO answerDefineDAO = new QuestionAnswerDefineDAO();
                    List<QuestionAnswerDefineDO> answerDefineList = new List<QuestionAnswerDefineDO>();

                    foreach (QuestionDefineDO questDefine in questDefineList)
                    {
                        answerDefineList.AddRange(answerDefineDAO.
                            GetQuestionAnswerDefineList(questDefine.Uid));
                    }

                    if ((answerDefineList == null) ||
                        (answerDefineList.Count == 0))
                    {
                        throw new InvalidOperationException("answerDefineList not found");
                    }

                    questEntity = new QuestionnaireEntity()
                    {
                        Uid = questionnaireDO.Uid,
                        QuestId = questionnaireDO.QuestId,
                        Version = questionnaireDO.Version,
                        Kind = questionnaireDO.Kind,
                        Name = questionnaireDO.Name,
                        Memo = questionnaireDO.Memo,
                        Ondate = questionnaireDO.Ondate,
                        Offdate = questionnaireDO.Offdate,
                        NeedScore = questionnaireDO.NeedScore,
                        QuestScore = questionnaireDO.QuestScore,
                        ScoreKind = questionnaireDO.ScoreKind,
                        HeadBackgroundImg = questionnaireDO.HeadBackgroundImg,
                        HeadDescription = questionnaireDO.HeadDescription,
                        FooterDescription = questionnaireDO.FooterDescription,
                        CreateUserId = questionnaireDO.CreateUserId,
                        CreateTime = questionnaireDO.CreateTime,
                        ModifyUserId = questionnaireDO.ModifyUserId,
                        ModifyTime = questionnaireDO.ModifyTime,

                        QuestDefineEntitys = ConvertQuestDefineEntity(questDefineList),
                        AnswerDefineEntitys = ConvertAnswerDefineEntity(answerDefineList),
                    };
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
        /// 取得投資風險評估切點時間範圍
        /// </summary>
        /// <returns>投資風險評估切點時間範圍</returns>
        private IEnumerable<DateTime> GetRiskEvaCuttime()
        {
            List<DateTime> cuttimeRange = null;

            string riskEvaCuttime = ConfigurationManager.AppSettings["risk.evaluation.cuttime"];

            string[] cuttimeArray = !String.IsNullOrEmpty(riskEvaCuttime) ?
                riskEvaCuttime.Split(',') :
                null;

            if (cuttimeArray != null)
            {
                List<DateTime> cuttimes = new List<DateTime>();

                foreach (string cuttime in cuttimeArray)
                {
                    if (!String.IsNullOrEmpty(cuttime) &&
                        (DateTime.TryParse(cuttime, out DateTime tempCuttime)))
                    {
                        cuttimes.Add(tempCuttime);
                    }
                }

                if (cuttimes.Count > 0)
                {
                    DateTime timeNow = DateTime.Now;
                    DateTime cuttimeMax = cuttimes.Max();
                    DateTime cuttimeMin = cuttimes.Min();

                    cuttimes.Add(new DateTime(cuttimeMax.Year, cuttimeMax.Month,
                        cuttimeMax.Day, cuttimeMax.Hour, cuttimeMax.Minute, 0).AddDays(-1));

                    cuttimes.Add(new DateTime(cuttimeMin.Year, cuttimeMin.Month,
                        cuttimeMin.Day, cuttimeMin.Hour, cuttimeMin.Minute, 0).AddDays(1));

                    cuttimeRange.Add(cuttimes.Where(x => x < timeNow).Max());
                    cuttimeRange.Add(cuttimes.Where(x => x > timeNow).Min());
                }
            }

            return cuttimeRange;
        }

        /// <summary>
        /// 轉換問卷答案定義資料物件成為問卷答案定義類別
        /// </summary>
        /// <param name="answerDefineList">問卷答案定義資料物件</param>
        /// <returns>問卷答案定義類別</returns>
        private IEnumerable<AnswerDefineEntity> ConvertAnswerDefineEntity(
            IEnumerable<QuestionAnswerDefineDO> answerDefineList)
        {
            return answerDefineList.Select(x => new AnswerDefineEntity
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
        /// 轉換問卷題目定義資料成為問卷題目定義實體
        /// </summary>
        /// <param name="questDefineList">問卷題目定義資料</param>
        /// <returns>問卷題目定義實體</returns>
        private IEnumerable<QuestDefineEntity> ConvertQuestDefineEntity(
            IEnumerable<QuestionDefineDO> questDefineList)
        {
            return questDefineList.Select(x => new QuestDefineEntity
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
