using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 問卷答案項目資料物件類別
    /// </summary>
    public class QuestionAnswerDefineDO
    {
        public System.Guid Uid { get; set; }
        public System.Guid QuestionUid { get; set; }
        public string AnswerCode { get; set; }
        public string AnswerContent { get; set; }
        public string Memo { get; set; }
        public string HaveOtherAnswer { get; set; }
        public string NeedOtherAnswer { get; set; }
        public Nullable<int> Score { get; set; }
        public Nullable<int> OrderSn { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
