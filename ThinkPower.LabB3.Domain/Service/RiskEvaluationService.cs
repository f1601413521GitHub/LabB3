using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.DataAccess.DAO;
using ThinkPower.LabB3.DataAccess.DO;
using ThinkPower.LabB3.Domain.DTO;
using ThinkPower.LabB3.Domain.Entity;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Entity.Risk;
using ThinkPower.LabB3.Domain.Resources;
using ThinkPower.LabB3.Domain.Service.Interface;

namespace ThinkPower.LabB3.Domain.Service
{
    /// <summary>
    /// 投資風險評估服務
    /// </summary>
    public class RiskEvaluationService : IRiskEvaluation
    {
        /// <summary>
        /// NLog Object
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 問卷服務 隱藏欄位
        /// </summary>
        private QuestionnaireService _questService;

        /// <summary>
        /// 評估投資風險等級結果的暫存鍵值
        /// </summary>
        private readonly string _cacheKeyRiskEvaResultDTO = "RiskEvaResultDTO";

        /// <summary>
        /// 投資風險評估結果DO物件的暫存鍵值
        /// </summary>
        private readonly string _cacheKeyRiskEvaluationDO = "RiskEvaluationDO";

        /// <summary>
        /// 問卷服務
        /// </summary>
        private QuestionnaireService QuestService
        {
            get
            {
                if (_questService == null)
                {
                    _questService = new QuestionnaireService();
                }

                return _questService;
            }
        }

        /// <summary>
        /// 用戶ID
        /// </summary>
        public string UserId { get; set; }








        /// <summary>
        /// 評估投資風險等級
        /// </summary>
        /// <param name="answer">風險評估填答資料</param>
        /// <returns></returns>
        public RiskEvaResultDTO EvaluateRiskRank(RiskEvaAnswerEntity answer)
        {
            RiskEvaResultDTO result = null;
            QuestionnaireResultEntity questResultEntity = null;
            RiskEvaQuestionnaireEntity riskEvaQuestEntity = null;
            RiskEvaluationEntity riskEvaluationEntity = null;

            try
            {
                if (answer == null)
                {
                    throw new ArgumentNullException("answer");
                }

                //TODO 1003 邏輯有嚴重問題!!
                riskEvaQuestEntity = CheckLatestRiskEvaluation(answer.QuestionnaireAnswerEntity.QuestUid);

                if (riskEvaQuestEntity == null)
                {
                    throw new InvalidOperationException("riskEvaQuestEntity not found");
                }

                if (!riskEvaQuestEntity.CanRiskEvaluation)
                {
                    result = new RiskEvaResultDTO()
                    {
                        QuestionnaireResultEntity = questResultEntity,
                        RiskEvaQuestionnaire = riskEvaQuestEntity,
                        RiskEvaluationEntity = riskEvaluationEntity,
                    };

                    return result;
                }




                questResultEntity = QuestService.Calculate(answer.QuestionnaireAnswerEntity);

                if (questResultEntity == null)
                {
                    throw new InvalidOperationException("questResultEntity not found");
                }
                else if (questResultEntity.QuestionnaireEntity == null)
                {
                    throw new InvalidOperationException("questResultEntity.QuestionnaireEntity not found");
                }

                if (questResultEntity.ValidateFailInfo.Count > 0)
                {
                    riskEvaQuestEntity = GetRiskQuestionnaire(questResultEntity.ValidateFailQuestId);

                    if (riskEvaQuestEntity == null)
                    {
                        throw new InvalidOperationException("riskEvaQuestEntity not found");
                    }
                }
                else
                {

                    if (questResultEntity.QuestionnaireEntity.NeedScore == "Y")
                    {

                        RiskRankDO riskRankDO = new RiskRankDAO().GetRiskRank(questResultEntity.ActualScore);

                        if (riskRankDO == null)
                        {
                            throw new InvalidOperationException("riskRankDO not found");
                        }

                        List<RiskRankDetailDO> riskRankDetailDOList = new RiskRankDetailDAO().GetRiskRankDetail(riskRankDO.Uid);

                        if (riskRankDetailDOList == null || riskRankDetailDOList.Count == 0)
                        {
                            throw new InvalidOperationException("riskRankDetailDOList not found");
                        }

                        //TODO 1003 check BusinessDate 跟作業期間有關係
                        DateTime currentTime = DateTime.Now;
                        riskEvaluationEntity = new RiskEvaluationEntity()
                        {
                            Uid = Guid.NewGuid(),
                            RiskEvaId = "FNDINV",
                            QuestAnswerId = questResultEntity.QuestAnswerId,
                            CliId = questResultEntity.TesteeId,
                            RiskResult = String.Join(";", questResultEntity.RiskResult.Select(x => $"[{x.Key}:{x.Value}]")),
                            RiskScore = questResultEntity.ActualScore,
                            RiskAttribute = riskRankDO.RiskRankKind,
                            EvaluationDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day),
                            BusinessDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day),
                            IsUsed = "N",
                            CreateUserId = questResultEntity.TesteeId,
                            CreateTime = currentTime,
                            ModifyUserId = null,
                            ModifyTime = null,
                        };
                    }
                }

