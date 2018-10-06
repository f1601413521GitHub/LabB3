namespace ThinkPower.LabB3.Web.ActionModels
{
    /// <summary>
    /// 投資風險評估問卷填答資料模型類別
    /// </summary>
    public class EvaluationRankActionModel
    {
        /// <summary>
        /// 問卷編號
        /// </summary>
        public string QuestId { get; set; }

        /// <summary> 
        ///問卷答題編號 
        /// </summary> 
        public string QuestAnswerId { get; set; }
    }
}