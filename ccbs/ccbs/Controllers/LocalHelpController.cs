using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.Net.Mail;
using System.IO;
using OfficeOpenXml;

namespace ccbs.Controllers
{
    public class LocalHelpController : Controller
    {
        private StudentModelContainer db = new StudentModelContainer();


        //
        // GET: /LocalHelp/

        public ViewResult Index()
        {
            return View();
        }

        public ActionResult _RecentLocalHelps()
        {
            var registerEntries = db.RegisterEntries.Where(r => r.Time > DateTime.Now).OrderBy(r => r.Time);

            List<LocalHelp> localhelps = new List<LocalHelp>();
            int count = 0;
            foreach (var reg in registerEntries)
            {
                if (!User.IsInRole(LWSFRoles.admin) && !User.IsInRole(LWSFRoles.localhelpAdmin))
                {
                    if (reg.LocalHelp.Restriction == LocalHelpRestriction.INSIDE_ORG_ONLY)
                    {
                        var currNewStudent = GetCurrentNewStudent();
                        var currVolunteer = GetCurrentVolunteer();
                        if ((currNewStudent == null || currNewStudent.Organization == null || reg.LocalHelp.OrganizationId != currNewStudent.Organization.Id)
                            && (currVolunteer == null || currVolunteer.OrganizationId != reg.LocalHelp.OrganizationId))
                        {
                            continue;
                        }
                    }
                }
                if (localhelps.Contains(reg.LocalHelp))
                {
                    continue;
                }
                localhelps.Add(reg.LocalHelp);
                count++;
                if (count >= 10)
                {
                    break;
                }
            }
            return View(localhelps);

        }

