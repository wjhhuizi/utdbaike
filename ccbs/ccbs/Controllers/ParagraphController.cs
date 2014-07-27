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
    [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.coworker)]
    public class ParagraphController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        private const string news_pic_path = "~/Photos/News";
        private const string news_pic_scaled_prefix = "scaled_";
        private const int news_pic_width = 730;
        private const int news_pic_height = 400;


        //
        // GET: /Paragraph/

        //public ViewResult Index()
        //{
        //    var paragraphs = db.Paragraphs.Include(p => p.News);
        //    return View(paragraphs.ToList());
        //}

        //
        // GET: /Paragraph/Details/5

        //public ViewResult Details(int id)
        //{
        //    Paragraph paragraph = db.Paragraphs.Find(id);
        //    return View(paragraph);
        //}

        //
        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        //
        // GET: /Paragraph/Create

        public ActionResult Create(int newsId)
        {
            var model = new Paragraph
            {
                NewsId = newsId,
            };
            return View(model);
        }

        //
        // POST: /Paragraph/Create

        [HttpPost]
        public ActionResult Create(Paragraph paragraph, IEnumerable<HttpPostedFileBase> newsPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                // The Name of the Upload component is "newsPhotos"
                if ((newsPhotos != null) && (newsPhotos.Count() > 0))
                {
                    foreach (var file in newsPhotos)
                    {
                        if (!HasFile(file))
                        {
                            continue;
                        }
                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName = Path.GetFileName(file.FileName);
                        var physicalPath = Path.Combine(Server.MapPath(news_pic_path), fileName);
                        while (System.IO.File.Exists(physicalPath))
                        {
                            fileName = count.ToString() + fileName;
                            physicalPath = Path.Combine(Server.MapPath(news_pic_path), fileName);
                        }
                        file.SaveAs(physicalPath);

                        string scaled_fileName = news_pic_scaled_prefix + fileName;
                        string scaled_physicalPath = Path.Combine(Server.MapPath(news_pic_path), scaled_fileName);
                        ImageOpt.ScaleImage(physicalPath, scaled_physicalPath, news_pic_width, news_pic_height);
                        System.IO.File.Delete(physicalPath);
                        paragraph.Image = Url.Content(news_pic_path + "/" + scaled_fileName);
                    }
                }
                db.Paragraphs.Add(paragraph);
                db.SaveChanges();
                return RedirectToAction("Details", "News", new { id = paragraph.NewsId });
            }
            return View(paragraph);
        }

        //
        // GET: /Paragraph/Edit/5

        public ActionResult Edit(int id)
        {
            Paragraph paragraph = db.Paragraphs.Find(id);
            return View(paragraph);
        }

        //
        // POST: /Paragraph/Edit/5

        [HttpPost]
        public ActionResult Edit(Paragraph paragraph, IEnumerable<HttpPostedFileBase> newsPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                // The Name of the Upload component is "newsPhotos" 
                if ((newsPhotos != null) && (newsPhotos.Count() > 0))
                {
                    foreach (var file in newsPhotos)
                    {
                        if (!HasFile(file))
                        {
                            continue;
                        }
                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName = Path.GetFileName(file.FileName);
                        var physicalPath = Path.Combine(Server.MapPath(news_pic_path), fileName);
                        while (System.IO.File.Exists(physicalPath))
                        {
                            fileName = count.ToString() + fileName;
                            physicalPath = Path.Combine(Server.MapPath(news_pic_path), fileName);
                        }
                        file.SaveAs(physicalPath);

                        string scaled_fileName = news_pic_scaled_prefix + fileName;
                        string scaled_physicalPath = Path.Combine(Server.MapPath(news_pic_path), scaled_fileName);
                        ImageOpt.ScaleImage(physicalPath, scaled_physicalPath, news_pic_width, news_pic_height);

                        System.IO.File.Delete(physicalPath);

                        if (!String.IsNullOrEmpty(paragraph.Image))
                        {//delete the previous one
                            System.IO.File.Delete(Server.MapPath(paragraph.Image));
                        }

                        paragraph.Image = Url.Content(news_pic_path + "/" + scaled_fileName);
                    }
                }
                db.Entry(paragraph).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "News", new { id = paragraph.NewsId });
            }
            return View(paragraph);
        }

        //
        // GET: /Paragraph/Delete/5

        public ActionResult Delete(int id)
        {
            Paragraph paragraph = db.Paragraphs.Find(id);
            return View(paragraph);
        }

        //
        // POST: /Paragraph/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Paragraph paragraph = db.Paragraphs.Find(id);
            if (!String.IsNullOrEmpty(paragraph.Image))
            {
                System.IO.File.Delete(Server.MapPath(paragraph.Image));
            }
            db.Paragraphs.Remove(paragraph);
            db.SaveChanges();
            return RedirectToAction("Details", "News", new { id = paragraph.NewsId });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}