using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThinkPower.LabB3.Domain.Entity.Risk;
using ThinkPower.LabB3.Domain.Service;
using ThinkPower.LabB3.Web.ActionModels;

namespace ThinkPower.LabB3.Web.Controllers
{
    /// <summary>
    /// 投資風險評估
    /// </summary>
    public class RiskEvaluationController : Controller
    {
        /// <summary>
        /// NLog Object
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 投資風險評估服務 隱藏欄位
        /// </summary>
        private RiskEvaluationService _riskService;

        /// <summary>
        /// 投資風險評估服務
        /// </summary>
        private RiskEvaluationService RiskService
        {
            get
            {
                if (_riskService == null)
                {
                    _riskService = new RiskEvaluationService();
                }

                return _riskService;
            }
        }

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
        /// <returns>投資風險評估問卷</returns>
        [HttpPost]
        public ActionResult EvaQuest(EvaluationRankActionModel actionModel)
        {
            RiskEvaQuestionnaireEntity riskEvaQuestEntity = null;
            HttpStatusCode? statusCode = null;

            if (actionModel == null)
            {
                //throw new ArgumentNullException("actionModel");
                statusCode = statusCode ?? HttpStatusCode.NotFound;
            }
            else if (String.IsNullOrEmpty(actionModel.questId))
            {
                statusCode = statusCode ?? HttpStatusCode.NotFound;
            }

            try
            {
                if (statusCode == null)
                {
                    riskEvaQuestEntity = RiskService.GetRiskQuestionnaire(actionModel.questId);

                    if (riskEvaQuestEntity == null)
                    {
                        statusCode = statusCode ?? HttpStatusCode.NotFound;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                statusCode = statusCode ?? HttpStatusCode.InternalServerError;
            }

            if (statusCode != null)
            {
                //return new HttpStatusCodeResult((int)statusCode);
                return Content($"系統發生錯誤，請於上班時段來電客服中心0800-015-000，造成不便敬請見諒。");
                //return Content($"您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View(riskEvaQuestEntity);
        }
    }
}