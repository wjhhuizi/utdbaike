using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.Web.Security;

namespace ccbs.Controllers
{
    public class StudentController : Controller
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /Student/

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult TransferAllFromNewStudent()
        {
            int count = 0;
            count = transferNewStudentsToStudents();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            return View(db.Students.ToList());
        }

        [Authorize(Roles = LWSFRoles.student)]
        public ActionResult StudentHome()
        {
            var currNewStudent = GetCurrentStudent();
            if (currNewStudent == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CurrNewStudent = currNewStudent;

            return View(currNewStudent);
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SetupRoles()
        {
            var students = db.Students.ToList();

            foreach (var stud in students)
            {
                Roles.RemoveUserFromRole(stud.UserName, LWSFRoles.newStudent);
                Roles.AddUserToRole(stud.UserName, LWSFRoles.student);
            }
            return Content("Setup completed");
        }


        //
        // GET: /Student/Details/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Details(int id)
        {
            Student student = db.Students.Find(id);
            return View(student);
        }

        //
        // GET: /Student/Create
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        //
        // GET: /Student/Edit/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(int id)
        {
            Student student = db.Students.Find(id);
            return View(student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        //
        // GET: /Student/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Delete(int id)
        {
            Student student = db.Students.Find(id);
            return View(student);
        }

        //
        // POST: /Student/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        int transferNewStudentsToStudents()
        {
            int count = 0;
            var allNewStudents = db.NewStudents.ToList();

            foreach (var newstud in allNewStudents)
            {
                var stud = NewStudentOperation.NewStudentToStudent(newstud);
                stud.Term = "Fall";
                stud.Year = 2013;
                db.Students.Add(stud);
                count++;
            }
            db.SaveChanges();

            return count;
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ManageDisclaimer()
        {
            var disclaimer = db.Disclaimers.FirstOrDefault();
            if (disclaimer == null)
            {
                return RedirectToAction("CreateDisclaimer");
            }
            return RedirectToAction("EditDisclaimer", new { id = disclaimer.Id });
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult CreateDisclaimer()
        {
            var disclaimer = new Disclaimer
            {
                LastUpdate = DateTime.Now,
            };
            return View(disclaimer);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult CreateDisclaimer(Disclaimer disclaimer)
        {
            if (ModelState.IsValid)
            {
                db.Disclaimers.Add(disclaimer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            disclaimer.Description = Server.HtmlDecode(disclaimer.Description);
            return View(disclaimer);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult EditDisclaimer(int id)
        {
            var disclaimer = db.Disclaimers.Find(id);
            disclaimer.Description = Server.HtmlDecode(disclaimer.Description);
            return View(disclaimer);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult EditDisclaimer(Disclaimer disclaimer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(disclaimer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            disclaimer.Description = Server.HtmlDecode(disclaimer.Description);
            return View(disclaimer);
        }

        private Student GetCurrentStudent()
        {
            string userName = User.Identity.Name;
            var currStudent = db.Students.Where(v => v.UserName == userName).FirstOrDefault();
            return currStudent;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}