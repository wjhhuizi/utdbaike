using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ccbs.Controllers
{
    public class BBCategoryController : Controller
    {
        private BangbangDBContainer db = new BangbangDBContainer();

        private const string icon_directory = "~/Photos/Bangbang";
        private const string icon_prefix = "icon_";
        private const int icon_width = 30;
        private const int icon_height = 30;


        //
        // GET: /BBCategory/

        public ViewResult Index()
        {
            return View(db.BBCategories.ToList());
        }

        //
        // GET: /BBCategory/Details/5

        public ViewResult Details(int id)
        {
            BBCategory bbcategory = db.BBCategories.Find(id);
            return View(bbcategory);
        }

        //
        // GET: /BBCategory/Create

        public ActionResult Create()
        {
            var category = new BBCategory
            {
                DateCreated = DateTime.Now,
                Icon = "",
            };
            return View(category);
        }

        //
        // POST: /BBCategory/Create

        [HttpPost]
        public ActionResult Create(BBCategory bbcategory, IEnumerable<HttpPostedFileBase> iconFiles)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                if ((iconFiles == null) || (iconFiles.Count() == 0))
                {
                    ModelState.AddModelError("", "请检查您是否忘记上传图片了");
                    return View(bbcategory);
                }
                foreach (var file in iconFiles)
                {
                    if (!HasFile(file))
                    {
                        ModelState.AddModelError("", "请检查您是否忘记上传图片了？");
                        return View(bbcategory);
                    }
                    // Some browsers send file names with full path. This needs to be stripped.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath(icon_directory), fileName);
                    while (System.IO.File.Exists(physicalPath))
                    {
                        fileName = count.ToString() + fileName;
                        physicalPath = Path.Combine(Server.MapPath(icon_directory), fileName);
                    }
                    file.SaveAs(physicalPath);

                    string icon_filename = icon_prefix + fileName;
                    string icon_path = Path.Combine(Server.MapPath(icon_directory), icon_filename);
                    ScaleImage(physicalPath, icon_path, icon_width, icon_height);

                    System.IO.File.Delete(physicalPath);
                    bbcategory.Icon = Url.Content(icon_directory + "/" + icon_filename);
                }
                db.BBCategories.Add(bbcategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bbcategory);
        }

        //
        // GET: /BBCategory/Edit/5

        public ActionResult Edit(int id)
        {
            BBCategory bbcategory = db.BBCategories.Find(id);
            return View(bbcategory);
        }

        //
        // POST: /BBCategory/Edit/5

        [HttpPost]
        public ActionResult Edit(BBCategory bbcategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bbcategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bbcategory);
        }

        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        public ActionResult UploadIcon(IEnumerable<HttpPostedFileBase> iconFiles, int categoryId)
        {
            var category = db.BBCategories.Find(categoryId);

            // The Name of the Upload component is "lyricsFiles" 
            foreach (var file in iconFiles)
            {
                if (!HasFile(file))
                {
                    continue;
                }
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/Photos/Bangbang"), fileName);
                while (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
                file.SaveAs(physicalPath);
                category.Icon = ToVirtualPath(fileName);
                db.SaveChanges();
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteIcon(string[] iconFiles, int categoryId)
        {
            var category = db.BBCategories.Find(categoryId);
            // The parameter of the Remove action must be called "fileNames"
            foreach (var fullName in iconFiles)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(Server.MapPath("~/Photos/Bangbang"), fileName);

                //    // TODO: Verify user permissions
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                    category.Icon = "";
                    db.SaveChanges();
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        //
        // GET: /BBCategory/Delete/5

        public ActionResult Delete(int id)
        {
            BBCategory bbcategory = db.BBCategories.Find(id);
            return View(bbcategory);
        }

        //
        // POST: /BBCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            BBCategory bbcategory = db.BBCategories.Find(id);
            db.BBCategories.Remove(bbcategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private string ToVirtualPath(string filename)
        {
            string path;
            if (String.IsNullOrEmpty(filename))
            {
                return "";
            }
            if (filename.Contains("/Photos/Bangbang/"))
            {//already the standard format
                return filename;
            }
            path = "../../Photos/Bangbang/" + filename;
            return path;
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