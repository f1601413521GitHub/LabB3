using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkPower.LabB3.Domain.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.Entity.Question;
using System.Configuration;
using ThinkPower.LabB3.Domain.Entity;
using Newtonsoft.Json;
using System.Data;

namespace ThinkPower.LabB3.Domain.Service.Tests
{
    [TestClass()]
    public class QuestionnaireServiceTests
    {
        [TestMethod()]
        public void CalculateScoreTest()
        {
            //Arrange
            int expected = 19;

            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(_answer,
                _questionnaire);

            int actual = result.ActualScore;


            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_AllRule_Then_Success()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>();

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_ValidateNeedAnswer_Then_Fail()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q002", "此題必須填答!"},
            };
            _answer.AnswerDetailEntities.First(x => x.QuestionId == "Q002").AnswerCode = "";

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_ValidateNeedAnswer_AllowNaCondition_Then_Fail()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題必須填答!"},
            };
            _answer.AnswerDetailEntities.First(x => x.QuestionId == "Q001").AnswerCode = "3";
            _answer.AnswerDetailEntities.First(x => x.QuestionId == "Q002").AnswerCode = "3";
            _answer.AnswerDetailEntities = _answer.AnswerDetailEntities.Where(x => x.QuestionId != "Q003");

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_ValidateMinMultipleAnswers_Then_Fail()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題至少須勾選3個項目!"},
            };
            _questionnaire.QuestDefineEntities.First(x => x.QuestionId == "Q003").MinMultipleAnswers = 3;

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_ValidateMaxMultipleAnswers_Then_Fail()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題至多僅能勾選1個項目!"},
            };
            _questionnaire.QuestDefineEntities.First(x => x.QuestionId == "Q003").MaxMultipleAnswers = 1;

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_ValidateSingleAnswerCondition_Then_Fail()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題僅能勾選1個項目!"},
            };
            _answer.AnswerDetailEntities.First(x => x.QuestionId == "Q003").AnswerCode = "6";

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRuleTest_When_ValidateOtherAnswer_Then_Fail()
        {
            //Arrange
            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題僅能勾選1個項目!"},
            };
            _answer.AnswerDetailEntities.First(x => x.QuestionId == "Q003").AnswerCode = "6";

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(_answer, _questionnaire);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #region ValidateRule TestData

        private QuestionnaireAnswerEntity _answer = new QuestionnaireAnswerEntity()
        {
            AnswerDetailEntities = new List<AnswerDetailEntity>()
                {
                    new AnswerDetailEntity() {QuestionId="Q001", AnswerCode = "5", OtherAnswer = "", Score = 5, },
                    new AnswerDetailEntity() {QuestionId="Q002", AnswerCode = "2", OtherAnswer = "", Score = 2, },
                    new AnswerDetailEntity() {QuestionId="Q003", AnswerCode = "3", OtherAnswer = "", Score = 3, },
                    new AnswerDetailEntity() {QuestionId="Q003", AnswerCode = "2", OtherAnswer = "", Score = 2, },
                    new AnswerDetailEntity() {QuestionId="Q004", AnswerCode = "1", OtherAnswer = "", Score = 1, },
                    new AnswerDetailEntity() {QuestionId="Q005", AnswerCode = "2", OtherAnswer = "", Score = 2, },
                    new AnswerDetailEntity() {QuestionId="Q006", AnswerCode = "3", OtherAnswer = "", Score = 3, },
                    new AnswerDetailEntity() {QuestionId="Q007", AnswerCode = "3", OtherAnswer = "", Score = 3, },
                },
        };

        private QuestionnaireEntity _questionnaire = new QuestionnaireEntity()
        {
            ScoreKind = "1",
            QuestScore = 35,
            QuestDefineEntities = new List<QuestDefineEntity>()
                {
                    new QuestDefineEntity() {Uid=Guid.Parse("F9DD6938-2DBD-48AF-9D44-428DCB2AC5D6"), QuestionId="Q001",    CountScoreType = "2",    NeedAnswer = "N",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="",                                                                                                                                AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                    new QuestDefineEntity() {Uid=Guid.Parse("E33802B8-8FE3-4213-BF25-CCF08EF653D9"), QuestionId="Q002",    CountScoreType = "2",    NeedAnswer = "Y",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="",                                                                                                                                AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                    new QuestDefineEntity() {Uid=Guid.Parse("268B8A9D-AC0E-42A2-BEB6-08BC8629DB6B"), QuestionId="Q003",    CountScoreType = "2",    NeedAnswer = "Y",    AnswerType = "M",     MinMultipleAnswers=1,        MaxMultipleAnswers=3,       SingleAnswerCondition="{\"Conditions\":[{\"QuestionId\": \"Q003\", \"AnswerCode\": [\"6\"]}]}",    AllowNaCondition="{\"Conditions\":[{\"QuestionId\": \"Q001\", \"AnswerCode\": [\"1\"]}, {\"QuestionId\": \"Q002\", \"AnswerCode\": [\"1\"]}]}",    AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                        new AnswerDefineEntity(){AnswerCode="6", Score = 1, },
                    } },
                    new QuestDefineEntity() {Uid=Guid.Parse("4EB9A231-9C57-4AE6-AA25-C2D586AA9EA4"), QuestionId="Q004",    CountScoreType = "2",    NeedAnswer = "Y",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="{\"Conditions\":[{\"QuestionId\": \"Q003\", \"AnswerCode\": [\"1\", \"2\", \"6\"]}]}",                                           AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                    new QuestDefineEntity() {Uid=Guid.Parse("43D9FD48-BB37-4A78-A400-BC79A35856C3"), QuestionId="Q005",    CountScoreType = "2",    NeedAnswer = "Y",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="",                                                                                                                                AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                    new QuestDefineEntity() {Uid=Guid.Parse("9FD91D96-F9C2-445E-B3CE-CD3EBF24478C"), QuestionId="Q006",    CountScoreType = "2",    NeedAnswer = "Y",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="",                                                                                                                                AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                    new QuestDefineEntity() {Uid=Guid.Parse("FC65A8F3-0685-4CBA-A591-EEEA961E00B8"), QuestionId="Q007",    CountScoreType = "2",    NeedAnswer = "Y",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="",                                                                                                                                AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                }
        };
        #endregion
    }
}