using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 投資風險等級資料物件類別
    /// </summary>
    public class RiskRankDO
    {
        public System.Guid Uid { get; set; }
        public string RiskEvaId { get; set; }
        public Nullable<int> MinScore { get; set; }
        public Nullable<int> MaxScore { get; set; }
        public string RiskRankKind { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
