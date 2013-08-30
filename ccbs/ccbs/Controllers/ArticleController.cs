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
    public class ArticleController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /Article/

        public ViewResult Index()
        {
            var root = db.SubDirectories.FirstOrDefault();

            if (root == null)
            {
                root = new SubDirectory
                {
                    Name = "Root",
                    Description = "This is the root directory, which is created automatically, please don't delete or edit!",
                    LastUpdate = DateTime.Now,
                };
                db.SubDirectories.Add(root);
                db.SaveChanges();
                root = db.SubDirectories.FirstOrDefault();
            }
            int showMode;
            if (User.IsInRole(LWSFRoles.admin) || User.IsInRole(MyRoles.baikeEditor))
            {
                showMode = TreeViewModel.SHOW_ALL_ARTICLE;
            }
            else
            {
                showMode = TreeViewModel.SHOW_PUBLISHED_ARTICLE;
            }
            var treeRootNode = TreeViewModel.BuildTreeViewModel(root, showMode);


            //var treeSubNodes = new List<TreeViewModel>();
            //treeSubNodes.Add(
            //    new TreeViewModel
            //    {
            //        Id = -1,
            //        Title = "No Directory",
            //    });
            //return View(treeSubNodes);

            return View(treeRootNode.SubNodes);
        }

        public ActionResult LatestArticles()
        {
            int count = 10;
            List<Article> latestArtiles = new List<Article>();
            var articles = db.Articles.OrderByDescending(a => a.LastUpdate);
            int i = 0;
            foreach (var article in articles)
            {
                latestArtiles.Add(article);
                i++;
                if (i >= count)
                {
                    break;
                }
            }
            return View("_ListArticles", latestArtiles);
        }

        public ActionResult PopularArticles()
        {
            int count = 10;
            List<Article> popularArtiles = new List<Article>();
            var articles = db.Articles.OrderByDescending(a => a.VisitCount);
            int i = 0;
            foreach (var article in articles)
            {
                popularArtiles.Add(article);
                i++;
                if (i >= count)
                {
                    break;
                }
            }
            return View("_ListArticles", popularArtiles);
        }

        public ActionResult ShowDeclaration()
        {
            var root = db.SubDirectories.FirstOrDefault();
            ViewBag.Description = Server.HtmlDecode(root.Description);
            return View();
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult EditDeclaration()
        {
            var root = db.SubDirectories.FirstOrDefault();
            ViewBag.Description = Server.HtmlDecode(root.Description);
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult EditDeclaration(string Description)
        {
            var root = db.SubDirectories.FirstOrDefault();
            root.Description = Server.HtmlEncode(Description);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ShowTreeNodeContent(string typeAndId)
        {
            int type, id;
            var array = typeAndId.Split(',');
            if (array.Length != 2)
            {
                return Content("");
            }
            type = Int32.Parse(array[0]);
            id = Int32.Parse(array[1]);
            switch (type)
            {
                case TreeNodeType.ArticleItem:
                    return RedirectToAction("_ShowArticle", new { id = id });
                case TreeNodeType.DirectoryItem:
                    return RedirectToAction("_ShowDirectory", new { id = id });
            }
            return Content("");
        }

        public ActionResult _ShowArticle(int id)
        {
            var article = db.Articles.Find(id);
            article.VisitCount++;
            db.SaveChanges();
            article.MainContent = Server.HtmlDecode(article.MainContent);
            return View(article);
        }

        public string UpdateArticleLikeCount(int articleId)
        {
            var article = db.Articles.Find(articleId);
            article.LikeCount++;
            db.SaveChanges();
            return article.LikeCount.ToString();
        }

        public string UpdateCommentLikeCount(int commentId)
        {
            var comment = db.ArticleComments.Find(commentId);
            comment.LikeCount++;
            db.SaveChanges();
            return comment.LikeCount.ToString();
        }

        //[Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult _ShowDirectory(int id)
        {
            var dir = db.SubDirectories.Find(id);
            dir.Description = Server.HtmlDecode(dir.Description);
            return View(dir);
        }

        //
        // GET: /Article/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult CreateArticle(int topDirId)
        {
            var topDir = db.SubDirectories.Find(topDirId);
            var article = new Article
            {
                SubDirectoryId = topDirId,
                LastUpdate = DateTime.Now,
                CreatedDate = DateTime.Now,
                Creator = User.Identity.Name,
                LikeCount = 0,
                VisitCount = 0,
                Published = false,
            };

            ViewBag.Number = new SelectList(topDir.Articles.OrderBy(a => a.Number).ToList(), "Number", "Title");

            return View(article);
        }

        //
        // POST: /Article/Create

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult CreateArticle(Article article)
        {
            if (article.Number == 0 || ModelState.IsValid)
            {
                var dir = db.SubDirectories.Find(article.SubDirectoryId);
                foreach (var art in dir.Articles)
                {
                    if (art.Number > article.Number)
                    {
                        art.Number++;
                    }
                }
                article.Number++;

                article.MainContent = Server.HtmlEncode(article.MainContent);
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var topDir = db.SubDirectories.Find(article.SubDirectoryId);
            ViewBag.Number = new SelectList(topDir.Articles.OrderBy(a => a.Number).ToList(), "Number", "Title", article.Number);
            return View(article);
        }

        //
        // GET: /Article/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult EditArticle(int id)
        {
            Article article = db.Articles.Find(id);
            article.MainContent = Server.HtmlDecode(article.MainContent);
            ViewBag.Number = new SelectList(article.Directory.Articles.OrderBy(a => a.Number).ToList(), "Number", "Title", article.Number);
            return View(article);
        }

        //
        // POST: /Article/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult EditArticle(Article article)
        {
            if (article.Number == 0 || ModelState.IsValid)
            {
                var a = db.Articles.Find(article.Id);
                if (article.Number != a.Number)
                {
                    var dir = db.SubDirectories.Find(article.SubDirectoryId);
                    foreach (var art in dir.Articles)
                    {
                        if (art.Number > article.Number)
                        {
                            art.Number++;
                        }
                    }
                    article.Number++;
                }

                article.LastUpdate = DateTime.Now;
                article.MainContent = Server.HtmlEncode(article.MainContent);

                db.Entry(a).CurrentValues.SetValues(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Number = new SelectList(article.Directory.Articles.OrderBy(a => a.Number).ToList(), "Number", "Title", article.Number);
            return View(article);
        }

        //
        // GET: /Article/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult DeleteArticle(int id)
        {
            Article article = db.Articles.Find(id);
            return View(article);
        }

        //
        // POST: /Article/Delete/5

        [HttpPost, ActionName("DeleteArticle")]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult DeleteArticleConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Article/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult CreateDirectory(int topDirId)
        {
            if (topDirId <= 1)
            {
                var root = db.SubDirectories.FirstOrDefault();
                topDirId = root.Id;
            }
            var dir = new SubDirectory
            {
                SubDirectoryId = topDirId,
                LastUpdate = DateTime.Now,
            };

            var topDir = db.SubDirectories.Find(topDirId);
            ViewBag.Number = new SelectList(topDir.SubDirectories.OrderBy(a => a.Number).ToList(), "Number", "Name");

            return View(dir);
        }

        //
        // POST: /Article/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult CreateDirectory(SubDirectory subdirectory)
        {
            var topDir = db.SubDirectories.Find(subdirectory.SubDirectoryId);
            if (subdirectory.Number == 0 || ModelState.IsValid)
            {
                foreach (var dir in topDir.SubDirectories)
                {
                    if (dir.Number > subdirectory.Number)
                    {
                        dir.Number++;
                    }
                }
                subdirectory.Number++;
                db.SubDirectories.Add(subdirectory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Number = new SelectList(topDir.SubDirectories.OrderBy(a => a.Number).ToList(), "Number", "Name", subdirectory.Number);
            return View(subdirectory);
        }

        //
        // GET: /Article/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult EditDirectory(int id)
        {
            SubDirectory subdirectory = db.SubDirectories.Find(id);
            ViewBag.Number = new SelectList(subdirectory.TopDirectory.SubDirectories.OrderBy(a => a.Number).ToList(), "Number", "Name", subdirectory.Number);
            return View(subdirectory);
        }

        //
        // POST: /Article/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult EditDirectory(SubDirectory subdirectory)
        {
            var topDir = db.SubDirectories.Find(subdirectory.SubDirectoryId);
            if (subdirectory.Number == 0 || ModelState.IsValid)
            {
                var d = db.SubDirectories.Find(subdirectory.Id);

                if (subdirectory.Number != d.Number)
                {
                    foreach (var dir in topDir.SubDirectories)
                    {
                        if (dir.Number > subdirectory.Number)
                        {
                            dir.Number++;
                        }
                    }
                    subdirectory.Number++;
                }

                subdirectory.LastUpdate = DateTime.Now;

                db.Entry(d).CurrentValues.SetValues(subdirectory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Number = new SelectList(topDir.SubDirectories.OrderBy(a => a.Number).ToList(), "Number", "Name", subdirectory.Number);
            return View(subdirectory);
        }

        //
        // GET: /Article/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult DeleteDirectory(int id)
        {
            SubDirectory subdirectory = db.SubDirectories.Find(id);
            return View(subdirectory);
        }

        //
        // POST: /Article/Delete/5

        [HttpPost, ActionName("DeleteDirectory")]
        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public ActionResult DeleteDirectoryConfirmed(int id)
        {
            SubDirectory subdirectory = db.SubDirectories.Find(id);
            db.SubDirectories.Remove(subdirectory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult ArticleComment()
        {
            ArticleComment comment = new ArticleComment
                {
                    LikeCount = 0,
                };

            comment.Comment = Request["Comment"];
            if (String.IsNullOrEmpty(comment.Comment))
            {
                return RedirectToAction("Index");
            }

            comment.ArticleId = Int32.Parse(Request["ArticleId"]);
            comment.LastUpdate = DateTime.Now;

            if (Request.IsAuthenticated)
            {
                comment.UserName = User.Identity.Name;
            }
            else
            {
                comment.UserName = "Guest";
            }
            db.ArticleComments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Index");

        }


        //
        // POST: /Article/Delete/5

        [Authorize(Roles = LWSFRoles.admin + ", " + MyRoles.baikeEditor)]
        public string DeleteArticleCommentConfirmed(int id)
        {
            ArticleComment comment = db.ArticleComments.Find(id);
            db.ArticleComments.Remove(comment);
            db.SaveChanges();
            return "";
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}