using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkPower.LabB3.Domain.Entity.Question
{
    /// <summary>
    /// 問卷類別
    /// </summary>
    public class QuestionnaireEntity : BaseEntity
    {
        public System.Guid Uid { get; set; }
        public string QuestId { get; set; }
        public string Version { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public Nullable<System.DateTime> Ondate { get; set; }
        public Nullable<System.DateTime> Offdate { get; set; }
        public string NeedScore { get; set; }
        public Nullable<int> QuestScore { get; set; }
        public string ScoreKind { get; set; }
        public string HeadBackgroundImg { get; set; }
        public string HeadDescription { get; set; }
        public string FooterDescription { get; set; }
        public string CreateUserId { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string ModifyUserId { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
