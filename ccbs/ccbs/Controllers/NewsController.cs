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
    public class NewsController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /News/

        public ViewResult Index()
        {
            return View(db.News.ToList());
        }

        public ActionResult _newsSidebar()
        {
            var allNews = db.News.Where(n => n.Hidden == false).OrderByDescending(n => n.UpdateDate).ToList();
            return View(allNews);
        }

        //
        // GET: /News/Details/5

        public ViewResult Details(int id)
        {
            News news = db.News.Find(id);
            return View(news);
        }

        //
        // GET: /News/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Create()
        {
            News n = new News
            {
                UpdateDate = DateTime.Now,
            };
            return View(n);
        }

        //
        // POST: /News/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Create(News news)
        {
            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = news.Id });
            }

            return View(news);
        }

        //
        // GET: /News/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Edit(int id)
        {
            News news = db.News.Find(id);
            return View(news);
        }

        //
        // POST: /News/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Edit(News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        //
        // GET: /News/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Delete(int id)
        {
            News news = db.News.Find(id);
            return View(news);
        }

        //
        // POST: /News/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}