        public ActionResult _ShowLocalhelps()
        {
            var registerEntries = db.RegisterEntries.Where(r => r.Time > DateTime.Now).OrderBy(r => r.Time);

            List<LocalHelp> localhelps = new List<LocalHelp>();

            foreach (var reg in registerEntries)
            {
                if (!User.IsInRole(LWSFRoles.admin) && !User.IsInRole(LWSFRoles.localhelpAdmin))
                {
                    if (reg.LocalHelp.Restriction == LocalHelpRestriction.INSIDE_ORG_ONLY)
                    {
                        var currNewStudent = GetCurrentNewStudent();
                        var currVolunteer = GetCurrentVolunteer();
                        if ((currNewStudent == null || currNewStudent.Organization == null || reg.LocalHelp.OrganizationId != currNewStudent.Organization.Id)
                            && (currVolunteer == null || currVolunteer.OrganizationId != reg.LocalHelp.OrganizationId))
                        {
                            continue;
                        }
                    }
                }
                if (localhelps.Contains(reg.LocalHelp))
                {
                    continue;
                }
                localhelps.Add(reg.LocalHelp);
            }
            return View(localhelps);
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult _ShowVolunteerRegisterEntries()
        {
            var registerEntries = db.RegisterEntries.Where(r => r.Time > DateTime.Now).OrderBy(r => r.Time);
            List<LocalHelp> localhelps = new List<LocalHelp>();
            var currVolunteer = GetCurrentVolunteer();

            foreach (var reg in registerEntries)
            {
                if (!User.IsInRole(LWSFRoles.admin) && !User.IsInRole(LWSFRoles.localhelpAdmin))
                {
                    if (currVolunteer == null || currVolunteer.OrganizationId != reg.LocalHelp.OrganizationId)
                    {
                        continue;
                    }
                }
                if (localhelps.Contains(reg.LocalHelp))
                {
                    continue;
                }
                localhelps.Add(reg.LocalHelp);
            }
            ViewBag.currVolunteer = currVolunteer;
            return View(localhelps);
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult ShowVolunteerRegisterEntries()
        {
            return View();
        }

        // GET: /LocalHelp/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult VolunteerRegisterDetails(int localhelpId)
        {
            var localhelp = db.LocalHelps.Find(localhelpId);

            if (!IsAuthorizedLocalHelpAdmin(localhelp))
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            return View(localhelp);
        }

        //
        // GET: /LocalHelp/
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult Manage(int? orgId)
        {
            List<LocalHelp> localhelps;
            if (orgId == null)
            {
                if (User.IsInRole(LWSFRoles.admin) || User.IsInRole(LWSFRoles.localhelpAdmin))
                {
                    localhelps = db.LocalHelps.ToList();
                }
                else
                {
                    var currentVolunteer = GetCurrentVolunteer();
                    if (currentVolunteer != null)
                    {
                        orgId = currentVolunteer.OrganizationId;
                        localhelps = db.LocalHelps.Where(l => l.OrganizationId == orgId).ToList();
                    }
                    else
                    {
                        return RedirectToAction("UnauthorizedError", "Home", null);
                    }
                }
            }
            else
            {
                var currVol = GetCurrentVolunteer();
                if (currVol.OrganizationId != orgId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
                localhelps = db.LocalHelps.Where(l => l.OrganizationId == orgId).ToList();
            }
            ViewBag.orgId = orgId;
            return View(localhelps);
        }

        //
        // GET: /LocalHelp/Details/5
        public ViewResult LocalhelpDetails(int id)
        {
            LocalHelp localhelp = db.LocalHelps.Find(id);
            localhelp.Description = Server.HtmlDecode(localhelp.Description);

            return View(localhelp);
        }

        public ActionResult _ShowRegisterEntries(int localhelpId)
        {
            var localhelp = db.LocalHelps.Find(localhelpId);

            List<RegisterEntry> historicalregs = new List<RegisterEntry>();
            List<RegisterEntry> comingregs;
            bool isAutorizedAdmin = IsAuthorizedLocalHelpAdmin(localhelp);

            if (isAutorizedAdmin)
            {
                comingregs = localhelp.RegisterEntries.Where(r => r.Time > DateTime.Now).OrderBy(r => r.Time).ToList();
                historicalregs = localhelp.RegisterEntries.Where(r => r.Time <= DateTime.Now).OrderByDescending(r => r.Time).ToList();
            }
            else
            {
                comingregs = localhelp.RegisterEntries.Where(r => r.Time > DateTime.Now).OrderBy(r => r.Time).ToList();
            }

            int totalCount = 0;

            foreach (var reg in localhelp.RegisterEntries)
            {
                totalCount += (reg.GuestParticipants.Count + reg.NewStudents.Count);
            }

            ViewBag.isAutorizedAdmin = isAutorizedAdmin;
            ViewBag.comingregs = comingregs;
            ViewBag.historicalregs = historicalregs;
            ViewBag.currentStudent = GetCurrentNewStudent();
            ViewBag.localhelpId = localhelpId;
            ViewBag.totalCount = totalCount;

            return View();
        }

        private bool IsAuthorizedLocalHelpAdmin(LocalHelp localhelp)
        {
            if (localhelp == null)
            {
                return false;
            }
            if (!Request.IsAuthenticated)
            {
                return false;
            }
            if (User.IsInRole(LWSFRoles.admin) || User.IsInRole(LWSFRoles.localhelpAdmin))
            {
                return true;
            }
            if (User.IsInRole(LWSFRoles.organizationLeader))
            {
                var vol = GetCurrentVolunteer();
                if (vol.OrganizationId == localhelp.OrganizationId)
                {
                    return true;
                }
            }
            return false;
        }

        // GET: /LocalHelp/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult RegisterDetails(int id)
        {
            var registerEngry = db.RegisterEntries.Find(id);

            if (!IsAuthorizedLocalHelpAdmin(registerEngry.LocalHelp))
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            return View(registerEngry);
        }

        [Authorize(Roles = LWSFRoles.newStudent)]
        public ActionResult AttendLocalHelp(int Rid, int Sid, string returnUrl)
        {
            var s = db.NewStudents.Find(Sid);
            var registerEntry = db.RegisterEntries.Find(Rid);
            if (registerEntry != null && s != null)
            {
                registerEntry.NewStudents.Add(s);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin + ", " + LWSFRoles.newStudent)]
        public ActionResult CancelLocalHelp(int Rid, int Sid, string returnUrl)
        {
            var s = db.NewStudents.Find(Sid);
            var registerEntry = db.RegisterEntries.Find(Rid);
            if (registerEntry != null && s != null)
            {
                registerEntry.NewStudents.Remove(s);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult VolunteerOfferLocalHelp(int Lid, int Vid, string returnUrl)
        {
            var vol = db.Volunteers.Find(Vid);
            var localHelp = db.LocalHelps.Find(Lid);
            if (localHelp != null && vol != null)
            {
                localHelp.Volunteers.Add(vol);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin + ", " + LWSFRoles.volunteer)]
        public ActionResult VolunteerCancelLocalHelp(int Lid, int Vid, string returnUrl)
        {
            var vol = db.Volunteers.Find(Vid);
            var localHelp = db.LocalHelps.Find(Lid);
            if (localHelp != null && vol != null)
            {
                localHelp.Volunteers.Remove(vol);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        //
        // GET: /LocalHelp/Create

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult CreateLocalHelp(int? orgId)
        {
            var vol = GetCurrentVolunteer();
            if (vol == null)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            if (!User.IsInRole(LWSFRoles.admin) && !User.IsInRole(LWSFRoles.localhelpAdmin))
            {
                if (vol.OrganizationId != orgId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }

            var localhelp = new LocalHelp
            {
                OrganizationId = orgId,
                Restriction = LocalHelpRestriction.INSIDE_ORG_ONLY,
            };
            return View(localhelp);
        }

        //
        // POST: /LocalHelp/Create

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult CreateLocalHelp(LocalHelp localhelp)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(localhelp.Title))
                {
                    ModelState.AddModelError("", "活动名称不能为空！");
                    return View(localhelp);
                }
                localhelp.Description = Server.HtmlEncode(localhelp.Description);
                db.LocalHelps.Add(localhelp);
                db.SaveChanges();
                return RedirectToAction("CreateRegisterEntry", new { localhelpId = localhelp.Id });
            }

            return View(localhelp);
        }

        [HttpGet]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult CreateRegisterEntry(int localhelpId)
        {
            var localhelp = db.LocalHelps.Find(localhelpId);
            localhelp.Description = Server.HtmlDecode(localhelp.Description);
            ViewBag.localhelp = localhelp;
            var reg = new RegisterEntry
            {
                LocalHelpId = localhelpId,
            };
            return View(reg);
        }

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult CreateRegisterEntry(RegisterEntry reg)
        {
            if (ModelState.IsValid)
            {
                db.RegisterEntries.Add(reg);
                db.SaveChanges();
                return RedirectToAction("CreateRegisterEntry", new { localhelpId = reg.LocalHelpId });
            }
            var localhelp = db.LocalHelps.Find(reg.LocalHelpId);
            localhelp.Description = Server.HtmlDecode(localhelp.Description);
            ViewBag.localhelp = localhelp;
            return View(reg);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult OpenEvent(int id, string returnUrl)
        {
            var registerEntry = db.RegisterEntries.Find(id);
            registerEntry.IsActive = true;
            db.SaveChanges();
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Manage", new { orgId = registerEntry.LocalHelp.OrganizationId });
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult CloseEvent(int id, string returnUrl)
        {
            var registerEntry = db.RegisterEntries.Find(id);
            registerEntry.IsActive = false;
            db.SaveChanges();
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Manage", new { orgId = registerEntry.LocalHelp.OrganizationId });
        }

        //
        // GET: /LocalHelp/Edit/5

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult EditLocalHelp(int id)
        {
            LocalHelp localhelp = db.LocalHelps.Find(id);
            localhelp.Description = Server.HtmlDecode(localhelp.Description);
            return View(localhelp);
        }

        //
        // POST: /LocalHelp/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult EditLocalHelp(LocalHelp localhelp, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                localhelp.Description = Server.HtmlEncode(localhelp.Description);
                db.Entry(localhelp).State = EntityState.Modified;
                db.SaveChanges();
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "LocalHelp", null);
                }
            }
            return View(localhelp);
        }

        //
        // GET: /LocalHelp/Edit/5

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult EditRegisterEntry(int id)
        {
            RegisterEntry entry = db.RegisterEntries.Find(id);
            return View(entry);
        }

        //
        // POST: /LocalHelp/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult EditRegisterEntry(RegisterEntry registerEntry, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registerEntry).State = EntityState.Modified;
                db.SaveChanges();
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "LocalHelp");
                }
            }
            return View(registerEntry);
        }

