using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.DataAccess.DO;

namespace ThinkPower.LabB3.Domain.Entity.Risk
{
    /// <summary>
    /// 投資風險等級類別
    /// </summary>
    public class RiskRankEntity : BaseEntity
    {
        /// <summary> 
        ///風險評估項目代號 
        /// </summary> 
        public string RiskEvaId { get; set; }

        /// <summary> 
        ///起始分數 
        /// </summary> 
        public Nullable<int> MinScore { get; set; }

        /// <summary> 
        ///截止分數 
        /// </summary> 
        public Nullable<int> MaxScore { get; set; }

        /// <summary> 
        ///投資屬性類型 
        /// </summary> 
        public string RiskRankKind { get; set; }

        /// <summary>
        /// 投資風險等級明細
        /// </summary>
        public IEnumerable<RiskRankDetailEntity> RiskRankDetailEntities { get; set; }
    }
}
