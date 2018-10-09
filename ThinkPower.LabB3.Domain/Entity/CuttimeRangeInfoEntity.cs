using System;

namespace ThinkPower.LabB3.Domain.Entity
{
    /// <summary>
    /// 投資風險評估切點時間範圍類別
    /// </summary>
    public class CuttimeRangeInfoEntity
    {
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}