                result = new RiskEvaResultDTO()
                {
                    QuestionnaireResultEntity = questResultEntity,
                    RiskEvaQuestionnaire = riskEvaQuestEntity,
                    RiskEvaluationEntity = riskEvaluationEntity,
                    RiskRankEntities = GetRiskRankEntities(),
                };

                //TODO 1003 cache
                CacheProvider.SetCache($"{_cacheKeyRiskEvaResultDTO}-{questResultEntity.QuestAnswerId}",
                    result, true);

            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
        }

        /// <summary>
        /// 依紀錄識別碼取得風險評估資料
        /// </summary>
        /// <param name="uid">紀錄識別碼</param>
        /// <returns></returns>
        public RiskEvaluationEntity Get(string uid)
        {
            RiskEvaluationEntity riskEvaEntity = null;

            if (String.IsNullOrEmpty(uid))
            {
                throw new ArgumentNullException("uid");
            }


            object cache = CacheProvider.GetCache($"{_cacheKeyRiskEvaluationDO}-{uid}");
            RiskEvaluationDO riskEvaDO = cache as RiskEvaluationDO;
            if (riskEvaDO == null)
            {
                throw new InvalidOperationException("riskEvaDO not found");
            }

            riskEvaEntity = ConvertRiskEvaluationDO(riskEvaDO);

            return riskEvaEntity;
        }

        /// <summary>
        /// 取得風險評估問卷資料
        /// </summary>
        /// <param name="questId">問卷編號</param>
        /// <returns>風險評估問卷資料</returns>
        public RiskEvaQuestionnaireEntity GetRiskQuestionnaire(string questId)
        {
            RiskEvaQuestionnaireEntity result = null;

            if (String.IsNullOrEmpty(questId))
            {
                throw new ArgumentNullException("questId");
            }

            QuestionnaireEntity activeQuestionnaire = QuestService.GetActiveQuestionnaire(questId);

            if (activeQuestionnaire == null)
            {
                var ex = new InvalidOperationException("activeQuestionnaire not found");
                ex.Data["questId"] = questId;
                throw ex;
            }


            QuestionnaireAnswerDO questAnswerDO = new QuestionnaireAnswerDAO().
                GetLatestQuestionnaireAnswer(activeQuestionnaire.Uid, UserId);

            if (questAnswerDO != null)
            {
                RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDAO().
                    GetLatestRiskEvaluation(questAnswerDO.QuestAnswerId);

                bool canUsedRiskEvaluation = CheckRiskEvaCondition(riskEvaluationDO);

                if (!canUsedRiskEvaluation)
                {
                    var ex = new InvalidOperationException("Not can used risk evaluation");
                    ex.Data["canUsedRiskEvaluation"] = canUsedRiskEvaluation;
                    throw ex;
                }
            }

            result = new RiskEvaQuestionnaireEntity()
            {
                QuestionnaireEntity = activeQuestionnaire,
            };

            return result;
        }

        /// <summary>
        /// 取得暫存的風險評估資料
        /// </summary>
        /// <param name="key">暫存資料識別碼</param>
        /// <returns></returns>
        public RiskEvaResultDTO GetRiskResult(string key)
        {
            object riskEvaResultCache = CacheProvider.GetCache($"{_cacheKeyRiskEvaResultDTO}-{key}");

            RiskEvaResultDTO riskEvaResultDto = (riskEvaResultCache as RiskEvaResultDTO);

            return riskEvaResultDto;
        }

        /// <summary>
        /// 依投資風險屬性取得可投資的風險等級項目
        /// </summary>
        /// <param name="riskRankKind">投資風險屬性</param>
        /// <returns></returns>
        public IEnumerable<string> RiskRank(string riskRankKind)
        {
            IEnumerable<string> result = new List<string>();

            RiskRankEntity riskRankEntity = GetRiskRankEntities().
                First(x => x.RiskRankKind == riskRankKind);

            result = riskRankEntity.RiskRankDetailEntities.OrderBy(x => x.ProfitRiskRank).
                Select(x => x.ProfitRiskRank);

            return result;
        }

