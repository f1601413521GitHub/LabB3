using System.Collections.Generic;
using ThinkPower.LabB3.Domain.Entity.Risk;

namespace ThinkPower.LabB3.Web.ViewModels
{
    /// <summary>
    /// 評估投資風險等級頁面呈現類別
    /// </summary>
    public class EvaluationRankViewModel
    {
        /// <summary>
        /// 投資風險評估結果
        /// </summary>
        public RiskEvaluationEntity RiskEvaluationResult { get; set; }

        /// <summary>
        /// 可投資的風險等級項目
        /// </summary>
        public IEnumerable<string> RiskRankEntryList { get; set; }
    }
}