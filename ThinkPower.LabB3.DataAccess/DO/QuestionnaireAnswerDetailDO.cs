using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.DataAccess.DO
{
    /// <summary>
    /// 問卷答題明細資料物件類別
    /// </summary>
    public class QuestionnaireAnswerDetailDO
    {
        public System.Guid Uid { get; set; }
        public System.Guid AnswerUid { get; set; }
        public System.Guid QuestionUid { get; set; }
        public string AnswerCode { get; set; }
        public string OtherAnswer { get; set; }
        public Nullable<int> Score { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
