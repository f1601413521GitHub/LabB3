using System.Collections.Generic;
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

        /// <summary>
        /// 問卷答題檢核失敗的資訊，包含問卷題目定義識別碼與錯誤訊息
        /// </summary>
        public Dictionary<string, string> ValidateFailInfo { get; set; }
    }
}