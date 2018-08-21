using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 問卷答題資料物件類別
    /// </summary>
    public class QuestionnaireAnswerDO
    {
        public System.Guid Uid { get; set; }
        public System.Guid QuestUid { get; set; }
        public string QuestAnswerId { get; set; }
        public string TesteeId { get; set; }
        public Nullable<int> QuestScore { get; set; }
        public Nullable<int> ActualScore { get; set; }
        public string TesteeSource { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
