using ThinkPower.LabB3.Domain.Entity.Question;

namespace ThinkPower.LabB3.Web.ActionModels
{
    /// <summary>
    /// 投資風險評估問卷填答資料模型類別
    /// </summary>
    public class EvaluateAnswerActionModel
    {
        public QuestionnaireAnswerEntity QuestionnaireAnswerEntity { get; set; }
    }
}