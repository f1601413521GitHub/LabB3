using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Entity.Risk;

namespace ThinkPower.LabB3.Web.ViewModels
{
    /// <summary>
    /// 投資風險評估問卷填答檢視模型類別
    /// </summary>
    public class EvaQuestViewModel
    {
        /// <summary>
        /// 投資風險評估問卷
        /// </summary>
        public RiskEvaQuestionnaireEntity RiskEvaQuestionnaireEntity { get; set; }

        /// <summary>
        /// 問卷填答評分結果
        /// </summary>
        public QuestionnaireResultEntity QuestionnaireResultEntity { get; set; }
    }
}