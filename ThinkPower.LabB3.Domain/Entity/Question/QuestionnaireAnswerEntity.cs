using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity.Question
{
    /// <summary>
    /// 問卷填答類別
    /// </summary>
    public class QuestionnaireAnswerEntity : BaseEntity
    {
        /// <summary> 
        /// 問卷識別碼
        /// </summary>
        public string QuestUid { get; set; }

        /// <summary>
        /// 問卷答案明細類別集合
        /// </summary>
        public IEnumerable<AnswerDetailEntity> AnswerDetailEntities { get; set; }
    }
}
