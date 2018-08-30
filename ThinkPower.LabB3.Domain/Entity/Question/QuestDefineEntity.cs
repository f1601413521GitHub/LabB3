using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity.Question
{
    /// <summary>
    /// 問卷題目定義類別
    /// </summary>
    public class QuestDefineEntity : BaseEntity
    {
        /// <summary>
        /// 問卷識別碼 
        /// </summary> 
        public System.Guid QuestUid { get; set; }

        /// <summary>
        /// 題目編號 
        /// </summary> 
        public string QuestionId { get; set; }

        /// <summary>
        /// 題目內容描述 
        /// </summary> 
        public string QuestionContent { get; set; }

        /// <summary>
        /// 是否必答 
        /// </summary> 
        public string NeedAnswer { get; set; }

        /// <summary>
        /// 可不做答條件 
        /// </summary> 
        public string AllowNaCondition { get; set; }

        /// <summary>
        /// 答題型態 
        /// </summary> 
        public string AnswerType { get; set; }

        /// <summary>
        /// 複選最少答項數 
        /// </summary> 
        public Nullable<int> MinMultipleAnswers { get; set; }

        /// <summary>
        /// 複選最多答項數 
        /// </summary> 
        public Nullable<int> MaxMultipleAnswers { get; set; }

        /// <summary>
        /// 複選限制單一做答條件 
        /// </summary> 
        public string SingleAnswerCondition { get; set; }

        /// <summary>
        /// 計分種類 
        /// </summary> 
        public string CountScoreType { get; set; }

        /// <summary>
        /// 備註說明 
        /// </summary> 
        public string Memo { get; set; }

        /// <summary>
        /// 題目排序序號 
        /// </summary> 
        public Nullable<int> OrderSn { get; set; }

        /// <summary>
        /// 問卷答案定義集合
        /// </summary>
        public IEnumerable<AnswerDefineEntity> AnswerDefineEntities { get; set; }
    }
}
