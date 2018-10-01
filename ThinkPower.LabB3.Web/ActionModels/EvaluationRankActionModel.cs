namespace ThinkPower.LabB3.Web.ActionModels
{
    /// <summary>
    /// 投資風險評估問卷填答資料模型類別
    /// </summary>
    public class EvaluationRankActionModel
    {
        /// <summary>
        /// 問卷編號
        /// 問卷1:FNDRE001
        /// 問卷2:FNDRE002
        /// </summary>
        public string questId { get; set; }


        /// <summary> 
        ///問卷答題編號 
        /// </summary> 
        public string QuestAnswerId { get; set; }
    }
}