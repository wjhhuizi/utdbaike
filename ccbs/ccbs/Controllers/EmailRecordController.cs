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
    public class EmailRecordController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /EmailRecord/
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            return View(db.EmailRecords.ToList());
        }

        //
        // GET: /EmailRecord/Details/5

        public ViewResult Details(int id)
        {
            EmailRecord emailrecord = db.EmailRecords.Find(id);
            emailrecord.Body = Server.HtmlDecode(emailrecord.Body);
            ViewBag.BodyText = emailrecord.Body;
            return View(emailrecord);
        }

        //
        // GET: /EmailRecord/Create
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create()
        {
            EmailRecord email = new EmailRecord();
            email.LastUpdate = DateTime.Now;
            return View(email);
        }

        //
        // POST: /EmailRecord/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        [ValidateInput(false)]
        public ActionResult Create(EmailRecord emailrecord)
        {
            if (ModelState.IsValid)
            {
                emailrecord.LastUpdate = DateTime.Now;
                emailrecord.Body = Server.HtmlEncode(emailrecord.Body);
                db.EmailRecords.Add(emailrecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(emailrecord);
        }

        //
        // GET: /EmailRecord/Edit/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(int id)
        {
            EmailRecord emailrecord = db.EmailRecords.Find(id);
            emailrecord.Body = Server.HtmlDecode(emailrecord.Body);
            return View(emailrecord);
        }

        //
        // POST: /EmailRecord/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        [ValidateInput(false)]
        public ActionResult Edit(EmailRecord emailrecord)
        {
            if (ModelState.IsValid)
            {
                emailrecord.Body = Server.HtmlEncode(emailrecord.Body);
                db.Entry(emailrecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(emailrecord);
        }

        //
        // GET: /EmailRecord/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Delete(int id)
        {
            EmailRecord emailrecord = db.EmailRecords.Find(id);
            emailrecord.Body = Server.HtmlDecode(emailrecord.Body);
            return View(emailrecord);
        }

        //
        // POST: /EmailRecord/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            EmailRecord emailrecord = db.EmailRecords.Find(id);
            db.EmailRecords.Remove(emailrecord);
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