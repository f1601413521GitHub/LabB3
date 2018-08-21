using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 問卷題目資料物件類別
    /// </summary>
    public class QuestionDefineDO
    {
        public System.Guid Uid { get; set; }
        public System.Guid QuestUid { get; set; }
        public string QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string NeedAnswer { get; set; }
        public string AllowNaCondition { get; set; }
        public string AnswerType { get; set; }
        public Nullable<int> MinMultipleAnswers { get; set; }
        public Nullable<int> MaxMultipleAnswers { get; set; }
        public string SingleAnswerCondition { get; set; }
        public string CountScoreType { get; set; }
        public string Memo { get; set; }
        public Nullable<int> OrderSn { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
