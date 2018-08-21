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
        /// <summary> 
        ///紀錄識別碼 
        /// </summary> 
        public System.Guid Uid { get; set; }

        /// <summary> 
        ///問卷題目識別碼 
        /// </summary> 
        public System.Guid QuestionUid { get; set; }

        /// <summary> 
        ///答案代碼 
        /// </summary> 
        public string AnswerCode { get; set; }

        /// <summary> 
        ///答案內容描敍 
        /// </summary> 
        public string AnswerContent { get; set; }

        /// <summary> 
        ///備註說明 
        /// </summary> 
        public string Memo { get; set; }

        /// <summary> 
        ///是否答題有輸入說明 
        /// </summary> 
        public string HaveOtherAnswer { get; set; }

        /// <summary> 
        ///答題說明是否為必填 
        /// </summary> 
        public string NeedOtherAnswer { get; set; }

        /// <summary> 
        ///計分分數 
        /// </summary> 
        public Nullable<int> Score { get; set; }

        /// <summary> 
        ///答案項目排序序號 
        /// </summary> 
        public Nullable<int> OrderSn { get; set; }

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
