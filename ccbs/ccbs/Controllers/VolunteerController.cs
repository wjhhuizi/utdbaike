using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.Web.Security;
using System.IO;
using OfficeOpenXml;

namespace ccbs.Controllers
{
    public class VolunteerController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /Volunteer/
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            var organizations = db.Organizations;
            return View(organizations.ToList());
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SetOrgLeader(int id)
        {
            Volunteer vol = db.Volunteers.Find(id);
            Roles.AddUserToRole(vol.UserName, LWSFRoles.organizationLeader);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult ViewDetails(int volId)
        {
            var vol = db.Volunteers.Find(volId);
            var currVol = GetCurrentVolunteer();
            bool showCancel = false;
            if (User.IsInRole(LWSFRoles.admin) || User.IsInRole(LWSFRoles.organizationLeader) || User.IsInRole(LWSFRoles.groupLeader))
            {
                showCancel = true;
            }
            if (vol.Id == currVol.Id)
            {
                showCancel = true;
            }
            ViewBag.showCancel = showCancel;
            return View("Details", vol);
        }

        [Authorize]
        public JsonResult _ViewDetails(int volId)
        {
            var vol = db.Volunteers.Find(volId);
            var currVol = GetCurrentVolunteer();
            bool showCancel = false;
            if (User.IsInRole(LWSFRoles.admin) || User.IsInRole(LWSFRoles.organizationLeader) || User.IsInRole(LWSFRoles.groupLeader))
            {
                showCancel = true;
            }
            if (vol.Id == currVol.Id)
            {
                showCancel = true;
            }
            ViewBag.showCancel = showCancel;
            return Json(new
            {
                PartialViewHtml = RenderPartialViewToString("Details", vol),
            });
        }

        //
        // GET: /Volunteer/Create
        [Authorize]
        public ActionResult Create()
        {
            var vol = new VolunteerInfoModel
            {
                UserName = User.Identity.Name,
                RegTime = DateTime.Now,
            };
            ViewBag.VolunteerOrganizationId = new SelectList(db.Organizations, "Id", "Name");
            return View(vol);
        }

        //
        // POST: /Volunteer/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(VolunteerInfoModel volunteer)
        {
            if (ModelState.IsValid)
            {
                var org = db.Organizations.Find(volunteer.VolunteerOrganizationId);

                if ((org.Groups != null) && (org.Groups.Count > 0))
                {
                    if (volunteer.VolunteerGroupId < 1)
                    {
                        ModelState.AddModelError("", "您所在的志愿者机构采用分组的方式，请选择您所在的小组");
                        return View(volunteer);
                    }
                    else
                    {
                        var group = db.Groups.Find(volunteer.VolunteerGroupId);
                        if (group.Passcode != volunteer.OrgPasscode)
                        {
                            ModelState.AddModelError("", "Wrong access passcode! If you don't know, please ask your group leader");
                            return View(volunteer);
                        }
                    }
                }
                else
                {
                    if (org.Passcode != volunteer.OrgPasscode)
                    {
                        ModelState.AddModelError("", "Wrong access passcode! If you don't know, please ask your organization coordinator");
                        return View(volunteer);
                    }
                }

                var vol = volunteer.GetVolunteerModel();
                vol.UserName = User.Identity.Name;
                db.Volunteers.Add(vol);
                db.SaveChanges();
                return RedirectToAction("VolunteerHome", "StudentMinistry");
            }
            ViewBag.VolunteerOrganizationId = new SelectList(db.Organizations, "Id", "Name");
            return View(volunteer);
        }

        //
        // GET: /Volunteer/Create

        public ActionResult VolunteerRegister()
        {
            ViewBag.VolunteerOrganizationId = new SelectList(db.Organizations, "Id", "Name");
            return View();
        }

        //
        // POST: /Volunteer/Create
        // @Html.ActionLink("Create New", "create", "Volunteer", new { returnUrl = Request.RawUrl }, null)

        [HttpPost]
        public ActionResult VolunteerRegister(VolunteerRegisterModel volunteerRegister)
        {
            MembershipCreateStatus createStatus;

            ViewBag.VolunteerOrganizationId = new SelectList(db.Organizations, "Id", "Name");
            ////ViewBag.Gender = new SelectList(new List<string>{"male", "female"});

            if (ModelState.IsValid)
            {
                var org = db.Organizations.Find(volunteerRegister.VolunteerOrganizationId);

                if ((org.Groups != null) && (org.Groups.Count > 0))
                {
                    if (volunteerRegister.VolunteerGroupId < 1)
                    {
                        ModelState.AddModelError("", "您所在的志愿者机构采用分组的方式，请选择您所在的小组");
                        return View(volunteerRegister);
                    }
                    else
                    {
                        var group = db.Groups.Find(volunteerRegister.VolunteerGroupId);
                        if (group.Passcode != volunteerRegister.OrgPasscode)
                        {
                            ModelState.AddModelError("", "Wrong access passcode! If you don't know, please ask your group leader");
                            return View(volunteerRegister);
                        }
                    }
                }
                else
                {
                    if (org.Passcode != volunteerRegister.OrgPasscode)
                    {
                        ModelState.AddModelError("", "Wrong access passcode! If you don't know, please ask your organization coordinator");
                        return View(volunteerRegister);
                    }
                }

                if (AccountController.RegisterNew(volunteerRegister.GetRegisterModel(), out createStatus) == false)
                {
                    ModelState.AddModelError("", AccountController.ErrorCodeToString(createStatus));
                    return View(volunteerRegister);
                }
                Roles.AddUserToRole(volunteerRegister.UserName, LWSFRoles.volunteer);

                db.Volunteers.Add(volunteerRegister.GetVolunteerModel());
                db.SaveChanges();

                return RedirectToAction("MyHomePage", "Account");

            }

            return View(volunteerRegister);
        }

        //
        // GET: /Volunteer/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult Edit(int id)
        {
            Volunteer volunteer = db.Volunteers.Find(id);

            var volunteerInfoModel = new VolunteerInfoModel(volunteer);

            return View(volunteerInfoModel);
        }

        //
        // POST: /Volunteer/Edit/5

        // @Html.ActionLink("Update Profile", "Edit", "Volunteer", new { returnUrl = Request.RawUrl }, null)

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult Edit(VolunteerInfoModel volunteerInfoModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var vol = db.Volunteers.Find(volunteerInfoModel.Id);
                volunteerInfoModel.UpdateVolunteerModel(vol);
                db.SaveChanges();

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("VolunteerHome", "StudentMinistry");
                }
            }
            return View(volunteerInfoModel);
        }

        [HttpPost]
        public JsonResult _GetDropDownListGroups(int? VolunteerOrganizationId)
        {
            return _GetGroups(VolunteerOrganizationId);
        }

        [HttpPost]
        public JsonResult _GetDropDownListVolunteers(int? DropDownList_Groups)
        {
            return _GetVolunteers(DropDownList_Groups);
        }

        //
        // GET: /Volunteer/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Delete(int id)
        {
            Volunteer volunteer = db.Volunteers.Find(id);

            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (User.IsInRole(LWSFRoles.organizationLeader))
                {
                    if (currVol.Organization.Id != volunteer.OrganizationId)
                    {
                        return RedirectToAction("UnauthorizedError", "Home", null);
                    }
                }
            }

            return View(volunteer);
        }

        //
        // POST: /Volunteer/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Volunteer volunteer = db.Volunteers.Find(id);
            string userName = User.Identity.Name;
            if (userName == volunteer.UserName)
            {
                return Redirect(returnUrl);
            }
            ClearVolunteer(volunteer);
            db.Volunteers.Remove(volunteer);
            db.SaveChanges();
            AccountController.DeleterUser(volunteer.UserName);
            return Redirect(returnUrl);
        }

        private void ClearVolunteer(Volunteer vol)
        {
            if (vol == null)
            {
                return;
            }

            if (vol.PickupNewStudents != null)
            {
                vol.PickupNewStudents.Clear();
            }
            if (vol.TempHouseNewStudents != null)
            {
                vol.TempHouseNewStudents.Clear();
            }
            if (vol.AdminOrg != null)
            {
                Roles.RemoveUserFromRole(vol.UserName, LWSFRoles.organizationLeader);
                vol.AdminOrg.OrgLeader = null;
                vol.AdminOrg = null;
            }
            return;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private JsonResult _GetGroups(int? Id)
        {
            var groups = db.Groups.AsQueryable<Group>();
            if (Id.HasValue)
            {
                groups = groups.Where(g => g.OrganizationId == Id.Value);
            }
            return Json(new SelectList(groups, "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        private JsonResult _GetVolunteers(int? Id)
        {
            IList<Volunteer> volunteers = new List<Volunteer>();
            if (Id.HasValue)
            {
                volunteers = db.Volunteers.Where(v => v.OrganizationId == Id.Value).ToList();
            }
            return Json(new SelectList(volunteers, "Id", "Name"), JsonRequestBehavior.AllowGet);
        }

        private Volunteer GetCurrentVolunteer()
        {
            string userName = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.UserName == userName).SingleOrDefault();
            return currVolunteer;
        }


        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult OrgVolunteersExportExcel(int orgId)
        {
            var org = db.Organizations.Find(orgId);
            var vols = org.Volunteers.ToList().OrderBy(v => v.Name).ToList();
            string filename = org.Name + "_Voluteers_" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportVolunteerToExcel(vols, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult GrpVolunteersExportExcel(int grpId)
        {
            var grp = db.Groups.Find(grpId);
            var vols = grp.Volunteers.ToList().OrderBy(v => v.Name).ToList();
            string filename = grp.Name + "_Voluteers_" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportVolunteerToExcel(vols, filename);
        }

        private enum ExcelFormat
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

        public ActionResult _ExportVolunteerToExcel(List<Volunteer> studList, string filename)
        {
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
                xlWorkSheet.Cell(1, (int)ExcelFormat.NAME).Value = "Name";
                xlWorkSheet.Cell(1, (int)ExcelFormat.GENDER).Value = "Gender";
                xlWorkSheet.Cell(1, (int)ExcelFormat.EMAIL).Value = "Email";
                xlWorkSheet.Cell(1, (int)ExcelFormat.PHONE).Value = "Phone";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ADDRESS).Value = "Adress";
                xlWorkSheet.Cell(1, (int)ExcelFormat.INTRO).Value = "Brief Introduction";
                xlWorkSheet.Cell(1, (int)ExcelFormat.COMMENT).Value = "Comments";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ORGANIZATION).Value = "Organization";

                int row = 2;
                foreach (var item in studList)
                {
                    xlWorkSheet.Cell(row, (int)ExcelFormat.NAME).Value = item.Name;
                    xlWorkSheet.Cell(row, (int)ExcelFormat.GENDER).Value = SystemGender.ToStringGender(item.Gender);
                    xlWorkSheet.Cell(row, (int)ExcelFormat.EMAIL).Value = item.Email;
                    xlWorkSheet.Cell(row, (int)ExcelFormat.PHONE).Value = item.Phone ?? "Not Provided";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.ADDRESS).Value = Server.HtmlEncode(item.Address ?? "Not Provided");
                    xlWorkSheet.Cell(row, (int)ExcelFormat.INTRO).Value = Server.HtmlEncode(item.BriefIntro ?? "Not Provided");
                    xlWorkSheet.Cell(row, (int)ExcelFormat.COMMENT).Value = Server.HtmlEncode(item.Note ?? "Not Provided");
                    xlWorkSheet.Cell(row, (int)ExcelFormat.ORGANIZATION).Value = item.Organization.Name;
                    row++;
                }
                xlWorkSheet.Column((int)ExcelFormat.NAME).Width = 15;
                xlWorkSheet.Column((int)ExcelFormat.EMAIL).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.PHONE).Width = 20;
                xlWorkSheet.Column((int)ExcelFormat.INTRO).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.ADDRESS).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.COMMENT).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.ORGANIZATION).Width = 25;

                xlPackage.Save();
            }

            byte[] ByteFile = System.IO.File.ReadAllBytes(physicalPath);
            MemoryStream m = new MemoryStream(ByteFile);
            return File(m, "application/vnd.ms-excel", filename);
        }
    }
}