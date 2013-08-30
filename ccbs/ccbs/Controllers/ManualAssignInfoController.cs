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
    public class ManualAssignInfoController : Controller
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /ManualAssignInfo/

        public ViewResult Index()
        {
            return View(db.ManualAssignInfoes.ToList());
        }

        //
        // GET: /ManualAssignInfo/Details/5

        public ViewResult Details(int id)
        {
            ManualAssignInfo manualassigninfo = db.ManualAssignInfoes.Find(id);
            return View(manualassigninfo);
        }

        //
        // GET: /ManualAssignInfo/Create

        public ActionResult Create(int studId)
        {
            return View();
        } 

        //
        // POST: /ManualAssignInfo/Create

        [HttpPost]
        public ActionResult Create(ManualAssignInfo manualassigninfo)
        {
            if (ModelState.IsValid)
            {
                db.ManualAssignInfoes.Add(manualassigninfo);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(manualassigninfo);
        }
        
        //
        // GET: /ManualAssignInfo/Edit/5
 
        public ActionResult Edit(int id)
        {
            ManualAssignInfo manualassigninfo = db.ManualAssignInfoes.Find(id);
            return View(manualassigninfo);
        }

        //
        // POST: /ManualAssignInfo/Edit/5

        [HttpPost]
        public ActionResult Edit(ManualAssignInfo manualassigninfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manualassigninfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(manualassigninfo);
        }

        //
        // GET: /ManualAssignInfo/Delete/5
 
        public ActionResult Delete(int id)
        {
            ManualAssignInfo manualassigninfo = db.ManualAssignInfoes.Find(id);
            return View(manualassigninfo);
        }

        //
        // POST: /ManualAssignInfo/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ManualAssignInfo manualassigninfo = db.ManualAssignInfoes.Find(id);
            db.ManualAssignInfoes.Remove(manualassigninfo);
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