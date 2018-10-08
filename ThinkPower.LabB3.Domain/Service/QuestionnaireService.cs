using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Service.Interface;
using ThinkPower.LabB3.DataAccess.DAO;
using ThinkPower.LabB3.DataAccess.DO;
using NLog;
using System.Runtime.ExceptionServices;
using System.Configuration;
using Newtonsoft.Json;
using System.Transactions;
using ThinkPower.LabB3.Domain.Entity;

namespace ThinkPower.LabB3.Domain.Service
{
    /// <summary>
    /// 問卷服務
    /// </summary>
    public class QuestionnaireService : IQuestionnaire
    {
        #region Private property

        /// <summary>
        /// NLog Object
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 問卷填寫來源代碼
        /// </summary>
        private readonly string _testeeSource = "LabB3";

        #endregion

        #region Public method

        /// <summary>
        /// 取得有效的問卷資料
        /// </summary>
        /// <param name="id">問卷編號</param>
        /// <returns>有效的問卷資料</returns>
        public QuestionnaireEntity GetActiveQuestionnaire(string id)
        {
            QuestionnaireEntity questionEntity = null;

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            QuestionnaireDO questionDO = new QuestionnaireDAO().GetActiveQuestionniare(id);

            if (questionDO == null)
            {
                var ex = new InvalidOperationException($"questionnaireDO not found");
                ex.Data["id"] = id;
                throw ex;
            }

            IEnumerable<QuestDefineEntity> questDefineEntities = GetQuestDefineEntities(
                questionDO.Uid);

            questionEntity = new QuestionnaireEntity()
            {
                Uid = questionDO.Uid,
                QuestId = questionDO.QuestId,
                Version = questionDO.Version,
                Kind = questionDO.Kind,
                Name = questionDO.Name,
                Memo = questionDO.Memo,
                Ondate = questionDO.Ondate,
                Offdate = questionDO.Offdate,
                NeedScore = questionDO.NeedScore,
                QuestScore = questionDO.QuestScore,
                ScoreKind = questionDO.ScoreKind,
                HeadBackgroundImg = questionDO.HeadBackgroundImg,
                HeadDescription = questionDO.HeadDescription,
                FooterDescription = questionDO.FooterDescription,
                CreateUserId = questionDO.CreateUserId,
                CreateTime = questionDO.CreateTime,
                ModifyUserId = questionDO.ModifyUserId,
                ModifyTime = questionDO.ModifyTime,

                QuestDefineEntities = questDefineEntities,
            };

            return questionEntity;
        }

        /// <summary>
        /// 取得問卷資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷資料</returns>
        public QuestionnaireEntity GetQuestionnaire(string uid)
        {
            QuestionnaireEntity questionEntity = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }


            QuestionnaireDO questionDO = new QuestionnaireDAO().GetQuestionnaire(uid);


            // TODO 1008 OK 抽出去 RiskEva Calculate 
            //DateTime currentTiem = DateTime.Now;

            if ((questionDO == null)
                //||
                //(questionDO.Ondate >= currentTiem) ||
                //((questionDO.Offdate != null) && (questionDO.Offdate <= currentTiem))
                )
            {
                var ex = new InvalidOperationException($"questionnaireDO not found");
                ex.Data["uid"] = uid;
                throw ex;
            }

            //IEnumerable<QuestDefineEntity> questDefineEntities = GetQuestDefineEntities(
            //    questionDO.Uid);

            questionEntity = new QuestionnaireEntity()
            {
                Uid = questionDO.Uid,
                QuestId = questionDO.QuestId,
                Version = questionDO.Version,
                Kind = questionDO.Kind,
                Name = questionDO.Name,
                Memo = questionDO.Memo,
                Ondate = questionDO.Ondate,
                Offdate = questionDO.Offdate,
                NeedScore = questionDO.NeedScore,
                QuestScore = questionDO.QuestScore,
                ScoreKind = questionDO.ScoreKind,
                HeadBackgroundImg = questionDO.HeadBackgroundImg,
                HeadDescription = questionDO.HeadDescription,
                FooterDescription = questionDO.FooterDescription,
                CreateUserId = questionDO.CreateUserId,
                CreateTime = questionDO.CreateTime,
                ModifyUserId = questionDO.ModifyUserId,
                ModifyTime = questionDO.ModifyTime,

                QuestDefineEntities = null,
            };

