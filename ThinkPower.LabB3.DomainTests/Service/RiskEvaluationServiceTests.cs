using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkPower.LabB3.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.DataAccess.DAO;
using Newtonsoft.Json;
using ThinkPower.LabB3.DataAccess.DO;
using ThinkPower.LabB3.Domain.Entity;

namespace ThinkPower.LabB3.Domain.Service.Tests
{
    [TestClass()]
    public class RiskEvaluationServiceTests
    {
        [TestMethod()]
        public void CheckRiskEvaCondition_When_RiskEvaluationDO_Is_Null_Then_Success()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });
            RiskEvaluationDO riskEvaluationDO = null;
            bool expected = true;

            //Actual
            bool actual = riskService.CheckCanEvaluteRisk(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_InCuttimeRange_And_IsUsed_Is_Y_Then_Fail()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = riskService.GetCurrentCuttimeRange().StartTime,
                IsUsed = "Y",
            };

            bool expected = false;

            //Actual
            bool actual = riskService.CheckCanEvaluteRisk(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_InCuttimeRange_And_IsUsed_Is_N_Then_Success()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = riskService.GetCurrentCuttimeRange().StartTime,
                IsUsed = "N",
            };

            bool expected = true;

            //Actual
            bool actual = riskService.CheckCanEvaluteRisk(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_NotInCuttimeRange_And_IsUsed_Is_Y_Then_Success()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = riskService.GetCurrentCuttimeRange().StartTime.AddSeconds(-1),
                IsUsed = "Y",
            };

            bool expected = true;

            //Actual
            bool actual = riskService.CheckCanEvaluteRisk(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_NotInCuttimeRange_And_IsUsed_Is_N_Then_Success()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = riskService.GetCurrentCuttimeRange().StartTime.AddSeconds(-1),
                IsUsed = "N",
            };

            bool expected = true;

            //Actual
            bool actual = riskService.CheckCanEvaluteRisk(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckInCuttimeRangeTest_When_EvaluationDate_InCuttimeRange_Then_Success()
        {
            //Arrage
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            var riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = riskService.GetCurrentCuttimeRange().StartTime,
            };

            var expected = true;

            //Actual
            var actual = riskService.CheckInCuttimeRange(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckInCuttimeRangeTest_When_EvaluationDate_NotInCuttimeRange_Then_Fail()
        {
            //Arrage
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            var riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = riskService.GetCurrentCuttimeRange().StartTime.AddSeconds(-1),
            };

            var expected = false;

            //Actual
            var actual = riskService.CheckInCuttimeRange(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void GetTest_Fail()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            string uid = null;

            bool? expected = false;

            //Actual
            bool? actual = null;

            try
            {
                riskService.Get(uid);
            }
            catch (NotImplementedException e)
            {
                actual = false;
            }

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RiskRankTest_Fail()
        {
            //Arrange
            var riskService = new RiskEvaluationService(new CuttimeRangeInfoEntity() {
                StartTime = new DateTime(2018,10,08,10,30,00),
                EndTime = new DateTime(2018,10,08,18,30,00),
            });

            string riskRankKind = null;
            bool? expected = false;

            //Actual
            bool? actual = null;

            try
            {
                riskService.RiskRank(riskRankKind);
            }
            catch (NotImplementedException e)
            {
                actual = false;
            }

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}