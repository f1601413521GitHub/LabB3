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
        [TestMethod()]
        public void RiskRankTest_When_RiskRankKind_Is_L_Then_Success()
        {
            //Arrage
            string riskRankKind = "L";
            List<string> expected = new List<string>()
            {
                "RR1","RR2",
            };


            //Actual
            IEnumerable<string> result = new RiskEvaluationService().RiskRank(riskRankKind);
            List<string> actual = result.ToList();

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RiskRankTest_When_RiskRankKind_Is_M_Then_Success()
        {
            //Arrage
            string riskRankKind = "M";
            List<string> expected = new List<string>()
            {
                "RR1","RR2","RR3",
            };


            //Actual
            IEnumerable<string> result = new RiskEvaluationService().RiskRank(riskRankKind);
            List<string> actual = result.ToList();

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RiskRankTest_When_RiskRankKind_Is_H_Then_Success()
        {
            //Arrage
            string riskRankKind = "H";
            List<string> expected = new List<string>()
            {
                "RR1","RR2","RR3","RR4","RR5",
            };


            //Actual
            IEnumerable<string> result = new RiskEvaluationService().RiskRank(riskRankKind);
            List<string> actual = result.ToList();

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaConditionTest_Faile()
        {
            //Arrange
            var currentTime = DateTime.Now;
            var riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                10, 30, 01),
                IsUsed = "Y",
            };
            bool expected = false;

            //Actual
            bool actual = new RiskEvaluationService().CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CheckRiskEvaConditionTest_Success()
        {
            //Arrange
            var currentTime = DateTime.Now;
            var riskEvaluationDO = new RiskEvaluationDO()
            {
                EvaluationDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                10, 29, 59),
                IsUsed = "Y",
            };
            bool expected = true;

            //Actual
            bool actual = new RiskEvaluationService().CheckRiskEvaCondition(riskEvaluationDO);

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}