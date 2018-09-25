using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity.Question
{
    /// <summary>
    /// 問卷答案明細類別
    /// </summary>
    public class AnswerDetailEntity : BaseEntity
    {
        /// <summary> 
        ///問卷答題識別碼 
        /// </summary> 
        //public System.Guid AnswerUid { get; set; }

        /// <summary> 
        ///問卷題目識別碼 
        /// </summary> 
        public System.Guid QuestionUid { get; set; }

        /// <summary> 
        ///答題計分分數 
        /// </summary> 
        public Nullable<int> Score { get; set; }




        /// <summary>
        /// 題目編號 
        /// </summary> 
        public string QuestionId { get; set; }

        /// <summary> 
        ///答案代碼 
        /// </summary> 
        public string AnswerCode { get; set; }

        /// <summary> 
        ///答題其他說明 
        /// </summary> 
        public string OtherAnswer { get; set; }
    }
}
