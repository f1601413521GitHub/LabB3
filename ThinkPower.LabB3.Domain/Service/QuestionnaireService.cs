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

namespace ThinkPower.LabB3.Domain.Service
{
    /// <summary>
    /// 問卷服務
    /// </summary>
    public class QuestionnaireService : IQuestionnaire
    {
        /// <summary>
        /// NLog Object
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 計算問卷填答得分
        /// </summary>
        /// <param name="answer">問卷填答資料</param>
        /// <returns>問卷填答評分結果</returns>
        public QuestionnaireResultEntity Calculate(QuestionnaireAnswerEntity answer)
        {
            //TODO 0921 QuestionnaireAnswerEntity換成自訂類別?
            QuestionnaireResultEntity result = null;
            QuestionnaireAnswerDO questionnaireAnswerDO = null;

            try
            {
                if (answer == null)
                {
                    throw new ArgumentNullException("沒有提供問卷填答資料");
                }

                QuestionnaireEntity questEntity = GetQuestionnaire(answer.QuestUid);

                if (questEntity == null)
                {
                    throw new InvalidOperationException("問卷資料不存在或沒有有效的問卷資料");
                }

                Dictionary<string, string> validates = new Dictionary<string, string>();
                string message = null;

                foreach (QuestDefineEntity questDefine in questEntity.QuestDefineEntities)
                {
                    IEnumerable<AnswerDetailEntity> questAnswerDetailList = answer.AnswerDetailEntities
                        .Where(x => x.QuestionId == questDefine.QuestionId);

                    message = null;

                    //TODO: TEST
                    //questDefine.MinMultipleAnswers = 2;
                    //questDefine.MaxMultipleAnswers = 3;

                    if (!ValidateNeedAnswer(questDefine, questAnswerDetailList, answer.AnswerDetailEntities))
                    {
                        message = $"此題必須填答!";
                    }
                    else if (!ValidateMinMultipleAnswers(questDefine, questAnswerDetailList))
                    {
                        message = $"此題至少須勾選{questDefine.MinMultipleAnswers}個項目!";
                    }
                    else if (!ValidateMaxMultipleAnswers(questDefine, questAnswerDetailList))
                    {
                        message = $"此題至多僅能勾選{questDefine.MaxMultipleAnswers}個項目!";
                    }
                    else if (!ValidateSingleAnswerCondition(questDefine, questAnswerDetailList,
                        answer.AnswerDetailEntities))
                    {
                        message = $"此題僅能勾選1個項目!";
                    }
                    else if (!ValidateOtherAnswer(questDefine, questAnswerDetailList))
                    {
                        message = $"請輸入其他說明文字!";
                    }

                    if (message != null)
                    {
                        validates.Add(questDefine.QuestionId, message);
                    }
                }

                if (validates.Count == 0)
                {
                    if (questEntity.NeedScore == "Y")
                    {
                        Tuple<int, List<AnswerDetailEntity>> calculateResult =
                            CalculateScore(answer, questEntity);


                        DateTime timeNow = DateTime.Now;
                        string userId = timeNow.ToString("yyyymm");
                        Guid questionAnswerUid = Guid.NewGuid();


                        questionnaireAnswerDO = new QuestionnaireAnswerDO()
                        {
                            Uid = questionAnswerUid,
                            QuestUid = questEntity.Uid,
                            QuestAnswerId = timeNow.ToString("yyMMddHHmmssfff"),
                            TesteeId = userId,
                            QuestScore = questEntity.QuestScore,
                            ActualScore = calculateResult.Item1,
                            TesteeSource = "LabB3",
                            CreateUserId = userId,
                            CreateTime = timeNow,
                            ModifyUserId = null,
                            ModifyTime = null,
                        };

                        if (!new QuestionnaireAnswerDAO().CreateQuestionnaireAnswer(questionnaireAnswerDO))
                        {
                            throw new InvalidOperationException("CreateQuestionnaireAnswer is fail");
                        }



                        QuestionnaireAnswerDetailDAO questAnswerDetailDAO =
                            new QuestionnaireAnswerDetailDAO();

                        QuestionnaireAnswerDetailDO questAnswerDetailDO = null;

                        foreach (AnswerDetailEntity answerDetail in calculateResult.Item2)
                        {
                            timeNow = DateTime.Now;
                            userId = timeNow.ToString("yyyymm");

                            questAnswerDetailDO = new QuestionnaireAnswerDetailDO()
                            {
                                Uid = Guid.NewGuid(),
                                AnswerUid = questionAnswerUid,
                                QuestionUid = answerDetail.QuestionUid,
                                AnswerCode = answerDetail.AnswerCode,
                                OtherAnswer = answerDetail.OtherAnswer,
                                Score = answerDetail.Score,
                                CreateUserId = userId,
                                CreateTime = timeNow,
                                ModifyUserId = null,
                                ModifyTime = null,
                            };

                            if (!questAnswerDetailDAO.CreateQuestionnaireAnswerDetail(questAnswerDetailDO))
                            {
                                throw new InvalidOperationException("CreateQuestionnaireAnswerDetail is fail");
                            }
                        }
                    }
                    else
                    {
                        //TODO 0925 您的問卷己填答完畢，謝謝您的參與
                    }
                }

                if (questionnaireAnswerDO != null)
                {
                    result = new QuestionnaireResultEntity()
                    {
                        Uid = questionnaireAnswerDO.Uid,
                        QuestUid = questionnaireAnswerDO.QuestUid,
                        QuestAnswerId = questionnaireAnswerDO.QuestAnswerId,
                        TesteeId = questionnaireAnswerDO.TesteeId,
                        QuestScore = questionnaireAnswerDO.QuestScore,
                        ActualScore = questionnaireAnswerDO.ActualScore,
                        TesteeSource = questionnaireAnswerDO.TesteeSource,
                        CreateUserId = questionnaireAnswerDO.CreateUserId,
                        CreateTime = questionnaireAnswerDO.CreateTime,
                        ModifyUserId = questionnaireAnswerDO.ModifyUserId,
                        ModifyTime = questionnaireAnswerDO.ModifyTime,

                        ValidateFailInfo = validates,
                        ValidateFailQuestId = questEntity.QuestId,
                        AnswerDetailEntities = answer.AnswerDetailEntities,
                    };
                }
                else
                {
                    result = new QuestionnaireResultEntity()
                    {
                        ValidateFailInfo = validates,
                        ValidateFailQuestId = questEntity.QuestId,
                        AnswerDetailEntities = answer.AnswerDetailEntities,
                    };
                }
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
        }

        /// <summary>
        /// 計算問卷得分，回傳問卷得分與問卷填答完整資料(包含問卷題目識別碼與答題分數)
        /// </summary>
        /// <param name="answer">問卷填答資料</param>
        /// <param name="questEntity">問卷定義資料</param>
        /// <returns>問卷得分與問卷填答完整資料</returns>
        private Tuple<int, List<AnswerDetailEntity>> CalculateScore(QuestionnaireAnswerEntity answer,
            QuestionnaireEntity questEntity)
        {
            List<AnswerDetailEntity> answerFullDetailList = new List<AnswerDetailEntity>();

            int questionScoreResult = 0;
            List<int> questionScoreList = new List<int>();

            foreach (QuestDefineEntity questDefine in questEntity.QuestDefineEntities)
            {
                if (questDefine.AnswerType == "S")
                {
                    IEnumerable<AnswerDetailEntity> answerDetailList = answer.AnswerDetailEntities
                        .Where(x => x.QuestionId == questDefine.QuestionId &&
                            !String.IsNullOrEmpty(x.AnswerCode));

                    if (answerDetailList.Count() == 0)
                    {
                        throw new InvalidOperationException("answerCode not found");
                    }
                    else if (answerDetailList.Count() > 1)
                    {
                        throw new InvalidOperationException("answerCode not the only");
                    }

                    AnswerDetailEntity answerDetail = answerDetailList.First();

                    AnswerDefineEntity answerDefine = questDefine.AnswerDefineEntities
                        .FirstOrDefault(x => x.AnswerCode == answerDetail.AnswerCode);

                    if (answerDefine == null)
                    {
                        throw new InvalidOperationException("answerDefine not found");
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
                    IEnumerable<AnswerDetailEntity> answerDetailList = answer.AnswerDetailEntities
                        .Where(x => x.QuestionId == questDefine.QuestionId &&
                            !String.IsNullOrEmpty(x.AnswerCode));

                    foreach (AnswerDetailEntity answerDetail in answerDetailList)
                    {
                        AnswerDefineEntity answerDefine = questDefine.AnswerDefineEntities
                            .FirstOrDefault(x => x.AnswerCode == answerDetail.AnswerCode);

                        if (answerDefine == null)
                        {
                            throw new InvalidOperationException("answerDefine not found");
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
                }
                else if (questDefine.AnswerType == "F")
                {
                    IEnumerable<AnswerDetailEntity> answerDetailList = answer.AnswerDetailEntities
                        .Where(x => x.QuestionId == questDefine.QuestionId &&
                            !String.IsNullOrEmpty(x.AnswerCode));

                    if (answerDetailList.Count() == 0)
                    {
                        throw new InvalidOperationException("answerCode not found");
                    }
                    else if (answerDetailList.Count() != 1)
                    {
                        throw new InvalidOperationException("answerCode not the only");
                    }

                    AnswerDetailEntity answerDetail = answerDetailList.First();

                    IEnumerable<AnswerDefineEntity> answerDefineList = questDefine.AnswerDefineEntities
                        .Where(x => x.AnswerCode == "");

                    if (answerDetailList.Count() == 0)
                    {
                        throw new InvalidOperationException("answerDefineList not found");
                    }
                    else if (answerDetailList.Count() != 1)
                    {
                        throw new InvalidOperationException("answerDefineList not the only");
                    }

                    AnswerDefineEntity answerDefine = answerDefineList.First();


                    answerFullDetailList.Add(new AnswerDetailEntity
                    {
                        QuestionUid = questDefine.Uid,
                        Score = answerDefine.Score,

                        QuestionId = answerDetail.QuestionId,
                        AnswerCode = null,
                        OtherAnswer = answerDetail.AnswerCode,
                    });
                }


                IEnumerable<int> questScoreList = answerFullDetailList
                    .Where(x => x.QuestionId == questDefine.QuestionId && x.Score != null)
                    .Select(x => (int)x.Score);

                if (questDefine.CountScoreType == "1")
                {
                    questionScoreList.Add(questScoreList.Sum());
                }
                else if (questDefine.CountScoreType == "2")
                {
                    questionScoreList.Add(questScoreList.Max());
                }
                else if (questDefine.CountScoreType == "3")
                {
                    questionScoreList.Add(questScoreList.Min());
                }
                else if (questDefine.CountScoreType == "4")
                {
                    questionScoreList.Add((int)Math.Round(questScoreList.Average()));
                }
            }


            if (questEntity.ScoreKind == "1")
            {
                questionScoreResult = questionScoreList.Sum();
            }

            if (questEntity.QuestScore != null &&
                questionScoreResult > questEntity.QuestScore)
            {
                questionScoreResult = (int)questEntity.QuestScore;
            }

            return Tuple.Create(questionScoreResult, answerFullDetailList);
        }

        /// <summary>
        /// 檢核答題說明是否必填規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="questAnswerDetailList">問卷題目答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateOtherAnswer(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> questAnswerDetailList)
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
                    AnswerDetailEntity answerDetail = questAnswerDetailList
                        .FirstOrDefault(x => x.AnswerCode == answerDefine.AnswerCode);

                    if (answerDetail == null)
                    {
                        continue;
                    }

                    if (answerDefine.HaveOtherAnswer == "Y" &&
                       answerDefine.NeedOtherAnswer == "Y" &&
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
        /// 檢核複選限制單一做答規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="questAnswerDetailList">問卷題目答案明細集合</param>
        /// <param name="answerDetailList">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateSingleAnswerCondition(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> questAnswerDetailList,
            IEnumerable<AnswerDetailEntity> answerDetailList)
        {
            bool validateResult = false;

            if (!String.IsNullOrEmpty(questDefine.SingleAnswerCondition))
            {
                var allowNaCondition = JsonConvert.DeserializeAnonymousType(questDefine.SingleAnswerCondition,
                    new { Conditions = new[] { new { QuestionId = "", AnswerCode = new[] { "" } } } });

                foreach (var condition in allowNaCondition.Conditions)
                {
                    IEnumerable<AnswerDetailEntity> conditionAnswerDetail = answerDetailList
                        .Where(x => x.QuestionId == condition.QuestionId);

                    if (conditionAnswerDetail == null)
                    {
                        throw new InvalidOperationException("conditionAnswerDetail not found");
                    }

                    if (condition.AnswerCode.Length != 1)
                    {
                        throw new InvalidOperationException("Condition answerCode not the only");
                    }

                    IEnumerable<string> effectiveAnswerCodeList = conditionAnswerDetail
                        .Where(x => !String.IsNullOrEmpty(x.AnswerCode))
                        .Select(x => x.AnswerCode);

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
            else
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核複選最多答項數規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="questAnswerDetailList">問卷題目答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateMaxMultipleAnswers(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> questAnswerDetailList)
        {
            bool validateResult = false;

            if (questDefine.AnswerType == "M" && questDefine.MaxMultipleAnswers != null)
            {
                if (questAnswerDetailList.Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Count() <=
                    questDefine.MaxMultipleAnswers)
                {
                    validateResult = true;
                }
            }
            else
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核複選最少答項數規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="questAnswerDetailList">問卷題目答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateMinMultipleAnswers(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> questAnswerDetailList)
        {
            bool validateResult = false;

            if (questDefine.AnswerType == "M" && questDefine.MinMultipleAnswers != null)
            {
                if (questAnswerDetailList.Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Count() >=
                    questDefine.MinMultipleAnswers)
                {
                    validateResult = true;
                }
            }
            else
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核必填規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義</param>
        /// <param name="questAnswerDetailList">問卷題目答案明細集合</param>
        /// <param name="answerDetailList">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateNeedAnswer(QuestDefineEntity questDefine,
            IEnumerable<AnswerDetailEntity> questAnswerDetailList,
            IEnumerable<AnswerDetailEntity> answerDetailList)
        {
            bool validateResult = false;

            if (questDefine.NeedAnswer == "Y")
            {
                bool conditionCanBeIgnored = false;

                if (!String.IsNullOrEmpty(questDefine.AllowNaCondition))
                {
                    var allowNaCondition = JsonConvert.DeserializeAnonymousType(questDefine.AllowNaCondition,
                        new { Conditions = new[] { new { QuestionId = "", AnswerCode = new[] { "" } } } });

                    foreach (var condition in allowNaCondition.Conditions)
                    {
                        IEnumerable<AnswerDetailEntity> conditionAnswerDetail = answerDetailList
                            .Where(x => x.QuestionId == condition.QuestionId);

                        if (conditionAnswerDetail == null)
                        {
                            throw new InvalidOperationException("conditionAnswerDetail not found");
                        }

                        if (String.Join(",", conditionAnswerDetail
                            .Where(x => !String.IsNullOrEmpty(x.AnswerCode))
                            .Select(x => x.AnswerCode)) ==
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
                else if (questDefine.AnswerType == "F")
                {
                    if (questAnswerDetailList.Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Count() > 0)
                    {
                        validateResult = true;
                    }
                }
                else
                {
                    if (questAnswerDetailList.Where(x => !String.IsNullOrEmpty(x.AnswerCode)).Count() > 0)
                    {
                        validateResult = true;
                    }
                }
            }
            else
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 取得有效的問卷資料
        /// </summary>
        /// <param name="id">問卷編號</param>
        /// <returns>有效的問卷資料</returns>
        public QuestionnaireEntity GetActiveQuestionnaire(string id)
        {
            QuestionnaireEntity questEntity = null;

            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                QuestionnaireDO quest = new QuestionnaireDAO().GetActiveQuestionniare(id);

                if (quest == null)
                {
                    throw new InvalidOperationException($"quest not found, id={id}");
                }

                IEnumerable<QuestionDefineDO> questDefineList =
                    new QuestionDefineDAO().GetQuestionDefineList(quest.Uid);

                if ((questDefineList == null) ||
                    (questDefineList.Count() == 0))
                {
                    throw new InvalidOperationException(
                        $"questDefineList not found,questUid={quest.Uid}");
                }

                List<QuestDefineEntity> questDefineEntities =
                    ConvertQuestDefineEntity(questDefineList).ToList();

                QuestionAnswerDefineDAO answerDefineDAO = new QuestionAnswerDefineDAO();
                IEnumerable<QuestionAnswerDefineDO> tempAnswerDefineList = null;


                foreach (QuestDefineEntity questDefineEntity in questDefineEntities)
                {
                    tempAnswerDefineList = answerDefineDAO.
                        GetQuestionAnswerDefineList(questDefineEntity.Uid);

                    if ((tempAnswerDefineList == null) ||
                        (tempAnswerDefineList.Count() == 0))
                    {
                        throw new InvalidOperationException(
                            $"answerDefineList not found, questDefineEntityUid={questDefineEntity.Uid}");
                    }

                    questDefineEntity.AnswerDefineEntities =
                        ConvertAnswerDefineEntity(tempAnswerDefineList);

                    tempAnswerDefineList = null;
                }

                questEntity = new QuestionnaireEntity()
                {
                    Uid = quest.Uid,
                    QuestId = quest.QuestId,
                    Version = quest.Version,
                    Kind = quest.Kind,
                    Name = quest.Name,
                    Memo = quest.Memo,
                    Ondate = quest.Ondate,
                    Offdate = quest.Offdate,
                    NeedScore = quest.NeedScore,
                    QuestScore = quest.QuestScore,
                    ScoreKind = quest.ScoreKind,
                    HeadBackgroundImg = quest.HeadBackgroundImg,
                    HeadDescription = quest.HeadDescription,
                    FooterDescription = quest.FooterDescription,
                    CreateUserId = quest.CreateUserId,
                    CreateTime = quest.CreateTime,
                    ModifyUserId = quest.ModifyUserId,
                    ModifyTime = quest.ModifyTime,

                    QuestDefineEntities = questDefineEntities,
                };
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questEntity;
        }

        /// <summary>
        /// 轉換問卷答案定義資料物件成為問卷答案定義類別
        /// </summary>
        /// <param name="answerDefineList">問卷答案定義資料物件</param>
        /// <returns>問卷答案定義類別</returns>
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
        /// 轉換問卷題目定義資料成為問卷題目定義實體
        /// </summary>
        /// <param name="questDefineList">問卷題目定義資料</param>
        /// <returns>問卷題目定義實體</returns>
        private IEnumerable<QuestDefineEntity> ConvertQuestDefineEntity(
            IEnumerable<QuestionDefineDO> questDefineList)
        {
            return questDefineList.Select(x => new QuestDefineEntity
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
        /// 取得問卷資料
        /// </summary>
        /// <param name="uid">問卷識別碼</param>
        /// <returns>問卷資料</returns>
        public QuestionnaireEntity GetQuestionnaire(string uid)
        {
            QuestionnaireEntity questEntity = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                QuestionnaireDO quest = new QuestionnaireDAO().GetQuestionnaire(uid);
                DateTime timeNow = DateTime.Now;

                if (quest == null || quest.Ondate >= timeNow ||
                    quest.Offdate != null && quest.Offdate <= timeNow)
                {
                    throw new InvalidOperationException($"quest not found, uid={uid}");
                }

                IEnumerable<QuestionDefineDO> questDefineList =
                    new QuestionDefineDAO().GetQuestionDefineList(quest.Uid);

                if ((questDefineList == null) ||
                    (questDefineList.Count() == 0))
                {
                    throw new InvalidOperationException(
                        $"questDefineList not found,questUid={quest.Uid}");
                }

                List<QuestDefineEntity> questDefineEntities =
                    ConvertQuestDefineEntity(questDefineList).ToList();

                QuestionAnswerDefineDAO answerDefineDAO = new QuestionAnswerDefineDAO();
                IEnumerable<QuestionAnswerDefineDO> tempAnswerDefineList = null;


                foreach (QuestDefineEntity questDefineEntity in questDefineEntities)
                {
                    tempAnswerDefineList = answerDefineDAO.
                        GetQuestionAnswerDefineList(questDefineEntity.Uid);

                    if ((tempAnswerDefineList == null) ||
                        (tempAnswerDefineList.Count() == 0))
                    {
                        throw new InvalidOperationException(
                            $"answerDefineList not found, questDefineEntityUid={questDefineEntity.Uid}");
                    }

                    questDefineEntity.AnswerDefineEntities =
                        ConvertAnswerDefineEntity(tempAnswerDefineList);

                    tempAnswerDefineList = null;
                }

                questEntity = new QuestionnaireEntity()
                {
                    Uid = quest.Uid,
                    QuestId = quest.QuestId,
                    Version = quest.Version,
                    Kind = quest.Kind,
                    Name = quest.Name,
                    Memo = quest.Memo,
                    Ondate = quest.Ondate,
                    Offdate = quest.Offdate,
                    NeedScore = quest.NeedScore,
                    QuestScore = quest.QuestScore,
                    ScoreKind = quest.ScoreKind,
                    HeadBackgroundImg = quest.HeadBackgroundImg,
                    HeadDescription = quest.HeadDescription,
                    FooterDescription = quest.FooterDescription,
                    CreateUserId = quest.CreateUserId,
                    CreateTime = quest.CreateTime,
                    ModifyUserId = quest.ModifyUserId,
                    ModifyTime = quest.ModifyTime,

                    QuestDefineEntities = questDefineEntities,
                };
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questEntity;
        }
    }
}
