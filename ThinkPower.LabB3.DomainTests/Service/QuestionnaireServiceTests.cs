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
        private QuestionnaireAnswerEntity QuestionAnswerEntity()
        {
            return new QuestionnaireAnswerEntity()
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
        }

        private QuestionnaireEntity QuestionEntity()
        {
            return new QuestionnaireEntity()
            {
                ScoreKind = "1",
                QuestScore = 35,
                NeedScore = "Y",
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
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsMax_ScoreIs19_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();

            int expected = 19;

            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsMax_ScoreIs0_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            question.NeedScore = "N";

            int expected = 0;


            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsMax_ScoreGreaterThan35_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();

            foreach (var questDefine in question.QuestDefineEntities)
            {
                foreach (var answerDefine in questDefine.AnswerDefineEntities)
                {
                    answerDefine.Score = 9;
                }
            }

            int expected = 35;


            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsAvg_ScoreIs19_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();

            foreach (var questDefine in question.QuestDefineEntities)
            {
                questDefine.CountScoreType = "4";
            }

            int expected = 19;

            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsSum_ScoreIs21_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();

            foreach (var questDefine in question.QuestDefineEntities)
            {
                questDefine.CountScoreType = "1";
            }

            int expected = 21;


            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsMin_ScoreIs18_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();

            foreach (var questDefine in question.QuestDefineEntities)
            {
                questDefine.CountScoreType = "3";
            }

            int expected = 18;


            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsMax_ScoreIs20_ChangeCheckItem_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q003").AnswerCode = "4";

            int expected = 20;

            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_CountScoreTypeIsMax_ScoreIs15_ChangeRadioButtonSelect_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q001").AnswerCode = "1";

            int expected = 15;

            //Actual
            CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);

            int actual = result.ActualScore;

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CalculateScoreTest_When_ChangeRadioButtonSelect_CauseAnException_Then_Fail()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q001").AnswerCode = "9";

            string expected = "9";

            //Actual
            string actual = null;

            try
            {
                CalculateScoreEntity result = new QuestionnaireService().CalculateScore(answer, question);
            }
            catch (InvalidOperationException e)
            {
                actual = e.Data["answerDetail.AnswerCode"] as string;
            }

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_AllRule_Then_Success()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            Dictionary<string, string> expected = new Dictionary<string, string>();

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_NeedAnswer_Then_Fail()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q002").AnswerCode = "";

            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q002", "此題必須填答!"},
            };


            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_NeedAnswer_AllowNaCondition_Then_Fail()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q001").AnswerCode = "3";
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q002").AnswerCode = "3";
            answer.AnswerDetailEntities = answer.AnswerDetailEntities.Where(x => x.QuestionId != "Q003");

            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題必須填答!"},
            };


            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_MinMultipleAnswers_Then_Fail()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            question.QuestDefineEntities.First(x => x.QuestionId == "Q003").MinMultipleAnswers = 3;

            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題至少須勾選3個項目!"},
            };

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_MaxMultipleAnswers_Then_Fail()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            question.QuestDefineEntities.First(x => x.QuestionId == "Q003").MaxMultipleAnswers = 1;

            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題至多僅能勾選1個項目!"},
            };


            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_SingleAnswerCondition_Then_Fail()
        {
            //Arrange
            var answer = QuestionAnswerEntity();
            var question = QuestionEntity();
            answer.AnswerDetailEntities.First(x => x.QuestionId == "Q003").AnswerCode = "6";

            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q003", "此題僅能勾選1個項目!"},
            };

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ValidateRule_When_OtherAnswer_Then_Fail()
        {
            //Arrange
            var answer = new QuestionnaireAnswerEntity()
            {
                AnswerDetailEntities = new List<AnswerDetailEntity>()
                {
                    new AnswerDetailEntity() {QuestionId="Q1", AnswerCode = "A", OtherAnswer = "", Score = null, },
                    new AnswerDetailEntity() {QuestionId="Q2", AnswerCode = "A", OtherAnswer = "", Score = null, },
                    new AnswerDetailEntity() {QuestionId="Q3", AnswerCode = "A", OtherAnswer = "", Score = null, },
                    new AnswerDetailEntity() {QuestionId="Q3", AnswerCode = "E", OtherAnswer = "", Score = null, },
                    new AnswerDetailEntity() {QuestionId="Q4", AnswerCode = "A", OtherAnswer = "", Score = null, },
                    new AnswerDetailEntity() {QuestionId="Q5", AnswerCode = "A", OtherAnswer = "", Score = null, },
                },
            };

            var question = new QuestionnaireEntity()
            {
                ScoreKind = "",
                QuestScore = null,
                NeedScore = "N",
                QuestDefineEntities = new List<QuestDefineEntity>()
                {
                    new QuestDefineEntity(){Uid=Guid.Empty,   QuestUid=Guid.Empty, QuestionId="Q1", QuestionContent="請輸入您的手機號碼(格式為0912345678，將作為活動通知使用)",NeedAnswer="Y",  AllowNaCondition="",                                                                       AnswerType="F", MinMultipleAnswers=null, MaxMultipleAnswers=null, SingleAnswerCondition="",   CountScoreType="",   Memo="",   OrderSn=10,   CreateUserId="",   CreateTime = null,     ModifyUserId="",    ModifyTime = null, AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode=" ", AnswerContent="旅遊行程(例如：來回機票、住宿券)", Memo="", HaveOtherAnswer="Y", NeedOtherAnswer="Y", Score=null, OrderSn=10,},
                    }   },
                    new QuestDefineEntity(){Uid=Guid.Empty,   QuestUid=Guid.Empty, QuestionId="Q2", QuestionContent="下列哪種活動贈品最吸引您參加活動?",                     NeedAnswer="Y",  AllowNaCondition="",                                                                       AnswerType="M", MinMultipleAnswers=1,    MaxMultipleAnswers=null, SingleAnswerCondition="",   CountScoreType="",   Memo="",   OrderSn=20,   CreateUserId="",   CreateTime = null,     ModifyUserId="",    ModifyTime = null, AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="A", AnswerContent="3C產品(例如：手機、平板電腦、相機)", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=10,},
                        new AnswerDefineEntity(){AnswerCode="B", AnswerContent="禮券(例如：知名百貨、量販店、便利商店禮券)", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=20,},
                        new AnswerDefineEntity(){AnswerCode="C", AnswerContent="簡訊", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=30,}
                    }   },
                    new QuestDefineEntity(){Uid=Guid.Empty,   QuestUid=Guid.Empty, QuestionId="Q3", QuestionContent="您最喜歡透過下列何種方式收到行銷優惠資訊?",              NeedAnswer="Y",  AllowNaCondition="",                                                                       AnswerType="M", MinMultipleAnswers=1,    MaxMultipleAnswers=null, SingleAnswerCondition="",   CountScoreType="",   Memo="",   OrderSn=30,   CreateUserId="",   CreateTime = null,     ModifyUserId="",    ModifyTime = null, AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="A", AnswerContent="電子DM", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=10,}                               ,
                        new AnswerDefineEntity(){AnswerCode="B", AnswerContent="實體DM", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=20,}                               ,
                        new AnswerDefineEntity(){AnswerCode="C", AnswerContent="即時通訊軟體(例如：LINE、what’s app)", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=30,}    ,
                        new AnswerDefineEntity(){AnswerCode="D", AnswerContent="其他", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=40,}                                 ,
                        new AnswerDefineEntity(){AnswerCode="E", AnswerContent="臨櫃", Memo="", HaveOtherAnswer="Y", NeedOtherAnswer="Y", Score=null, OrderSn=50,}                                 ,
                    }  },
                    new QuestDefineEntity(){Uid=Guid.Empty,   QuestUid=Guid.Empty, QuestionId="Q4", QuestionContent="下列哪一項通路服務是您最常使用的?(一個月至少使用3次以上)", NeedAnswer="Y",  AllowNaCondition="",                                                                       AnswerType="S", MinMultipleAnswers=null, MaxMultipleAnswers=null, SingleAnswerCondition="",   CountScoreType="",   Memo="",   OrderSn=40,   CreateUserId="",   CreateTime = null,     ModifyUserId="",    ModifyTime = null, AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="A", AnswerContent="ATM", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=10,}       ,
                        new AnswerDefineEntity(){AnswerCode="B", AnswerContent="網站", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=20,}      ,
                        new AnswerDefineEntity(){AnswerCode="C", AnswerContent="行動APP", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=30,}   ,
                        new AnswerDefineEntity(){AnswerCode="D", AnswerContent="都沒有", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=40,}     ,
                        new AnswerDefineEntity(){AnswerCode="E", AnswerContent="1分", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null,   OrderSn=50,}     ,
                    }  },
                    new QuestDefineEntity(){Uid=Guid.Empty,   QuestUid=Guid.Empty, QuestionId="Q5", QuestionContent="您對我們的服務滿意度1至5分是?",                        NeedAnswer="Y",  AllowNaCondition="{\"Conditions\":[{\"QuestionId\": \"Q4\", \"AnswerCode\": [\"E\"]}]}",   AnswerType="S", MinMultipleAnswers=null, MaxMultipleAnswers=null, SingleAnswerCondition="",   CountScoreType="",   Memo="",   OrderSn=50,   CreateUserId="",   CreateTime = null,     ModifyUserId="",    ModifyTime = null, AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="A", AnswerContent="2分", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=10,},
                        new AnswerDefineEntity(){AnswerCode="B", AnswerContent="3分", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=20,},
                        new AnswerDefineEntity(){AnswerCode="C", AnswerContent="4分", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=30,},
                        new AnswerDefineEntity(){AnswerCode="D", AnswerContent="5分", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=40,},
                        new AnswerDefineEntity(){AnswerCode="E", AnswerContent="", Memo="", HaveOtherAnswer="N", NeedOtherAnswer="N", Score=null, OrderSn=50,}   ,
                    } },


                    new QuestDefineEntity() {Uid=Guid.Parse("F9DD6938-2DBD-48AF-9D44-428DCB2AC5D6"), QuestionId="Q001",    CountScoreType = "2",    NeedAnswer = "N",    AnswerType = "S",     MinMultipleAnswers=null,     MaxMultipleAnswers=null,    SingleAnswerCondition="",                                                                           AllowNaCondition="",                                                                                                                                AnswerDefineEntities = new List<AnswerDefineEntity>(){
                        new AnswerDefineEntity(){AnswerCode="1", Score = 1, },
                        new AnswerDefineEntity(){AnswerCode="2", Score = 2, },
                        new AnswerDefineEntity(){AnswerCode="3", Score = 3, },
                        new AnswerDefineEntity(){AnswerCode="4", Score = 4, },
                        new AnswerDefineEntity(){AnswerCode="5", Score = 5, },
                    } },
                }
            };


            Dictionary<string, string> expected = new Dictionary<string, string>() {
                { "Q3", "請輸入其他說明文字!"},
            };

            //Actual
            Dictionary<string, string> actual = new QuestionnaireService().ValidateRule(answer, question);


            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }

    }
}