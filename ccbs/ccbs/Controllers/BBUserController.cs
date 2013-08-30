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
    public class BBUserController : BaseController
    {
        private BangbangDBContainer db = new BangbangDBContainer();

        //
        // GET: /BBUser/

        public ViewResult Index()
        {
            return View(db.BBUsers.ToList());
        }

        //
        // GET: /BBUser/Details/5

        public ViewResult Details(int id)
        {
            BBUser bbuser = db.BBUsers.Find(id);
            return View(bbuser);
        }

        [Authorize(Roles = LWSFRoles.BBUser)]
        public ActionResult BBHome()
        {
            var currentUser = GetCurrentBBUser();
            var allCategories = db.BBCategories.ToList();
            ViewBag.allCategories = allCategories;
            return View(currentUser);
        }

        // GET: /BBUser/BBUserRegister

        public ActionResult BBUserRegister()
        {
            var user = new BBUserRegisterModel
            {
                RegTime = DateTime.Now,
                Avatar = null,
            };
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "Name");
            return View(user);
        }

        //
        // @Html.ActionLink("Register", "BBUserRegister", "BBUser", new { returnUrl = Request.RawUrl }, null)

        [HttpPost]
        public ActionResult BBUserRegister(BBUserRegisterModel bbRegister)
        {
            MembershipCreateStatus createStatus;
            if (ModelState.IsValid)
            {
                var location = db.Locations.Find(bbRegister.LocationId);
                if (location == null)
                {
                    ModelState.AddModelError("", "Please select a valid location");
                    return View(bbRegister);
                }
                if (!bbRegister.IsValideEduEmail(bbRegister.Email, location.Domain))
                {
                    ModelState.AddModelError("", "Sorry, Currently we only allow the Campus Email domain to register");
                    return View(bbRegister);
                }
                if (AccountController.RegisterNew(bbRegister.GetRegisterModel(), out createStatus) == false)
                {
                    ModelState.AddModelError("", AccountController.ErrorCodeToString(createStatus));
                    return View(bbRegister);
                }
                Roles.AddUserToRole(bbRegister.Username, LWSFRoles.BBUser);

                var bbuser = bbRegister.GetBBUserModel();
                db.BBUsers.Add(bbuser);
                db.SaveChanges();
                return RedirectToAction("BBHome");
            }
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "Name", bbRegister.LocationId);
            return View(bbRegister);
        }

        public ActionResult ActivateMyAccount(int bbUserId, string emailAddr, string hashCode)
        {
            var bbUser = db.BBUsers.Find(bbUserId);
            if (bbUser == null)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }
            if (String.IsNullOrEmpty(emailAddr) || string.IsNullOrEmpty(hashCode))
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }
            string hash = ResetPasswordModel.HashResetParams(bbUser.Username, emailAddr);

            if (hash != hashCode)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }
            bbUser.IsActive = true;
            db.SaveChanges();
            return View("_AccountActivationSucceed");
        }

        //
        // GET: /BBUser/Edit/5

        public ActionResult Edit(int id)
        {
            BBUser bbuser = db.BBUsers.Find(id);
            return View(bbuser);
        }

        //
        // POST: /BBUser/Edit/5

        [HttpPost]
        public ActionResult Edit(BBUser bbuser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bbuser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bbuser);
        }

        //
        // GET: /BBUser/Delete/5

        public ActionResult Delete(int id)
        {
            BBUser bbuser = db.BBUsers.Find(id);
            return View(bbuser);
        }

        //
        // POST: /BBUser/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            BBUser bbuser = db.BBUsers.Find(id);
            db.BBUsers.Remove(bbuser);
            db.SaveChanges();
            return RedirectToAction("Index");
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

        public ActionResult SendBBUserActivationEmail(int bbUserId)
        {
            SendActivationEmail(bbUserId);
            return Content("Activation Email Sent successfully!");
        }

        public bool SendActivationEmail(int bbUserId)
        {
            var bbUser = db.BBUsers.Find(bbUserId);
            bool state;
            if (bbUser == null)
            {
                return false;
            }
            var emailSendModel = new EmailSentModel();
            emailSendModel.Subject = bbUser.Name + "Welcome To Join Bangbang";

            string hashCode = ResetPasswordModel.HashResetParams(bbUser.Username, bbUser.Email);
            ViewBag.emailAddr = bbUser.Email;
            ViewBag.hashCode = hashCode;

            emailSendModel.Body = RenderPartialViewToString("_BBUserActivationPage", bbUser);
            emailSendModel.To.Add(bbUser.Email);
            state = emailSendModel.Send();
            return state;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}