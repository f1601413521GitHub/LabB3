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
using ThinkPower.LabB3.Domain.DTO;
using System.Configuration;

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
            throw new NotImplementedException();
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
            QuestionnaireEntity questInfo = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }

            try
            {
                //TODO
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return questInfo;
        }
    }
}
