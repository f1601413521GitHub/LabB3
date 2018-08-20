using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkPower.LabB3.Domain.Entity.Question;
using ThinkPower.LabB3.Domain.Service.Interface;

namespace ThinkPower.LabB3.Domain.Service
{
    /// <summary>
    /// 問卷服務
    /// </summary>
    public class QuestionnaireService : IQuestionnaire
    {
        public QuestionnaireResultEntity Calculate(QuestionnaireAnswerEntity answer)
        {
            throw new NotImplementedException();
        }

        public QuestionnaireEntity GetActiveQuestionaire(string id)
        {
            throw new NotImplementedException();
        }

        public QuestionnaireEntity GetQuestionnaire(string uid)
        {
            throw new NotImplementedException();
        }
    }
}
