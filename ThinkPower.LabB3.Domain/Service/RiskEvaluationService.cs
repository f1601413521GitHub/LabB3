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
        public RiskEvaResultDTO EvaluateRiskRank(RiskEvaAnswerEntity answer)
        {
            throw new NotImplementedException();
        }

        public RiskEvaluationEntity Get(string uid)
        {
            throw new NotImplementedException();
        }

        public RiskEvaQuestionnaireEntity GetRiskQuestionnaire(string questId)
        {
            throw new NotImplementedException();
        }

        public RiskEvaResultDTO GetRiskResult(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> RiskRank(string riskRankKind)
        {
            throw new NotImplementedException();
        }

        public void SaveRiskRank(string riskResultId)
        {
            throw new NotImplementedException();
        }
    }
}
