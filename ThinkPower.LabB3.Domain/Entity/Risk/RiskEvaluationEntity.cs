using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity.Risk
{
    /// <summary>
    /// 投資風險評估結果類別
    /// </summary>
    public class RiskEvaluationEntity : BaseEntity
    {
        /// <summary> 
        ///風險評估項目代號 
        /// </summary> 
        public string RiskEvaId { get; set; }

        /// <summary> 
        ///問卷答題編號 
        /// </summary> 
        public string QuestAnswerId { get; set; }

        /// <summary> 
        ///使用者代號 
        /// </summary> 
        public string CliId { get; set; }

        /// <summary> 
        ///風險問卷填寫結果 
        /// </summary> 
        public string RiskResult { get; set; }

        /// <summary> 
        ///風險評估分數 
        /// </summary> 
        public Nullable<int> RiskScore { get; set; }

        /// <summary> 
        ///投資屬性 
        /// </summary> 
        public string RiskAttribute { get; set; }

        /// <summary> 
        ///風險評估日期 
        /// </summary> 
        public Nullable<System.DateTime> EvaluationDate { get; set; }

        /// <summary> 
        ///資料日期 
        /// </summary> 
        public Nullable<System.DateTime> BusinessDate { get; set; }

        /// <summary> 
        ///後台是否採用 
        /// </summary> 
        public string IsUsed { get; set; }
    }
}
