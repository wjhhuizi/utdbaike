using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;

namespace ccbs.Controllers
{
    using Telerik.Web.Mvc.UI;
    using Telerik.Web.Mvc;
    using System.Data;

    public class StudentMinistryController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();
        private WebModelContainer web_db = new WebModelContainer();

        //
        // GET: /NewStudent/

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult VolunteerHome()
        {
            var currVolunteer = GetCurrentVolunteer();

            if (currVolunteer == null)
            {
                return RedirectToAction("Create", "Volunteer", null);
            }

            var message = web_db.SMMessages.Find(1);
            if (message == null)
            {
                ViewBag.message = new SMMessage
                    {
                        Title = " ",
                        Description = " ",
                    };
            }
            else
            {
                ViewBag.message = message;
            }

            return View(currVolunteer);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult ShowOrgMembersInfo()
        {
            var currVolunteer = GetCurrentVolunteer();
            return View(currVolunteer.Organization);
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult EditShortMessage()
        {
            SMMessage message = web_db.SMMessages.Find(1);
            if (message == null)
            {
                message = new SMMessage
                {
                    Title = "Empty",
                    Description = "Empty",
                    PostDate = DateTime.Now
                };
                web_db.SMMessages.Add(message);
                web_db.SaveChanges();
            }
            return View(message);
        }


        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin)]
        [ValidateInput(false)]
        public ActionResult EditShortMessage(SMMessage message)
        {
            if (ModelState.IsValid)
            {
                string str;
                bool toAllNewStudents;
                bool toAllVolunteers;

                str = Request["toAllNewStudents"];
                if (str == "on")
                {
                    toAllNewStudents = true;
                    message.Type = SystemNoticeType.SetBit(message.Type, SystemNoticeType.NewStudentNotice);
                }
                else
                {
                    toAllNewStudents = false;
                }
                str = Request["toAllVolunteers"];
                if (str == "on")
                {
                    toAllVolunteers = true;
                    message.Type = SystemNoticeType.SetBit(message.Type, SystemNoticeType.VolunteerNotice);
                }
                else
                {
                    toAllVolunteers = false;
                }

                message.PostDate = DateTime.Now;
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Manage");
        }


        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult CancelPickup(int id)
        {
            var student = db.NewStudents.Find(id);
            if (student == null)
            {
                return Content("");
            }
            Volunteer vol = student.PickupVolunteer;
            if (vol == null)
            {
                return Content("");
            }
            vol.PickupNewStudents.Remove(student);
            db.SaveChanges();

            return Content("sucessfully canceled!");
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult CancelTempHousing(int id)
        {
            var student = db.NewStudents.Find(id);
            if (student == null)
            {
                return Content("");
            }
            Volunteer vol = student.TempHouseVolunteer;
            if (vol == null)
            {
                return Content("");
            }
            vol.TempHouseNewStudents.Remove(student);
            db.SaveChanges();

            return Content("sucessfully canceled!");
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult NewStudentList()
        {
            Volunteer currVol = GetCurrentVolunteer();
            List<NewStudentViewModel> studList = new List<NewStudentViewModel>();
            if (currVol.Organization.ModelType == OrgModelType.IntGroupless)
            {
                studList = NewStudentListOps._GetOrgImcompletedNewStudents(currVol.OrganizationId);
            }
            else if (currVol.GroupId != null)
            {
                studList = NewStudentListOps._GetGrpUnassignedNewStudents(currVol.GroupId.Value);
            }

            if (NewStudentListOps.IsOrgFacss(currVol.Organization.Name))
            {
                foreach (var stud in studList)
                {
                    stud.CnName = stud.Id.ToString();
                }
            }

            return View(studList);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult _NewStudentList()
        {
            Volunteer currVol = GetCurrentVolunteer();
            List<NewStudentViewModel> studList = new List<NewStudentViewModel>();
            if (currVol.Organization.ModelType == OrgModelType.IntGroupless)
            {
                studList = NewStudentListOps._GetOrgImcompletedNewStudents(currVol.OrganizationId);
            }
            else if (currVol.GroupId != null)
            {
                studList = NewStudentListOps._GetGrpUnassignedNewStudents(currVol.GroupId.Value);
            }

            if (NewStudentListOps.IsOrgFacss(currVol.Organization.Name))
            {
                foreach (var stud in studList)
                {
                    stud.CnName = stud.Id.ToString();
                }
            }

            return View(new GridModel { Data = studList });
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult AllNewStudentList()
        {
            Volunteer currVol = GetCurrentVolunteer();
            List<NewStudentViewModel> studList = new List<NewStudentViewModel>();
            if (currVol.Organization.ModelType == OrgModelType.IntGroupless)
            {
                studList = NewStudentListOps._GetNewStudentFromOrg(currVol.OrganizationId);
            }
            else if (currVol.GroupId != null)
            {
                studList = NewStudentListOps._GetNewStudentFromGrp(currVol.GroupId.Value);
            }

            if (NewStudentListOps.IsOrgFacss(currVol.Organization.Name))
            {
                foreach (var stud in studList)
                {
                    stud.CnName = stud.Id.ToString();
                }
            }

            return View(studList);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.volunteer)]
        public ActionResult _AllNewStudentList()
        {
            Volunteer currVol = GetCurrentVolunteer();
            List<NewStudentViewModel> studList = new List<NewStudentViewModel>();
            if (currVol.Organization.ModelType == OrgModelType.IntGroupless)
            {
                studList = NewStudentListOps._GetNewStudentFromOrg(currVol.OrganizationId);
            }
            else if (currVol.GroupId != null)
            {
                studList = NewStudentListOps._GetNewStudentFromGrp(currVol.GroupId.Value);
            }

            if (NewStudentListOps.IsOrgFacss(currVol.Organization.Name))
            {
                foreach (var stud in studList)
                {
                    stud.CnName = stud.Id.ToString();
                }
            }

            return View(new GridModel { Data = studList });
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public JsonResult PickupNewStudent(int Id)
        {
            string status = "";
            NewStudentViewModel student = null;

            var s = db.NewStudents.FirstOrDefault(o => o.Id == Id);

            var currVol = GetCurrentVolunteer();
            if ((s.Organization == null) || (currVol.OrganizationId != s.Organization.Id))
            {
                status = "out-group";
                goto exit;
            }

            if (currVol.Organization.ModelType == OrgModelType.IntGrouped)
            {
                if (s.Group == null || currVol.GroupId != s.Group.Id)
                {
                    status = "out-group";
                    goto exit;
                }
            }

            if (s.ManualAssignInfoes != null && s.ManualAssignInfoes.Count > 0)
            {
                var pickManualAssign = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).SingleOrDefault();
                if (pickManualAssign != null)
                {
                    status = "pickup-assigned";
                    goto exit;
                }
            }

            if (s.NeedPickup == false)
            {
                status = "pickup-no-need";
                goto exit;
            }
            else if (s.PickupVolunteer != null)
            {
                status = "pickup-assigned";
                goto exit;
            }

            currVol.PickupNewStudents.Add(s);
            db.SaveChanges();

            student = NewStudentListOps._GetOneNewStudent(Id);

        exit:
            return Json(new { status = status, student = student }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = LWSFRoles.volunteer)]
        public JsonResult TempHousingNewStudent(int Id)
        {
            string status = "";
            NewStudentViewModel student = null;
            var s = db.NewStudents.FirstOrDefault(o => o.Id == Id);

            var currVol = GetCurrentVolunteer();
            if (s.Organization == null || currVol.OrganizationId != s.Organization.Id)
            {
                status = "out-group";
                goto exit;
            }

            if (currVol.Organization.ModelType == OrgModelType.IntGrouped)
            {
                if (s.Group == null || currVol.GroupId != s.Group.Id)
                {
                    status = "out-group";
                    goto exit;
                }
            }

            if (s.ManualAssignInfoes != null && s.ManualAssignInfoes.Count > 0)
            {
                var housingManualAssign = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).SingleOrDefault();

                if (housingManualAssign != null)
                {
                    status = "housing-assigned";
                    goto exit;
                }
            }

            if (s.NeedTempHousing == false)
            {
                status = "housing-no-need";
                goto exit;
            }
            else if (s.TempHouseVolunteer != null)
            {
                status = "housing-assigned";
                goto exit;
            }

            if (currVol.Gender != SystemGender.Family)
            {
                if (currVol.Gender != s.Gender)
                {
                    status = "different-gender";
                    goto exit;
                }
            }

            currVol.TempHouseNewStudents.Add(s);
            db.SaveChanges();

            student = NewStudentListOps._GetOneNewStudent(Id);

        exit:
            return Json(new { status = status, student = student }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader + ", " + LWSFRoles.volunteer)]
        public ActionResult CurrVolunteerStatistics()
        {
            var currVolunteer = GetCurrentVolunteer();
            int orgId = currVolunteer.OrganizationId;

            var orgStatistics = OrganizationStatistics(orgId);
            var allStatistics = AllOrgnizationStatistics();

            ViewBag.orgStatistics = orgStatistics;
            ViewBag.allStatistics = allStatistics;
            ViewBag.org = currVolunteer.Organization;

            return View();
        }

        public List<StatisticTableView> OrganizationStatistics(int orgId)
        {
            var org = db.Organizations.Find(orgId);

            if (org.ModelType == OrgModelType.IntGroupless)
            {
                return null;
            }

            int pickupNum = 0, housingNum = 0;
            string name = "";
            var groupStatistics = new List<StatisticTableView>();

            foreach (var g in org.Groups)
            {
                name = g.Name;
                pickupNum = 0; housingNum = 0;
                if (g.NewStudents != null)
                {
                    foreach (var stud in g.NewStudents)
                    {
                        if (stud.PickupVolunteer != null)
                        {
                            pickupNum++;
                        }
                        if (stud.TempHouseVolunteer != null)
                        {
                            housingNum++;
                        }
                        if (stud.ManualAssignInfoes != null)
                        {
                            foreach (var manual in stud.ManualAssignInfoes)
                            {
                                if (manual.Type == ManualAssignType.IntPickup)
                                {
                                    pickupNum++;
                                }
                                else if (manual.Type == ManualAssignType.IntHousing)
                                {
                                    housingNum++;
                                }
                            }
                        }
                    }
                }
                groupStatistics.Add(new StatisticTableView
                {
                    Name = name,
                    TotalStud = (g.NewStudents != null) ? g.NewStudents.Count() : 0,
                    PickupNum = pickupNum,
                    HousingNum = housingNum,
                    Total = pickupNum + housingNum,
                });
            }

            return groupStatistics.OrderByDescending(p => p.Total).ToList();
        }

        public List<StatisticTableView> AllOrgnizationStatistics()
        {
            int pickupNum = 0, housingNum = 0;
            string name = "";
            var orgStatistics = new List<StatisticTableView>();

            foreach (var org in db.Organizations)
            {
                name = org.Name;
                pickupNum = 0; housingNum = 0;

                if (org.NewStudents != null)
                {
                    foreach (var stud in org.NewStudents)
                    {
                        if (stud.PickupVolunteer != null)
                        {
                            pickupNum++;
                        }
                        if (stud.TempHouseVolunteer != null)
                        {
                            housingNum++;
                        }
                        if (stud.ManualAssignInfoes != null)
                        {
                            foreach (var manual in stud.ManualAssignInfoes)
                            {
                                if (manual.Type == ManualAssignType.IntPickup)
                                {
                                    pickupNum++;
                                }
                                else if (manual.Type == ManualAssignType.IntHousing)
                                {
                                    housingNum++;
                                }
                            }
                        }
                    }
                }


                orgStatistics.Add(new StatisticTableView
                {
                    Name = name,
                    TotalStud = (org.NewStudents != null) ? org.NewStudents.Count() : 0,
                    PickupNum = pickupNum,
                    HousingNum = housingNum,
                    Total = pickupNum + housingNum,
                });
            }

            return orgStatistics.OrderByDescending(p => p.Total).ToList();
        }

        private Volunteer GetCurrentVolunteer()
        {
            string username = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.UserName == username).SingleOrDefault();
            return currVolunteer;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            web_db.Dispose();
            base.Dispose(disposing);
        }
    }
}