            return questionEntity;
        }

        /// <summary>
        /// 計算問卷填答得分
        /// </summary>
        /// <param name="answer">問卷填答資料</param>
        /// <returns>問卷填答評分結果</returns>
        public QuestionnaireResultEntity Calculate(QuestionnaireAnswerEntity answer)
        {
            QuestionnaireResultEntity questionResultEntity = null;
            QuestionnaireAnswerDO questionAnswerDO = null;

            if (answer == null)
            {
                throw new ArgumentNullException("沒有提供問卷填答資料");
            }

            // TODO 1008 OK 補判斷問卷是否有效(onDate/offDate), 若有效取出完整的問卷題目+答題定義資料
            QuestionnaireEntity questionEntity = GetQuestionnaire(answer.QuestUid);

            if (questionEntity == null)
            {
                var ex = new InvalidOperationException("問卷資料不存在或沒有有效的問卷資料");
                ex.Data["QuestionUis"] = answer.QuestUid;
                throw ex;
            }

            DateTime currentTime = DateTime.Now;

            if ((questionEntity.Ondate >= currentTime) ||
                ((questionEntity.Offdate != null) && (questionEntity.Offdate <= currentTime)))
            {
                var ex = new InvalidOperationException($"問卷資料不存在或沒有有效的問卷資料");
                ex.Data["QuestionUis"] = answer.QuestUid;
                throw ex;
            }

            questionEntity.QuestDefineEntities = GetQuestDefineEntities(questionEntity.Uid);


            Dictionary<string, string> validateResult = ValidateRule(answer, questionEntity);

            if (validateResult == null)
            {
                throw new InvalidOperationException("validateResult not found");
            }

            Dictionary<string, string> riskResult = null;
            string dialogMsg = String.Empty;

            if (validateResult.Count == 0)
            {
                CalculateScoreEntity calculateResult = CalculateScore(answer, questionEntity);

                if (calculateResult == null)
                {
                    throw new InvalidOperationException("calculateResult not found");
                }

                riskResult = calculateResult.FullAnswerDetailList.
                        Where(x => !String.IsNullOrEmpty(x.AnswerCode)).
                        GroupBy(x => x.QuestionId).
                        Select(group => new
                        {
                            group.Key,
                            Value = String.Join(",", group.Select(item => item.AnswerCode))
                        }).ToDictionary(x => x.Key, x => x.Value);

                if (riskResult == null)
                {
                    throw new InvalidOperationException("riskResult not found");
                }

                questionAnswerDO = SaveQuestionAnswer(questionEntity, calculateResult,
                    answer.UserId);

                if (questionAnswerDO == null)
                {
                    throw new InvalidOperationException("questionAnswerDO not found");
                }

                if (questionEntity.NeedScore != "Y")
                {
                    dialogMsg = "您的問卷己填答完畢，謝謝您的參與";
                }
            }


            questionResultEntity = new QuestionnaireResultEntity()
            {
                QuestionnaireEntity = questionEntity,
                AnswerDetailEntities = answer.AnswerDetailEntities,
                ValidateFailInfo = validateResult,
                RiskResult = riskResult,
                QuestionnaireMessage = dialogMsg,
            };

            if (questionAnswerDO != null)
            {
                questionResultEntity.Uid = questionAnswerDO.Uid;
                questionResultEntity.QuestUid = questionAnswerDO.QuestUid;
                questionResultEntity.QuestAnswerId = questionAnswerDO.QuestAnswerId;
                questionResultEntity.TesteeId = questionAnswerDO.TesteeId;
                questionResultEntity.QuestScore = questionAnswerDO.QuestScore;
                questionResultEntity.ActualScore = questionAnswerDO.ActualScore;
                questionResultEntity.TesteeSource = questionAnswerDO.TesteeSource;
                questionResultEntity.CreateUserId = questionAnswerDO.CreateUserId;
                questionResultEntity.CreateTime = questionAnswerDO.CreateTime;
                questionResultEntity.ModifyUserId = questionAnswerDO.ModifyUserId;
                questionResultEntity.ModifyTime = questionAnswerDO.ModifyTime;
            }

            return questionResultEntity;
        }

