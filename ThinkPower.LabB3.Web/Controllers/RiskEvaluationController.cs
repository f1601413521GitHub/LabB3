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
using ThinkPower.LabB3.Web.ViewModels;

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
        [HttpPost]
        public ActionResult EvaluationRank(FormCollection answer)
        {
            //TODO EvaluationRank 執行評估投資風險等級
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (var item in answer.AllKeys)
            {
                if (item != null)
                {
                    values.Add(item, answer[item]);
                }
            }
            return RedirectToAction("EvaQuest", "Home");
        }

        /// <summary>
        /// 進行投資風險評估問卷填答
        /// </summary>
        /// <param name="actionModel">來源資料</param>
        /// <returns>投資風險評估問卷</returns>
        [HttpPost]
        public ActionResult EvaQuest(EvaluationRankActionModel actionModel)
        {
            HttpStatusCode? statusCode = null;
            RiskEvaQuestionnaireEntity riskEvaQuestEntity = null;
            EvaQuestViewModel evaQuestVM = null;

            if ((actionModel == null) ||
                String.IsNullOrEmpty(actionModel.questId))
            {
                //throw new ArgumentNullException("actionModel");
                statusCode = HttpStatusCode.NotFound;
            }

            try
            {
                if (statusCode == null)
                {
                    riskEvaQuestEntity = RiskService.GetRiskQuestionnaire(actionModel.questId);

                    evaQuestVM = new EvaQuestViewModel()
                    {
                        RiskEvaQuestionnaire = riskEvaQuestEntity,
                    };
                }
            }
            catch (Exception e)
            {
                //TODO 提供詳細的錯誤資訊
                logger.Error(e);
                statusCode = HttpStatusCode.InternalServerError;
            }

            if (statusCode != null)
            {
                //TODO ViewModel Error 機制
                ModelState.AddModelError("",
                    "系統發生錯誤，請於上班時段來電客服中心0800-015-000，造成不便敬請見諒。");
            }
            else if (!evaQuestVM.RiskEvaQuestionnaire.CanUseRiskEvaluation)
            {
                ModelState.AddModelError("",
                    "您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View(evaQuestVM);
        }
    }
}