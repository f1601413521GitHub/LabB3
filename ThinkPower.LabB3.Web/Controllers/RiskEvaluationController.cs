using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
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

        /// <summary>
        /// 系統錯誤提示訊息
        /// </summary>
        private readonly string _systemErrorMsg = "系統發生錯誤，請於上班時段來電客服中心0800-015-000，造成不便敬請見諒。";

        /// <summary>
        /// 己有生效的投資風險評估紀錄提示訊息
        /// </summary>
        private readonly string _existEffectiveAnRiskEvaluationMsg = "您己有生效的投資風險評估紀錄，無法重新進行風險評估。";






        /// <summary>
        /// 進行投資風險評估問卷填答
        /// </summary>
        /// <param name="actionModel">來源資料</param>
        /// <returns>投資風險評估問卷頁面</returns>
        [HttpGet]
        public ActionResult EvaQuest(EvaluationRankActionModel actionModel)
        {
            EvaQuestViewModel viewModel = null;
            string validationSummary = String.Empty;
            string userId = Session["id"] as string;

            try
            {
                if (actionModel == null)
                {
                    throw new ArgumentNullException("actionModel");
                }

                //TODO review
                if (!String.IsNullOrEmpty(actionModel.QuestAnswerId))
                {
                    Domain.DTO.RiskEvaResultDTO riskEvaResult = RiskService.
                        GetRiskResult(actionModel.QuestAnswerId);

                    if (riskEvaResult == null)
                    {
                        var exception = new InvalidOperationException("riskEvaResult not found");
                        exception.Data["QuestAnswerId"] = actionModel.QuestAnswerId;

                        throw exception;
                    }

                    viewModel = new EvaQuestViewModel()
                    {
                        RiskEvaQuestionnaireEntity = riskEvaResult.RiskEvaQuestionnaire,
                        QuestionnaireResultEntity = riskEvaResult.QuestionnaireResultEntity,
                    };
                }
                else
                {
                    RiskService.UserId = userId;
                    RiskEvaQuestionnaireEntity riskEvaQuestEntity = RiskService.GetRiskQuestionnaire(
                        actionModel.QuestId);

                    if (riskEvaQuestEntity == null)
                    {
                        var ex = new InvalidOperationException("riskEvaQuestEntity not found");
                        ex.Data["QuestId"] = actionModel.QuestId;
                        throw ex;
                    }

                    viewModel = new EvaQuestViewModel()
                    {
                        RiskEvaQuestionnaireEntity = riskEvaQuestEntity,
                    };
                }

                //TODO review property CanRiskEvaluation
            }
            catch (InvalidOperationException e)
            {
                validationSummary = ConvertValidateMsgByRiskEvaluation(e);
            }
            catch (Exception e)
            {
                logger.Error(e);
                validationSummary = _systemErrorMsg;
            }

            if (!String.IsNullOrEmpty(validationSummary))
            {
                ModelState.AddModelError("", validationSummary);
            }

            return View(viewModel);
        }

        /// <summary>
        /// 執行評估投資風險等級
        /// </summary>
        /// <param name="answer">投資風險評估問卷填答資料</param>
        /// <returns>評估投資風險等級頁面</returns>
        [HttpPost]
        public ActionResult EvaluationRank(FormCollection answer)
        {
            HttpStatusCode? statusCode = null;
            Domain.DTO.RiskEvaResultDTO evaluateResult = null;
            EvaluationRankViewModel evaRankViewModel = null;

            try
            {
                if (!answer.HasKeys())
                {
                    throw new ArgumentNullException("answer");
                }

                RiskEvaAnswerEntity riskEvaAnswerEntity = new RiskEvaAnswerEntity()
                {
                    QuestionnaireAnswerEntity = new QuestionnaireAnswerEntity()
                    {
                        QuestUid = answer["questEntity.Uid"],
                        AnswerDetailEntities = ConvertAnswerDetailList(answer),
                    },
                };

                evaluateResult = RiskService.EvaluateRiskRank(riskEvaAnswerEntity);

                if (evaluateResult == null)
                {
                    throw new InvalidOperationException("evaluateResult not found");
                }



                if (evaluateResult.QuestionnaireResultEntity != null)
                {
                    if (evaluateResult.QuestionnaireResultEntity.ValidateFailInfo.Count > 0)
                    {
                        return View("EvaQuest", new EvaQuestViewModel()
                        {
                            RiskEvaQuestionnaireEntity = evaluateResult.RiskEvaQuestionnaire,
                            QuestionnaireResultEntity = evaluateResult.QuestionnaireResultEntity,
                        });
                    }
                    else
                    {
                        evaRankViewModel = new EvaluationRankViewModel()
                        {
                            QuestionnaireResultEntity = evaluateResult.QuestionnaireResultEntity,
                        };

                        if (evaluateResult.RiskEvaluationEntity != null)
                        {
                            evaRankViewModel.RiskEvaluationResult = evaluateResult.RiskEvaluationEntity;
                            evaRankViewModel.RiskRankEntities = evaluateResult.RiskRankEntities;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("evaluateResult.QuestionnaireResultEntity not found");
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
            else if (!evaluateResult.RiskEvaQuestionnaire.CanRiskEvaluation)
            {
                ModelState.AddModelError("", "您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View(evaRankViewModel);
        }

        /// <summary>
        /// 確認接受投資風險評估結果
        /// </summary>
        /// <param name="actionModel">投資風險評估資料</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AcceptRiskRank(SaveRankActionModel actionModel)
        {
            HttpStatusCode? statusCode = null;
            RiskEvaluationEntity riskEvaEntity = null;
            EvaluationRankViewModel evaRankViewModel = null;

            try
            {
                if (actionModel == null)
                {
                    statusCode = HttpStatusCode.NotFound;
                }

                RiskService.SaveRiskRank(actionModel.QuestAnswerId);
                riskEvaEntity = RiskService.Get(actionModel.QuestAnswerId);

                if (riskEvaEntity == null)
                {
                    throw new InvalidOperationException("riskEvaEntity not found");
                }

                if (riskEvaEntity.IsUsed == "N")
                {
                    evaRankViewModel = new EvaluationRankViewModel
                    {
                        QuestionnaireResultEntity = new QuestionnaireResultEntity()
                        {
                            QuestionnaireMessage = "風險評估結果儲存成功",
                        }
                    };
                }

            }
            catch (Exception e)
            {
                logger.Error(e);
                statusCode = HttpStatusCode.InternalServerError;
            }

            if (statusCode != null)
            {
                ModelState.AddModelError("", "系統發生錯誤，請於上班時段來電客服中心0800-015-000，" +
                    "造成不便敬請見諒。");
            }
            else if (riskEvaEntity.IsUsed == "Y")
            {
                ModelState.AddModelError("", "您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View("EvaluationRank", evaRankViewModel);
        }










        /// <summary>
        /// 轉換提示訊息(不可重做風險評估問卷、例外錯誤狀況)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string ConvertValidateMsgByRiskEvaluation(InvalidOperationException e)
        {
            string validationSummary = String.Empty;
            bool? canUsedRiskEvaluation = e.Data["canUsedRiskEvaluation"] as bool?;

            if ((canUsedRiskEvaluation == null) || canUsedRiskEvaluation.Value)
            {
                logger.Error(e);
                validationSummary = _systemErrorMsg;
            }
            else if (!canUsedRiskEvaluation.Value)
            {
                validationSummary = _existEffectiveAnRiskEvaluationMsg;
            }

            return validationSummary;
        }

        /// <summary>
        /// 取得問卷答案明細集合
        /// </summary>
        /// <param name="answer">問卷填答字典</param>
        /// <returns>問卷答案明細集合</returns>
        private List<AnswerDetailEntity> ConvertAnswerDetailList(FormCollection answer)
        {
            string questionnaireUid = answer["questEntity.Uid"];
            if (questionnaireUid == null)
            {
                throw new InvalidOperationException("questionnaireUid not found");
            }

            string questIdList = answer["questDefine.QuestionId"];
            if (questIdList == null)
            {
                throw new InvalidOperationException("questIdList not found");
            }

            List<AnswerDetailEntity> answerDetailEntities = new List<AnswerDetailEntity>();

            foreach (string questId in questIdList.Split(','))
            {
                IEnumerable<string> answerCodeKeys = answer.AllKeys
                    .Where(x => x.StartsWith($"answer-{questId}-code"));

                if (answerCodeKeys.Count() == 0)
                {
                    continue;
                }

                if (answerCodeKeys.Count() > 1)
                {
                    throw new InvalidOperationException("answerCodeKeys not the only");
                }

                string[] answerCodeList = answer[answerCodeKeys.First()].Split(',');

                IEnumerable<string> answerOtherKeys = answer.AllKeys
                    .Where(x => x.StartsWith($"answer-{questId}-other"));

                foreach (string answerCode in answerCodeList)
                {
                    if (answerOtherKeys.Count() != 0 &&
                        answerOtherKeys.Any(x => x == $"answer-{questId}-other-{answerCode}"))
                    {
                        IEnumerable<string> answerOtherList = answerOtherKeys
                            .Where(x => x == $"answer-{questId}-other-{answerCode}");

                        if (answerOtherList.Count() != 1)
                        {
                            throw new InvalidOperationException("answerOtherList not the only");
                        }

                        string answerOther = answer[answerOtherList.First()];

                        answerDetailEntities.Add(new AnswerDetailEntity()
                        {
                            QuestionId = questId,
                            AnswerCode = answerCode,
                            OtherAnswer = answerOther,
                        });
                    }
                    else
                    {
                        answerDetailEntities.Add(new AnswerDetailEntity()
                        {
                            QuestionId = questId,
                            AnswerCode = answerCode,
                        });
                    }
                }
            }

            return answerDetailEntities;
        }
    }
}