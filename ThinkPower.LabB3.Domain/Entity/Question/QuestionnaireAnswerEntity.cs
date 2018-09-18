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
        /// 問卷填答評分結果類別
        /// </summary>
        public QuestionnaireResultEntity QuestionnaireResultEntity { get; set; }

        /// <summary>
        /// 問卷答案明細類別集合
        /// </summary>
        public IEnumerable<AnswerDetailEntity> AnswerDetailEntity { get; set; }
    }
}
