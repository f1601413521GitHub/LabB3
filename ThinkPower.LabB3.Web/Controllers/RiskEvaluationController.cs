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
                            RiskEvaQuestionnaire = evaluateResult.RiskEvaQuestionnaire,
                            QuestionnaireResultEntity = evaluateResult.QuestionnaireResultEntity,
                        });
                    }
                    else if (evaluateResult.RiskEvaluationEntity != null)
                    {
                        evaRankViewModel = new EvaluationRankViewModel()
                        {
                            RiskEvaluationResult = evaluateResult.RiskEvaluationEntity,
                            RiskRankEntryList = RiskService.RiskRank(evaluateResult.RiskEvaluationEntity.RiskAttribute),
                            QuestionnaireResultEntity = evaluateResult.QuestionnaireResultEntity,
                        };
                    }
                    else
                    {
                        evaRankViewModel = new EvaluationRankViewModel()
                        {
                            QuestionnaireResultEntity = evaluateResult.QuestionnaireResultEntity,
                        };
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
            else if (!evaluateResult.RiskEvaQuestionnaire.CanUseRiskEvaluation)
            {
                ModelState.AddModelError("", "您己有生效的投資風險評估紀錄，無法重新進行風險評估。");
            }

            return View(evaRankViewModel);
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

        [HttpGet]
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