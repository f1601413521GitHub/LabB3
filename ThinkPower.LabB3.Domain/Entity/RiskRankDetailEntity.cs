namespace ThinkPower.LabB3.Domain.Entity
{
    /// <summary>
    /// 投資風險等級明細類別
    /// </summary>
    public class RiskRankDetailEntity : BaseEntity
    {

        /// <summary> 
        ///紀錄識別碼 
        /// </summary> 
        public System.Guid RiskRankUid { get; set; }

        /// <summary> 
        ///投資標的風險收益等級 
        /// </summary> 
        public string ProfitRiskRank { get; set; }

        /// <summary> 
        ///是否有效 
        /// </summary> 
        public string IsEffective { get; set; }

    }
}