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
    public class MeetingController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /Meeting/
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ViewResult Index()
        {
            var meetings = db.Meetings.OrderByDescending(m => m.Date).ToList();
            return View(meetings);
        }

        //
        // GET: /Meeting/Details/5
        public ViewResult Details(int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            return View(meeting);
        }

        //
        // GET: /Meeting/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Meeting/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ActionResult Create(Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                if (meeting.Date == null)
                {
                    return RedirectToAction("Index");
                }
                db.Meetings.Add(meeting);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(meeting);
        }
        
        //
        // GET: /Meeting/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ActionResult Edit(int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            return View(meeting);
        }

        //
        // POST: /Meeting/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ActionResult Edit(Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(meeting);
        }

        //
        // GET: /Meeting/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ActionResult Delete(int id)
        {
            Meeting meeting = db.Meetings.Find(id);
            return View(meeting);
        }

        //
        // POST: /Meeting/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
        public ActionResult DeleteConfirmed(int id)
        {            
            Meeting meeting = db.Meetings.Find(id);
            db.Meetings.Remove(meeting);
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