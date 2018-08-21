using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 投資風險標的等級明細資料物件類別
    /// </summary>
    public class RiskRankDetailDO
    {
        /// <summary> 
        ///紀錄識別碼 
        /// </summary> 
        public System.Guid Uid { get; set; }

        /// <summary> 
        ///紀錄識別碼 
        /// </summary> 
        public System.Guid RiskRankUid { get; set; }

        /// <summary> 
        ///投資標的風險收益等級 
        /// </summary> 
        public string ProfitRiskRank { get; set; }

        /// <summary> 
        ///是否有效 
        /// </summary> 
        public string IsEffective { get; set; }

        /// <summary> 
        ///建立人員代號 
        /// </summary> 
        public string CreateUserId { get; set; }

        /// <summary> 
        ///建立時間 
        /// </summary> 
        public Nullable<System.DateTime> CreateTime { get; set; }

        /// <summary> 
        ///修改人員代號 
        /// </summary> 
        public string ModifyUserId { get; set; }

        /// <summary> 
        ///修改時間 
        /// </summary> 
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
