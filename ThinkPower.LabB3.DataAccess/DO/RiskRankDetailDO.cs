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
        public System.Guid Uid { get; set; }
        public System.Guid RiskRankUid { get; set; }
        public string ProfitRiskRank { get; set; }
        public string IsEffective { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
