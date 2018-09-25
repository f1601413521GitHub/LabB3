using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.DataAccess.DAO;
using ThinkPower.LabB3.DataAccess.DO;
using ThinkPower.LabB3.Domain.DTO;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Entity.Risk;
using ThinkPower.LabB3.Domain.Service.Interface;

namespace ThinkPower.LabB3.Domain.Service
{
    /// <summary>
    /// 投資風險評估服務
    /// </summary>
    public class RiskEvaluationService : IRiskEvaluation
    {
        /// <summary>
        /// NLog Object
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 問卷服務 隱藏欄位
        /// </summary>
        private QuestionnaireService _questService;

        /// <summary>
        /// 問卷服務
        /// </summary>
        private QuestionnaireService QuestService
        {
            get
            {
                if (_questService == null)
                {
                    _questService = new QuestionnaireService();
                }

                return _questService;
            }
        }

        /// <summary>
        /// 評估投資風險等級
        /// </summary>
        /// <param name="answer">風險評估填答資料</param>
        /// <returns></returns>
        public RiskEvaResultDTO EvaluateRiskRank(RiskEvaAnswerEntity answer)
        {
            RiskEvaResultDTO result = null;
            QuestionnaireResultEntity questResultEntity = null;
            RiskEvaQuestionnaireEntity riskEvaQuestEntity = null;

            try
            {
                if (answer == null)
                {
                    throw new ArgumentNullException("answer");
                }

                questResultEntity = QuestService.Calculate(answer.QuestionnaireAnswerEntity);

                if (questResultEntity == null)
                {
                    throw new InvalidOperationException("questResultEntity not found");
                }

                if (questResultEntity.ValidateFailInfo.Count > 0)
                {
                    riskEvaQuestEntity = GetRiskQuestionnaire(questResultEntity.ValidateFailQuestId);

                    if (riskEvaQuestEntity == null)
                    {
                        throw new InvalidOperationException("riskEvaQuestEntity not found");
                    }
                }

                result = new RiskEvaResultDTO()
                {
                    QuestionnaireResultEntity = questResultEntity,
                    RiskEvaQuestionnaire = riskEvaQuestEntity,
                };
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
        }

        /// <summary>
        /// 依紀錄識別碼取得風險評估資料
        /// </summary>
        /// <param name="uid">紀錄識別碼</param>
        /// <returns></returns>
        public RiskEvaluationEntity Get(string uid)
        {
            RiskEvaluationEntity riskEvaEntity = null;

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
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return riskEvaEntity;
        }

        /// <summary>
        /// 取得風險評估問卷資料
        /// </summary>
        /// <param name="questId">問卷編號</param>
        /// <returns>風險評估問卷資料</returns>
        public RiskEvaQuestionnaireEntity GetRiskQuestionnaire(string questId)
        {
            RiskEvaQuestionnaireEntity riskEvaQuest = null;

            if (String.IsNullOrEmpty(questId))
            {
                throw new ArgumentNullException("questId");
            }

            try
            {
                QuestionnaireEntity activeQuest = QuestService.GetActiveQuestionnaire(questId);

                QuestionnaireAnswerDO questAnswer =
                    new QuestionnaireAnswerDAO().GetQuestionnaireAnswer(activeQuest.Uid);

                if (questAnswer == null)
                {
                    throw new InvalidOperationException(
                        $"questAnswer not found,activeQuestUid={activeQuest.Uid}");
                }

                RiskEvaluationDO riskEvaluation =
                    new RiskEvaluationDAO().GetLatestRiskEvaluation(questAnswer.QuestAnswerId);

                bool riskEvaluationInCuttimeRange = false;

                if (riskEvaluation != null)
                {
                    IEnumerable<DateTime> cuttimeRange = GetRiskEvaCuttime();

                    if (cuttimeRange == null)
                    {
                        throw new InvalidOperationException("cuttimeRange not found");
                    }

                    if ((riskEvaluation.EvaluationDate < cuttimeRange.Max()) &&
                        (riskEvaluation.EvaluationDate >= cuttimeRange.Min()))
                    {
                        riskEvaluationInCuttimeRange = true;
                    }
                }

                riskEvaQuest = new RiskEvaQuestionnaireEntity()
                {
                    QuestionnaireEntity = activeQuest,
                    CanUseRiskEvaluation = !riskEvaluationInCuttimeRange,
                };
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return riskEvaQuest;
        }

        /// <summary>
        /// 取得暫存的風險評估資料
        /// </summary>
        /// <param name="key">暫存資料識別碼</param>
        /// <returns></returns>
        public RiskEvaResultDTO GetRiskResult(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 依投資風險屬性取得可投資的風險等級項目
        /// </summary>
        /// <param name="riskRankKind">投資風險屬性</param>
        /// <returns></returns>
        public IEnumerable<string> RiskRank(string riskRankKind)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 儲存評估投資風險評估結果資料
        /// </summary>
        /// <param name="riskResultId">風險評估結果識別代號</param>
        public void SaveRiskRank(string riskResultId)
        {
            throw new NotImplementedException();
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
                    DateTime cuttimesMax = cuttimes.Max();
                    DateTime cuttimesMin = cuttimes.Min();

                    cuttimes.Add(cuttimesMax.AddDays(-1));
                    cuttimes.Add(cuttimesMin.AddDays(1));

                    cuttimeRange = new List<DateTime>()
                    {
                        cuttimes.Where(x => x < timeNow).Max(),
                        cuttimes.Where(x => x > timeNow).Min()
                    };
                }
            }

            return cuttimeRange;
        }
    }
}
