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
        #region Private property

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

        #endregion

        #region Public method

        /// <summary>
        /// 進行投資風險評估問卷填答
        /// </summary>
        /// <param name="actionModel">來源資料</param>
        /// <returns>投資風險評估問卷頁面</returns>
        [HttpGet]
        public ActionResult EvaQuest(EvaluationRankActionModel actionModel)
        {
            EvaQuestViewModel evaQuestViewModel = null;
            string validationSummary = String.Empty;

            try
            {
                if (actionModel == null)
                {
                    throw new ArgumentNullException("actionModel");
                }

                RiskEvaQuestionnaireEntity riskEvaQuestEntity = RiskService.GetRiskQuestionnaire(
                    actionModel.QuestId, Session["id"] as string);

                if (riskEvaQuestEntity == null)
                {
                    var ex = new InvalidOperationException("riskEvaQuestEntity not found");
                    ex.Data["QuestId"] = actionModel.QuestId;
                    throw ex;
                }

                evaQuestViewModel = new EvaQuestViewModel()
                {
                    RiskEvaQuestionnaireEntity = riskEvaQuestEntity,
                };
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

            return View(evaQuestViewModel);
        }

        /// <summary>
        /// 執行評估投資風險等級
        /// </summary>
        /// <param name="answer">投資風險評估問卷填答資料</param>
        /// <returns>評估投資風險等級頁面</returns>
        [HttpPost]
        public ActionResult EvaluationRank(FormCollection answer)
        {
            EvaluationRankViewModel evaluationRankViewModel = null;
            string validationSummary = String.Empty;

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
                        UserId = Session["id"] as string,
                        AnswerDetailEntities = ConvertAnswerDetailList(answer),
                    },
                };

                Domain.DTO.RiskEvaResultDTO riskEvaResultDTO = RiskService.EvaluateRiskRank(
                    riskEvaAnswerEntity);

                if (riskEvaResultDTO == null)
                {
                    throw new InvalidOperationException("riskEvaResultDTO not found");
                }

                if ((riskEvaResultDTO.QuestionnaireResultEntity.ValidateFailInfo != null) &&
                    (riskEvaResultDTO.QuestionnaireResultEntity.ValidateFailInfo.Count > 0))
                {
                    return View("EvaQuest", new EvaQuestViewModel()
                    {
                        RiskEvaQuestionnaireEntity = riskEvaResultDTO.RiskEvaQuestionnaireEntity,
                        QuestionnaireResultEntity = riskEvaResultDTO.QuestionnaireResultEntity,
                    });
                }
                else
                {
                    evaluationRankViewModel = new EvaluationRankViewModel()
                    {
                        QuestionnaireResultEntity = riskEvaResultDTO.QuestionnaireResultEntity,
                        RiskRankEntities = riskEvaResultDTO.RiskRankEntities,
                    };
                }
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

            return View(evaluationRankViewModel);
        }

        /// <summary>
        /// 確認接受投資風險評估結果
        /// </summary>
        /// <param name="actionModel">投資風險評估資料</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AcceptRiskRank(SaveRankActionModel actionModel)
        {
            EvaluationRankViewModel evaRankViewModel = null;
            string validationSummary = String.Empty;

            try
            {
                if (actionModel == null)
                {
                    throw new ArgumentNullException("actionModel");
                }

                RiskService.SaveRiskRank(actionModel.QuestAnswerId);

                evaRankViewModel = new EvaluationRankViewModel
                {
                    QuestionnaireResultEntity = new QuestionnaireResultEntity()
                    {
                        QuestionnaireMessage = "風險評估結果儲存成功",
                    }
                };

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

            return View("EvaluationRank", evaRankViewModel);
        }

        #endregion

        #region Private method

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

        /// <summary>
        /// 轉換提示訊息(不可重做風險評估問卷、例外錯誤狀況)
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string ConvertValidateMsgByRiskEvaluation(InvalidOperationException e)
        {
            string validationSummary = String.Empty;
            bool? canUsedRiskEvaluation = e.Data["canUsedRiskEvaluation"] as bool?;

            if ((canUsedRiskEvaluation == null) ||
                canUsedRiskEvaluation.Value)
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

        #endregion
    }
}