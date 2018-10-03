using System.Collections.Generic;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Entity.Risk;

namespace ThinkPower.LabB3.Web.ViewModels
{
    /// <summary>
    /// 評估投資風險等級檢視模型類別
    /// </summary>
    public class EvaluationRankViewModel
    {
        /// <summary>
        /// 投資風險評估結果
        /// </summary>
        public RiskEvaluationEntity RiskEvaluationResult { get; set; }

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