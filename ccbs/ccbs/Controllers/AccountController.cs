using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ccbs.Models;

namespace ccbs.Controllers
{
    public class AccountController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();
        private string[] configurableRoles = { LWSFRoles.newStudentAdmin, LWSFRoles.localhelpAdmin, LWSFRoles.organizationLeader, LWSFRoles.groupLeader, LWSFRoles.coworker, LWSFRoles.worshipManager, MyRoles.baikeEditor, MyRoles.baikeQandA, LWSFRoles.emailAdmin };

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult ManageAccount()
        {
            var users = Roles.GetUsersInRole(LWSFRoles.volunteer);
            var roles = Roles.GetAllRoles();

            List<MembershipUser> userList = new List<MembershipUser>();

            foreach (var username in users)
            {
                MembershipUser user = Membership.GetUser(username);
                userList.Add(user);
            }
            userList = userList.OrderBy(u => u.UserName).ToList();
            return View(userList);
        }

        public ActionResult _DisplayUsers()
        {
            var users = Roles.GetUsersInRole(LWSFRoles.volunteer);
            var roles = Roles.GetAllRoles();

            List<MembershipUser> userList = new List<MembershipUser>();

            foreach (var username in users)
            {
                MembershipUser user = Membership.GetUser(username);
                userList.Add(user);
            }
            userList = userList.OrderBy(u => u.UserName).ToList();
            return View(userList);
        }

        public JsonResult _SearchUsers(string s)
        {
            var users = Roles.GetUsersInRole(LWSFRoles.volunteer);
            var result = users.Where(u => u.Contains(s)).ToList();

            List<MembershipUser> userList = new List<MembershipUser>();

            foreach (var r in result)
            {
                MembershipUser user = Membership.GetUser(r);
                userList.Add(user);
            }
            userList = userList.OrderBy(u => u.UserName).ToList();

            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("_DisplayUsers", userList),
            });
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SetRoles(string username)
        {
            var inRoles = Roles.GetRolesForUser(username);
            var cfgRoles = configurableRoles;

            bool[] cfgValues = new bool[cfgRoles.Count()];

            int i;

            for (i = 0; i < cfgRoles.Count(); i++)
            {
                if (inRoles.Contains(cfgRoles[i]))
                {
                    cfgValues[i] = true;
                }
                else
                {
                    cfgValues[i] = false;
                }
            }
            ViewBag.username = username;
            ViewBag.cfgRoles = cfgRoles;
            ViewBag.cfgValues = cfgValues;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public string SetRoles(string username, string[] checkedRecords)
        {
            MembershipUser user = Membership.GetUser(username);

            var roles = Roles.GetRolesForUser(username);

            foreach (var role in roles)
            {
                if (!configurableRoles.Contains(role))
                {
                    continue;
                }
                if (checkedRecords.Contains(role))
                {
                    continue;
                }
                Roles.RemoveUserFromRole(username, role);
            }

            foreach (var role in checkedRecords)
            {
                if (roles.Contains(role))
                {
                    continue;
                }
                Roles.AddUserToRole(username, role);
            }

            return "Setup successfully";
        }

        [Authorize]
        public ActionResult MyHomePage()
        {
            if (User.IsInRole(LWSFRoles.volunteer))
            {
                return RedirectToAction("VolunteerHome", "StudentMinistry", null);
            }
            else if (User.IsInRole(LWSFRoles.newStudent))
            {
                return RedirectToAction("NewStudentHome", "NewStudent", null);
            }
            else if (User.IsInRole(LWSFRoles.student))
            {
                return RedirectToAction("StudentHome", "Student", null);
            }
            return RedirectToAction("Index", "Home", null);
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(string returnUrl)
        {
            LogOnModel model = new LogOnModel();
            string username;

            TryUpdateModel(model);

            if (ModelState.IsValid)
            {
                var user = Membership.GetUser(model.UserName);

                if (user == null)
                {
                    username = Membership.GetUserNameByEmail(model.UserName);
                    if (!String.IsNullOrEmpty(username))
                    {
                        user = Membership.GetUser(username);
                        if (user != null)
                        {
                            model.UserName = username;
                        }
                    }
                }


                if (user != null)
                {
                    if (user.IsLockedOut)
                    {
                        DateTime lastLockout = user.LastLockoutDate;
                        DateTime unlockDate = lastLockout.AddMinutes(Membership.PasswordAttemptWindow);
                        if (DateTime.Now > unlockDate)
                        {
                            user.UnlockUser();
                        }
                    }
                }

                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("MyHomePage", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SyncEmail()
        {
            int count = 0;
            var vlist = db.Volunteers.ToList();
            var slist = db.NewStudents.ToList();

            foreach (var v in vlist)
            {
                MembershipUser u = Membership.GetUser(v.UserName);
                if (u.Email != v.Email)
                {
                    u.Email = v.Email;
                    Membership.UpdateUser(u);
                    count++;
                }
            }
            foreach (var s in slist)
            {
                MembershipUser u = Membership.GetUser(s.UserName);
                if (u.Email != s.Email)
                {
                    u.Email = s.Email;
                    Membership.UpdateUser(u);
                    count++;
                }
            }
            return Content(count + "emails has been updated");
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }


        //Controllers for the reset page
        public ActionResult ResetPassword(string reset, string username)
        {
            if ((reset != null) && (username != null))
            {
                MembershipUser currentUser = Membership.GetUser(username);

                if (ResetPasswordModel.HashResetParams(currentUser.UserName, currentUser.ProviderUserKey.ToString()) == reset)
                {
                    if (currentUser != null)
                    {
                        if (currentUser.IsLockedOut)
                        {
                            DateTime lastLockout = currentUser.LastLockoutDate;
                            DateTime unlockDate = lastLockout.AddMinutes(Membership.PasswordAttemptWindow);
                            if (DateTime.Now > unlockDate)
                            {
                                currentUser.UnlockUser();
                            }
                        }
                    }

                    ViewBag.newPass = currentUser.ResetPassword();
                    ViewBag.userName = username;
                    return View("ResetPasswordSuccess");
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel Model)
        {
            MembershipUser currentUser = null;
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(Model.Username) && String.IsNullOrEmpty(Model.Email))
                {
                    ModelState.AddModelError("", "Username and Email cannot be both empty!");
                    return View(Model);
                }
                if (!String.IsNullOrEmpty(Model.Username))
                {
                    currentUser = Membership.GetUser(Model.Username);
                    if (currentUser == null)
                    {
                        Model.Username = Membership.GetUserNameByEmail(Model.Email);
                        if (!String.IsNullOrEmpty(Model.Username))
                        {
                            currentUser = Membership.GetUser(Model.Username);
                        }
                    }
                }
                else
                {
                    Model.Username = Membership.GetUserNameByEmail(Model.Email);
                    if (!String.IsNullOrEmpty(Model.Username))
                    {
                        currentUser = Membership.GetUser(Model.Username);
                    }
                }
                if (currentUser == null)
                {
                    ModelState.AddModelError("", "The Username and Email you entered don't not exist in our system.");
                    return View(Model);
                }

                ResetPasswordModel.SendResetEmail(currentUser);
                ViewBag.Email = currentUser.Email;
                return View("ResetPswEmailSent");
            }
            return View(Model);
        }

        internal static bool RegisterNew(RegisterModel model, out MembershipCreateStatus createStatus)
        {
            // Attempt to register the user
            Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                return true;
            }
            // If we got this far, something failed, redisplay form
            return false;
        }

        internal static bool DeleterUser(string username)
        {
            Membership.DeleteUser(username);
            return true;
        }

        #region Status Codes
        internal static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
