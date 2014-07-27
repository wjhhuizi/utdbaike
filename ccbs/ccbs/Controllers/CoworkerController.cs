using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.IO;

namespace ccbs.Controllers
{
    public class CoworkerController : Controller
    {
        private StudentModelContainer db = new StudentModelContainer();
        private WebModelContainer web_db = new WebModelContainer();

        //
        // GET: /Coworker/

        public ViewResult Index()
        {
            return View(web_db.Coworkers.ToList());
        }


        public ViewResult Profile(int id)
        {
            Coworker coworker = web_db.Coworkers.Find(id);
            return View(coworker);
        }

        //
        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        private string ParsePhotoPath(string filename)
        {
            string path;
            if (String.IsNullOrEmpty(filename))
            {
                return "";
            }
            if (filename.Contains("/Photos/"))
            {//already the standard format
                return filename;
            }
            path = "../../Photos/" + filename;
            return path;
        }

        public ActionResult UploadSelfPhoto(IEnumerable<HttpPostedFileBase> SelfPhotos, int coworkerId)
        {
            var coworker = web_db.Coworkers.Find(coworkerId);
            int count = 0;

            // The Name of the Upload component is "lyricsFiles" 
            foreach (var file in SelfPhotos)
            {
                if (!HasFile(file))
                {
                    continue;
                }
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/Photos"), fileName);
                while (System.IO.File.Exists(physicalPath))
                {
                    fileName = (count++).ToString() + "_" + fileName;
                    physicalPath = Path.Combine(Server.MapPath("~/Photos"), fileName);
                }
                file.SaveAs(physicalPath);
                coworker.SelfPhoto = ParsePhotoPath(fileName);
                web_db.SaveChanges();
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult UploadBaptisePhoto(IEnumerable<HttpPostedFileBase> BaptisePhotos, int coworkerId)
        {
            var coworker = web_db.Coworkers.Find(coworkerId);
            int count = 0;

            // The Name of the Upload component is "lyricsFiles" 
            foreach (var file in BaptisePhotos)
            {
                if (!HasFile(file))
                {
                    continue;
                }
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/Photos"), fileName);
                while (System.IO.File.Exists(physicalPath))
                {
                    fileName = (count++).ToString() + "_" + fileName;
                    physicalPath = Path.Combine(Server.MapPath("~/Photos"), fileName);
                }
                file.SaveAs(physicalPath);
                coworker.BaptisePhoto = ParsePhotoPath(fileName);
                web_db.SaveChanges();
            }
            // Return an empty string to signify success
            return Content("");
        }
        //
        // GET: /Coworker/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ViewResult Details(int id)
        {
            Coworker coworker = web_db.Coworkers.Find(id);
            return View(coworker);
        }

        //
        // GET: /Coworker/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Create()
        {
            var username = User.Identity.Name;
            var coworker = new Coworker();
            if (!User.IsInRole(LWSFRoles.admin))
            {
                coworker.Username = username;
                var vol = db.Volunteers.Where(v => v.Email == username);
                if (vol == null || vol.Count() == 0)
                {
                    var stud = db.NewStudents.Where(s => s.Email == username);
                    if (stud == null || stud.Count() == 0)
                    {

                    }
                    else
                    {
                        coworker.Name = stud.SingleOrDefault().Name;
                        coworker.Gender = (stud.SingleOrDefault().Gender == SystemGender.singleMale) ? 1 : 0;
                        coworker.Email = stud.SingleOrDefault().Email;
                        coworker.Phone = stud.SingleOrDefault().Phone;
                    }
                }
                else
                {
                    coworker.Name = vol.SingleOrDefault().Name;
                    coworker.Gender = (vol.SingleOrDefault().Gender == SystemGender.singleMale) ? 1 : 0;
                    coworker.Email = vol.SingleOrDefault().Email;
                    coworker.Phone = vol.SingleOrDefault().Phone;
                }
            }
            return View(coworker);
        }

        //
        // POST: /Coworker/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Create(Coworker coworker)
        {
            if (ModelState.IsValid)
            {
                web_db.Coworkers.Add(coworker);
                web_db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(coworker);
        }

        //
        // GET: /Coworker/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Edit(int id)
        {
            Coworker coworker = web_db.Coworkers.Find(id);
            return View(coworker);
        }

        //
        // POST: /Coworker/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Edit(Coworker coworker)
        {
            if (ModelState.IsValid)
            {
                web_db.Entry(coworker).State = EntityState.Modified;
                web_db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(coworker);
        }

        //
        // GET: /Coworker/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Delete(int id)
        {
            Coworker coworker = web_db.Coworkers.Find(id);
            return View(coworker);
        }

        //
        // POST: /Coworker/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            Coworker coworker = web_db.Coworkers.Find(id);
            web_db.Coworkers.Remove(coworker);
            web_db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}