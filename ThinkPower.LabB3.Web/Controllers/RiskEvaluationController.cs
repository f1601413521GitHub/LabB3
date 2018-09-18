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

            try
            {
                if (answer.Count == 0)
                {
                    throw new ArgumentNullException("answer");
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

                Domain.DTO.RiskEvaResultDTO reuslt = RiskService.EvaluateRiskRank(riskEvaAnswerEntity);

                //TODO: viewModel = ConvertRiskEvaResultDTO(reuslt);

                TempData["FormCollection2"] = JsonConvert.SerializeObject(answerDetailList, Formatting.Indented);
                TempData["FormCollection3"] = JsonConvert.SerializeObject(riskEvaAnswerEntity, Formatting.Indented);
            }
            catch (Exception e)
            {
                //TODO 提供詳細的錯誤資訊
                logger.Error(e);
                statusCode = HttpStatusCode.InternalServerError;
            }

            if (statusCode != null)
            {
                ModelState.AddModelError("",
                    "系統發生錯誤，請於上班時段來電客服中心0800-015-000，造成不便敬請見諒。");
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
            List<AnswerDetailEntity> answerList = new List<AnswerDetailEntity>();

            foreach (string questId in answer["questDefine.QuestionId"].Split(','))
            {
                AnswerDetailEntity answerEntity = new AnswerDetailEntity()
                {
                    QuestionId = questId
                };

                foreach (KeyValuePair<string, string> questAnswer
                    in answer.AllKeys.Where(x => x.EndsWith(questId))
                        .Select(x => new KeyValuePair<string, string>(x, answer[x])))
                {
                    if (questAnswer.Key.StartsWith("answerCode"))
                    {
                        if (questAnswer.Value.StartsWith("true") ||
                            questAnswer.Value.StartsWith("false"))
                        {
                            string[] valueSplit = questAnswer.Value.Split(',');
                            List<string> answerItem = new List<string>();
                            int count = 1;
                            for (int j = 0; j < valueSplit.Length; j++)
                            {
                                if (valueSplit[j] == "true")
                                {
                                    answerItem.Add($"{count}");
                                    count++;
                                    j++;
                                }
                                else
                                {
                                    count++;
                                }
                            }

                            answerEntity.AnswerCode = String.Join(",", answerItem);
                        }
                        else
                        {
                            answerEntity.AnswerCode = questAnswer.Value;
                        }
                    }
                    else if (questAnswer.Key.StartsWith("otherAnswer"))
                    {
                        answerEntity.OtherAnswer = questAnswer.Value;
                    }
                    else
                    {
                        throw new Exception("Key not in condition");
                    }
                }
                answerList.Add(answerEntity);
            }

            return answerList;
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