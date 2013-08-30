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
    public class OrgRequestController : Controller
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /OrgRequest/
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            return View(db.OrgRequests.ToList());
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ViewResult OrgViewRequests()
        {
            Volunteer currVol = GetCurrentVolunteer();
            Organization org = currVol.Organization;
            var requests = org.OrgRequests.ToList();
            return View(requests);
        }

        //
        // GET: /OrgRequest/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ViewResult Details(int id)
        {
            OrgRequest orgrequest = db.OrgRequests.Find(id);
            return View(orgrequest);
        }

        //
        // GET: /OrgRequest/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /OrgRequest/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Create(OrgRequest orgrequest, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                orgrequest.RequestDate = DateTime.Now;
                orgrequest.Progress = "Processing";
                Volunteer currVol = GetCurrentVolunteer();
                Organization org = currVol.Organization;
                org.OrgRequests.Add(orgrequest);
                orgrequest.Organization = org;
                db.OrgRequests.Add(orgrequest);
                db.SaveChanges();
                return Redirect(returnUrl);
            }

            return View(orgrequest);
        }

        //
        // GET: /OrgRequest/Edit/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(int id)
        {
            OrgRequest orgrequest = db.OrgRequests.Find(id);
            return View(orgrequest);
        }

        //
        // POST: /OrgRequest/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(OrgRequest orgrequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orgrequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orgrequest);
        }

        //
        // GET: /OrgRequest/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Delete(int id)
        {
            OrgRequest orgrequest = db.OrgRequests.Find(id);
            return View(orgrequest);
        }

        //
        // POST: /OrgRequest/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            OrgRequest orgrequest = db.OrgRequests.Find(id);
            db.OrgRequests.Remove(orgrequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private Volunteer GetCurrentVolunteer()
        {
            string VouluteerEmail = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.Email == VouluteerEmail).SingleOrDefault();
            return currVolunteer;
        }
    }
}