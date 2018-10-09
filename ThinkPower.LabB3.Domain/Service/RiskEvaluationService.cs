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
        #region Private property

        /// <summary>
        /// NLog Object
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 投資風險評估切點時間範圍
        /// </summary>
        private CuttimeRangeInfoEntity _cuttimeRange = null;

        /// <summary>
        /// 問卷服務 隱藏欄位
        /// </summary>
        private QuestionnaireService _questService = null;

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
        /// 風險評估項目代號(基金投資)
        /// </summary>
        private readonly string _riskEvaIdFundInvestment = "FNDINV";

        /// <summary>
        /// 問卷填答評分結果暫存鍵值
        /// </summary>
        private readonly string _cacheKeyQuestionResultEntity = "QuestionnaireResultEntity";

        /// <summary>
        /// 投資風險等級與等級明細暫存鍵值
        /// </summary>
        private readonly string _cacheKeyRiskRankEntities = "RiskRankEntities";

        #endregion

        #region Constructor

        public RiskEvaluationService()
        {
        }

        /// <summary>
        /// 此建構函式會設定投資風險評估切點時間範圍的起訖時間。
        /// </summary>
        /// <param name="cuttimeRange">投資風險評估切點時間範圍</param>
        public RiskEvaluationService(CuttimeRangeInfoEntity cuttimeRange)
        {
            _cuttimeRange = cuttimeRange;
        }

        #endregion

        #region Public method

        /// <summary>
        /// 取得風險評估問卷資料
        /// </summary>
        /// <param name="questId">問卷編號</param>
        /// <param name="userId">用戶ID</param>
        /// <returns>風險評估問卷資料</returns>
        public RiskEvaQuestionnaireEntity GetRiskQuestionnaire(string questId, string userId)
        {
            RiskEvaQuestionnaireEntity result = null;

            if (String.IsNullOrEmpty(questId))
            {
                throw new ArgumentNullException("questId");
            }
            else if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("userId");
            }

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDAO().GetLatestRiskEvaluation(userId);

            bool canEvaluteRisk = CheckCanEvaluteRisk(riskEvaluationDO);

            if (!canEvaluteRisk)
            {
                var ex = new InvalidOperationException("Not can used risk evaluation");
                ex.Data["canEvaluteRisk"] = canEvaluteRisk;
                throw ex;
            }

            QuestionnaireEntity activeQuestionnaire = QuestService.GetActiveQuestionnaire(questId);

            if (activeQuestionnaire == null)
            {
                var ex = new InvalidOperationException("activeQuestionnaire not found");
                ex.Data["questId"] = questId;
                throw ex;
            }

            result = new RiskEvaQuestionnaireEntity()
            {
                QuestionnaireEntity = activeQuestionnaire,
            };

            return result;
        }

        /// <summary>
        /// 評估投資風險等級
        /// </summary>
        /// <param name="answer">風險評估填答資料</param>
        /// <returns></returns>
        public RiskEvaResultDTO EvaluateRiskRank(RiskEvaAnswerEntity answer)
        {
            RiskEvaResultDTO riskEvaResultDTO = null;
            QuestionnaireResultEntity questionResultEntity = null;

            if (answer == null)
            {
                throw new ArgumentNullException("answer");
            }
            else if (answer.QuestionnaireAnswerEntity == null)
            {
                throw new ArgumentNullException("QuestionnaireAnswerEntity");
            }

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDAO().GetLatestRiskEvaluation(
                answer.QuestionnaireAnswerEntity.UserId);

            bool canEvaluteRisk = CheckCanEvaluteRisk(riskEvaluationDO);

            if (!canEvaluteRisk)
            {
                var ex = new InvalidOperationException("Not can used risk evaluation");
                ex.Data["canEvaluteRisk"] = canEvaluteRisk;
                throw ex;
            }




            questionResultEntity = QuestService.Calculate(answer.QuestionnaireAnswerEntity);

            if (questionResultEntity == null)
            {
                throw new InvalidOperationException("questResultEntity not found");
            }

            if (questionResultEntity.QuestionnaireEntity.NeedScore == "Y")
            {
                RiskRankDO riskRankDO = new RiskRankDAO().GetRiskRank(questionResultEntity.ActualScore,
                    _riskEvaIdFundInvestment);

                if (riskRankDO == null)
                {
                    var ex = new InvalidOperationException("riskRankDO not found");
                    ex.Data["QuestId"] = questionResultEntity.QuestionnaireEntity.QuestId;
                    ex.Data["UserId"] = answer.QuestionnaireAnswerEntity.UserId;
                    ex.Data["ActualScore"] = questionResultEntity.ActualScore;
                    ex.Data["riskEvaId"] = _riskEvaIdFundInvestment;
                    throw ex;
                }

                questionResultEntity.RiskRankKind = riskRankDO.RiskRankKind;
            }




            riskEvaResultDTO = new RiskEvaResultDTO()
            {
                QuestionnaireResultEntity = questionResultEntity,
                RiskRankEntities = GetRiskRankEntities(_riskEvaIdFundInvestment),
            };

            CacheProvider.SetCache($"{_cacheKeyQuestionResultEntity}-{questionResultEntity.QuestAnswerId}",
                questionResultEntity);

            return riskEvaResultDTO;
        }

        /// <summary>
        /// 儲存評估投資風險評估結果資料
        /// </summary>
        /// <param name="riskResultId">風險評估結果識別代號</param>
        public void SaveRiskRank(string riskResultId)
        {
            if (String.IsNullOrEmpty(riskResultId))
            {
                throw new ArgumentNullException("riskResultId");
            }

            RiskEvaResultDTO riskEvaResultDTO = GetRiskResult(riskResultId);

            if (riskEvaResultDTO == null)
            {
                var ex = new InvalidOperationException("riskEvaResultDTO not found");
                ex.Data["riskResultId"] = riskResultId;
                throw ex;
            }
            else if (riskEvaResultDTO.QuestionnaireResultEntity == null)
            {
                var ex = new InvalidOperationException("QuestionnaireResultEntity not found");
                ex.Data["riskResultId"] = riskResultId;
                throw ex;
            }

            QuestionnaireResultEntity questResultEntity = riskEvaResultDTO.QuestionnaireResultEntity;

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDAO().GetLatestRiskEvaluation(
                questResultEntity.TesteeId);

            bool canEvaluteRisk = CheckCanEvaluteRisk(riskEvaluationDO);

            if (!canEvaluteRisk)
            {
                var ex = new InvalidOperationException("Not can used risk evaluation");
                ex.Data["canEvaluteRisk"] = canEvaluteRisk;
                throw ex;
            }


            DateTime currenTime = DateTime.Now;

            if (riskEvaluationDO == null)
            {
                riskEvaluationDO = new RiskEvaluationDO()
                {
                    Uid = Guid.NewGuid(),
                    RiskEvaId = _riskEvaIdFundInvestment,
                    QuestAnswerId = questResultEntity.QuestAnswerId,
                    CliId = questResultEntity.TesteeId,
                    RiskResult = string.Join(";", questResultEntity.RiskResult.Select(x => $"[{x.Key}:{x.Value}]")),
                    RiskScore = questResultEntity.ActualScore,
                    RiskAttribute = questResultEntity.RiskRankKind,
                    EvaluationDate = currenTime,
                    BusinessDate = ConvertBusinessDate(currenTime),
                    IsUsed = "N",
                    CreateUserId = questResultEntity.TesteeId,
                    CreateTime = currenTime,
                    ModifyUserId = null,
                    ModifyTime = null,
                };

                new RiskEvaluationDAO().Insert(riskEvaluationDO);
            }
            else
            {
                bool inCuttimeRange = CheckInCuttimeRange(riskEvaluationDO);

                if (inCuttimeRange)
                {
                    riskEvaluationDO.QuestAnswerId = questResultEntity.QuestAnswerId;
                    riskEvaluationDO.RiskResult = String.Join(";", questResultEntity.RiskResult.Select(x => $"[{x.Key}:{x.Value}]"));
                    riskEvaluationDO.RiskScore = questResultEntity.ActualScore;
                    riskEvaluationDO.RiskAttribute = questResultEntity.RiskRankKind;
                    riskEvaluationDO.EvaluationDate = currenTime;
                    riskEvaluationDO.BusinessDate = ConvertBusinessDate(currenTime);
                    riskEvaluationDO.IsUsed = "N";
                    riskEvaluationDO.ModifyUserId = questResultEntity.TesteeId;
                    riskEvaluationDO.ModifyTime = currenTime;

                    new RiskEvaluationDAO().Update(riskEvaluationDO);
                }
                else
                {
                    riskEvaluationDO = new RiskEvaluationDO()
                    {
                        Uid = Guid.NewGuid(),
                        RiskEvaId = _riskEvaIdFundInvestment,
                        QuestAnswerId = questResultEntity.QuestAnswerId,
                        CliId = questResultEntity.TesteeId,
                        RiskResult = String.Join(";", questResultEntity.RiskResult.Select(x => $"[{x.Key}:{x.Value}]")),
                        RiskScore = questResultEntity.ActualScore,
                        RiskAttribute = questResultEntity.RiskRankKind,
                        EvaluationDate = currenTime,
                        BusinessDate = ConvertBusinessDate(currenTime),
                        IsUsed = "N",
                        CreateUserId = questResultEntity.TesteeId,
                        CreateTime = currenTime,
                        ModifyUserId = null,
                        ModifyTime = null,
                    };

                    new RiskEvaluationDAO().Insert(riskEvaluationDO);
                }
            }
        }

        /// <summary>
        /// 取得暫存的風險評估資料
        /// </summary>
        /// <param name="key">風險評估結果識別代號</param>
        /// <returns>風險評估資料</returns>
        public RiskEvaResultDTO GetRiskResult(string key)
        {
            return new RiskEvaResultDTO()
            {
                QuestionnaireResultEntity = CacheProvider.GetCache(
                    $"{_cacheKeyQuestionResultEntity}-{key}") as QuestionnaireResultEntity,
            };
        }

        /// <summary>
        /// 依紀錄識別碼取得風險評估資料
        /// </summary>
        /// <param name="uid">紀錄識別碼</param>
        /// <returns></returns>
        public RiskEvaluationEntity Get(string uid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 依投資風險屬性取得可投資的風險等級項目
        /// </summary>
        /// <param name="riskRankKind">投資風險屬性</param>
        /// <returns></returns>
        public IEnumerable<string> RiskRank(string riskRankKind)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private method

        /// <summary>
        /// 判斷可否重做風險評估問卷
        /// </summary>
        /// <param name="riskEvaluationDO">風險評估紀錄</param>
        /// <returns></returns>
        public bool CheckCanEvaluteRisk(RiskEvaluationDO riskEvaluationDO)
        {
            bool canEvaluteRisk = true;

            if (riskEvaluationDO != null)
            {
                bool inCuttimeRange = CheckInCuttimeRange(riskEvaluationDO);

                if (inCuttimeRange && (riskEvaluationDO.IsUsed == "Y"))
                {
                    canEvaluteRisk = false;
                }
            }

            return canEvaluteRisk;
        }

        /// <summary>
        /// 檢查風險紀錄評估時間是否在目前作業週期內
        /// </summary>
        /// <param name="riskEvaluationDO">投資風險評估結果</param>
        /// <returns>檢查結果</returns>
        public bool CheckInCuttimeRange(RiskEvaluationDO riskEvaluationDO)
        {
            bool inCuttimeRange = false;

            CuttimeRangeInfoEntity cuttimeRange = GetCurrentCuttimeRange();

            if (cuttimeRange == null)
            {
                throw new InvalidOperationException("cuttimeRange not found");
            }

            if ((riskEvaluationDO.EvaluationDate < cuttimeRange.EndTime) &&
                (riskEvaluationDO.EvaluationDate >= cuttimeRange.StartTime))
            {
                inCuttimeRange = true;
            }

            return inCuttimeRange;
        }

        /// <summary>
        /// 取得投資風險評估切點時間範圍
        /// </summary>
        /// <returns>投資風險評估切點時間範圍</returns>
        public CuttimeRangeInfoEntity GetCurrentCuttimeRange()
        {
            CuttimeRangeInfoEntity cuttimeRange = null;

            if (_cuttimeRange != null)
            {
                return _cuttimeRange;
            }

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

                    cuttimeRange = new CuttimeRangeInfoEntity
                    {
                        StartTime = cuttimes.Where(x => x < timeNow).Max(),
                        EndTime = cuttimes.Where(x => x > timeNow).Min()
                    };
                }
            }

            return cuttimeRange;
        }

        /// <summary>
        /// 取出投資風險等級與等級明細
        /// </summary>
        /// <param name="riskEvaId">風險評估項目代號</param>
        /// <returns>投資風險等級資料</returns>
        private IEnumerable<RiskRankEntity> GetRiskRankEntities(string riskEvaId)
        {
            IEnumerable<RiskRankEntity> riskRankEntities = CacheProvider.GetCache(_cacheKeyRiskRankEntities)
                as IEnumerable<RiskRankEntity>;

            if ((riskRankEntities == null) ||
                (riskRankEntities.Count() == 0))
            {
                IEnumerable<RiskRankDO> riskRankList = new RiskRankDAO().ReadAll(riskEvaId);

                IEnumerable<RiskRankDetailDO> riskRankDetailList = new RiskRankDetailDAO().ReadAll();

                riskRankEntities = riskRankList.OrderBy(x => x.MinScore).
                    Select(riskRank => new RiskRankEntity()
                    {
                        RiskEvaId = riskRank.RiskEvaId,
                        MinScore = riskRank.MinScore,
                        MaxScore = riskRank.MaxScore,
                        RiskRankKind = riskRank.RiskRankKind,

                        RiskRankDetailEntities = ConvertRiskRankDetailDO(riskRankDetailList.
                            Where(riskRankDetail => riskRankDetail.RiskRankUid == riskRank.Uid)),
                    });

                CacheProvider.SetCache(_cacheKeyRiskRankEntities, riskRankEntities);
            }

            return riskRankEntities;
        }

        /// <summary>
        /// 轉換投資風險等級明細資料
        /// </summary>
        /// <param name="riskRankDetailDOList">投資風險等級明細資料</param>
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
        /// 轉換資料時間
        /// </summary>
        /// <param name="currenTime">目前時間</param>
        /// <returns>資料時間</returns>
        private DateTime ConvertBusinessDate(DateTime currenTime)
        {
            return (currenTime.Hour < 16) ? currenTime.Date : currenTime.Date.AddDays(1);
        }

        #endregion
    }
}
