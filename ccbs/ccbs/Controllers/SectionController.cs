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
    public class SectionController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        private const string gallery_path = "~/Photos/Gallery";
        private const int full_width = 350;
        private const int full_height = 175;
        private const string prefix = "scaled_";

        //
        // GET: /Section/
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            return View(db.Sections.OrderBy(s => s.Order).ToList());
        }

        public ActionResult _ShowHomeSections()
        {
            var sections = db.Sections.Where(s => s.IsOnHomePage == true).OrderBy(s => s.Order).ToList();
            return View("_ShowSections", sections);
        }

        //
        // GET: /Section/Details/5

        public ViewResult _ShowSection(int id)
        {
            Section section = db.Sections.Find(id);
            return View(section);
        }

        //
        // GET: /Section/Details/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult SectionDetails(int id)
        {
            Section section = db.Sections.Find(id);
            return View(section);
        }

        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        //
        // GET: /Section/Create
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult CreateSection()
        {
            var section = new Section
            {
                TitleColor = "#87CEFA",
            };
            return View(section);
        }

        //
        // POST: /Section/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult CreateSection(Section section)
        {
            if (ModelState.IsValid)
            {
                db.Sections.Add(section);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(section);
        }

        //
        // GET: /Section/Edit/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult EditSection(int id)
        {
            Section section = db.Sections.Find(id);
            return View(section);
        }

        //
        // POST: /Section/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult EditSection(Section section)
        {
            if (ModelState.IsValid)
            {
                db.Entry(section).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(section);
        }

        //
        // GET: /Section/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteSection(int id)
        {
            Section section = db.Sections.Find(id);
            return View(section);
        }

        //
        // POST: /Section/Delete/5

        [HttpPost, ActionName("DeleteSection")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteSectionConfirmed(int id)
        {
            Section section = db.Sections.Find(id);
            foreach (var item in section.GalleryItems.ToList())
            {
                db.GalleryItems.Remove(item);
            }
            foreach (var sub in section.SubItems.ToList())
            {
                db.SectionSubItems.Remove(sub);
            }
            db.Sections.Remove(section);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult AddSubItem()
        {
            SectionSubItem subItem = new SectionSubItem();

            subItem.Text = Request["Text"];
            subItem.Link = Request["Link"];
            subItem.SectionId = Int32.Parse(Request["SectionId"]);
            subItem.Type = 0;
            subItem.LastUpdate = DateTime.Now;

            if (String.IsNullOrEmpty(subItem.Text) || String.IsNullOrEmpty(subItem.Link))
            {
                return RedirectToAction("SectionDetails", new { id = subItem.SectionId });
            }

            db.SectionSubItems.Add(subItem);
            db.SaveChanges();
            return RedirectToAction("SectionDetails", new { id = subItem.SectionId });
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult RemoveSubItem(int subItemId)
        {
            var subItem = db.SectionSubItems.Find(subItemId);
            int sectionId = subItem.SectionId;
            db.SectionSubItems.Remove(subItem);
            db.SaveChanges();
            return RedirectToAction("SectionDetails", new { id = sectionId });
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult AddGalleryItem(int sectionId)
        {
            var galleryItem = new GalleryItem
            {
                SectionId = sectionId,
                Order = 1,
            };
            return View(galleryItem);
        }

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult AddGalleryItem(GalleryItem galleryItem, IEnumerable<HttpPostedFileBase> newsPhotos)
        {
            int count = 0;
            if (ModelState.IsValid)
            {
                // The Name of the Upload component is "newsPhotos" 
                if ((newsPhotos == null) || (newsPhotos.Count() == 0))
                {
                    ModelState.AddModelError("", "请检查您是否忘记上传图片了");
                    return View(galleryItem);
                }
                foreach (var file in newsPhotos)
                {
                    if (!HasFile(file))
                    {
                        ModelState.AddModelError("", "请检查您是否忘记上传图片了？");
                        return View(galleryItem);
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

                    string full_fileName = prefix + fileName;
                    string full_path = Path.Combine(Server.MapPath(gallery_path), full_fileName);
                    ImageOpt.ScaleImage(physicalPath, full_path, full_width, full_height);

                    System.IO.File.Delete(physicalPath);

                    galleryItem.Picture = Url.Content(gallery_path + "/" + full_fileName);
                }
                if (!String.IsNullOrEmpty(galleryItem.HyperLink))
                {
                    galleryItem.HyperLink = GetAbsoluteUrl(galleryItem.HyperLink);
                }
                db.GalleryItems.Add(galleryItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(galleryItem);
        }

        // GET: /HomeGallery/Edit/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult EditGalleryItem(int id)
        {
            GalleryItem galleryItem = db.GalleryItems.Find(id);
            return View(galleryItem);
        }

        //
        // POST: /HomeGallery/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult EditGalleryItem(GalleryItem galleryItem, IEnumerable<HttpPostedFileBase> newsPhotos)
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

                        string full_fileName = prefix + fileName;
                        string full_path = Path.Combine(Server.MapPath(gallery_path), full_fileName);
                        ImageOpt.ScaleImage(physicalPath, full_path, full_width, full_height);

                        System.IO.File.Delete(physicalPath);
                        System.IO.File.Delete(Server.MapPath(galleryItem.Picture));

                        galleryItem.Picture = Url.Content(gallery_path + "/" + full_fileName);
                    }
                }
                if (!String.IsNullOrEmpty(galleryItem.HyperLink))
                {
                    galleryItem.HyperLink = GetAbsoluteUrl(galleryItem.HyperLink);
                }
                db.Entry(galleryItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(galleryItem);
        }

        //
        // GET: /HomeGallery/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteGalleryItem(int id)
        {
            GalleryItem galleryItem = db.GalleryItems.Find(id);
            return View(galleryItem);
        }

        //
        // POST: /HomeGallery/Delete/5

        [HttpPost, ActionName("DeleteGalleryItem")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteGalleryItemConfirmed(int id)
        {
            GalleryItem galleryItem = db.GalleryItems.Find(id);
            System.IO.File.Delete(Server.MapPath(galleryItem.Picture));
            db.GalleryItems.Remove(galleryItem);
            db.SaveChanges();
            return RedirectToAction("Index");
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