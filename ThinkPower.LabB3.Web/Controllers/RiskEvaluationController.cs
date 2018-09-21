using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThinkPower.LabB3.Domain.Entity.Question;
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

        [HttpGet]
        /// <summary>
        /// 確認接受投資風險評估結果
        /// </summary>
        /// <param name="actionModel">投資風險評估資料</param>
        /// <returns></returns>
        public ActionResult AcceptRiskRank(SaveRankActionModel actionModel)
        {
            //TODO AcceptRiskRank 確認接受投資風險評估結果
            return View();
        }

        [HttpPost]
        /// <summary>
        /// 執行評估投資風險等級
        /// </summary>
        /// <param name="answer">投資風險評估問卷填答資料</param>
        /// <returns>評估投資風險等級頁面</returns>
        public ActionResult EvaluationRank(FormCollection answer)
        {
            HttpStatusCode? statusCode = null;
            Domain.DTO.RiskEvaResultDTO reuslt = null;

            try
            {
                if (answer.Count == 0)
                {
                    throw new ArgumentNullException("answer");
                }
                else if (answer["questEntity.Uid"] == null)
                {
                    throw new InvalidOperationException("questEntityUid not found");
                }

                List<AnswerDetailEntity> answerDetailList = GetAnswerDetailList(answer);

                if (answerDetailList == null || answerDetailList.Count == 0)
                {
                    throw new InvalidOperationException("answerDetailList not found");
                }
                RiskEvaAnswerEntity riskEvaAnswerEntity = new RiskEvaAnswerEntity()
                {
                    QuestionnaireAnswerEntity = new QuestionnaireAnswerEntity()
                    {
                        QuestUid = answer["questEntity.Uid"],
                        QuestionnaireResultEntity = new QuestionnaireResultEntity(),
                        AnswerDetailEntities = answerDetailList,
                    },
                };

                reuslt = RiskService.EvaluateRiskRank(riskEvaAnswerEntity);

                if (reuslt.QuestionnaireResultEntity.ValidateFailInfo != null &&
                    reuslt.QuestionnaireResultEntity.ValidateFailInfo.Count > 0)
                {
                    return View("EvaQuest", new EvaQuestViewModel()
                    {
                        RiskEvaQuestionnaire = reuslt.RiskEvaQuestionnaire,
                        QuestionnaireResultEntity = reuslt.QuestionnaireResultEntity,
                    });
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
                ModelState.AddModelError("", "系統發生錯誤，請於上班時段來電客服中心0800-015-000，" +
                    "造成不便敬請見諒。");
            }
            else if (true)
            {
                ModelState.AddModelError("", "您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View();
        }

        /// <summary>
        /// 取得問卷答案明細集合
        /// </summary>
        /// <param name="answer">問卷填答字典</param>
        /// <returns>問卷答案明細集合</returns>
        private List<AnswerDetailEntity> GetAnswerDetailList(FormCollection answer)
        {
            Dictionary<string, string> answerKeyValues = new Dictionary<string, string>();
            foreach (string key in answer.AllKeys)
            {
                answerKeyValues.Add(key, answer[key]);
            }

            //TODO TEST FormCollection
            TempData["FormCollection"] = JsonConvert.SerializeObject(answerKeyValues);


            List<AnswerDetailEntity> answerDetailList = new List<AnswerDetailEntity>();
            foreach (string questId in answerKeyValues.Where(x => x.Key.StartsWith("questId")).Select(x => x.Value))
            {
                IEnumerable<KeyValuePair<string, string>> questAnswerList =
                    answerKeyValues.Where(x => x.Key.EndsWith(questId));

                string questAnswerType =
                    questAnswerList.FirstOrDefault(x => x.Key.StartsWith("questAnswerType")).Value;

                string questUid =
                    questAnswerList.FirstOrDefault(x => x.Key.StartsWith("questUid")).Value;

                if (String.IsNullOrEmpty(questUid) ||
                    !Guid.TryParse(questUid, out Guid tempQuestUid))
                {
                    throw new InvalidOperationException("questUid not found");
                }

                foreach (KeyValuePair<string, string> answerUid
                    in questAnswerList.Where(x => x.Key.StartsWith("answerUid")))
                {
                    if (String.IsNullOrEmpty(answerUid.Value) ||
                        !Guid.TryParse(answerUid.Value, out Guid tempAnswerUid))
                    {
                        throw new InvalidOperationException("answerUid not found");
                    }

                    AnswerDetailEntity answerDetailEntity = new AnswerDetailEntity()
                    {
                        QuestionId = questId,
                        QuestionUid = tempQuestUid,
                        AnswerUid = tempAnswerUid,
                    };

                    string answerIndex = answerUid.Key.Split('-')[1];

                    KeyValuePair<string, string> answerCode = new KeyValuePair<string, string>();

                    if (questAnswerType == "S")
                    {
                        answerCode = questAnswerList
                            .FirstOrDefault(x => x.Key.StartsWith($"answerCode-radio"));

                        if (answerCode.Key != null &&
                            answerCode.Value.Split(',')[1] == answerIndex)
                        {
                            answerDetailEntity.AnswerCode = answerCode.Value.Split(',')[0];
                        }
                    }
                    else if (questAnswerType == "M")
                    {
                        answerCode = questAnswerList
                            .FirstOrDefault(x => x.Key.StartsWith($"answerCode-{answerIndex}"));

                        if (answerCode.Key != null &&
                            answerCode.Value != "false")
                        {
                            answerDetailEntity.AnswerCode = answerCode.Value.Split(',')[0];
                        }

                        KeyValuePair<string, string> otherAnswer = questAnswerList
                            .FirstOrDefault(x => x.Key.StartsWith($"otherAnswer-{answerIndex}"));

                        if (otherAnswer.Key != null)
                        {
                            answerDetailEntity.OtherAnswer = otherAnswer.Value;
                        }
                    }
                    else if (questAnswerType == "F")
                    {
                        answerCode = questAnswerList
                            .FirstOrDefault(x => x.Key.StartsWith("answerCode-text"));

                        if (answerCode.Key != null)
                        {
                            answerDetailEntity.OtherAnswer = answerCode.Value;
                        }
                    }

                    answerDetailList.Add(answerDetailEntity);
                }
            }

            //TODO TEST FormCollection2
            TempData["FormCollection2"] = JsonConvert.SerializeObject(answerDetailList);

            return answerDetailList;
        }

        [HttpPost]
        /// <summary>
        /// 進行投資風險評估問卷填答
        /// </summary>
        /// <param name="actionModel">來源資料</param>
        /// <returns>投資風險評估問卷頁面</returns>
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
                ModelState.AddModelError("", "系統發生錯誤，請於上班時段來電客服中心0800-015-000，" +
                    "造成不便敬請見諒。");
            }
            else if (!evaQuestVM.RiskEvaQuestionnaire.CanUseRiskEvaluation)
            {
                ModelState.AddModelError("", "您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View(evaQuestVM);
        }
    }
}