        #endregion

        #region Private method

        /// <summary>
        /// 取得問卷題目定義資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷題目定義資料</returns>
        private IEnumerable<QuestDefineEntity> GetQuestDefineEntities(Guid uid)
        {
            List<QuestDefineEntity> questDefineEntities = null;

            IEnumerable<QuestionDefineDO> questDefineList = new QuestionDefineDAO().
                GetQuestionDefineList(uid);

            if ((questDefineList == null) ||
                (questDefineList.Count() == 0))
            {
                var ex = new InvalidOperationException($"questDefineList not found");
                ex.Data["questionnaireUid"] = uid;
                throw ex;
            }

            questDefineEntities = ConvertQuestDefineEntity(questDefineList).ToList();

            QuestionAnswerDefineDAO answerDefineDAO = new QuestionAnswerDefineDAO();

            IEnumerable<QuestionAnswerDefineDO> answerDefineList = null;

            foreach (QuestDefineEntity questDefineEntity in questDefineEntities)
            {
                answerDefineList = null;
                answerDefineList = answerDefineDAO.GetAnswerDefineList(questDefineEntity.Uid);

                if ((answerDefineList == null) ||
                    (answerDefineList.Count() == 0))
                {
                    var ex = new InvalidOperationException($"answerDefineList not found");
                    ex.Data["questDefineUid"] = questDefineEntity.Uid;
                    throw ex;
                }

                questDefineEntity.AnswerDefineEntities = ConvertAnswerDefineEntity(answerDefineList);
            }

            return questDefineEntities;
        }

        /// <summary>
        /// 轉換問卷題目資料
        /// </summary>
        /// <param name="questionDefineList">問卷題目資料</param>
        /// <returns>問卷題目定義資料</returns>
        private IEnumerable<QuestDefineEntity> ConvertQuestDefineEntity(IEnumerable<QuestionDefineDO>
            questionDefineList)
        {
            return questionDefineList.Select(x => new QuestDefineEntity
            {
                Uid = x.Uid,
                QuestUid = x.QuestUid,
                QuestionId = x.QuestionId,
                QuestionContent = x.QuestionContent,
                NeedAnswer = x.NeedAnswer,
                AllowNaCondition = x.AllowNaCondition,
                AnswerType = x.AnswerType,
                MinMultipleAnswers = x.MinMultipleAnswers,
                MaxMultipleAnswers = x.MaxMultipleAnswers,
                SingleAnswerCondition = x.SingleAnswerCondition,
                CountScoreType = x.CountScoreType,
                Memo = x.Memo,
                OrderSn = x.OrderSn,
                CreateUserId = x.CreateUserId,
                CreateTime = x.CreateTime,
                ModifyUserId = x.ModifyUserId,
                ModifyTime = x.ModifyTime,
            });
        }

        /// <summary>
        /// 轉換問卷答案項目資料
        /// </summary>
        /// <param name="answerDefineList">問卷答案項目資料</param>
        /// <returns>問卷答案定義資料集合</returns>
        private IEnumerable<AnswerDefineEntity> ConvertAnswerDefineEntity(
            IEnumerable<QuestionAnswerDefineDO> answerDefineList)
        {
            return answerDefineList.Select(x => new AnswerDefineEntity
            {
                Uid = x.Uid,
                QuestionUid = x.QuestionUid,
                AnswerCode = x.AnswerCode,
                AnswerContent = x.AnswerContent,
                Memo = x.Memo,
                HaveOtherAnswer = x.HaveOtherAnswer,
                NeedOtherAnswer = x.NeedOtherAnswer,
                Score = x.Score,
                OrderSn = x.OrderSn,
                CreateUserId = x.CreateUserId,
                CreateTime = x.CreateTime,
                ModifyUserId = x.ModifyUserId,
                ModifyTime = x.ModifyTime,
            });
        }