        //
        // GET: /LocalHelp/Delete/5

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult DeleteLocalHelp(int id)
        {
            LocalHelp localhelp = db.LocalHelps.Find(id);
            localhelp.Description = Server.HtmlDecode(localhelp.Description);
            return View(localhelp);
        }

        //
        // POST: /LocalHelp/Delete/5

        [HttpPost, ActionName("DeleteLocalHelp")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult DeleteLocalHelpConfirmed(int id)
        {
            LocalHelp localhelp = db.LocalHelps.Find(id);
            foreach (var r in localhelp.RegisterEntries.ToList())
            {
                r.NewStudents.Clear();
                foreach (var g in r.GuestParticipants.ToList())
                {
                    db.GuestParticipants.Remove(g);
                }
                db.RegisterEntries.Remove(r);
            }
            db.LocalHelps.Remove(localhelp);
            db.SaveChanges();
            return RedirectToAction("Manage", new { orgId = localhelp.OrganizationId });
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult DeleteRegisterEntry(int id)
        {
            var entry = db.RegisterEntries.Find(id);

            if (!IsAuthorizedLocalHelpAdmin(entry.LocalHelp))
            {
                return Content("Failed: You are not authorized!");
            }

            entry.NewStudents.Clear();
            foreach (var g in entry.GuestParticipants.ToList())
            {
                db.GuestParticipants.Remove(g);
            }
            db.RegisterEntries.Remove(entry);
            db.SaveChanges();
            return Content("Removed Successfully!");
        }

        public ActionResult GuestRegister(int registerEntryId)
        {
            var r = db.RegisterEntries.Find(registerEntryId);
            ViewBag.registerEntry = r;

            GuestParticipant p = new GuestParticipant
            {
                RegisterEntryId = r.Id,
                IsChristian = false,
            };
            return View(p);
        }

        //
        // POST: /Participant/Create

        [HttpPost]
        public ActionResult GuestRegister(GuestParticipant participant)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(participant.Name) || String.IsNullOrEmpty(participant.Email) || String.IsNullOrEmpty(participant.Phone))
                {
                    ModelState.AddModelError("", "Failed: Name, Email and Phone are neccessary for registration, please fill them in");
                    ViewBag.registerEntry = db.RegisterEntries.Find(participant.RegisterEntryId);
                    return View(participant);
                }
                db.GuestParticipants.Add(participant);
                db.SaveChanges();
                return RedirectToAction("GuestRegisterSuccess", new { registerEntryId = participant.RegisterEntryId });
            }

