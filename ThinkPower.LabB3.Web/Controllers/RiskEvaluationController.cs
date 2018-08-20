using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThinkPower.LabB3.Web.ActionModels;

namespace ThinkPower.LabB3.Web.Controllers
{
    /// <summary>
    /// 投資風險評估
    /// </summary>
    public class RiskEvaluationController : Controller
    {
        /// <summary>
        /// 確認接受投資風險評估結果
        /// </summary>
        /// <param name="actionModel">投資風險評估資料</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AcceptRiskRank(SaveRankActionModel actionModel)
        {
            //TODO AcceptRiskRank 確認接受投資風險評估結果
            return View();
        }

        /// <summary>
        /// 執行評估投資風險等級
        /// </summary>
        /// <param name="answer">投資風險評估問卷填答資料</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EvaluationRank(FormCollection answer)
        {
            //TODO EvaluationRank 執行評估投資風險等級
            return View();
        }

        /// <summary>
        /// 進行投資風險評估問卷填答
        /// </summary>
        /// <param name="actionModel">來源資料</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EvaQuest(EvaluationRankActionModel actionModel)
        {
            //TODO EvaQuest 進行投資風險評估問卷填答
            return View();
        }
    }
}