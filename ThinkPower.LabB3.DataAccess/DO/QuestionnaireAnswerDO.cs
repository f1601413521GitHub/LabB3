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
        /// <summary> 
        ///紀錄識別碼 
        /// </summary> 
        public System.Guid Uid { get; set; }

        /// <summary> 
        ///問卷識別碼 
        /// </summary> 
        public System.Guid QuestUid { get; set; }

        /// <summary> 
        ///問卷答題編號 
        /// </summary> 
        public string QuestAnswerId { get; set; }

        /// <summary> 
        ///填寫人員編號 
        /// </summary> 
        public string TesteeId { get; set; }

        /// <summary> 
        ///問卷總分 
        /// </summary> 
        public Nullable<int> QuestScore { get; set; }

        /// <summary> 
        ///問卷得分 
        /// </summary> 
        public Nullable<int> ActualScore { get; set; }

        /// <summary> 
        ///問卷填寫來源代號 
        /// </summary> 
        public string TesteeSource { get; set; }

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
