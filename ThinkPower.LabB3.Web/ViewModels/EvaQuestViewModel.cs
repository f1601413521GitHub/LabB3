using System.ComponentModel.DataAnnotations;
using ThinkPower.LabB3.Domain.Entity.Risk;

namespace ThinkPower.LabB3.Web.ViewModels
{
    /// <summary>
    /// 投資風險評估問卷填答頁面呈現類別
    /// </summary>
    public class EvaQuestViewModel
    {
        /// <summary>
        /// 投資風險評估問卷
        /// </summary>
        public RiskEvaQuestionnaireEntity RiskEvaQuestionnaire { get; set; }
    }
}