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
    public class ContactController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /Contact/

        public ViewResult Index()
        {
            return View(db.Contacts.ToList());
        }

        //
        // GET: /Contact/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Contact/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Create(Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(contact);
        }
        
        //
        // GET: /Contact/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Edit(int id)
        {
            Contact contact = db.Contacts.Find(id);
            return View(contact);
        }

        //
        // POST: /Contact/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Edit(Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        //
        // GET: /Contact/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Delete(int id)
        {
            Contact contact = db.Contacts.Find(id);
            return View(contact);
        }

        //
        // POST: /Contact/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult DeleteConfirmed(int id)
        {            
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
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