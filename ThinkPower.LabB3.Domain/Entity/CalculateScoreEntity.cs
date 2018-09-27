using System.Collections.Generic;
using ThinkPower.LabB3.Domain.Entity.Question;

namespace ThinkPower.LabB3.Domain.Entity
{
    /// <summary>
    /// 問卷得分類別
    /// </summary>
    public class CalculateScoreEntity : BaseEntity
    {
        /// <summary>
        /// 問卷得分
        /// </summary>
        public int ActualScore { get; set; }

        /// <summary>
        /// 問卷填答完整資料(包含問卷題目識別碼與答題分數)
        /// </summary>
        public List<AnswerDetailEntity> FullAnswerDetailList { get; set; }
    }
}