        /// <summary>
        /// 驗證問卷填答規則
        /// </summary>
        /// <param name="answer">問卷填答資料</param>
        /// <param name="questionEntity">問卷資料</param>
        /// <returns>驗證結果</returns>
        public Dictionary<string, string> ValidateRule(QuestionnaireAnswerEntity answer,
            QuestionnaireEntity questionEntity)
        {
            Dictionary<string, string> validateResult = new Dictionary<string, string>();
            IEnumerable<AnswerDetailEntity> answerDetailList = null;
            string errorMsg = String.Empty;


            foreach (QuestDefineEntity questDefineEntity in questionEntity.QuestDefineEntities)
            {
                errorMsg = String.Empty;
                answerDetailList = null;

                answerDetailList = answer.AnswerDetailEntities.
                    Where(x => x.QuestionId == questDefineEntity.QuestionId);


                if (!ValidateNeedAnswer(questDefineEntity, answerDetailList, answer.AnswerDetailEntities))
                {
                    errorMsg = $"此題必須填答!";
                }
                else if (!ValidateMinMultipleAnswers(questDefineEntity, answerDetailList))
                {
                    errorMsg = $"此題至少須勾選{questDefineEntity.MinMultipleAnswers}個項目!";
                }
                else if (!ValidateMaxMultipleAnswers(questDefineEntity, answerDetailList))
                {
                    errorMsg = $"此題至多僅能勾選{questDefineEntity.MaxMultipleAnswers}個項目!";
                }
                else if (!ValidateSingleAnswerCondition(questDefineEntity, answerDetailList,
                    answer.AnswerDetailEntities))
                {
                    errorMsg = $"此題僅能勾選1個項目!";
                }
                else if (!ValidateOtherAnswer(questDefineEntity, answerDetailList))
                {
                    errorMsg = $"請輸入其他說明文字!";
                }

                if (!String.IsNullOrEmpty(errorMsg))
                {
                    validateResult.Add(questDefineEntity.QuestionId, errorMsg);
                }
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核必填規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="questAnswerDetailList">問卷題目答案明細集合</param>
        /// <param name="allAnswerDetailList">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateNeedAnswer(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> questAnswerDetailList,
            IEnumerable<AnswerDetailEntity> allAnswerDetailList)
        {
            bool validateResult = false;

            if (questAnswerDetailList.Any(x => !String.IsNullOrEmpty(x.AnswerCode)))
            {
                validateResult = true;
            }
            else if (questDefine.NeedAnswer != "Y")
            {
                validateResult = true;
            }
            else
            {
                bool conditionCanBeIgnored = false;

                if (!String.IsNullOrEmpty(questDefine.AllowNaCondition))
                {
                    var conditionObject = JsonConvert.DeserializeAnonymousType(questDefine.AllowNaCondition,
                        new { Conditions = new[] { new { QuestionId = "", AnswerCode = new[] { "" } } } });

                    foreach (var condition in conditionObject.Conditions)
                    {
                        IEnumerable<string> speciftQuestionAnswerCode = allAnswerDetailList.
                            Where(x => (x.QuestionId == condition.QuestionId) &&
                                !String.IsNullOrEmpty(x.AnswerCode)).Select(x => x.AnswerCode);

                        if (speciftQuestionAnswerCode == null)
                        {
                            var ex = new InvalidOperationException("speciftQuestionAnswerCode not found");
                            ex.Data["conditionQuestionId"] = condition.QuestionId;
                            throw ex;
                        }

                        if (String.Join(",", speciftQuestionAnswerCode) ==
                            String.Join(",", condition.AnswerCode))
                        {
                            conditionCanBeIgnored = true;
                        }
                    }
                }

                if (conditionCanBeIgnored)
                {
                    validateResult = true;
                }
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核複選最少答項數規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="answerDetailList">問卷題目答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateMinMultipleAnswers(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> answerDetailList)
        {
            bool validateResult = false;

            if (questDefine.AnswerType != "M" || questDefine.MinMultipleAnswers == null)
            {
                validateResult = true;
            }
            else if (answerDetailList.Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Count() >=
                questDefine.MinMultipleAnswers)
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核複選最多答項數規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="answerDetailList">問卷題目答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateMaxMultipleAnswers(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> answerDetailList)
        {
            bool validateResult = false;

            if (questDefine.AnswerType != "M" || questDefine.MaxMultipleAnswers == null)
            {
                validateResult = true;
            }
            else if (answerDetailList.Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Count() <=
                questDefine.MaxMultipleAnswers)
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核複選限制單一做答規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="answerDetailList">問卷題目答案明細集合</param>
        /// <param name="allAnswerDetailList">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateSingleAnswerCondition(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> answerDetailList,
            IEnumerable<AnswerDetailEntity> allAnswerDetailList)
        {
            bool validateResult = false;

            if (String.IsNullOrEmpty(questDefine.SingleAnswerCondition))
            {
                validateResult = true;
            }
            else
            {
                var conditionObject = JsonConvert.DeserializeAnonymousType(questDefine.SingleAnswerCondition,
                    new { Conditions = new[] { new { QuestionId = "", AnswerCode = new[] { "" } } } });

                foreach (var condition in conditionObject.Conditions)
                {
                    IEnumerable<AnswerDetailEntity> speciftQuestAnswerDetail = allAnswerDetailList.
                        Where(x => x.QuestionId == condition.QuestionId);

                    if (speciftQuestAnswerDetail == null)
                    {
                        var ex = new InvalidOperationException("conditionAnswerDetail not found");
                        ex.Data["condition.QuestionId"] = condition.QuestionId;
                        throw ex;
                    }

                    if (condition.AnswerCode.Length != 1)
                    {
                        var ex = new InvalidOperationException("ondition answerCode not the only");
                        ex.Data["condition.AnswerCode"] = String.Join(",", condition.AnswerCode);
                        throw ex;
                    }

                    IEnumerable<string> effectiveAnswerCodeList = speciftQuestAnswerDetail.
                        Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Select(x => x.AnswerCode);

                    if (effectiveAnswerCodeList.Count() == 1)
                    {
                        validateResult = true;
                    }
                    else if (!effectiveAnswerCodeList.Contains(condition.AnswerCode.First()))
                    {
                        validateResult = true;
                    }
                }
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核答題說明是否必填規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="answerDetailList">問卷題目答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateOtherAnswer(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> answerDetailList)
        {
            bool validateResult = false;
            bool hasOtherAnswerCondition = false;

            if (questDefine.AnswerType == "F")
            {
                validateResult = true;
            }
            else
            {
                foreach (AnswerDefineEntity answerDefine in questDefine.AnswerDefineEntities)
                {
                    AnswerDetailEntity answerDetail = answerDetailList.
                        FirstOrDefault(x => x.AnswerCode == answerDefine.AnswerCode);

                    if (answerDetail == null)
                    {
                        continue;
                    }

                    if ((answerDefine.HaveOtherAnswer == "Y") &&
                        (answerDefine.NeedOtherAnswer == "Y") &&
                        !String.IsNullOrEmpty(answerDetail.AnswerCode))
                    {
                        hasOtherAnswerCondition = true;
                        if (!String.IsNullOrEmpty(answerDetail.OtherAnswer))
                        {
                            validateResult = true;
                        }
                    }
                }

                if (!hasOtherAnswerCondition)
                {
                    validateResult = true;
                }
            }

            return validateResult;
        }

        /// <summary>
        /// 計算問卷得分，回傳問卷得分類別
        /// </summary>
        /// <param name="answer">問卷填答資料</param>
        /// <param name="questionEntity">問卷定義資料</param>
        /// <returns>問卷得分類別</returns>
        public CalculateScoreEntity CalculateScore(QuestionnaireAnswerEntity answer,
            QuestionnaireEntity questionEntity)
        {
            List<AnswerDetailEntity> answerFullDetailList = new List<AnswerDetailEntity>();

            int actualScore = 0;
            List<int> questionScoreList = new List<int>();

            IEnumerable<AnswerDetailEntity> answerDetailList = null;
            AnswerDetailEntity answerDetail = null;
            AnswerDefineEntity answerDefine = null;
            IEnumerable<int> answerScoreList = null;

            foreach (QuestDefineEntity questDefine in questionEntity.QuestDefineEntities)
            {
                answerDetailList = null;
                answerDetail = null;
                answerDefine = null;
                answerScoreList = null;

                answerDetailList = answer.AnswerDetailEntities.
                    Where(x => (x.QuestionId == questDefine.QuestionId) &&
                        !String.IsNullOrEmpty(x.AnswerCode));

                if (questDefine.AnswerType == "S")
                {

                    if (answerDetailList.Count() == 0)
                    {
                        continue;
                    }
                    else if (answerDetailList.Count() > 1)
                    {
                        var ex = new InvalidOperationException("answerCode not the only");
                        ex.Data["questDefine.QuestionId"] = questDefine.QuestionId;
                        ex.Data["answerCodeList"] = String.Join(",", answerDetailList);
                        throw ex;
                    }

                    answerDetail = answerDetailList.First();

                    answerDefine = questDefine.AnswerDefineEntities.
                        FirstOrDefault(x => x.AnswerCode == answerDetail.AnswerCode);

                    if (answerDefine == null)
                    {
                        var ex = new InvalidOperationException("answerDefine not found");
                        ex.Data["answerDetail.AnswerCode"] = answerDetail.AnswerCode;
                        throw ex;
                    }

                    answerFullDetailList.Add(new AnswerDetailEntity
                    {
                        QuestionUid = questDefine.Uid,
                        Score = answerDefine.Score,

                        QuestionId = answerDetail.QuestionId,
                        AnswerCode = answerDetail.AnswerCode,
                        OtherAnswer = answerDetail.OtherAnswer,
                    });

                }
                else if (questDefine.AnswerType == "M")
                {
                    foreach (AnswerDetailEntity answerDetailEntity in answerDetailList)
                    {
                        answerDefine = null;

                        answerDefine = questDefine.AnswerDefineEntities.
                            FirstOrDefault(x => x.AnswerCode == answerDetailEntity.AnswerCode);

                        if (answerDefine == null)
                        {
                            var ex = new InvalidOperationException("answerDefine not found");
                            ex.Data["questDefine.QuestionId"] = questDefine.QuestionId;
                            ex.Data["answerDetailEntity.AnswerCode"] = answerDetailEntity.AnswerCode;
                            throw ex;
                        }

                        answerFullDetailList.Add(new AnswerDetailEntity
                        {
                            QuestionUid = questDefine.Uid,
                            Score = answerDefine.Score,

                            QuestionId = answerDetailEntity.QuestionId,
                            AnswerCode = answerDetailEntity.AnswerCode,
                            OtherAnswer = answerDetailEntity.OtherAnswer,
                        });
                    }
                }
                else if (questDefine.AnswerType == "F")
                {
                    if (answerDetailList.Count() == 0)
                    {
                        var ex = new InvalidOperationException("answerCode not the only");
                        ex.Data["questDefine.QuestionId"] = questDefine.QuestionId;
                        throw ex;
                    }
                    else if (answerDetailList.Count() != 1)
                    {
                        var ex = new InvalidOperationException("answerCode not the only");
                        ex.Data["questDefine.QuestionId"] = questDefine.QuestionId;
                        ex.Data["answerCodeList"] = String.Join(",", answerDetailList);
                        throw ex;
                    }

                    answerDetail = answerDetailList.First();

                    IEnumerable<AnswerDefineEntity> answerDefineList = questDefine.AnswerDefineEntities.
                        Where(x => x.AnswerCode == String.Empty);

                    if (answerDefineList.Count() == 0)
                    {
                        var ex = new InvalidOperationException("answerDefineList not found");
                        ex.Data["questDefine.QuestionId"] = questDefine.QuestionId;
                        throw ex;
                    }
                    else if (answerDefineList.Count() != 1)
                    {
                        var ex = new InvalidOperationException("answerDefineList not the only");
                        ex.Data["questDefine.QuestionId"] = questDefine.QuestionId;
                        throw ex;
                    }

                    answerDefine = answerDefineList.First();

                    answerFullDetailList.Add(new AnswerDetailEntity
                    {
                        QuestionUid = questDefine.Uid,
                        Score = answerDefine.Score,

                        QuestionId = answerDetail.QuestionId,
                        AnswerCode = null,
                        OtherAnswer = answerDetail.AnswerCode,
                    });
                }


                answerScoreList = answerFullDetailList.
                    Where(x => (x.QuestionId == questDefine.QuestionId) && (x.Score != null)).
                    Select(x => x.Score.Value);


                switch (questDefine.CountScoreType)
                {
                    case "1":
                    default:
                        questionScoreList.Add(answerScoreList.Sum());
                        break;
                    case "2":
                        questionScoreList.Add(answerScoreList.Max());
                        break;
                    case "3":
                        questionScoreList.Add(answerScoreList.Min());
                        break;
                    case "4":
                        questionScoreList.Add(Convert.ToInt32(Math.
                            Round(answerScoreList.Average(), 0, MidpointRounding.AwayFromZero)));
                        break;
                }
            }


            if (questionEntity.ScoreKind == "1")
            {
                actualScore = questionScoreList.Sum();
            }

            if ((questionEntity.QuestScore != null) &&
                (actualScore > questionEntity.QuestScore.Value))
            {
                actualScore = questionEntity.QuestScore.Value;
            }

            return new CalculateScoreEntity()
            {
                ActualScore = actualScore,
                FullAnswerDetailList = answerFullDetailList,
            };
        }

        /// <summary>
        /// 紀錄問卷填答資料至問卷答題主檔與答題明細
        /// </summary>
        /// <param name="questEntity">問卷資料</param>
        /// <param name="calculateResult">問卷得分與填答資料</param>
        /// <param name="userId">用戶ID</param>
        /// <returns>問卷答題資料</returns>
        private QuestionnaireAnswerDO SaveQuestionAnswer(QuestionnaireEntity questEntity,
            CalculateScoreEntity calculateResult, string userId)
        {
            QuestionnaireAnswerDO questionnaireAnswerDO = null;

            if (questEntity == null)
            {
                throw new ArgumentNullException("questEntity");
            }
            else if (calculateResult == null)
            {
                throw new ArgumentNullException("calculateResult");
            }
            else if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            DateTime currentTiem = DateTime.Now;

            using (TransactionScope scope = new TransactionScope())
            {
                questionnaireAnswerDO = new QuestionnaireAnswerDO()
                {
                    Uid = Guid.NewGuid(),
                    QuestUid = questEntity.Uid,
                    QuestAnswerId = currentTiem.ToString("yyMMddHHmmssfff"),
                    TesteeId = userId,
                    QuestScore = questEntity.QuestScore,
                    ActualScore = calculateResult.ActualScore,
                    TesteeSource = _testeeSource,
                    CreateUserId = userId,
                    CreateTime = currentTiem,
                    ModifyUserId = null,
                    ModifyTime = null,
                };

                IEnumerable<QuestionnaireAnswerDetailDO> questAnswerDetailList =
                    calculateResult.FullAnswerDetailList.Select(answerDetail =>
                    new QuestionnaireAnswerDetailDO()
                    {
                        Uid = Guid.NewGuid(),
                        AnswerUid = questionnaireAnswerDO.Uid,
                        QuestionUid = answerDetail.QuestionUid,
                        AnswerCode = answerDetail.AnswerCode,
                        OtherAnswer = answerDetail.OtherAnswer,
                        Score = answerDetail.Score,
                        CreateUserId = userId,
                        CreateTime = currentTiem,
                        ModifyUserId = null,
                        ModifyTime = null,
                    });

                new QuestionnaireAnswerDAO().Insert(questionnaireAnswerDO);
                new QuestionnaireAnswerDetailDAO().Insert(questAnswerDetailList);

                scope.Complete();
            }

            return questionnaireAnswerDO;
        }

        #endregion

    }
}
