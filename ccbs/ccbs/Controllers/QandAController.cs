using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;

namespace ccbs.Controllers
{
    public class QandAController : BaseController
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /QandA/

        public ViewResult AllQuestions()
        {
            var questions = db.Questions.Include(q => q.Answers).OrderByDescending(q => q.LastUpdate).ToList();
            return View(questions);
        }

        public ViewResult _ShowQuestions(string username)
        {
            List<Question> questions;
            List<Question> top15Questions = new List<Question>();
            int count;
            int i;

            if (!String.IsNullOrEmpty(username))
            {
                questions = db.Questions.Where(q => q.UserName == username).OrderByDescending(q => q.LastUpdate).ToList();
            }
            else
            {
                questions = db.Questions.Include(q => q.Answers).OrderByDescending(q => q.LastUpdate).ToList();
            }
            if (questions == null)
            {
                count = 0;
            }
            else if (questions.Count < 15)
            {
                count = questions.Count;
            }
            else
            {
                count = 15;
            }

            i = 0;
            foreach (var question in questions)
            {
                top15Questions.Add(question);
                i++;
                if (i >= count)
                {
                    break;
                }
            }

            ViewBag.username = username;
            return View(top15Questions);
        }

        //
        // GET: /QandA/Details/5

        public JsonResult QuestionDetails(int id)
        {
            Question question = db.Questions.Find(id);
            question.VisitCount++;
            db.SaveChanges();
            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("QuestionDetails", question),
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _QuestionDetails(int id)
        {
            Question question = db.Questions.Find(id);

            return View("QuestionDetails", question);
        }

        public string UpdateQuestionLikeCount(int questionId)
        {
            var question = db.Questions.Find(questionId);
            question.LikeCount++;
            db.SaveChanges();
            return question.LikeCount.ToString();
        }

        public string UpdateAnswerLikeCount(int answerId)
        {
            var answer = db.Answers.Find(answerId);
            answer.LikeCount++;
            db.SaveChanges();
            return answer.LikeCount.ToString();
        }

        [Authorize]
        public ActionResult NewStudentAskQuestion()
        {
            return View("CreateQuestion");
        }

        //
        // POST: /QandA/Create

        [HttpPost]
        [Authorize]
        public JsonResult CreateQuestion()
        {
            Question question = new Question
            {
                UserName = User.Identity.Name,
                LikeCount = 0,
                LastUpdate = DateTime.Now,
            };

            question.Title = Request["Title"];
            question.MainContent = Request["MainContent"];

            if (!String.IsNullOrEmpty(question.Title))
            {
                db.Questions.Add(question);
                db.SaveChanges();

                return Json(new
                {
                    Success = true,
                    PartialViewHtml = RenderPartialViewToString("CreateQuestion"),
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateAnswer(int questionId)
        {
            Answer answer = new Answer
            {
                LikeCount = 0,
                LastUpdate = DateTime.Now,
                QuestionId = questionId,
            };
            if (Request.IsAuthenticated)
            {
                answer.UserName = User.Identity.Name;
            }
            else
            {
                answer.UserName = "Guest";
            }
            answer.MainContent = Request["MainContent"];

            if (!String.IsNullOrEmpty(answer.MainContent))
            {
                db.Answers.Add(answer);
                db.SaveChanges();
            }
            return QuestionDetails(questionId);
        }

        //
        // POST: /QandA/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeQandA)]
        public string DeleteQuestion(int id)
        {
            Question question = db.Questions.Find(id);
            foreach (var answer in question.Answers.ToList())
            {
                db.Answers.Remove(answer);
            }
            db.Questions.Remove(question);
            db.SaveChanges();
            return "";
        }

        //
        // POST: /QandA/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeQandA)]
        public string DeleteAnswer(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
            db.SaveChanges();
            return "";
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}