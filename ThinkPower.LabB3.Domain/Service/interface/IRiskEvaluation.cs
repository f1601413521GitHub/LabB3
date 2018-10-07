using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.DTO;
using ThinkPower.LabB3.Domain.Entity.Risk;

namespace ThinkPower.LabB3.Domain.Service.Interface
{
    /// <summary>
    /// 投資風險評估服務公開介面
    /// </summary>
    interface IRiskEvaluation
    {
        /// <summary>
        /// 評估投資風險等級
        /// </summary>
        /// <param name="answer">風險評估填答資料</param>
        /// <returns></returns>
        RiskEvaResultDTO EvaluateRiskRank(RiskEvaAnswerEntity answer);

        /// <summary>
        /// 依紀錄識別碼取得風險評估資料
        /// </summary>
        /// <param name="uid">紀錄識別碼</param>
        /// <returns></returns>
        RiskEvaluationEntity Get(string uid);

        /// <summary>
        /// 取得風險評估問卷資料
        /// </summary>
        /// <param name="questId">問卷編號</param>
        /// <param name="userId">用戶ID</param>
        /// <returns></returns>
        RiskEvaQuestionnaireEntity GetRiskQuestionnaire(string questId, string userId);

        /// <summary>
        /// 取得暫存的風險評估資料
        /// </summary>
        /// <param name="key">暫存資料識別碼</param>
        /// <returns></returns>
        RiskEvaResultDTO GetRiskResult(string key);

        /// <summary>
        /// 依投資風險屬性取得可投資的風險等級項目
        /// </summary>
        /// <param name="riskRankKind">投資風險屬性</param>
        /// <returns></returns>
        IEnumerable<string> RiskRank(string riskRankKind);

        /// <summary>
        /// 儲存評估投資風險評估結果資料
        /// </summary>
        /// <param name="riskResultId">風險評估結果識別代號</param>
        void SaveRiskRank(string riskResultId); 
    }
}
