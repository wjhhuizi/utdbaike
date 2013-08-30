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
using System.Web.UI;

namespace ccbs.Controllers
{
    public class HomeGalleryController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        private const int full_width = 1080;
        private const int full_height = 270;
        private const int thumbnail_width = 40;
        private const int thumbnail_height = 10;
        private const string thumbnail_prefix = "thumbnail_";
        private const string full_prefix = "full_";
        private const string gallery_path = "~/Photos/Gallery";

        //
        // GET: /HomeGallery/

        public ViewResult Index()
        {
            var gallery = db.HomeGalleries.OrderBy(g => g.Order).ToList();
            return View(gallery);
        }

        public ActionResult _ShowHomeGallery()
        {
            var gallery = db.HomeGalleries.OrderBy(g => g.Order).ToList();
            return View(gallery);
        }

        //
        //// GET: /HomeGallery/Details/5

        //public ViewResult Details(int id)
        //{
        //    HomeGallery homegallery = db.HomeGalleries.Find(id);
        //    return View(homegallery);
        //}

        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        //
        // GET: /HomeGallery/Create

        public ActionResult Create()
        {
            var homegallery = new HomeGallery
            {
            };
            return View(homegallery);
        }

        //
        // POST: /HomeGallery/Create

        [HttpPost]
        public ActionResult Create(HomeGallery homegallery, IEnumerable<HttpPostedFileBase> newsPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                // The Name of the Upload component is "newsPhotos" 
                if ((newsPhotos == null) || (newsPhotos.Count() == 0))
                {
                    ModelState.AddModelError("", "请检查您是否忘记上传图片了");
                    return View(homegallery);
                }
                foreach (var file in newsPhotos)
                {
                    if (!HasFile(file))
                    {
                        ModelState.AddModelError("", "请检查您是否忘记上传图片了？");
                        return View(homegallery);
                    }
                    // Some browsers send file names with full path. This needs to be stripped.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath(gallery_path), fileName);
                    while (System.IO.File.Exists(physicalPath))
                    {
                        fileName = count.ToString() + fileName;
                        physicalPath = Path.Combine(Server.MapPath(gallery_path), fileName);
                    }
                    file.SaveAs(physicalPath);

                    string full_fileName = full_prefix + fileName;
                    string full_path = Path.Combine(Server.MapPath(gallery_path), full_fileName);
                    ImageOpt.ScaleImage(physicalPath, full_path, full_width, full_height);

                    string thumbnail_path = Path.Combine(Server.MapPath(gallery_path), thumbnail_prefix + full_fileName);
                    ImageOpt.ScaleImage(physicalPath, thumbnail_path, thumbnail_width, thumbnail_height);

                    System.IO.File.Delete(physicalPath);

                    homegallery.Picture = Url.Content(gallery_path + "/" + full_fileName);
                }
                if (!String.IsNullOrEmpty(homegallery.HyperLink))
                {
                    homegallery.HyperLink = GetAbsoluteUrl(homegallery.HyperLink);
                }
                db.HomeGalleries.Add(homegallery);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(homegallery);
        }

        //
        // GET: /HomeGallery/Edit/5

        public ActionResult Edit(int id)
        {
            HomeGallery homegallery = db.HomeGalleries.Find(id);
            return View(homegallery);
        }

        //
        // POST: /HomeGallery/Edit/5

        [HttpPost]
        public ActionResult Edit(HomeGallery homegallery, IEnumerable<HttpPostedFileBase> newsPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                if ((newsPhotos != null) && (newsPhotos.Count() > 0))
                {
                    // The Name of the Upload component is "newsPhotos" 
                    foreach (var file in newsPhotos)
                    {
                        if (!HasFile(file))
                        {
                            continue;
                        }
                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName = Path.GetFileName(file.FileName);
                        var physicalPath = Path.Combine(Server.MapPath(gallery_path), fileName);
                        while (System.IO.File.Exists(physicalPath))
                        {
                            fileName = count.ToString() + fileName;
                            physicalPath = Path.Combine(Server.MapPath(gallery_path), fileName);
                        }
                        file.SaveAs(physicalPath);

                        string full_fileName = full_prefix + fileName;
                        string full_path = Path.Combine(Server.MapPath(gallery_path), full_fileName);
                        ImageOpt.ScaleImage(physicalPath, full_path, full_width, full_height);

                        string thumbnail_path = Path.Combine(Server.MapPath(gallery_path), thumbnail_prefix + full_fileName);
                        ImageOpt.ScaleImage(physicalPath, thumbnail_path, thumbnail_width, thumbnail_height);

                        System.IO.File.Delete(physicalPath);
                        System.IO.File.Delete(Server.MapPath(homegallery.Picture));
                        System.IO.File.Delete(Server.MapPath(ToThumbnailPath(homegallery.Picture)));

                        homegallery.Picture = Url.Content(gallery_path + "/" + full_fileName);
                    }
                }
                if (!String.IsNullOrEmpty(homegallery.HyperLink))
                {
                    homegallery.HyperLink = GetAbsoluteUrl(homegallery.HyperLink);
                }
                db.Entry(homegallery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(homegallery);
        }

        //
        // GET: /HomeGallery/Delete/5

        public ActionResult Delete(int id)
        {
            HomeGallery homegallery = db.HomeGalleries.Find(id);
            return View(homegallery);
        }

        //
        // POST: /HomeGallery/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            HomeGallery homegallery = db.HomeGalleries.Find(id);
            System.IO.File.Delete(Server.MapPath(homegallery.Picture));
            System.IO.File.Delete(Server.MapPath(ToThumbnailPath(homegallery.Picture)));
            db.HomeGalleries.Remove(homegallery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public static string ToThumbnailPath(string src)
        {
            string des = "";
            int i;

            var temp = src.Split('/');

            if (temp.Length < 1)
            {
                return "";
            }

            for (i = 0; i < temp.Length - 1; i++)
            {
                des += (temp[i] + "/");
            }

            des += (thumbnail_prefix + temp[temp.Length - 1]);

            return des;
        }

        private string GetAbsoluteUrl(string url)
        {
            //VALIDATE INPUT FOR ALREADY ABSOLUTE URL
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return url;
            }
            return "http://" + url;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}