using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.DataAccess.Helper;

namespace ThinkPower.LabB3.DataAccess.DAO
{
    
    /// <summary>
    /// 領域物件基底類別
    /// </summary>
    public abstract class BaseDAO
    {
        /// <summary>
        /// SQL資料庫連線物件
        /// </summary>
        public SqlConnection DbConnection { get; set; }

        /// <summary>
        /// 取得資料筆數
        /// </summary>
        public abstract int Count();

        /// <summary>
        /// 呼叫DbHelper取得資料庫連線物件。
        /// </summary>
        /// <returns></returns>
        protected SqlConnection GetConnection()
        {
            //TODO GetConnection
            return DbHelper.GetConnection(ConfigurationManager.AppSettings[""]);
        }
    }
}
