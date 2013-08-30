using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.Web.Security;
using Telerik.Web.Mvc;
using System.IO;
using OfficeOpenXml;

namespace ccbs.Controllers
{
    public class VolunteerOrganizationController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /Default1/
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            return View(db.Organizations.ToList());
        }

        //
        // GET: /Default1/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Details(int id)
        {
            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (currVol.Organization.Id != id)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }
            Organization organization = db.Organizations.Find(id);
            ViewBag.orgId = id;
            return View(organization);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public JsonResult ShowOrgGroups(int orgId)
        {
            var groups = db.Organizations.Find(orgId).Groups.ToList().OrderBy(g => g.Name);
            ViewBag.orgId = orgId;
            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("_ShowOrgGroups", groups),
            });
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _ShowOrgGroups(int orgId)
        {
            var groups = db.Organizations.Find(orgId).Groups.ToList().OrderBy(g => g.Name);
            ViewBag.orgId = orgId;
            return View(groups);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public JsonResult ShowOrgVolunteers(int orgId)
        {
            var vols = db.Organizations.Find(orgId).Volunteers.ToList().OrderByDescending(v => v.RegTime);
            var volViews = VolunteerViewModel.GetVolunteerViews(vols);
            ViewBag.orgId = orgId;
            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("_ShowOrgVolunteers", volViews),
            });
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _ShowOrgVolunteers(int orgId)
        {
            var vols = db.Organizations.Find(orgId).Volunteers.ToList().OrderByDescending(v => v.RegTime);
            var volViews = VolunteerViewModel.GetVolunteerViews(vols);
            ViewBag.orgId = orgId;
            return View(volViews);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _OrgVolunteerList(int orgId)
        {
            var vols = db.Organizations.Find(orgId).Volunteers.ToList().OrderByDescending(v => v.RegTime);
            var volViews = VolunteerViewModel.GetVolunteerViews(vols);
            return View(new GridModel(volViews));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _SaveVolunteerAjaxEditing(int id)
        {
            var volunteer = db.Volunteers.Find(id);
            var volView = new VolunteerViewModel(volunteer);
            TryUpdateModel(volView);
            volView.UpdateVolunteerModel(volunteer);
            db.Entry(volunteer).State = EntityState.Modified;
            db.SaveChanges();
            return _OrgVolunteerList(volunteer.OrganizationId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _DeleteVolunteerAjaxEditing(int id)
        {
            var volunteer = db.Volunteers.Find(id);
            if (volunteer != null)
            {
                db.Volunteers.Remove(volunteer);
                db.SaveChanges();
                Membership.DeleteUser(volunteer.UserName);
            }

            //Rebind the grid
            return _OrgVolunteerList(volunteer.OrganizationId);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public JsonResult ShowOrgNewStudents(int orgId)
        {
            var newStudents = NewStudentListOps._GetNewStudentFromOrg(orgId);
            ViewBag.orgId = orgId;

            var org = db.Organizations.Find(orgId);
            string PartialViewHtml;

            if (org.ModelType == OrgModelType.IntGrouped)
            {
                ViewBag.DropDownList_VolunteerGroups = new SelectList(org.Groups, "Id", "Name");
                ViewBag.orgId = orgId;
                PartialViewHtml = RenderPartialViewToString("_ShowGroupedOrgNewStudents", newStudents);
            }
            else
            {
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem() { Text = ConfirmedStage.StringUnConfirmed, Value = ConfirmedStage.UnConfirmed.ToString(), Selected = false });
                items.Add(new SelectListItem() { Text = ConfirmedStage.StringInfoConfirmed, Value = ConfirmedStage.InfoConfirmed.ToString(), Selected = false });
                items.Add(new SelectListItem() { Text = ConfirmedStage.StringNeedConfirmed, Value = ConfirmedStage.NeedConfirmed.ToString(), Selected = false });
                items.Add(new SelectListItem() { Text = ConfirmedStage.StringAllConfirmed, Value = ConfirmedStage.AllConfirmed.ToString(), Selected = false });
                SelectList sl = new SelectList(items, "Value", "Text");
                ViewBag.DropDownList_ConfirmedStages = sl;
                PartialViewHtml = RenderPartialViewToString("_ShowUngroupedOrgNewStudents", newStudents);
            }
            return Json(new
            {
                Success = true,
                PartialViewHtml = PartialViewHtml,
            });
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _OrgNewStudentList(int orgId, int? grpId)
        {
            List<NewStudentViewModel> newStudents;
            if (grpId == null)
            {
                newStudents = NewStudentListOps._GetNewStudentFromOrg(orgId);
            }
            else if (grpId == -1)
            {
                newStudents = NewStudentListOps._GetOrgUnassignedNewStudents(orgId);
            }
            else if (grpId > 0)
            {
                newStudents = NewStudentListOps._GetNewStudentFromGrp(grpId.Value);
            }
            else
            {
                newStudents = NewStudentListOps._GetNewStudentFromOrg(orgId);
            }

            return View(new GridModel(newStudents));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult _SaveNewStudentAjaxEditing(int id)
        {
            var student = NewStudentListOps._GetOneNewStudent(id);
            TryUpdateModel(student);
            NewStudent s = db.NewStudents.Find(id);
            s = student.UpdateNewStudentModel(s);
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();
            return _OrgVolunteerList(s.Organization.Id);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult NewStudentsExportExcel(int orgId)
        {
            var org = db.Organizations.Find(orgId);
            var studs = NewStudentListOps._GetNewStudentFromOrg(orgId);
            string filename = org.Name + "_NewStudents_" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return new NewStudentController()._ExportNewStudentToExcel(studs, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public string SetNewStudentConfirmStage(int[] checkedRecords, int confirmstage)
        {
            int count = 0;
            checkedRecords = checkedRecords ?? new int[] { };
            var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id));

            foreach (var r in records)
            {
                r.Confirmed = confirmstage;
                count++;
            }
            db.SaveChanges();
            return count + " records changed to " + ConfirmedStage.ToString(confirmstage);
        }

        //
        // GET: /Default1/Create
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Default1/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                db.Organizations.Add(organization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organization);
        }

        //
        // GET: /Default1/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Edit(int id)
        {
            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (currVol.Organization.Id != id)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }
            Organization organization = db.Organizations.Find(id);
            return View(organization);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Edit(Organization organization, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organization).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect(returnUrl);
            }
            return View(organization);
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SetOrgLeader(int orgId)
        {
            Organization org = db.Organizations.Find(orgId);
            List<Volunteer> vols = org.Volunteers.ToList();

            if (org.OrgLeader != null)
            {
                ViewBag.LeaderId = new SelectList(vols, "Id", "Name", org.OrgLeader.Id);
            }
            else
            {
                ViewBag.LeaderId = new SelectList(vols, "Id", "Name", 0);
            }
            var model = new SetLeaderModel
            {
                Id = orgId,
            };
            return View(model);
        }

        // @Html.ActionLink("Update Profile", "SetGroupLeader", "VolunteerGroup", new { returnUrl = Request.RawUrl }, null)
        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SetOrgLeader(SetLeaderModel OrgLeader, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Volunteer vol = db.Volunteers.Find(OrgLeader.LeaderId);
                if (vol == null)
                {
                    return Redirect(returnUrl);
                }
                var org = vol.Organization;
                if (org.OrgLeader != null)
                {
                    Roles.RemoveUserFromRole(org.OrgLeader.UserName, LWSFRoles.organizationLeader);
                }
                org.OrgLeader = vol;
                vol.AdminOrg = org;
                db.SaveChanges();
                Roles.AddUserToRole(vol.UserName, LWSFRoles.organizationLeader);
            }
            return Redirect(returnUrl);
        }

        //
        // GET: /Default1/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Delete(int id)
        {
            Organization organization = db.Organizations.Find(id);

            return View(organization);
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            Organization organization = db.Organizations.Find(id);
            db.Organizations.Remove(organization);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private Volunteer GetCurrentVolunteer()
        {
            string username = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.UserName == username).SingleOrDefault();
            return currVolunteer;
        }
    }
}