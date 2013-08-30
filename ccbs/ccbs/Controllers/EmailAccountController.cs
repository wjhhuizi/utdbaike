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
    public class EmailAccountController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /EmailAccount/

        public ViewResult Index()
        {
            return View(db.EmailAccounts.ToList());
        }

        //
        // GET: /EmailAccount/Details/5

        public ViewResult Details(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            return View(emailaccount);
        }

        //
        // GET: /EmailAccount/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /EmailAccount/Create

        [HttpPost]
        public ActionResult Create(EmailAccount emailaccount)
        {
            if (ModelState.IsValid)
            {
                db.EmailAccounts.Add(emailaccount);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(emailaccount);
        }
        
        //
        // GET: /EmailAccount/Edit/5
 
        public ActionResult Edit(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            return View(emailaccount);
        }

        //
        // POST: /EmailAccount/Edit/5

        [HttpPost]
        public ActionResult Edit(EmailAccount emailaccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emailaccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(emailaccount);
        }

        //
        // GET: /EmailAccount/Delete/5
 
        public ActionResult Delete(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            return View(emailaccount);
        }

        //
        // POST: /EmailAccount/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            db.EmailAccounts.Remove(emailaccount);
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