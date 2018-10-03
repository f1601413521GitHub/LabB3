using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.Entity.Question;

namespace ThinkPower.LabB3.Domain.Entity.Risk
{
    /// <summary>
    /// 投資風險評估問卷類別
    /// </summary>
    public class RiskEvaQuestionnaireEntity : BaseEntity
    {
        /// <summary>
        /// 問卷類別
        /// </summary>
        public QuestionnaireEntity QuestionnaireEntity { get; set; }

        /// <summary>
        /// 可否進行風險評估問卷填答
        /// </summary>
        public bool CanRiskEvaluation { get; set; }
    }
}
