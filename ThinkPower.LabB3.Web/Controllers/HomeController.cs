using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThinkPower.LabB3.Domain.Entity.Question;

namespace ThinkPower.LabB3.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult EvaQuest()
        {
            return View();
        }

        //public ActionResult QuestionnaireEntity()
        //{
        //    return View(new QuestionnaireEntity());
        //}

        //public ActionResult QuestDefineEntity()
        //{
        //    return View(new QuestDefineEntity());
        //}

        //public ActionResult AnswerDefineEntity()
        //{
        //    return View(new AnswerDefineEntity());
        //}
    }
}