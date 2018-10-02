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
        /// 投資風險評估結果的暫存鍵值
        /// </summary>
        private readonly string _cacheKeyRiskEvaluation = "RiskEvaluation";

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

                riskEvaQuestEntity = CheckLatestRiskEvaluation(answer.QuestionnaireAnswerEntity.QuestUid);

                if (riskEvaQuestEntity == null)
                {
                    throw new InvalidOperationException("riskEvaQuestEntity not found");
                }

                if (!riskEvaQuestEntity.CanUseRiskEvaluation)
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


                        CacheProvider.GetCache($"{_cacheKeyRiskEvaluation}-{questResultEntity.QuestAnswerId}",
                            riskEvaluationEntity, true);

                    }
                }

                result = new RiskEvaResultDTO()
                {
                    QuestionnaireResultEntity = questResultEntity,
                    RiskEvaQuestionnaire = riskEvaQuestEntity,
                    RiskEvaluationEntity = riskEvaluationEntity,
                };

                CacheProvider.GetCache($"{_cacheKeyRiskEvaResultDTO}-{questResultEntity.QuestAnswerId}",
                    result, true);

            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
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
                    $"questAnswer not found,activeQuestUid={questEntity.Uid}");
            }

            RiskEvaluationDO riskEvaluation = new RiskEvaluationDAO()
                .GetLatestUsedRiskEvaluation(questAnswer.QuestAnswerId);

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
                CanUseRiskEvaluation = !riskEvaluationInCuttimeRange,
            };

            return riskEvaQuestEntity;
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

            try
            {
                object cache = CacheProvider.GetCache($"{_cacheKeyRiskEvaluationDO}-{uid}", null, false);
                RiskEvaluationDO riskEvaDO = cache as RiskEvaluationDO;
                if (riskEvaDO == null)
                {
                    throw new InvalidOperationException("riskEvaDO not found");
                }

                riskEvaEntity = ConvertRiskEvaluationDO(riskEvaDO);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return riskEvaEntity;
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

            try
            {
                QuestionnaireEntity activeQuest = QuestService.GetActiveQuestionnaire(questId);

                QuestionnaireAnswerDO questAnswer =
                    new QuestionnaireAnswerDAO().GetQuestionnaireAnswer(activeQuest.Uid);

                if (questAnswer == null)
                {
                    var ex = new InvalidOperationException("questAnswer not found");
                    ex.Data["ActiveQuestionnaire-Uid"] = activeQuest.Uid;

                    throw ex;
                }

                RiskEvaluationDO riskEvaluation =
                    new RiskEvaluationDAO().GetLatestUsedRiskEvaluation(questAnswer.QuestAnswerId);

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

                result = new RiskEvaQuestionnaireEntity()
                {
                    QuestionnaireEntity = activeQuest,
                    CanUseRiskEvaluation = !riskEvaluationInCuttimeRange,
                };
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }

            return result;
        }

        /// <summary>
        /// 取得暫存的風險評估資料
        /// </summary>
        /// <param name="key">暫存資料識別碼</param>
        /// <returns></returns>
        public RiskEvaResultDTO GetRiskResult(string key)
        {
            object riskEvaResultCache = CacheProvider.
                GetCache($"{_cacheKeyRiskEvaResultDTO}-{key}", null, false);

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
            List<string> result = new List<string>();

            List<RiskRankDO> riskRankList = new RiskRankDAO().ReadAll("FNDINV");
            List<RiskRankDetailDO> riskRankDetailList = new RiskRankDetailDAO().ReadAll();

            string riskState = "";
            string riskTile = "";
            string riskAttribute = "";
            string riskLevel = "";

            foreach (RiskRankDO riskRank in riskRankList.OrderBy(x => x.MinScore))
            {
                IEnumerable<RiskRankDetailDO> rankDetailList = riskRankDetailList
                    .Where(x => x.RiskRankUid == riskRank.Uid).OrderBy(x => x.ProfitRiskRank);


                riskState = (riskRank.RiskRankKind == riskRankKind) ? "■" : "□";
                riskTile = "";
                riskAttribute = "";
                riskLevel = String.Join("、", rankDetailList.Select(x => x.ProfitRiskRank));

                switch (riskRank.RiskRankKind)
                {
                    case "L":
                        riskTile = $"{riskState}{riskRank.MaxScore}分以下";
                        riskAttribute = "低(保守)";
                        break;

                    case "M":
                        riskTile = $"{riskState}{riskRank.MinScore}~{riskRank.MaxScore}分以下";
                        riskAttribute = "中(穩健)";
                        break;

                    case "H":
                        riskTile = $"{riskState}{riskRank.MaxScore ?? riskRank.MinScore}分(含)以上";
                        riskAttribute = "高(成長)";
                        break;

                    default:
                        break;
                }

                result.Add(String.Join("|", riskTile, riskAttribute, riskLevel));
            }

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

                CacheProvider.GetCache($"{_cacheKeyRiskEvaluationDO}-" +
                            $"{riskEvaResult.QuestionnaireResultEntity.QuestAnswerId}", riskEvaluation, true);
            }
            catch (Exception e)
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
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



        /// <summary>
        /// 取得投資風險評估切點時間範圍
        /// </summary>
        /// <returns>投資風險評估切點時間範圍</returns>
        private IEnumerable<DateTime> GetRiskEvaCuttime()
        {
            List<DateTime> cuttimeRange = null;

            string riskEvaCuttime = ConfigurationManager.AppSettings["risk.evaluation.cuttime"];

            string[] cuttimeArray = !String.IsNullOrEmpty(riskEvaCuttime) ?
                riskEvaCuttime.Split(',') :
                null;

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
    }
}
