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

namespace ThinkPower.LabB3.Domain.Service.Tests
{
    [TestClass()]
    public class RiskEvaluationServiceTests
    {
        private static RiskEvaluationService _riskService = new RiskEvaluationService();
        private IEnumerable<DateTime> _cuttime = _riskService.GetRiskEvaCuttime();

        [TestMethod()]
        public void GetRiskEvaCuttimeTest_Success()
        {
            //Arrange
            bool expected = true;

            //Actual
            bool actual = (_cuttime != null && _cuttime.Count() > 0);

            //Aseert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaCondition_When_RiskEvaluationDO_Is_Null_Then_Success()
        {
            //Arrange
            RiskEvaluationDO riskEvaluationDO = null;
            bool expected = true;

            //Actual
            bool actual = _riskService.CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_InCuttimeRange_And_IsUsed_Is_Y_Then_Fail()
        {
            //Arrange
            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = _cuttime.Min(),
                IsUsed = "Y",
            };
            bool expected = false;

            //Actual
            bool actual = _riskService.CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_InCuttimeRange_And_IsUsed_Is_N_Then_Success()
        {
            //Arrange
            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = _cuttime.Min(),
                IsUsed = "N",
            };
            bool expected = true;

            //Actual
            bool actual = _riskService.CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_NotInCuttimeRange_And_IsUsed_Is_Y_Then_Success()
        {
            //Arrange
            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = _cuttime.Min().AddSeconds(-1),
                IsUsed = "Y",
            };
            bool expected = true;

            //Actual
            bool actual = _riskService.CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod()]
        public void CheckRiskEvaCondition_When_EvaluationDate_NotInCuttimeRange_And_IsUsed_Is_N_Then_Success()
        {
            //Arrange
            RiskEvaluationDO riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = _cuttime.Min().AddSeconds(-1),
                IsUsed = "N",
            };
            bool expected = true;

            //Actual
            bool actual = _riskService.CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }



        [TestMethod()]
        public void CheckInCuttimeRangeTest_When_EvaluationDate_InCuttimeRange_Then_Success()
        {
            //Arrage
            var riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = _cuttime.Min(),
            };

            var expected = true;

            //Actual
            var actual = _riskService.CheckInCuttimeRange(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckInCuttimeRangeTest_When_EvaluationDate_NotInCuttimeRange_Then_Fail()
        {
            //Arrage
            var riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = _cuttime.Min().AddSeconds(-1),
            };

            var expected = false;

            //Actual
            var actual = _riskService.CheckInCuttimeRange(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GetTest_Fail()
        {
            //Arrange
            string uid = null;
            bool? expected = false;

            //Actual
            bool? actual = null;

            try
            {
                _riskService.Get(uid);
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
            string riskRankKind = null;
            bool? expected = false;

            //Actual
            bool? actual = null;

            try
            {
                _riskService.RiskRank(riskRankKind);
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