using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.DTO;
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
        /// 問卷服務
        /// </summary>
        private QuestionnaireService _questService;

        public RiskEvaluationService()
        {
            _questService = new QuestionnaireService();
        }

        /// <summary>
        /// 評估投資風險等級
        /// </summary>
        /// <param name="answer">風險評估填答資料</param>
        /// <returns></returns>
        public RiskEvaResultDTO EvaluateRiskRank(RiskEvaAnswerEntity answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 依紀錄識別碼取得風險評估資料
        /// </summary>
        /// <param name="uid">紀錄識別碼</param>
        /// <returns></returns>
        public RiskEvaluationEntity Get(string uid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 取得風險評估問卷資料
        /// </summary>
        /// <param name="questId">問卷編號</param>
        /// <returns></returns>
        public RiskEvaQuestionnaireEntity GetRiskQuestionnaire(string questId)
        {
            RiskEvaQuestionnaireEntity riskEvaQuest;

            if (!String.IsNullOrEmpty(questId))
            {
                var result = _questService.GetActiveQuestionaire(questId);

                riskEvaQuest = new RiskEvaQuestionnaireEntity();
            }
            else
            {
                riskEvaQuest = new RiskEvaQuestionnaireEntity();
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
    }
}
