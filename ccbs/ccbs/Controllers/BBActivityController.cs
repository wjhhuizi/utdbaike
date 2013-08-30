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
    public class BBActivityController : Controller
    {
        private BangbangDBContainer db = new BangbangDBContainer();

        //
        // GET: /BBActivity/

        public ViewResult Index()
        {
            var bbCategories = db.BBCategories.ToList();
            return View(bbCategories);
        }

        public ActionResult CatergoryActivities(int catergoryId)
        {
            var catergory = db.BBCategories.Find(catergoryId);

            List<BBActivity> futureActivities = new List<BBActivity>();

            foreach (var activity in catergory.BBActivities)
            {
                var futureEntries = activity.BBRegisterEntries.Where(r => r.Time > DateTime.Now);
                if (futureEntries == null || futureEntries.Count() == 0)
                {
                    continue;
                }
                futureActivities.Add(activity);
            }
            ViewBag.futureActivities = futureActivities;

            var currentBBUser = GetCurrentBBUser();
            ViewBag.currentBBUser = currentBBUser;

            return View(catergory);
        }

        public ActionResult Manage(int? bbuserId)
        {
            var currUser = GetCurrentBBUser();
            List<BBActivity> bbactivities;
            List<BBRegisterEntry> attendedEntries;
            if (bbuserId == null)
            {
                if (User.IsInRole(LWSFRoles.admin) || User.IsInRole(LWSFRoles.bangbangAdmin))
                {
                    bbactivities = db.BBActivities.ToList();
                }
                else
                {

                    bbactivities = currUser.CreatedActivities.ToList();
                }
                attendedEntries = currUser.BBRegisterEntries.ToList();
            }
            else
            {
                if (!User.IsInRole(LWSFRoles.admin) && !User.IsInRole(LWSFRoles.bangbangAdmin) && currUser.Id != bbuserId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
                var user = db.BBUsers.Find(bbuserId);
                bbactivities = user.CreatedActivities.ToList();
                attendedEntries = user.BBRegisterEntries.ToList();
            }
            return View(bbactivities);
        }

        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult ManageMyCreatedBBActivity()
        {
            var currUser = GetCurrentBBUser();
            List<BBActivity> bbactivities;
            bbactivities = currUser.CreatedActivities.OrderBy(a => a.PostDate).ToList();

            return View(bbactivities);
        }

        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult ShowRegisteredUsers(int registerEntryId)
        {
            var registerEntry = db.BBRegisterEntries.Find(registerEntryId);
            return View(registerEntry);
        }

        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult ManageMyAttendedBBActivity()
        {
            var currUser = GetCurrentBBUser();
            List<BBRegisterEntry> attendedEntries;
            attendedEntries = currUser.BBRegisterEntries.OrderBy(r => r.Time).ToList();

            var currentBBUser = GetCurrentBBUser();
            ViewBag.currentBBUser = currentBBUser;

            return View(attendedEntries);
        }

        //
        // GET: /BBActivity/Details/5

        public ViewResult BBActivityDetails(int id)
        {
            BBActivity bbactivity = db.BBActivities.Find(id);
            return View(bbactivity);
        }

        //
        // GET: /BBActivity/Create
        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult CreateBBActivity()
        {
            var bbUser = GetCurrentBBUser();
            var activity = new BBActivity
            {
                BBUserId = bbUser.Id,
                PostDate = DateTime.Now,
                LastUpdate = DateTime.Now,
            };
            ViewBag.BBCategoryId = new SelectList(db.BBCategories, "Id", "Name");
            return View();
        }

        //
        // POST: /BBActivity/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult CreateBBActivity(BBActivity bbactivity)
        {
            if (ModelState.IsValid)
            {
                db.BBActivities.Add(bbactivity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BBCategoryId = new SelectList(db.BBCategories, "Id", "Name", bbactivity.BBCategoryId);
            return View(bbactivity);
        }

        public ActionResult CreateBBRegisterEntry(int bbactivityId)
        {
            var ac = db.BBActivities.Find(bbactivityId);
            ViewBag.bbactivity = ac;
            var entry = new BBRegisterEntry
            {
                BBActivityId = bbactivityId,
            };
            return View(entry);
        }

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult CreateBBRegisterEntry(BBRegisterEntry bbregisterentry)
        {
            if (ModelState.IsValid)
            {
                db.BBRegisterEntries.Add(bbregisterentry);
                db.SaveChanges();
                return RedirectToAction("CreateRegisterEntry");
            }
            var ac = db.BBActivities.Find(bbregisterentry.BBActivity);
            ViewBag.bbactivity = ac;
            return View(bbregisterentry);
        }

        public ActionResult EditBBRegisterEntry(int registerEntryId)
        {
            var registerEntry = db.BBRegisterEntries.Find(registerEntryId);
            return View(registerEntry);
        }

        [HttpPost]
        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult EditBBRegisterEntry(BBRegisterEntry bbregisterentry, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bbregisterentry).State = EntityState.Modified;
                db.SaveChanges();
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(bbregisterentry);
        }

        //
        // GET: /BBActivity/Edit/5

        public ActionResult EditBBActivity(int id)
        {
            BBActivity bbactivity = db.BBActivities.Find(id);
            ViewBag.BBUserId = new SelectList(db.BBUsers, "Id", "Name", bbactivity.BBUserId);
            return View(bbactivity);
        }

        //
        // POST: /BBActivity/Edit/5

        [HttpPost]
        public ActionResult EditBBActivity(BBActivity bbactivity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bbactivity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BBUserId = new SelectList(db.BBUsers, "Id", "Name", bbactivity.BBUserId);
            return View(bbactivity);
        }

        //
        // GET: /BBActivity/Delete/5

        public ActionResult DeleteBBActivity(int id)
        {
            BBActivity bbactivity = db.BBActivities.Find(id);
            return View(bbactivity);
        }

        //
        // POST: /BBActivity/Delete/5

        [HttpPost, ActionName("DeleteBBActivity")]
        public ActionResult DeleteBBActivityConfirmed(int id)
        {
            BBActivity bbactivity = db.BBActivities.Find(id);
            db.BBActivities.Remove(bbactivity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult AttendBBActivity(int Rid, int? Uid, string returnUrl)
        {
            BBUser user;
            if (Uid != null)
            {
                user = db.BBUsers.Find(Uid.Value);
            }
            else
            {
                user = GetCurrentBBUser();
            }
            var registerEntry = db.BBRegisterEntries.Find(Rid);
            if (registerEntry != null && user != null)
            {
                registerEntry.RegisteredBBUsers.Add(user);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult CancelBBActivity(int Rid, int Uid, string returnUrl)
        {
            var user = db.BBUsers.Find(Uid);
            var registerEntry = db.BBRegisterEntries.Find(Rid);
            if (registerEntry != null && user != null)
            {
                registerEntry.RegisteredBBUsers.Remove(user);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public BBUser GetCurrentBBUser()
        {
            var username = User.Identity.Name;
            if (String.IsNullOrEmpty(username))
            {
                return null;
            }
            var currUser = db.BBUsers.Where(u => u.Username == username).FirstOrDefault();
            return currUser;
        }
    }
}