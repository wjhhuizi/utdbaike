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

namespace ccbs.Controllers
{
    public class VolunteerGroupController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /VolunteerGroup/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult Details(int id)
        {
            Group volunteergroup = db.Groups.Find(id);
            var currVol = GetCurrentVolunteer();
            ViewBag.grpId = id;
            if (User.IsInRole(LWSFRoles.admin))
            {
                return View(volunteergroup);
            }
            if (User.IsInRole(LWSFRoles.organizationLeader))
            {
                if (currVol.Organization.Id != volunteergroup.OrganizationId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
                return View(volunteergroup);
            }
            if (User.IsInRole(LWSFRoles.groupLeader))
            {
                if (currVol.AdminGroup.Id != id)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
                return View(volunteergroup);
            }

            return View(volunteergroup);
        }

        [Authorize]
        public ActionResult AllGroupMembers(int groupId)
        {
            var group = db.Groups.Find(groupId);

            var currVol = GetCurrentVolunteer();

            if (!User.IsInRole(LWSFRoles.admin))
            {
                if (!User.IsInRole(LWSFRoles.organizationLeader))
                {
                    if (currVol.GroupId != group.Id)
                    {
                        return RedirectToAction("UnauthorizedError", "Home", null);
                    }
                }
                else
                {
                    if (currVol.Organization.Id != group.OrganizationId)
                    {
                        return RedirectToAction("UnauthorizedError", "Home", null);
                    }
                }
            }
            var volunteers = group.Volunteers.OrderBy(v => v.Name).ToList();
            return View(volunteers);
        }


        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public JsonResult ShowGrpVolunteers(int grpId)
        {
            var vols = db.Groups.Find(grpId).Volunteers.ToList().OrderByDescending(v => v.RegTime);
            var volViews = VolunteerViewModel.GetVolunteerViews(vols);
            ViewBag.grpId = grpId;
            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("_ShowGrpVolunteers", volViews),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult _ShowGrpVolunteers(int grpId)
        {
            var vols = db.Groups.Find(grpId).Volunteers.ToList().OrderByDescending(v => v.RegTime);
            var volViews = VolunteerViewModel.GetVolunteerViews(vols);
            ViewBag.grpId = grpId;
            return View(volViews);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult _GrpVolunteerList(int grpId)
        {
            var vols = db.Groups.Find(grpId).Volunteers.ToList().OrderByDescending(v => v.RegTime);
            var volViews = VolunteerViewModel.GetVolunteerViews(vols);
            return View(new GridModel(volViews));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult _SaveVolunteerAjaxEditing(int id)
        {
            var volunteer = db.Volunteers.Find(id);
            var volView = new VolunteerViewModel(volunteer);
            TryUpdateModel(volView);
            volView.UpdateVolunteerModel(volunteer);
            db.Entry(volunteer).State = EntityState.Modified;
            db.SaveChanges();
            return _GrpVolunteerList(volunteer.GroupId.Value);
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
            return _GrpVolunteerList(volunteer.GroupId.Value);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public JsonResult ShowGrpNewStudents(int grpId)
        {
            var newStudents = NewStudentListOps._GetNewStudentFromGrp(grpId);
            ViewBag.grpId = grpId;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = ConfirmedStage.StringUnConfirmed, Value = ConfirmedStage.UnConfirmed.ToString(), Selected = false });
            items.Add(new SelectListItem() { Text = ConfirmedStage.StringInfoConfirmed, Value = ConfirmedStage.InfoConfirmed.ToString(), Selected = false });
            items.Add(new SelectListItem() { Text = ConfirmedStage.StringNeedConfirmed, Value = ConfirmedStage.NeedConfirmed.ToString(), Selected = false });
            items.Add(new SelectListItem() { Text = ConfirmedStage.StringAllConfirmed, Value = ConfirmedStage.AllConfirmed.ToString(), Selected = false });
            SelectList sl = new SelectList(items, "Value", "Text");

            ViewBag.DropDownList_ConfirmedStages = sl;
            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("_ShowGrpNewStudents", newStudents),
            }, JsonRequestBehavior.AllowGet);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult _GrpNewStudentList(int grpId)
        {
            var newStudents = NewStudentListOps._GetNewStudentFromGrp(grpId);
            return View(new GridModel(newStudents));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult _SaveNewStudentAjaxEditing(int id)
        {
            var student = NewStudentListOps._GetOneNewStudent(id);
            TryUpdateModel(student);
            NewStudent s = db.NewStudents.Find(id);
            s = student.UpdateNewStudentModel(s);
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();
            return _GrpNewStudentList(s.Group.Id);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult NewStudentsExportExcel(int grpId)
        {
            var grp = db.Groups.Find(grpId);
            var studs = NewStudentListOps._GetNewStudentFromGrp(grpId);
            string filename = grp.Name + "_NewStudents_" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return new NewStudentController()._ExportNewStudentToExcel(studs, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
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


        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public JsonResult GrpSelectNewStudents(int orgId)
        {
            var currVol = GetCurrentVolunteer();
            var newStudents = NewStudentListOps._GetOrgUnassignedNewStudents(orgId);
            ViewBag.orgId = orgId;
            ViewBag.grpId = currVol.GroupId;

            return Json(new
            {
                Success = true,
                PartialViewHtml = RenderPartialViewToString("_GrpSelectNewStudents", newStudents),
            }, JsonRequestBehavior.AllowGet);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult _GrpSelectNewStudents(int orgId)
        {
            ViewBag.orgId = orgId;
            var newStudents = NewStudentListOps._GetOrgUnassignedNewStudents(orgId);
            return View(new GridModel(newStudents));
        }

        //
        // GET: /VolunteerGroup/Create
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create()
        {
            ViewBag.OrgnizationId = new SelectList(db.Organizations, "Id", "Name");
            return View();
        }

        //
        // POST: /VolunteerGroup/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create(Group volunteergroup)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(volunteergroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OrgnizationId = new SelectList(db.Organizations, "Id", "Name", volunteergroup.OrganizationId);
            return View(volunteergroup);
        }

        //
        // GET: /VolunteerGroup/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult OrgCreate(int orgId)
        {
            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (currVol.Organization.Id != orgId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }
            var group = new Group
            {
                OrganizationId = orgId,
            };
            return View(group);
        }

        //
        // POST: /VolunteerGroup/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult OrgCreate(Group volunteergroup)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(volunteergroup);
                db.SaveChanges();
                return RedirectToAction("Details", "VolunteerOrganization", new { id = volunteergroup.OrganizationId });
            }
            return View(volunteergroup);
        }

        //
        // GET: /VolunteerGroup/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult Edit(int id)
        {
            Group volunteergroup = db.Groups.Find(id);

            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (User.IsInRole(LWSFRoles.organizationLeader))
                {
                    if (currVol.Organization.Id != volunteergroup.OrganizationId)
                    {
                        return RedirectToAction("UnauthorizedError", "Home", null);
                    }
                }
                else
                {
                    if (currVol.AdminGroup.Id != id)
                    {
                        return RedirectToAction("UnauthorizedError", "Home", null);
                    }
                }
            }
            return View(volunteergroup);
        }

        //
        // POST: /VolunteerGroup/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public ActionResult Edit(Group volunteergroup, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(volunteergroup).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect(returnUrl);
            }
            return View(volunteergroup);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult SetGroupLeader(int grpId)
        {
            Group volunteergroup = db.Groups.Find(grpId);

            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (currVol.Organization.Id != volunteergroup.OrganizationId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }

            if (volunteergroup.Leader != null)
            {
                ViewBag.LeaderId = new SelectList(volunteergroup.Volunteers, "Id", "Name", volunteergroup.Leader.Id);
            }
            else
            {
                ViewBag.LeaderId = new SelectList(volunteergroup.Volunteers, "Id", "Name", 0);
            }
            var model = new SetLeaderModel
                {
                    Id = grpId,
                };
            return View(model);
        }

        // @Html.ActionLink("Update Profile", "SetGroupLeader", "VolunteerGroup", new { returnUrl = Request.RawUrl }, null)
        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult SetGroupLeader(SetLeaderModel GroupLeader, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Volunteer vol = db.Volunteers.Find(GroupLeader.LeaderId);
                if (vol == null)
                {
                    return Redirect(returnUrl);
                }
                var grp = vol.Group;
                if (grp.Leader != null)
                {
                    if (Roles.GetRolesForUser(grp.Leader.UserName).Contains(LWSFRoles.groupLeader))
                    {
                        Roles.RemoveUserFromRole(grp.Leader.UserName, LWSFRoles.groupLeader);
                    }
                }
                grp.Leader = vol;
                vol.AdminGroup = grp;
                db.SaveChanges();
                Roles.AddUserToRole(vol.UserName, LWSFRoles.groupLeader);
            }
            return Redirect(returnUrl);
        }

        //
        // GET: /VolunteerGroup/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult Delete(int id)
        {
            Group volunteergroup = db.Groups.Find(id);

            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();
                if (currVol.Organization.Id != volunteergroup.OrganizationId)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }
            return View(volunteergroup);
        }

        //
        // POST: /VolunteerGroup/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult DeleteConfirmed(int id, string returnUrl)
        {
            Group volunteergroup = db.Groups.Find(id);

            foreach (var vol in volunteergroup.Volunteers.ToList())
            {
                volunteergroup.Volunteers.Remove(vol);
            }

            foreach (var stud in volunteergroup.NewStudents.ToList())
            {
                volunteergroup.NewStudents.Remove(stud);
            }

            db.Groups.Remove(volunteergroup);
            db.SaveChanges();
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private Volunteer GetCurrentVolunteer()
        {
            string userName = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.UserName == userName).SingleOrDefault();
            return currVolunteer;
        }
    }
}