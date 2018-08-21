using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 投資風險評估結果資料物件類別
    /// </summary>
    public class RiskEvaluationDO
    {
        public System.Guid Uid { get; set; }
        public string RiskEvaId { get; set; }
        public string QuestAnswerId { get; set; }
        public string CliId { get; set; }
        public string RiskResult { get; set; }
        public Nullable<int> RiskScore { get; set; }
        public string RiskAttribute { get; set; }
        public Nullable<System.DateTime> EvaluationDate { get; set; }
        public Nullable<System.DateTime> BusinessDate { get; set; }
        public string IsUsed { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