            ViewBag.registerEntry = db.RegisterEntries.Find(participant.RegisterEntryId);
            return View(participant);
        }

        public ActionResult GuestRegisterSuccess(int registerEntryId)
        {
            var r = db.RegisterEntries.Find(registerEntryId);
            return View(r);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult DeleteGuest(int id)
        {
            GuestParticipant participant = db.GuestParticipants.Find(id);
            return View(participant);
        }

        //
        // POST: /Participant/Delete/5

        [HttpPost, ActionName("DeleteGuest")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult DeleteGuestConfirmed(int id, string returnUrl)
        {
            GuestParticipant participant = db.GuestParticipants.Find(id);
            db.GuestParticipants.Remove(participant);
            db.SaveChanges();
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private NewStudent GetCurrentNewStudent()
        {
            if (!Request.IsAuthenticated || !User.IsInRole(LWSFRoles.newStudent))
            {
                return null;
            }
            string username = User.Identity.Name;
            var currNewStudent = db.NewStudents.Where(v => v.UserName == username).FirstOrDefault();
            return currNewStudent;
        }

        private Volunteer GetCurrentVolunteer()
        {
            string userName = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.UserName == userName).SingleOrDefault();
            return currVolunteer;
        }

        /*private void SendLocalHelpEmailToGroup(LocalHelp localHelp, VolunteerGroup group, List<NewStudent> ss)
        {
            MailMessage email = new MailMessage();

            email.From = new MailAddress("utd.livingwatersf@gmail.com");
            email.To.Add(new MailAddress("Lasonxia@gmail.com"));
            email.CC.Add(new MailAddress("utd.livingwatersf@gmail.com"));

            email.Subject = "Local Help For 2012 Fall UTD New Student ";
            email.IsBodyHtml = true;

            string link = "www.livingwatersf.org/LocalHelp/GroupLocalHelpStudents?grpId=" + group.Id + "&Lid=" + localHelp.Id;

            email.Body = "<p>" + "Local Help For " + group.Name + "</p>";
            email.Body = "<p>The students that hosted by your cell group registered for local help</p>";
            email.Body += "<p>" + "Please click the following link to see the local help information: <a href='" + link + "'>" + "Local Help" + "</a></p>";
            email.Body += "<p>If you cannot open the hyper link, please follow the following address:</p>" + "<p>" + link + "</p>";
            email.Body += "<p>If your group cannot provide the help for them. Please reply to this email. If not reply, by default we think that you will provide the help. Thanks</p>";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.EnableSsl = true;  //must be here, otherwise there will be execption
            smtpClient.Send(email);

            email.Dispose();
        }*/

        private enum LocalHelpRegistration
        {
            NAME = 1,
            GENDER,
            MAJOR,
            EMAIL,
            PHONE,
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult ExportExcelLocalHelp(int registerEngryId)
        {
            var registerEngry = db.RegisterEntries.Find(registerEngryId);
            string filename = "Local Help" + "-" + registerEngry.Time.ToString("MM-dd-yyyy-HH-mm") + ".xlsx";

            var physicalPath = Path.Combine(Server.MapPath("~/Download/"), filename);
            FileInfo file = new FileInfo(physicalPath);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(physicalPath);
            }

            using (ExcelPackage xlPackage = new ExcelPackage(file))
            {
                ExcelWorksheet xlWorkSheet = xlPackage.Workbook.Worksheets.Add("Local Help Registration");
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.NAME).Value = "Name";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.GENDER).Value = "Gender";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.MAJOR).Value = "Major";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.EMAIL).Value = "Email";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.PHONE).Value = "Phone";

                int row = 2;
                foreach (var item in registerEngry.NewStudents)
                {
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.NAME).Value = item.CnName ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.GENDER).Value = SystemGender.ToStringGender(item.Gender);
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.MAJOR).Value = item.Major ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.EMAIL).Value = item.Email ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.PHONE).Value = item.Phone;
                    xlWorkSheet.Row(row).Style = (row % 2 == 0) ? "Good" : "Bad";
                    row++;
                }

                foreach (var item in registerEngry.GuestParticipants)
                {
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.NAME).Value = item.Name ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.GENDER).Value = " ";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.MAJOR).Value = " ";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.EMAIL).Value = item.Email ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.PHONE).Value = item.Phone;
                    xlWorkSheet.Row(row).Style = (row % 2 == 0) ? "Good" : "Bad";
                    row++;
                }

                xlWorkSheet.Column((int)LocalHelpRegistration.NAME).Width = 15;
                xlWorkSheet.Column((int)LocalHelpRegistration.GENDER).Width = 15;
                xlWorkSheet.Column((int)LocalHelpRegistration.MAJOR).Width = 20;
                xlWorkSheet.Column((int)LocalHelpRegistration.EMAIL).Width = 25;
                xlWorkSheet.Column((int)LocalHelpRegistration.PHONE).Width = 25;

                xlPackage.Save();
            }

            byte[] ByteFile = System.IO.File.ReadAllBytes(physicalPath);
            MemoryStream m = new MemoryStream(ByteFile);
            return File(m, "application/vnd.ms-excel", filename);
        }

        private enum VolunteerExcelFormat
        {
            NAME = 1,
            GENDER,
            EMAIL,
            PHONE,
            ADDRESS,
            RELATION,
            INTRO,
            COMMENT,
            ORGANIZATION,
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.localhelpAdmin)]
        public ActionResult _ExportVolunteerToExcel(int localhelpId)
        {
            var localhelp = db.LocalHelps.Find(localhelpId);
            List<Volunteer> volList = localhelp.Volunteers.ToList();
            string filename = "LocalHelp-Voulteers.xlsx";
            var physicalPath = Path.Combine(Server.MapPath("~/Download/"), filename);
            FileInfo file = new FileInfo(physicalPath);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(physicalPath);
            }

            using (ExcelPackage xlPackage = new ExcelPackage(file))
            {
                ExcelWorksheet xlWorkSheet = xlPackage.Workbook.Worksheets.Add("Volunteer List");
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.NAME).Value = "Name";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.GENDER).Value = "Gender";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.EMAIL).Value = "Email";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.PHONE).Value = "Phone";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.ADDRESS).Value = "Adress";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.INTRO).Value = "Brief Introduction";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.COMMENT).Value = "Comments";
                xlWorkSheet.Cell(1, (int)VolunteerExcelFormat.ORGANIZATION).Value = "Organization";

                int row = 2;
                foreach (var item in volList)
                {
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.NAME).Value = item.Name;
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.GENDER).Value = SystemGender.ToStringGender(item.Gender);
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.EMAIL).Value = item.Email;
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.PHONE).Value = item.Phone ?? "Not Provided";
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.ADDRESS).Value = Server.HtmlEncode(item.Address ?? "Not Provided");
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.INTRO).Value = Server.HtmlEncode(item.BriefIntro ?? "Not Provided");
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.COMMENT).Value = Server.HtmlEncode(item.Note ?? "Not Provided");
                    xlWorkSheet.Cell(row, (int)VolunteerExcelFormat.ORGANIZATION).Value = item.Organization.Name;
                    row++;
                }
                xlWorkSheet.Column((int)VolunteerExcelFormat.NAME).Width = 15;
                xlWorkSheet.Column((int)VolunteerExcelFormat.EMAIL).Width = 25;
                xlWorkSheet.Column((int)VolunteerExcelFormat.PHONE).Width = 20;
                xlWorkSheet.Column((int)VolunteerExcelFormat.INTRO).Width = 25;
                xlWorkSheet.Column((int)VolunteerExcelFormat.ADDRESS).Width = 25;
                xlWorkSheet.Column((int)VolunteerExcelFormat.COMMENT).Width = 25;
                xlWorkSheet.Column((int)VolunteerExcelFormat.ORGANIZATION).Width = 25;

                xlPackage.Save();
            }

            byte[] ByteFile = System.IO.File.ReadAllBytes(physicalPath);
            MemoryStream m = new MemoryStream(ByteFile);
            return File(m, "application/vnd.ms-excel", filename);
        }
    }
}