using System.Collections.Generic;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Entity.Risk;

namespace ThinkPower.LabB3.Domain.DTO
{
    /// <summary>
    /// 評估投資風險等級評估結果類別
    /// </summary>
    public class RiskEvaResultDTO
    {
        /// <summary>
        /// 投資風險評估問卷
        /// </summary>
        public RiskEvaQuestionnaireEntity RiskEvaQuestionnaireEntity { get; set; }

        /// <summary>
        /// 問卷填答評分結果
        /// </summary>
        public QuestionnaireResultEntity QuestionnaireResultEntity { get; set; }

        /// <summary>
        /// 投資風險等級資料集合
        /// </summary>
        public IEnumerable<RiskRankEntity> RiskRankEntities { get; set; }
    }
}