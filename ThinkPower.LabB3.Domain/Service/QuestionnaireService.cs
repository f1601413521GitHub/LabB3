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
        /// <returns></returns>
        public QuestionnaireResultEntity Calculate(QuestionnaireAnswerEntity answer)
        {
            QuestionnaireResultEntity result = null;
            string message = null;

            try
            {
                if (answer == null)
                {
                    throw new ArgumentNullException("answer");
                }

                QuestionnaireEntity questEntity = GetQuestionnaire(answer.QuestUid);

                if (questEntity == null)
                {
                    throw new InvalidOperationException("questEntity not found");
                }

                //TODO: TEST
                //List<string> validates = new List<string>();

                foreach (QuestDefineEntity questDefine in questEntity.QuestDefineEntities)
                {
                    AnswerDetailEntity answerDetail = answer.AnswerDetailEntities
                        .Where(x => x.QuestionId == questDefine.QuestionId).FirstOrDefault();

                    if (answerDetail == null)
                    {
                        throw new InvalidOperationException("answerDetail not found");
                    }

                    message = null;

                    //TODO: TEST
                    //questDefine.MinMultipleAnswers = 2;
                    //questDefine.MaxMultipleAnswers = 3;

                    if (!ValidateNeedAnswer(questDefine, answerDetail, answer.AnswerDetailEntities))
                    {
                        message = $"{answerDetail.QuestionId}此題必須填答!";
                    }
                    else if (!ValidateMinMultipleAnswers(questDefine, answerDetail))
                    {
                        message = $"{answerDetail.QuestionId}此題至少須勾選{questDefine.MinMultipleAnswers}個項目!";
                    }
                    else if (!ValidateMaxMultipleAnswers(questDefine, answerDetail))
                    {
                        message = $"{answerDetail.QuestionId}此題至多僅能勾選{questDefine.MaxMultipleAnswers}個項目!";
                    }
                    else if (!ValidateSingleAnswerCondition(questDefine, answerDetail,
                        answer.AnswerDetailEntities))
                    {
                        message = $"{answerDetail.QuestionId}此題僅能勾選1個項目!";
                    }
                    else if (!ValidateOtherAnswer(questDefine, answerDetail))
                    {
                        message = $"{answerDetail.QuestionId}請輸入其他說明文字!";
                    }

                    //TODO: TEST
                    //validates.Add(message + Environment.NewLine);
                }

                //TODO: TEST
                //string re = String.Join(",", validates);
                //var stop = false;
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
        }

        /// <summary>
        /// 檢核答題說明是否必填規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義類別</param>
        /// <param name="answerDetail">問卷答案明細類別</param>
        /// <param name="answerDetailEntities">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateOtherAnswer(QuestDefineEntity questDefine, AnswerDetailEntity answerDetail)
        {
            bool validateResult = false;
            int hasOtherAnswerCondition = 0;
            int count = 0;

            foreach (AnswerDefineEntity answerDefine in questDefine.AnswerDefineEntities)
            {
                count++;

                if (questDefine.AnswerType == "F")
                {
                    hasOtherAnswerCondition++;
                    if (!String.IsNullOrEmpty(answerDetail.OtherAnswer))
                    {
                        validateResult = true;
                    }
                }
                else if (answerDefine.HaveOtherAnswer == "Y" &&
                    answerDefine.NeedOtherAnswer == "Y" &&
                    answerDetail.AnswerCode.Contains($"{count}"))
                {
                    hasOtherAnswerCondition++;
                    if (!String.IsNullOrEmpty(answerDetail.OtherAnswer))
                    {
                        validateResult = true;
                    }
                }
            }

            if (hasOtherAnswerCondition == 0)
            {
                validateResult = true;
            }

            return validateResult;
        }

        /// <summary>
        /// 檢核複選限制單一做答規則
        /// </summary>
        /// <param name="questDefine">問卷題目定義類別</param>
        /// <param name="answerDetail">問卷答案明細類別</param>
        /// <param name="answerDetailEntities">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateSingleAnswerCondition(QuestDefineEntity questDefine, AnswerDetailEntity answerDetail,
            IEnumerable<AnswerDetailEntity> answerDetailEntities)
        {
            bool validateResult = false;

            if (!String.IsNullOrEmpty(questDefine.SingleAnswerCondition))
            {
                var allowNaCondition = JsonConvert.DeserializeAnonymousType(questDefine.SingleAnswerCondition,
                    new { Conditions = new[] { new { QuestionId = "", AnswerCode = new[] { "" } } } });

                foreach (var condition in allowNaCondition.Conditions)
                {
                    AnswerDetailEntity conditionAnswerDetail = answerDetailEntities
                        .Where(x => x.QuestionId == condition.QuestionId).FirstOrDefault();

                    if (conditionAnswerDetail == null)
                    {
                        throw new InvalidOperationException("conditionAnswerDetail not found");
                    }

                    if (conditionAnswerDetail.AnswerCode == String.Join(",", condition.AnswerCode))
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
        /// <param name="questDefine">問卷題目定義類別</param>
        /// <param name="answerDetail">問卷答案明細類別</param>
        /// <returns>檢核結果</returns>
        private bool ValidateMaxMultipleAnswers(QuestDefineEntity questDefine, AnswerDetailEntity answerDetail)
        {
            bool validateResult = false;

            if (questDefine.AnswerType == "M" && questDefine.MaxMultipleAnswers != null)
            {
                if (answerDetail.AnswerCode.Split(',').Length <= questDefine.MaxMultipleAnswers)
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
        /// <param name="questDefine">問卷題目定義類別</param>
        /// <param name="answerDetail">問卷答案明細類別</param>
        /// <returns>檢核結果</returns>
        private bool ValidateMinMultipleAnswers(QuestDefineEntity questDefine, AnswerDetailEntity answerDetail)
        {
            bool validateResult = false;

            if (questDefine.AnswerType == "M" && questDefine.MinMultipleAnswers != null)
            {
                if (answerDetail.AnswerCode.Split(',').Length >= questDefine.MinMultipleAnswers)
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
        /// <param name="questDefine">問卷題目定義類別</param>
        /// <param name="answerDetail">問卷答案明細類別</param>
        /// <param name="answerDetailEntities">問卷答案明細集合</param>
        /// <returns>檢核結果</returns>
        private bool ValidateNeedAnswer(QuestDefineEntity questDefine, AnswerDetailEntity answerDetail,
            IEnumerable<AnswerDetailEntity> answerDetailEntities)
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
                        AnswerDetailEntity conditionAnswerDetail = answerDetailEntities
                            .Where(x => x.QuestionId == condition.QuestionId).FirstOrDefault();

                        if (conditionAnswerDetail == null)
                        {
                            throw new InvalidOperationException("conditionAnswerDetail not found");
                        }

                        if (conditionAnswerDetail.AnswerCode == String.Join(",", condition.AnswerCode))
                        {
                            conditionCanBeIgnored = true;
                        }
                    }
                }

                if (!conditionCanBeIgnored)
                {
                    if (!String.IsNullOrEmpty(answerDetail.AnswerCode))
                    {
                        validateResult = true;
                    }
                }
                else
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

                if (quest == null)
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
