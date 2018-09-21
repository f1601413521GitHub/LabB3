using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity.Question
{
    /// <summary>
    /// 問卷填答評分結果類別
    /// </summary>
    public class QuestionnaireResultEntity : BaseEntity
    {
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
        /// 問卷答題檢核失敗的資訊，包含問卷題目定義識別碼與錯誤訊息
        /// </summary>
        public Dictionary<string, string> ValidateFailInfo { get; set; }

        /// <summary>
        ///  問卷答題檢核失敗的問卷編號
        /// </summary>
        public string ValidateFailQuestId { get; set; }

        /// <summary>
        /// 問卷答案明細類別集合
        /// </summary>
        public IEnumerable<AnswerDetailEntity> AnswerDetailEntities { get; set; }
    }
}
