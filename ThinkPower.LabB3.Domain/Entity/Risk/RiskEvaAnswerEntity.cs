using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.Entity.Question;

namespace ThinkPower.LabB3.Domain.Entity.Risk
{
    /// <summary>
    /// 投資風險評估填答明細類別
    /// </summary>
    public class RiskEvaAnswerEntity : BaseEntity
    {
        /// <summary>
        /// 問卷填答類別
        /// </summary>
        public QuestionnaireAnswerEntity QuestionnaireAnswerEntity { get; set; }
    }
}
