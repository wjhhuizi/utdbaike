using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ccbs.Controllers
{
    public class ActivityController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        private const string flyer_pic_path = "~/Photos/flyers";
        private const string flyer_pic_scaled_prefix = "scaled_";
        private const int flyer_pic_width = 730;
        private const int flyer_pic_height = 400;

        //
        // GET: /Activity/

        public ViewResult Index()
        {
            var today = DateTime.Now;
            var futureActivities = db.Activities.Where(a => a.TimeFrom >= today).OrderBy(a => a.TimeFrom).ToList();
            return View(futureActivities);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ViewResult Manage()
        {
            var today = DateTime.Now;
            var futureActivities = db.Activities.Where(a => a.TimeFrom >= today).OrderBy(a => a.TimeFrom).ToList();
            var pastActivities = db.Activities.Where(a => a.TimeFrom < today).OrderByDescending(a => a.TimeFrom).ToList();
            ViewBag.futureActivities = futureActivities;
            ViewBag.pastActivities = pastActivities;
            return View();
        }

        public ActionResult ComingEvents()
        {
            var al = db.Activities.OrderBy(a => a.TimeFrom).ToList();
            var today = DateTime.Now;
            var thisWeek = today.AddDays(7);
            var comingEvents = al.Where(a => (a.TimeFrom > today) && (a.TimeFrom < thisWeek)).ToList();
            return View("_ListEvents", comingEvents);
        }

        //
        // GET: /Activity/Details/5
        public ViewResult Details(int id)
        {
            Activity activity = db.Activities.Find(id);
            return View(activity);
        }

        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        //
        // GET: /Activity/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Activity/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Create(Activity activity, IEnumerable<HttpPostedFileBase> flyerPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                if ((flyerPhotos != null) && (flyerPhotos.Count() > 0))
                {
                    foreach (var file in flyerPhotos)
                    {
                        if (!HasFile(file))
                        {
                            continue;
                        }
                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName = Path.GetFileName(file.FileName);
                        var physicalPath = Path.Combine(Server.MapPath(flyer_pic_path), fileName);
                        while (System.IO.File.Exists(physicalPath))
                        {
                            fileName = count.ToString() + fileName;
                            physicalPath = Path.Combine(Server.MapPath(flyer_pic_path), fileName);
                        }
                        file.SaveAs(physicalPath);

                        string scaled_fileName = flyer_pic_scaled_prefix + fileName;
                        string scaled_physicalPath = Path.Combine(Server.MapPath(flyer_pic_path), scaled_fileName);
                        ScaleImage(physicalPath, scaled_physicalPath, flyer_pic_width, flyer_pic_height);
                        System.IO.File.Delete(physicalPath);
                        activity.Picture = Url.Content(flyer_pic_path + "/" + scaled_fileName);
                    }
                }

                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Manage");
            }

            return View(activity);
        }

        //
        // GET: /Activity/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Edit(int id)
        {
            Activity activity = db.Activities.Find(id);
            return View(activity);
        }

        //
        // POST: /Activity/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Edit(Activity activity, IEnumerable<HttpPostedFileBase> flyerPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                if ((flyerPhotos != null) && (flyerPhotos.Count() > 0))
                {
                    foreach (var file in flyerPhotos)
                    {
                        if (!HasFile(file))
                        {
                            continue;
                        }
                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName = Path.GetFileName(file.FileName);
                        var physicalPath = Path.Combine(Server.MapPath(flyer_pic_path), fileName);
                        while (System.IO.File.Exists(physicalPath))
                        {
                            fileName = count.ToString() + fileName;
                            physicalPath = Path.Combine(Server.MapPath(flyer_pic_path), fileName);
                        }
                        file.SaveAs(physicalPath);

                        string scaled_fileName = flyer_pic_scaled_prefix + fileName;
                        string scaled_physicalPath = Path.Combine(Server.MapPath(flyer_pic_path), scaled_fileName);
                        ScaleImage(physicalPath, scaled_physicalPath, flyer_pic_width, flyer_pic_height);

                        System.IO.File.Delete(physicalPath);

                        if (!String.IsNullOrEmpty(activity.Picture))
                        {//delete the previous one
                            System.IO.File.Delete(Server.MapPath(activity.Picture));
                        }

                        activity.Picture = Url.Content(flyer_pic_path + "/" + scaled_fileName);
                    }
                }
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(activity);
        }

        //
        // GET: /Activity/Delete/5

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult Delete(int id)
        {
            Activity activity = db.Activities.Find(id);
            return View(activity);
        }

        //
        // POST: /Activity/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.coworker)]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            db.Activities.Remove(activity);
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        private void ScaleImage(string src, string des, int maxWidth, int maxHeight)
        {
            var image = Image.FromFile(src);

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            newImage.Save(des, ImageFormat.Jpeg);
            image.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}