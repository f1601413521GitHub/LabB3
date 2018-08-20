using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity
{
    /// <summary>
    /// 領域物件基底類別
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// 識別碼
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// 資料建立人員ID
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 資料建檔時間
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 資料修改人員ID
        /// </summary>
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 資料修改時間
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}