        /// <summary>
        /// 儲存評估投資風險評估結果資料
        /// </summary>
        /// <param name="riskResultId">風險評估結果識別代號</param>
        public void SaveRiskRank(string riskResultId)
        {
            try
            {
                RiskEvaResultDTO riskEvaResult = GetRiskResult(riskResultId);

                if (riskEvaResult == null)
                {
                    throw new InvalidOperationException("riskEvaResult not found");
                }

                RiskEvaluationDAO riskEvaluationDAO = new RiskEvaluationDAO();

                RiskEvaluationDO riskEvaluation = riskEvaluationDAO
                    .GetLatestRiskEvaluation(riskEvaResult.QuestionnaireResultEntity.QuestAnswerId);


                if (riskEvaluation == null)
                {
                    riskEvaluation = GetRiskEvaluationDO(riskEvaResult, "add");
                    riskEvaluationDAO.Insert(riskEvaluation);
                }
                else
                {
                    bool riskEvaluationInCuttimeRange = false;

                    if (riskEvaluation != null)
                    {
                        IEnumerable<DateTime> cuttimeRange = GetRiskEvaCuttime();

                        if (cuttimeRange == null)
                        {
                            throw new InvalidOperationException("cuttimeRange not found");
                        }

                        if ((riskEvaluation.EvaluationDate < cuttimeRange.Max()) &&
                            (riskEvaluation.EvaluationDate >= cuttimeRange.Min()))
                        {
                            riskEvaluationInCuttimeRange = true;
                        }
                    }

                    if (riskEvaluationInCuttimeRange)
                    {

                        if (riskEvaluation.IsUsed == "N")
                        {
                            riskEvaluation = GetRiskEvaluationDO(riskEvaResult, "update", riskEvaluation);
                            riskEvaluationDAO.Update(riskEvaluation);
                        }
                        else if (riskEvaluation.IsUsed == "Y")
                        {
                        }
                        else
                        {
                            throw new InvalidOperationException("riskEvaluation.IsUsed not found");
                        }
                    }
                }

                CacheProvider.SetCache($"{_cacheKeyRiskEvaluationDO}-" +
                            $"{riskEvaResult.QuestionnaireResultEntity.QuestAnswerId}", riskEvaluation, true);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }













        /// <summary>
        /// 判斷可否重做風險評估問卷
        /// </summary>
        /// <param name="riskEvaluationDO">風險評估紀錄</param>
        /// <returns></returns>
        public bool CheckRiskEvaCondition(RiskEvaluationDO riskEvaluationDO)
        {
            bool canUsedRiskEvaluation = true;

            if (riskEvaluationDO != null)
            {
                bool inCuttimeRange = false;

                IEnumerable<DateTime> cuttimeRange = GetRiskEvaCuttime();

                if (cuttimeRange == null)
                {
                    throw new InvalidOperationException("cuttimeRange not found");
                }

                if ((riskEvaluationDO.EvaluationDate < cuttimeRange.Max()) &&
                    (riskEvaluationDO.EvaluationDate >= cuttimeRange.Min()))
                {
                    inCuttimeRange = true;
                }

                if (inCuttimeRange && (riskEvaluationDO.IsUsed == "Y"))
                {
                    canUsedRiskEvaluation = false;
                }
            }

            return canUsedRiskEvaluation;
        }

        /// <summary>
        /// 取得投資風險評估切點時間範圍
        /// </summary>
        /// <returns>投資風險評估切點時間範圍</returns>
        private IEnumerable<DateTime> GetRiskEvaCuttime()
        {
            List<DateTime> cuttimeRange = null;

            string riskEvaCuttime = ConfigurationManager.AppSettings["risk.evaluation.cuttime"];

            string[] cuttimeArray = String.IsNullOrEmpty(riskEvaCuttime) ? null :
                riskEvaCuttime.Split(',');

            if (cuttimeArray != null)
            {
                List<DateTime> cuttimes = new List<DateTime>();

                foreach (string cuttime in cuttimeArray)
                {
                    if (!String.IsNullOrEmpty(cuttime) &&
                        (DateTime.TryParse(cuttime, out DateTime tempCuttime)))
                    {
                        cuttimes.Add(tempCuttime);
                    }
                }

                if (cuttimes.Count > 0)
                {
                    DateTime timeNow = DateTime.Now;
                    DateTime cuttimesMax = cuttimes.Max();
                    DateTime cuttimesMin = cuttimes.Min();

                    cuttimes.Add(cuttimesMax.AddDays(-1));
                    cuttimes.Add(cuttimesMin.AddDays(1));

                    cuttimeRange = new List<DateTime>()
                    {
                        cuttimes.Where(x => x < timeNow).Max(),
                        cuttimes.Where(x => x > timeNow).Min()
                    };
                }
            }

            return cuttimeRange;
        }




        /// <summary>
        /// 取出投資風險等級與等級明細
        /// </summary>
        /// <returns>投資風險等級資料</returns>
        private IEnumerable<RiskRankEntity> GetRiskRankEntities()
        {
            IEnumerable<RiskRankEntity> result = CacheProvider.GetCache("riskRankEntityList")
                as IEnumerable<RiskRankEntity>;

            if ((result == null) ||
                (result.Count() == 0))
            {
                IEnumerable<RiskRankDO> riskRankList = CacheProvider.
                    GetCache("riskRankList") as IEnumerable<RiskRankDO>;

                if ((riskRankList == null) ||
                    (riskRankList.Count() == 0))
                {
                    riskRankList = CacheProvider.SetCache("riskRankList", new RiskRankDAO().
                        ReadAll("FNDINV")) as IEnumerable<RiskRankDO>;
                }

                IEnumerable<RiskRankDetailDO> riskRankDetailList = CacheProvider.
                    GetCache("riskRankDetailList") as IEnumerable<RiskRankDetailDO>;

                if ((riskRankDetailList == null) ||
                    (riskRankDetailList.Count() == 0))
                {
                    riskRankDetailList = CacheProvider.SetCache("riskRankDetailList", new RiskRankDetailDAO().
                        ReadAll()) as IEnumerable<RiskRankDetailDO>;
                }

                List<RiskRankEntity> riskRankEntityList = new List<RiskRankEntity>();
                foreach (RiskRankDO riskRank in riskRankList.OrderBy(x => x.MinScore))
                {
                    riskRankEntityList.Add(new RiskRankEntity()
                    {
                        RiskEvaId = riskRank.RiskEvaId,
                        MinScore = riskRank.MinScore,
                        MaxScore = riskRank.MaxScore,
                        RiskRankKind = riskRank.RiskRankKind,
                        RiskRankDetailEntities = ConvertRiskRankDetailDO(riskRankDetailList.Where(x => x.RiskRankUid == riskRank.Uid)),
                    });
                }

                result = CacheProvider.SetCache("riskRankEntityList", riskRankEntityList)
                    as IEnumerable<RiskRankEntity>;
            }

            return result;
        }

        /// <summary>
        /// 轉換投資風險等級明細資料物件
        /// </summary>
        /// <param name="riskRankDetailDOList">投資風險等級明細資料集合</param>
        /// <returns>投資風險等級明細</returns>
        private IEnumerable<RiskRankDetailEntity> ConvertRiskRankDetailDO(
            IEnumerable<RiskRankDetailDO> riskRankDetailDOList)
        {
            return riskRankDetailDOList.Select(x => new RiskRankDetailEntity()
            {
                RiskRankUid = x.RiskRankUid,
                ProfitRiskRank = x.ProfitRiskRank,
                IsEffective = x.IsEffective,
            });
        }

        /// <summary>
        /// 檢查是否己有生效的風險評估紀錄
        /// </summary>
        /// <param name="questUid">問卷識別碼</param>
        /// <returns>投資風險評估問卷</returns>
        private RiskEvaQuestionnaireEntity CheckLatestRiskEvaluation(string questUid)
        {
            RiskEvaQuestionnaireEntity riskEvaQuestEntity = null;

            QuestionnaireEntity questEntity = QuestService.GetQuestionnaire(questUid);

            if (questEntity == null)
            {
                throw new InvalidOperationException("questEntity not found");
            }

            QuestionnaireAnswerDO questAnswer = new QuestionnaireAnswerDAO()
                .GetQuestionnaireAnswer(questEntity.Uid);

            if (questAnswer == null)
            {
                throw new InvalidOperationException(
                    $"questAnswerDO not found,activeQuestUid={questEntity.Uid}");
            }

            RiskEvaluationDO riskEvaluation = new RiskEvaluationDAO()
                .GetLatestUsedRiskEvaluation(questAnswer.QuestAnswerId);


            //TODO 1003 後面這段沒問題
            bool riskEvaluationInCuttimeRange = false;

            if (riskEvaluation != null)
            {
                IEnumerable<DateTime> cuttimeRange = GetRiskEvaCuttime();

                if (cuttimeRange == null)
                {
                    throw new InvalidOperationException("cuttimeRange not found");
                }

                if ((riskEvaluation.EvaluationDate < cuttimeRange.Max()) &&
                    (riskEvaluation.EvaluationDate >= cuttimeRange.Min()))
                {
                    riskEvaluationInCuttimeRange = true;
                }
            }

            riskEvaQuestEntity = new RiskEvaQuestionnaireEntity()
            {
                QuestionnaireEntity = questEntity,
                CanRiskEvaluation = !riskEvaluationInCuttimeRange,
            };

            return riskEvaQuestEntity;
        }

        /// <summary>
        /// 轉換投資風險評估結果資料物件
        /// </summary>
        /// <param name="riskEvaDO">投資風險評估結果資料物件</param>
        /// <returns>投資風險評估結果</returns>
        private RiskEvaluationEntity ConvertRiskEvaluationDO(RiskEvaluationDO riskEvaDO)
        {
            return new RiskEvaluationEntity()
            {
                Uid = riskEvaDO.Uid,
                RiskEvaId = riskEvaDO.RiskEvaId,
                QuestAnswerId = riskEvaDO.QuestAnswerId,
                CliId = riskEvaDO.CliId,
                RiskResult = riskEvaDO.RiskResult,
                RiskScore = riskEvaDO.RiskScore,
                RiskAttribute = riskEvaDO.RiskAttribute,
                EvaluationDate = riskEvaDO.EvaluationDate,
                BusinessDate = riskEvaDO.BusinessDate,
                IsUsed = riskEvaDO.IsUsed,
                CreateUserId = riskEvaDO.CreateUserId,
                CreateTime = riskEvaDO.CreateTime,
                ModifyUserId = riskEvaDO.ModifyUserId,
                ModifyTime = riskEvaDO.ModifyTime,
            };
        }

        /// <summary>
        /// 取得投資風險評估結果資料
        /// </summary>
        /// <param name="riskEvaResult">投資風險等級評估結果</param>
        /// <param name="action">操作動作</param>
        /// <returns>投資風險評估結果資料</returns>
        private RiskEvaluationDO GetRiskEvaluationDO(RiskEvaResultDTO riskEvaResult, string action,
            RiskEvaluationDO riskEvaluation = null)
        {
            RiskEvaluationDO result = null;

            RiskEvaluationEntity riskEvaEntity = riskEvaResult.RiskEvaluationEntity;
            if (riskEvaEntity == null)
            {
                throw new InvalidOperationException("riskEvaEntity not found");
            }


            DateTime currentTime = DateTime.Now;
            string userId = riskEvaEntity.CliId;
            DateTime datetimeFormat =
                new DateTime(currentTime.Year, currentTime.Month, currentTime.Day);


            if (action == "add")
            {
                result = new RiskEvaluationDO()
                {
                    Uid = Guid.NewGuid(),
                    RiskEvaId = "FNDINV",
                    QuestAnswerId = riskEvaEntity.QuestAnswerId,
                    CliId = userId,
                    RiskResult = riskEvaEntity.RiskResult,
                    RiskScore = riskEvaEntity.RiskScore,
                    RiskAttribute = riskEvaEntity.RiskAttribute,
                    EvaluationDate = datetimeFormat,
                    BusinessDate = (currentTime.Hour < 16) ? datetimeFormat : datetimeFormat.AddDays(1),
                    IsUsed = "N",
                    CreateUserId = userId,
                    CreateTime = currentTime,
                    ModifyUserId = null,
                    ModifyTime = null,
                };
            }
            else if (action == "update")
            {
                result = new RiskEvaluationDO()
                {
                    Uid = riskEvaluation.Uid,
                    RiskEvaId = riskEvaluation.RiskEvaId,
                    QuestAnswerId = riskEvaEntity.QuestAnswerId,
                    CliId = riskEvaluation.CliId,
                    RiskResult = riskEvaEntity.RiskResult,
                    RiskScore = riskEvaEntity.RiskScore,
                    RiskAttribute = riskEvaEntity.RiskAttribute,
                    EvaluationDate = datetimeFormat,
                    BusinessDate = (currentTime.Hour < 16) ? datetimeFormat : datetimeFormat.AddDays(1),
                    IsUsed = "N",
                    CreateUserId = riskEvaluation.CreateUserId,
                    CreateTime = riskEvaluation.CreateTime,
                    ModifyUserId = userId,
                    ModifyTime = datetimeFormat,
                };
            }
            else
            {
                throw new InvalidOperationException("action not found");
            }

            return result;
        }

    }
}
