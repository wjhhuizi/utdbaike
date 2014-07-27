using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.Extensions;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Web.Security;
using System.Net.Mail;
using utdbaike;

namespace ccbs.Controllers
{
    public class NewStudentController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();
        private WebModelContainer web_db = new WebModelContainer();
        //

        // GET: /NewStudent/
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ViewResult Index()
        {
            var students = NewStudentListOps._GetAllNewStudentList();
            ViewBag.DropDownList_Organizations = new SelectList(db.Organizations, "Id", "Name");
            return View(students);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult _Index(int orgId)
        {
            List<NewStudentViewModel> students;
            if (orgId == 0)
            {
                students = NewStudentListOps._GetAllNewStudentList();
            }
            else if (orgId == -1)
            {
                students = NewStudentListOps._GetAllUnasignedNewStudent();
            }
            else if (orgId == -2)
            {
                students = NewStudentListOps._GetNoNeedNewStudent();
            }
            else
            {
                students = NewStudentListOps._GetNewStudentFromOrg(orgId);
            }
            return View(new GridModel(students));
        }

        public ActionResult ShowOperationRecords()
        {
            int i, id;
            var records = db.OperationRecords
                .Where(o => (o.Type == NewStudentOperation.ASSIGN_TO_ORG
                    || o.Type == NewStudentOperation.RETURN_FROM_ORG))
                .OrderByDescending(o => o.CreatedDate);

            List<NewStudent> studentList = new List<NewStudent>();

            foreach (var record in records)
            {
                if (!String.IsNullOrEmpty(record.Link))
                {
                    continue;
                }
                var studentIds = record.Data.Split(',');
                for (i = 0; i < studentIds.Length; i++)
                {
                    if (Int32.TryParse(studentIds[i], out id))
                    {
                        var student = db.NewStudents.Find(id);
                        if (student != null)
                        {
                            studentList.Add(student);
                        }
                    }
                }
                if (studentList.Count > 0)
                {
                    String filename = "NewStudent-" + record.CreatedDate.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
                    var physicalPath = SaveToExcel(NewStudentListOps._ConvertStudentList(studentList), filename);
                    var virtualPath = GetVirtualPath(physicalPath);
                    record.Link = Url.Content(virtualPath);
                }
            }
            db.SaveChanges();
            return View(records.ToList());
        }

        public ActionResult ShowOrgOperationRecords(int orgId)
        {
            int i, id;
            var records = db.OperationRecords
                .Where(o => (o.Type == NewStudentOperation.ASSIGN_TO_ORG
                    || o.Type == NewStudentOperation.RETURN_FROM_ORG) && (o.Arg1 == orgId))
                .OrderByDescending(o => o.CreatedDate);

            List<NewStudent> studentList = new List<NewStudent>();

            foreach (var record in records)
            {
                if (!String.IsNullOrEmpty(record.Link))
                {
                    continue;
                }
                var studentIds = record.Data.Split(',');
                for (i = 0; i < studentIds.Length; i++)
                {
                    if (Int32.TryParse(studentIds[i], out id))
                    {
                        var student = db.NewStudents.Find(id);
                        if (student != null)
                        {
                            studentList.Add(student);
                        }
                    }
                }
                if (studentList.Count > 0)
                {
                    String filename = "NewStudent-" + record.CreatedDate.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
                    var physicalPath = SaveToExcel(NewStudentListOps._ConvertStudentList(studentList), filename);
                    var virtualPath = GetVirtualPath(physicalPath);
                    record.Link = Url.Content(virtualPath);
                }
            }
            db.SaveChanges();
            return View("ShowOperationRecords", records.ToList());
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult SystemManage()
        {
            var students = NewStudentListOps._GetAllNewStudentList();
            ViewBag.DropDownList_Organizations = new SelectList(db.Organizations, "Id", "Name");
            return View(students);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult _SystemManage(int orgId)
        {
            List<NewStudentViewModel> students;
            if (orgId == 0)
            {
                students = NewStudentListOps._GetAllNewStudentList();
            }
            else if (orgId == -1)
            {
                students = NewStudentListOps._GetAllUnasignedNewStudent();
            }
            else
            {
                students = NewStudentListOps._GetNewStudentFromOrg(orgId);
            }
            return View(new GridModel(students));
        }

        [HttpPost]
        public ActionResult _GetOrganizations()
        {
            IQueryable<Organization> orgs = db.Organizations;



            var result = new List<Organization>();

            result.Add(
                new Organization
                {
                    Id = 0,
                    Name = "系统所有新生"
                });
            result.Add(
                new Organization
                {
                    Id = -1,
                    Name = "未分配的新生"
                });
            result.Add(
                new Organization
                {
                    Id = -2,
                    Name = "不需要接机和临住的新生"
                });

            result = result.Concat(orgs.ToList()).ToList();

            var selectList = new SelectList(result, "Id", "Name");

            return new JsonResult
            {
                Data = selectList
            };
        }

        [HttpPost]
        public ActionResult _GetGroups(int orgId)
        {
            var org = db.Organizations.Find(orgId);

            var result = new List<Group>();

            result.Add(
                new Group
                {
                    Id = 0,
                    Name = "所有的新生"
                });
            result.Add(
                new Group
                {
                    Id = -1,
                    Name = "未分配到小组的新生"
                });

            result = result.Concat(org.Groups.ToList()).ToList();

            var selectList = new SelectList(result, "Id", "Name");

            return new JsonResult
            {
                Data = selectList
            };
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin, Users = "admin, admin@livingwatersf.org")]
        public ActionResult ExportExcelForSystemOrg(int orgId)
        {
            if (orgId == -1)
            {
                return ExportExcelAllUnassigned();
            }
            else if (orgId == 0)
            {
                return ExportExcelForAll();
            }
            else
            {
                return ExportExcelOrg(orgId);
            }
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.organizationLeader)]
        public ActionResult ExportExcelForSystemGrp(int orgId, int grpId)
        {
            if (grpId == -1)
            {
                return ExportExcelOrgUnassigned(orgId);
            }
            else if (grpId == 0)
            {
                return ExportExcelOrg(orgId);
            }
            else
            {
                return ExportExcelGrp(grpId);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin, Users = "admin, admin@livingwatersf.org")]
        public ActionResult _SaveAjaxEditing(int id)
        {
            var student = NewStudentListOps._GetOneNewStudent(id);
            TryUpdateModel(student);
            NewStudent s = db.NewStudents.Find(id);
            s = student.UpdateNewStudentModel(s);
            db.Entry(s).State = EntityState.Modified;
            db.SaveChanges();
            return View(new GridModel(NewStudentListOps._GetAllNewStudentList()));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin, Users = "admin, admin@livingwatersf.org")]
        public ActionResult _DeleteAjaxEditing(int id)
        {
            var student = db.NewStudents.Find(id);
            if (student != null)
            {
                delete_new_student(id);
                Membership.DeleteUser(student.UserName);
            }

            //Rebind the grid
            return View(new GridModel(NewStudentListOps._GetAllNewStudentList()));
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin, Users = "admin, admin@livingwatersf.org")]
        public ActionResult ExportExcelForAll()
        {
            var students = NewStudentListOps._GetAllNewStudentList();
            string filename = "New-Student-Registration-Form-ALL" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportNewStudentToExcel(students, filename);
        }

        public ActionResult RemoveExcel(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            //foreach (var fullName in fileNames)
            //{
            //    var fileName = Path.GetFileName(fullName);
            //    var physicalPath = Path.Combine(Server.MapPath("~/Upload/AdminUploadedSongs"), fileName);

            //    // TODO: Verify user permissions
            //    if (System.IO.File.Exists(physicalPath))
            //    {
            //        // The files are not actually removed in this demo
            //        System.IO.File.Delete(physicalPath);
            //    }
            //}
            // Return an empty string to signify success
            return Content("");
        }
        //
        // GET: /NewStudent/Details/5

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ViewResult AllUnassignedNewStudentList()
        {
            var students = NewStudentListOps._GetAllUnasignedNewStudent();
            return View(students);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + "," + LWSFRoles.organizationLeader)]
        public ActionResult _AllUnassignedNewStudentList()
        {
            var students = NewStudentListOps._GetAllUnasignedNewStudent();
            return View(new GridModel(students));
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ShowTempPool()
        {
            ViewBag.TempPool_DropDownList_Organizations = new SelectList(db.Organizations, "Id", "Name");
            return View();
        }

        [GridAction]
        public ActionResult _ShowTempPool()
        {
            var username = User.Identity.Name;
            var pool = db.TempPools.Where(t => t.UserName == username).FirstOrDefault();
            if (pool == null)
            {
                pool = new TempPool
                {
                    UserName = username,
                    LastUpdate = DateTime.Now,
                };
                db.TempPools.Add(pool);
            }
            return View(new GridModel(NewStudentListOps._ConvertStudentList(pool.NewStudents.ToList())));
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        [GridAction]
        public ActionResult _RemoveFromTempPool(int id)
        {
            var username = User.Identity.Name;
            var pool = db.TempPools.Where(t => t.UserName == username).FirstOrDefault();

            var stud = db.NewStudents.Find(id);

            pool.NewStudents.Remove(stud);
            db.SaveChanges();

            return View(new GridModel(NewStudentListOps._ConvertStudentList(pool.NewStudents.ToList())));
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public String SelectTempRecords(int[] checkedRecords)
        {
            int count = 0;
            int i = 0;
            checkedRecords = checkedRecords ?? new int[] { };

            var username = User.Identity.Name;
            var pool = db.TempPools.Where(t => t.UserName == username).FirstOrDefault();
            if (pool == null)
            {
                pool = new TempPool
                {
                    UserName = username,
                    LastUpdate = DateTime.Now,
                };
                db.TempPools.Add(pool);
            }

            for (i = 0; i < checkedRecords.Length; i++)
            {
                var s = db.NewStudents.Find(checkedRecords[i]);
                if ((s != null) && (s.Organization == null) && (s.TempPool == null))
                {
                    pool.NewStudents.Add(s);
                    count++;
                }
            }
            db.SaveChanges();

            String output = count + "新生added to cart";

            return output;
        }

        //[Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        //public string RemoveSelectedTempRecords(int[] checkedRecords)
        //{
        //    int count = 0;
        //    checkedRecords = checkedRecords ?? new int[] { };

        //    var pool = db.TempPools.FirstOrDefault();

        //    var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id));
        //    foreach (var r in records)
        //    {
        //        pool.NewStudents.Remove(r);
        //        count++;
        //    }
        //    db.SaveChanges();

        //    String output = count + "新生Removed From Temp pool";
        //    return output;
        //}

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public String TempPoolToOrg(int org)
        {
            int count = 0;
            var username = User.Identity.Name;
            var pool = db.TempPools.Where(t => t.UserName == username).FirstOrDefault();
            var organization = db.Organizations.Find(org);

            var tempList = pool.NewStudents.ToList();
            var records = new List<NewStudent>();
            string studentIds = "";
            foreach (var s in tempList)
            {
                if ((s != null) && (s.Organization == null))
                {
                    studentIds += (s.Id.ToString() + ",");
                    records.Add(s);
                    organization.NewStudents.Add(s);
                    count++;
                }
                pool.NewStudents.Remove(s);
            }
            if (count > 0)
            {
                string description = "系统分配" + count + "个新生到机构:" + organization.Name;
                var operationRecord = NewStudentOperation.AssignToOrgRecord(org, studentIds, description);
                db.OperationRecords.Add(operationRecord);
            }
            db.SaveChanges();

            String filename = organization.Name + "_" + count.ToString() + "_Students" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            var physicalPath = SaveToExcel(NewStudentListOps._ConvertStudentList(records), filename);
            var virtualPath = GetVirtualPath(physicalPath);

            String output = "<a title='点击下载' href='" + Url.Content(virtualPath) + "'>" + count + "新生分配给" + organization.Name + "</a>";

            return output;
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public String AssignRecordsToOrg(int[] checkedRecords, int org)
        {
            int count = 0;
            int i = 0;
            checkedRecords = checkedRecords ?? new int[] { };

            var organization = db.Organizations.Find(org);

            var records = new List<NewStudent>();
            string studentIds = "";
            for (i = 0; i < checkedRecords.Length; i++)
            {
                var s = db.NewStudents.Find(checkedRecords[i]);
                if ((s != null) && (s.Organization == null))
                {
                    studentIds += (s.Id.ToString() + ",");
                    records.Add(s);
                    organization.NewStudents.Add(s);
                    count++;
                }
            }
            if (count > 0)
            {
                string description = "系统分配" + count + "个新生到机构:" + organization.Name;
                var operationRecord = NewStudentOperation.AssignToOrgRecord(org, studentIds, description);
                db.OperationRecords.Add(operationRecord);
            }
            db.SaveChanges();

            String filename = organization.Name + "_" + count.ToString() + "_Students" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            var physicalPath = SaveToExcel(NewStudentListOps._ConvertStudentList(records), filename);
            var virtualPath = GetVirtualPath(physicalPath);

            String output = "<a title='点击下载' href='" + Url.Content(virtualPath) + "'>" + count + "新生分配给" + organization.Name + "</a>";

            return output;
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + "," + LWSFRoles.organizationLeader)]
        public String SelectRecordsToMyOrg(int[] checkedRecords)
        {
            int count = 0;
            int i = 0;

            var currVol = GetCurrentVolunteer();
            var organization = currVol.Organization;

            checkedRecords = checkedRecords ?? new int[] { };

            var records = new List<NewStudent>();
            string studentIds = "";
            for (i = 0; i < checkedRecords.Length; i++)
            {
                var s = db.NewStudents.Find(checkedRecords[i]);
                if ((s != null) && (s.Organization == null))
                {
                    studentIds += (s.Id.ToString() + ",");
                    records.Add(s);
                    organization.NewStudents.Add(s);
                    count++;
                }
            }
            if (count > 0)
            {
                string description = "系统分配" + count + "个新生到机构:" + organization.Name;
                var operationRecord = NewStudentOperation.AssignToOrgRecord(organization.Id, studentIds, description);
                db.OperationRecords.Add(operationRecord);
            }
            db.SaveChanges();

            String filename = organization.Name + "_" + count.ToString() + "_Students" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            var physicalPath = SaveToExcel(NewStudentListOps._ConvertStudentList(records), filename);
            var virtualPath = GetVirtualPath(physicalPath);

            String output = "<a title='点击下载' href='" + Url.Content(virtualPath) + "'>" + count + "新生分配给" + organization.Name + "</a>";

            return output;
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelAllUnassigned()
        {
            var students = NewStudentListOps._GetAllUnasignedNewStudent();
            string filename = "New-Student-Registration-Form-Unassigned-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportNewStudentToExcel(students, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public ViewResult OrgNewStudentList(int? orgId)
        {
            int Id;
            if (!orgId.HasValue)
            {
                Id = 1;
            }
            else
            {
                Id = orgId.Value;
            }

            var org = db.Organizations.Find(Id);

            ViewBag.orgName = org.Name;
            ViewBag.orgId = Id;
            var students = NewStudentListOps._GetNewStudentFromOrg(Id);

            return View(students);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult _OrgNewStudentList(int? orgId)
        {
            int Id;
            if (!orgId.HasValue)
            {
                Id = 1;
            }
            else
            {
                Id = orgId.Value;
            }

            var students = NewStudentListOps._GetNewStudentFromOrg(Id);
            return View(new GridModel(students));
        }


        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public string ReturnRecordsToSystem(int[] checkedRecords)
        {
            int count = 0;
            checkedRecords = checkedRecords ?? new int[] { };
            var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id) && o.Organization != null && o.PickupVolunteer == null && o.TempHouseVolunteer == null && o.Group == null);

            var groupedRecords = records.GroupBy(r => r.Organization);

            foreach (var rs in groupedRecords)
            {
                string studentIds = "";
                Organization org;
                var firstone = rs.FirstOrDefault();

                org = firstone.Organization;

                foreach (var r in rs)
                {
                    if (r.PickupVolunteer != null || r.TempHouseVolunteer != null || r.Group != null)
                    {
                        continue;
                    }

                    var manualAssigns = db.ManualAssignInfoes.Where(m => m.NewStudentId == r.Id).ToList();
                    foreach (var a in manualAssigns)
                    {// remove all manual assigns
                        db.ManualAssignInfoes.Remove(a);
                    }

                    r.Organization.NewStudents.Remove(r);
                    studentIds += r.Id.ToString() + ",";
                    count++;
                }
                if (count > 0)
                {
                    string description = "系统退回" + count + "个新生";
                    var operationRecord = NewStudentOperation.ReturnFromOrgRecord(org.Id, studentIds, description);
                    db.OperationRecords.Add(operationRecord);
                }
            }

            /*string studentIds = "";
            foreach (var r in records)
            {
                if (r.PickupVolunteer != null || r.TempHouseVolunteer != null || r.Group != null)
                {
                    continue;
                }

                var manualAssigns = db.ManualAssignInfoes.Where(m => m.NewStudentId == r.Id).ToList();
                foreach (var a in manualAssigns)
                {// remove all manual assigns
                    db.ManualAssignInfoes.Remove(a);
                }

                r.Organization.NewStudents.Remove(r);
                studentIds += r.Id.ToString() + ",";
                count++;
            }
            if (count > 0)
            {
                string description = "系统退回" + count + "个新生";
                var operationRecord = NewStudentOperation.ReturnFromOrgRecord(-1, studentIds, description);
                db.OperationRecords.Add(operationRecord);
            }*/
            db.SaveChanges();
            String filename = "返还_" + count.ToString() + "_Students" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            var physicalPath = SaveToExcel(NewStudentListOps._ConvertStudentList(records), filename);
            var virtualPath = GetVirtualPath(physicalPath);

            String output = "<a title='点击下载' href='" + Url.Content(virtualPath) + "'>" + count + "新生返回到系统" + "</a>";
            return output;
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public JsonResult PickupManualAssign(int studId)
        {
            var stud = db.NewStudents.Find(studId);
            var manualAssignInfo = stud.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).SingleOrDefault();
            if (manualAssignInfo == null)
            {
                manualAssignInfo = new ManualAssignInfo();
                manualAssignInfo.Id = -1;
                manualAssignInfo.Type = ManualAssignType.IntPickup;
                manualAssignInfo.NewStudent = stud;
            }
            return Json(new
            {
                PartialViewHtml = RenderPartialViewToString("ManualAssignEdit", manualAssignInfo),
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public JsonResult HousingManualAssign(int studId)
        {
            var stud = db.NewStudents.Find(studId);
            var manualAssignInfo = stud.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).SingleOrDefault();
            if (manualAssignInfo == null)
            {
                manualAssignInfo = new ManualAssignInfo();
                manualAssignInfo.Id = -1;
                manualAssignInfo.Type = ManualAssignType.IntHousing;
                manualAssignInfo.NewStudent = stud;
            }
            return Json(new
            {
                PartialViewHtml = RenderPartialViewToString("ManualAssignEdit", manualAssignInfo),
            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader)]
        public JsonResult ManualAssignEdit()
        {
            int studId;
            bool status = true;
            string message = "Assigned Successfully!";
            ManualAssignInfo manualAssignInfo = new ManualAssignInfo();

            studId = Int32.Parse(Request["studId"]);
            manualAssignInfo.Id = Int32.Parse(Request["Id"]);
            manualAssignInfo.VolName = Request["VolName"];
            manualAssignInfo.VolGender = Int32.Parse(Request["VolGender"]);
            manualAssignInfo.VolEmail = Request["VolEmail"];
            manualAssignInfo.VolPhone = Request["VolPhone"];
            manualAssignInfo.VolAddr = Request["VolAddr"];
            manualAssignInfo.Type = Int32.Parse(Request["Type"]);
            manualAssignInfo.LastUpdate = DateTime.Now;

            var stud = db.NewStudents.Find(studId);
            var assign = db.ManualAssignInfoes.Find(manualAssignInfo.Id);
            manualAssignInfo.NewStudent = stud;
            manualAssignInfo.NewStudentId = stud.Id;

            if (manualAssignInfo.Type == ManualAssignType.IntDelete)
            {
                if (assign != null)
                {
                    db.ManualAssignInfoes.Remove(assign);
                    db.SaveChanges();
                }
            }
            else
            {
                if (String.IsNullOrEmpty(manualAssignInfo.VolName) || String.IsNullOrEmpty(manualAssignInfo.VolEmail))
                {
                    status = false;
                    message = "Failed: Name and Email are required";
                    goto exit;
                }

                if (manualAssignInfo.Type == ManualAssignType.IntPickup)
                {
                    if (stud.PickupVolunteer != null)
                    {
                        status = false;
                        message = "Failed: this student is already assigned to " + stud.PickupVolunteer.Name;
                        goto exit;
                    }
                }
                if (manualAssignInfo.Type == ManualAssignType.IntHousing)
                {
                    if (stud.TempHouseVolunteer != null)
                    {
                        status = false;
                        message = "Failed: this student is already assigned to " + stud.TempHouseVolunteer.Name;
                        goto exit;
                    }
                }
                if (assign != null)
                {
                    db.Entry(assign).CurrentValues.SetValues(manualAssignInfo);
                    db.SaveChanges();
                }
                else
                {
                    db.ManualAssignInfoes.Add(manualAssignInfo);
                    db.SaveChanges();
                }
            }

        exit:
            return Json(new
            {
                Success = status,
                Message = message,
            });
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult MatchManualAssignToAuto()
        {
            int count = 0;
            var mList = db.ManualAssignInfoes.ToList();

            foreach (var m in mList)
            {
                var vol = db.Volunteers.Where(v => v.Email == m.VolEmail).FirstOrDefault();
                var stud = m.NewStudent;
                if (vol == null)
                {
                    continue;
                }
                if (m.Type == ManualAssignType.IntPickup)
                {
                    vol.PickupNewStudents.Add(stud);
                }
                else if (m.Type == ManualAssignType.IntHousing)
                {
                    vol.TempHouseNewStudents.Add(stud);
                }
                db.ManualAssignInfoes.Remove(m);
                count++;
            }
            db.SaveChanges();
            return Content(count + " has been matched to auto");
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelOrg(int orgId)
        {
            var students = NewStudentListOps._GetNewStudentFromOrg(orgId);
            var g = db.Organizations.Find(orgId);
            string filename = "New-Student-Registration-Form-" + g.Name + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportNewStudentToExcel(students, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelOrgUnassigned(int orgId)
        {
            var students = NewStudentListOps._GetOrgUnassignedNewStudents(orgId);
            var o = db.Organizations.Find(orgId);
            string filename = "New-Student-Registration-Form-unassigned-" + o.Name + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportNewStudentToExcel(students, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult OrgUnassignedNewStudentList(int? orgId)
        {
            int Id;
            if (!orgId.HasValue)
            {
                Id = 1;
            }
            else
            {
                Id = orgId.Value;
            }

            if (!User.IsInRole(LWSFRoles.admin))
            {
                var currVol = GetCurrentVolunteer();

                if (currVol.OrganizationId != Id)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }

            ViewBag.orgName = db.Organizations.Find(Id).Name;
            ViewBag.orgId = Id;
            var students = NewStudentListOps._GetOrgUnassignedNewStudents(Id);
            return View(students);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult _OrgUnassignedNewStudentList(int? orgId)
        {
            int Id;
            if (!orgId.HasValue)
            {
                Id = 1;
            }
            else
            {
                Id = orgId.Value;
            }

            ViewBag.orgId = Id;

            var students = NewStudentListOps._GetOrgUnassignedNewStudents(Id);
            return View(new GridModel(students));
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader + ", " + LWSFRoles.newStudentAdmin)]
        public string AssignRecordsToGrp(int[] checkedRecords, int grp)
        {
            int count = 0;
            checkedRecords = checkedRecords ?? new int[] { };
            var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id));
            var group = db.Groups.Find(grp);

            //Volunteer currVol = GetCurrentVolunteer();
            string studentIds = "";
            foreach (var r in records)
            {
                if (group.Organization != r.Organization)
                {
                    continue;
                }
                if (r.Group != null)
                {
                    continue;
                }
                group.NewStudents.Add(r);
                studentIds += r.Id.ToString() + ",";
                count++;
            }

            if (count > 0)
            {
                string description = group.Organization.Name + "分配" + count + "个新生到小组:" + group.Name;
                var operationRecord = NewStudentOperation.AssignToGrpRecord(grp, studentIds, description);
                db.OperationRecords.Add(operationRecord);
            }
            db.SaveChanges();

            return count + " New Students added to" + group.Name;
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.organizationLeader + ", " + LWSFRoles.groupLeader + ", " + LWSFRoles.newStudentAdmin)]
        public string GrpReturnRecordsToOrg(int[] checkedRecords)
        {
            int count = 0;
            int fail = 0;
            checkedRecords = checkedRecords ?? new int[] { };
            var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id));

            var dl = DateTime.Now.AddDays(ccbs.Models.SetupParameters.CancelDeadline);
            string studentIds = "";
            foreach (var r in records)
            {
                if ((r.PickupVolunteer != null) || (r.TempHouseVolunteer != null))
                {
                    fail++;
                    continue;
                }
                bool cancelEnabled = (DateTime.Compare(dl, r.ArrivalTime) < 0);
                if (User.IsInRole(LWSFRoles.admin))
                {
                    cancelEnabled = true;
                }
                if (cancelEnabled == false)
                {
                    fail++;
                    continue;
                }
                r.Group.NewStudents.Remove(r);
                studentIds += r.Id.ToString() + ",";
                count++;
            }
            if (count > 0)
            {
                string description = "从小组退回" + count + "个新生到机构";
                var operationRecord = NewStudentOperation.ReturnFromGrpRecord(-1, studentIds, description);
                db.OperationRecords.Add(operationRecord);
            }
            db.SaveChanges();
            return count + " records returned to my Organization and " + fail + " failed";
        }

        [Authorize]
        public ViewResult GrpNewStudentList(int grpId)
        {
            Group grp = db.Groups.Find(grpId);
            ViewBag.grpName = grp.Name;
            ViewBag.grpId = grp.Id;
            ViewBag.orgId = grp.OrganizationId;
            var students = NewStudentListOps._GetNewStudentFromGrp(grpId);
            return View(students);
        }

        [GridAction]
        [Authorize]
        public ActionResult _GrpNewStudentList(int grpId)
        {
            var students = NewStudentListOps._GetNewStudentFromGrp(grpId);
            return View(new GridModel(students));
        }

        [Authorize]
        public ActionResult ExportExcelGrp(int grpId)
        {
            var students = NewStudentListOps._GetNewStudentFromGrp(grpId);
            var g = db.Groups.Find(grpId);
            string filename = "New-Student-Registration-Form-" + g.Name + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportNewStudentToExcel(students, filename);
        }

        [Authorize]
        public ViewResult GrpUnassignedNewStudentList(int grpId)
        {
            Group grp = db.Groups.Find(grpId);

            ViewBag.grpName = grp.Name;
            ViewBag.grpId = grp.Id;

            var students = NewStudentListOps._GetGrpUnassignedNewStudents(grp.Id);
            return View(students);
        }

        [Authorize]
        [GridAction]
        public ActionResult _GrpUnassignedNewStudentList(int grpId)
        {
            Group grp = db.Groups.Find(grpId);

            ViewBag.grpName = grp.Name;
            ViewBag.grpId = grp.Id;

            var students = NewStudentListOps._GetGrpUnassignedNewStudents(grp.Id);
            return View(new GridModel(students));
        }

        [Authorize]
        public ActionResult OrgNewStudentsExportExcel(int orgId)
        {
            var students = NewStudentListOps._GetNewStudentFromOrg(orgId);
            var org = db.Organizations.Find(orgId);
            string filename = org.Name + "_NewStudents" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return _ExportNewStudentToExcel(students, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ViewResult NewStudentHelpList()
        {
            var studHasApt = db.NewStudents.Where(n => n.HasApt == true);
            var students = NewStudentListOps._ConvertStudentList(studHasApt);
            return View(students);
        }

        [GridAction]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult _NewStudentHelpList()
        {
            var studHasApt = db.NewStudents.Where(n => n.HasApt == true);
            var students = NewStudentListOps._ConvertStudentList(studHasApt);
            return View(new GridModel(students));
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public string MarkNewStudents(int[] checkedRecords)
        {
            int count = 0;
            checkedRecords = checkedRecords ?? new int[] { };
            var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id));

            foreach (var r in records)
            {
                if (r.Marked == false)
                {
                    r.Marked = true;
                    count++;
                }
            }
            db.SaveChanges();
            return count + " new records marked";
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public string UnMarkNewStudents(int[] checkedRecords)
        {
            int count = 0;
            checkedRecords = checkedRecords ?? new int[] { };
            var records = db.NewStudents.Where(o => checkedRecords.Contains(o.Id));

            foreach (var r in records)
            {
                if (r.Marked == true)
                {
                    r.Marked = false;
                    count++;
                }
            }
            db.SaveChanges();
            return count + " new records UnMarked";
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelAllOfferHelp()
        {
            var studHasApt = db.NewStudents.Where(n => n.HasApt == true);
            var students = NewStudentListOps._ConvertStudentList(studHasApt);
            string filename = "Help-Students" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return ExportExcelOfferHelp(students, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelMarked()
        {
            var studHasApt = db.NewStudents.Where(n => n.HasApt == true);
            var students = NewStudentListOps._ConvertStudentList(studHasApt).Where(s => s.Marked == true).ToList();
            string filename = "Help-Students-marked" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return ExportExcelOfferHelp(students, filename);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelUnmarked()
        {
            var studHasApt = db.NewStudents.Where(n => n.HasApt == true);
            var students = NewStudentListOps._ConvertStudentList(studHasApt).Where(s => s.Marked != true).ToList();
            string filename = "Help-Students-unmarked" + "-" + DateTime.Now.ToString("MM-dd-yyyy-H-mm") + ".xlsx";
            return ExportExcelOfferHelp(students, filename);
        }

        //
        // GET: /NewStudent/Edit/5
        [Authorize(Roles = LWSFRoles.newStudent)]
        public ActionResult SelfEdit(string returnUrl)
        {
            var newstudent = GetCurrentNewStudent();
            return RedirectToAction("Edit", "NewStudent", new { id = newstudent.Id, returnUrl = returnUrl });
        }

        //
        // GET: /NewStudent/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudent + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Edit(int id)
        {
            NewStudent newstudent = db.NewStudents.Find(id);
            var newStudentInfo = new NewStudentInfoModel(newstudent);
            return View(newStudentInfo);
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult CancelNoNeedAssignment()
        {
            int pickCount = 0, housingCount = 0;

            var students = db.NewStudents.Where(s => s.NeedPickup == false || s.NeedTempHousing == false).ToList();
            foreach (var s in students)
            {
                if (!s.NeedPickup)
                {
                    if (s.PickupVolunteer != null)
                    {
                        s.PickupVolunteer.PickupNewStudents.Remove(s);
                        pickCount++;
                    }
                    if (s.ManualAssignInfoes != null && s.ManualAssignInfoes.Count > 0)
                    {
                        var pickManual = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).FirstOrDefault();
                        if (pickManual != null)
                        {
                            db.ManualAssignInfoes.Remove(pickManual);
                            pickCount++;
                        }
                    }
                }
                if (!s.NeedTempHousing)
                {
                    if (s.TempHouseVolunteer != null)
                    {
                        s.TempHouseVolunteer.TempHouseNewStudents.Remove(s);
                        housingCount++;
                    }
                    if (s.ManualAssignInfoes != null && s.ManualAssignInfoes.Count > 0)
                    {
                        var housingManual = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).FirstOrDefault();
                        if (housingManual != null)
                        {
                            db.ManualAssignInfoes.Remove(housingManual);
                            housingCount++;
                        }
                    }
                }
            }
            db.SaveChanges();
            return Content("Pickup Count: " + pickCount + ", Temp Housing Count: " + housingCount);
        }

        //
        // POST: /NewStudent/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudent + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Edit(NewStudentInfoModel newStudentInfo, string returnUrl)
        {
            bool needNotice = false;

            if (ModelState.IsValid)
            {
                var s = db.NewStudents.Find(newStudentInfo.Id);

                if (newStudentInfo.Flight != s.Flight)
                {
                    needNotice = true;
                }
                else if (newStudentInfo.ArrivalTime != s.ArrivalTime)
                {
                    needNotice = true;
                }
                else if (newStudentInfo.NeedPickup != s.NeedPickup)
                {
                    needNotice = true;
                }
                else if (newStudentInfo.NeedTempHousing != s.NeedTempHousing)
                {
                    needNotice = true;
                }

                if (needNotice)
                {
                    s.LastUpdate = DateTime.Now;
                }
                var newstudent = newStudentInfo.GetNewStudentModel(s);
                db.Entry(s).CurrentValues.SetValues(newstudent);

                if (!s.NeedPickup)
                {
                    if (s.PickupVolunteer != null)
                    {
                        s.PickupVolunteer.PickupNewStudents.Remove(s);
                    }
                    if (s.ManualAssignInfoes != null && s.ManualAssignInfoes.Count > 0)
                    {
                        var pickManual = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).FirstOrDefault();
                        if (pickManual != null)
                        {
                            db.ManualAssignInfoes.Remove(pickManual);
                        }
                    }
                }
                if (!s.NeedTempHousing)
                {
                    if (s.TempHouseVolunteer != null)
                    {
                        s.TempHouseVolunteer.TempHouseNewStudents.Remove(s);
                    }
                    if (s.ManualAssignInfoes != null && s.ManualAssignInfoes.Count > 0)
                    {
                        var housingManual = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).FirstOrDefault();
                        if (housingManual != null)
                        {
                            db.ManualAssignInfoes.Remove(housingManual);
                        }
                    }
                }

                db.SaveChanges();

                if (needNotice)
                {
                    NewStudentChangeInfoEmail(s);
                }

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(newStudentInfo);
        }

        //
        // GET: /NewStudent/HelpForm/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudent + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult HelpForm(int id)
        {
            NewStudent newstudent = db.NewStudents.Find(id);
            return View(newstudent);
        }

        //
        // POST: /NewStudent/HelpForm/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudent + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult HelpForm(NewStudent newstudent, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newstudent).State = EntityState.Modified;

                db.SaveChanges();

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(newstudent);
        }

        //
        // GET: /NewStudent/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Delete(int id)
        {
            NewStudent newstudent = db.NewStudents.Find(id);
            return View(newstudent);
        }

        //
        // POST: /NewStudent/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult DeleteConfirmed(int id)
        {
            delete_new_student(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Clear_NewStudents()
        {
            clearNewStudentPool();
            return RedirectToAction("Index");
        }

        void clearNewStudentPool()
        {
            var allNewStudents = db.NewStudents.ToList();
            foreach (var stud in allNewStudents)
            {
                if (stud.ArrivalTime < DateTime.Now)
                {
                    delete_new_student(stud.Id);
                }
            }
        }

        void delete_new_student(int id)
        {
            var student = db.NewStudents.Find(id);
            if (student != null)
            {
                //Delete the record
                if (student.PickupVolunteer != null)
                {
                    student.PickupVolunteer.PickupNewStudents.Remove(student);
                }
                if (student.TempHouseVolunteer != null)
                {
                    student.TempHouseVolunteer.TempHouseNewStudents.Remove(student);
                }
                if (student.Group != null)
                {
                    student.Group.NewStudents.Remove(student);
                }
                if (student.Organization != null)
                {
                    student.Organization.NewStudents.Remove(student);
                }
                if (student.RegisterEntries != null)
                {
                    student.RegisterEntries.Clear();
                }
                if (student.ManualAssignInfoes != null)
                {
                    foreach (var a in student.ManualAssignInfoes.ToList())
                    {
                        db.ManualAssignInfoes.Remove(a);
                    }
                }
                if (student.EmailHistories != null)
                {
                    foreach (var h in student.EmailHistories.ToList())
                    {
                        db.EmailHistories.Remove(h);
                    }
                }
                db.NewStudents.Remove(student);
                db.SaveChanges();
            }
        }


        public ActionResult Disclaimer()
        {
            var disclaimer = db.Disclaimers.FirstOrDefault();
            if (disclaimer == null)
            {
                return RedirectToAction("NewStudentRegister");
            }
            disclaimer.Description = Server.HtmlDecode(disclaimer.Description);
            return View(disclaimer);
        }

        // GET: /NewStudent/NewStudentRegister
        public ActionResult NewStudentRegister()
        {
            return View();
        }

        //
        // @Html.ActionLink("Register", "NewStudentRegister", "NewStudent", new { returnUrl = Request.RawUrl }, null)

        [HttpPost]
        public ActionResult NewStudentRegister(NewStudentRegisterModel NewStudentRegister)
        {
            MembershipCreateStatus createStatus;
            string flightCheck = null;
            if (ModelState.IsValid)
            {
                flightCheck = CheckFilghtNumber(NewStudentRegister.Flight);
                if (flightCheck != null)
                {
                    ModelState.AddModelError("", flightCheck);
                    return View(NewStudentRegister);
                }

                var emailCheck = IsValidEmail(NewStudentRegister.Email);
                if (!emailCheck)
                {
                    ModelState.AddModelError("", "Email is not valid!");
                    return View(NewStudentRegister);
                }

                if (AccountController.RegisterNew(NewStudentRegister.GetRegisterModel(), out createStatus) == false)
                {
                    ModelState.AddModelError("", AccountController.ErrorCodeToString(createStatus));
                    return View(NewStudentRegister);
                }
                Roles.AddUserToRole(NewStudentRegister.UserName, LWSFRoles.newStudent);

                var newStudent = NewStudentRegister.GetNewStudentModel();
                db.NewStudents.Add(newStudent);
                db.SaveChanges();
                sendFirstLetterToNewStudent(newStudent.Email);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }

            return View(NewStudentRegister);
        }

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult NewStudentAutoRegister()
        {
            int total = 30;
            int count = 0;

            for (count = 16; count <= total; count++)
            {
                string username = "newstudent" + count;
                string name = "newstudent" + count;
                string cnname = "新生" + count;
                int gender = (count > 5) ? SystemGender.singleFemale : SystemGender.singleMale;
                string email = "new" + count + "@utdbaike.com";
                DateTime arrival = DateTime.Now.AddDays(count + 10).AddHours(count);
                var stud = new NewStudentRegisterModel
                {
                    UserName = username,
                    Password = "123456",
                    ConfirmPassword = "123456",
                    Name = name,
                    CnName = cnname,
                    Gender = gender,
                    ComeFrom = "上海",
                    Email = email,
                    Phone = "123456789",
                    Major = "Accounting",
                    Flight = "AA1234",
                    ArrivalTime = arrival,
                    EntryPort = "Dallas",
                    NeedPickup = true,
                    NeedTempHousing = true,
                    Note = "Thank you all",

                };
                NewStudentRegister(stud);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudent + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Details(int id)
        {
            NewStudent newstudent = db.NewStudents.Find(id);
            if (!User.IsInRole(LWSFRoles.admin))
            {
                if (User.Identity.Name != newstudent.UserName)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }
            return View(newstudent);
        }

        [Authorize]
        public JsonResult ViewDetails(int studId)
        {
            var stud = NewStudentListOps._GetOneNewStudent(studId);
            return Json(new { student = stud }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudent)]
        public ActionResult StudentInfoConfirmation(int id)
        {
            NewStudent newstudent = db.NewStudents.Find(id);
            if (!User.IsInRole(LWSFRoles.admin))
            {
                if (User.Identity.Name != newstudent.UserName)
                {
                    return RedirectToAction("UnauthorizedError", "Home", null);
                }
            }
            newstudent.Confirmed = ConfirmedStage.InfoConfirmed;
            db.SaveChanges();
            return RedirectToAction("NewStudentHome");
        }

        public ActionResult SendNewStudentArrivalNotification()
        {
            var checkDeadline = DateTime.Now.AddDays(SetupParameters.EmailNotificationDays);

            foreach (var stud in db.NewStudents.ToList())
            {
                if (!stud.NeedPickup && !stud.NeedTempHousing)
                {
                    continue;
                }
                if (stud.ArrivalTime > checkDeadline)
                {//student will arrive in more than 2 days
                    continue;
                }

                if (stud.ArrivalTime < DateTime.Now)
                {//student already arrived
                    continue;
                }

                var emailSendModel = new SmtpEmail();
                emailSendModel.Subject = "Last Notification: " + stud.CnName + " will arrive at " + stud.ArrivalTime.ToString("MM/dd/yyyy hh:mm tt") + "!";
                emailSendModel.Body = RenderPartialViewToString("_ImportantDetails", stud);
                emailSendModel.Bcc.Add(stud.Email);
                if (stud.NeedPickup)
                {
                    var manualPickup = stud.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).FirstOrDefault();
                    if (stud.PickupVolunteer != null)
                    {
                        emailSendModel.Bcc.Add(stud.PickupVolunteer.Email);
                    }
                    else if (manualPickup != null)
                    {
                        emailSendModel.Bcc.Add(manualPickup.VolEmail);
                    }
                }
                if (stud.NeedTempHousing)
                {
                    var manualHousing = stud.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).FirstOrDefault();
                    if (stud.TempHouseVolunteer != null)
                    {
                        emailSendModel.Bcc.Add(stud.TempHouseVolunteer.Email);
                    }
                    else if (manualHousing != null)
                    {
                        emailSendModel.Bcc.Add(manualHousing.VolEmail);
                    }
                }
                emailSendModel.Send();
            }
            return Content("Check finished at " + DateTime.Now);
        }

        public ActionResult CheckAndSendAllThreePartiesConfirmation()
        {
            foreach (var stud in db.NewStudents.ToList())
            {
                CheckAndSendThreePartiesEmail(stud);
            }
            return Content("Check finished at " + DateTime.Now);
        }

        private void CheckAndSendThreePartiesEmail(NewStudent student)
        {
            var checkDeadline = DateTime.Now.AddDays(SetupParameters.ConfirmEmailDays);
            if (!student.NeedPickup && !student.NeedTempHousing)
            {
                return;
            }

            if (student.ArrivalTime > checkDeadline)
            {//student will arrive in more than 10 days
                return;
            }

            if (student.ArrivalTime < DateTime.Now)
            {//student already arrived
                return;
            }

            var histories = student.EmailHistories;

            var studentEmail = histories.Where(e => e.Type == EmailType.ArriveNoticeToStud).FirstOrDefault();
            var pickEmail = histories.Where(e => e.Type == EmailType.ArriveNoticeToPickup).FirstOrDefault();
            var housingEmail = histories.Where(e => e.Type == EmailType.ArriveNoticeToHost).FirstOrDefault();

            var pickManualAssign = student.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).FirstOrDefault();
            var housingManualAssign = student.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).FirstOrDefault();

            var dl = DateTime.Now.AddDays(-3);
            if (student.NeedPickup)
            {
                if (pickEmail == null || (!pickEmail.Confirmed && (pickEmail.LastSend.CompareTo(dl) < 0)))
                {
                    if (student.PickupVolunteer != null)
                    {
                        SendPickupConfirmationEmail(student, student.PickupVolunteer.Email);
                    }
                    else if (pickManualAssign != null)
                    {
                        SendPickupConfirmationEmail(student, pickManualAssign.VolEmail);
                    }
                }
            }
            if (student.NeedTempHousing)
            {
                if (housingEmail == null || (!housingEmail.Confirmed && (housingEmail.LastSend.CompareTo(dl) < 0)))
                {
                    if (student.TempHouseVolunteer != null)
                    {
                        SendHousingConfirmationEmail(student, student.TempHouseVolunteer.Email);
                    }
                    else if (housingManualAssign != null)
                    {
                        SendHousingConfirmationEmail(student, housingManualAssign.VolEmail);
                    }
                }
            }

            if (studentEmail == null || (!studentEmail.Confirmed && (studentEmail.LastSend.CompareTo(dl) < 0)))
            {
                SendStudentConfirmationEmail(student, student.Email);
            }
        }

        private void SendStudentConfirmationEmail(NewStudent student, string emailAddr)
        {
            var emailSendModel = new SmtpEmail();
            emailSendModel.Bcc.Add(emailAddr);
            emailSendModel.Subject = Semester.this_year() + " " + Semester.this_semester() + " UTD New Student Pickup and Temp Housing";
            ViewBag.confirmedRole = "student";

            string hashCode = ResetPasswordModel.HashResetParams(student.UserName, emailAddr);
            ViewBag.emailAddr = emailAddr;
            ViewBag.hashCode = hashCode;

            emailSendModel.Body = RenderPartialViewToString("_FinalConfirmation", student);
            emailSendModel.Send();

            if (!emailSendModel.ErrorExist())
            { // no error
                EmailHistory history = student.EmailHistories.Where(e => e.Type == EmailType.ArriveNoticeToStud).FirstOrDefault();

                if (history == null)
                {
                    history = new EmailHistory
                    {
                        Type = EmailType.ArriveNoticeToStud,
                        NewStudentId = student.Id,
                        Title = emailSendModel.Subject,
                        Body = emailSendModel.Body,
                        Confirmed = false,
                        ToEmail = emailAddr,
                        LastSend = DateTime.Now,
                    };
                    db.EmailHistories.Add(history);
                }
                else
                {
                    history.Title = emailSendModel.Subject;
                    history.Body = emailSendModel.Body;
                    history.ToEmail = emailAddr;
                    history.LastSend = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        private void SendPickupConfirmationEmail(NewStudent student, string emailAddr)
        {
            var emailSendModel = new SmtpEmail();

            emailSendModel.Bcc.Add(emailAddr);
            emailSendModel.Subject = Semester.this_year() + " " + Semester.this_semester() + "  UTD New Student Pickup and Temp Housing";
            ViewBag.confirmedRole = "pickup";

            string hashCode = ResetPasswordModel.HashResetParams(student.UserName, emailAddr);
            ViewBag.emailAddr = emailAddr;
            ViewBag.hashCode = hashCode;

            emailSendModel.Body = RenderPartialViewToString("_FinalConfirmation", student);
            emailSendModel.Send();

            if (!emailSendModel.ErrorExist())
            {
                EmailHistory history = student.EmailHistories.Where(e => e.Type == EmailType.ArriveNoticeToPickup).FirstOrDefault();

                if (history == null)
                {
                    history = new EmailHistory
                    {
                        Type = EmailType.ArriveNoticeToPickup,
                        NewStudentId = student.Id,
                        Title = emailSendModel.Subject,
                        Body = emailSendModel.Body,
                        Confirmed = false,
                        ToEmail = emailAddr,
                        LastSend = DateTime.Now,
                    };
                    db.EmailHistories.Add(history);
                }
                else
                {
                    history.Title = emailSendModel.Subject;
                    history.Body = emailSendModel.Body;
                    history.ToEmail = emailAddr;
                    history.LastSend = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        private void SendHousingConfirmationEmail(NewStudent student, string emailAddr)
        {
            var emailSendModel = new SmtpEmail();

            emailSendModel.Bcc.Add(emailAddr);
            emailSendModel.Subject = Semester.this_year() + " " + Semester.this_semester() + " UTD New Student Pickup and Temp Housing";
            ViewBag.confirmedRole = "housing";

            string hashCode = ResetPasswordModel.HashResetParams(student.UserName, emailAddr);
            ViewBag.emailAddr = emailAddr;
            ViewBag.hashCode = hashCode;

            emailSendModel.Body = RenderPartialViewToString("_FinalConfirmation", student);
            emailSendModel.Send();

            if (!emailSendModel.ErrorExist())
            {
                EmailHistory history = student.EmailHistories.Where(e => e.Type == EmailType.ArriveNoticeToHost).FirstOrDefault();
                if (history == null)
                {
                    history = new EmailHistory
                    {
                        Type = EmailType.ArriveNoticeToHost,
                        NewStudentId = student.Id,
                        Title = emailSendModel.Subject,
                        Body = emailSendModel.Body,
                        Confirmed = false,
                        ToEmail = emailAddr,
                        LastSend = DateTime.Now,
                    };
                    db.EmailHistories.Add(history);
                }
                else
                {
                    history.Title = emailSendModel.Subject;
                    history.Body = emailSendModel.Body;
                    history.ToEmail = emailAddr;
                    history.LastSend = DateTime.Now;
                }
                db.SaveChanges();
            }
        }

        public ActionResult StudentConfirmed(int studentId, string emailAddr, string hashCode)
        {
            var student = db.NewStudents.Find(studentId);
            if (student == null)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            if (String.IsNullOrEmpty(emailAddr) || string.IsNullOrEmpty(hashCode))
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            string hash = ResetPasswordModel.HashResetParams(student.UserName, emailAddr);

            if (hash != hashCode)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            var history = db.EmailHistories.Where(e => e.NewStudentId == studentId && e.Type == EmailType.ArriveNoticeToStud).FirstOrDefault();
            history.Confirmed = true;
            db.SaveChanges();

            ViewBag.confirmedRole = "confirmed";
            return View("_FinalConfirmation", student);
        }

        public ActionResult PickupHostConfirmed(int studentId, string emailAddr, string hashCode)
        {
            var student = db.NewStudents.Find(studentId);
            if (student == null)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            if (String.IsNullOrEmpty(emailAddr) || string.IsNullOrEmpty(hashCode))
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            string hash = ResetPasswordModel.HashResetParams(student.UserName, emailAddr);

            if (hash != hashCode)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            var history = db.EmailHistories.Where(e => e.NewStudentId == studentId && e.Type == EmailType.ArriveNoticeToPickup).FirstOrDefault();
            history.Confirmed = true;
            db.SaveChanges();

            ViewBag.confirmedRole = "confirmed";
            return View("_FinalConfirmation", student);
        }

        public ActionResult HousingHostConfirmed(int studentId, string emailAddr, string hashCode)
        {
            var student = db.NewStudents.Find(studentId);
            if (student == null)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            if (String.IsNullOrEmpty(emailAddr) || string.IsNullOrEmpty(hashCode))
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            string hash = ResetPasswordModel.HashResetParams(student.UserName, emailAddr);

            if (hash != hashCode)
            {
                return RedirectToAction("UnauthorizedError", "Home", null);
            }

            var history = db.EmailHistories.Where(e => e.NewStudentId == studentId && e.Type == EmailType.ArriveNoticeToHost).FirstOrDefault();
            history.Confirmed = true;
            db.SaveChanges();

            ViewBag.confirmedRole = "confirmed";
            return View("_FinalConfirmation", student);
        }

        [Authorize(Roles = LWSFRoles.newStudent)]
        public ActionResult NewStudentHome()
        {
            var currNewStudent = GetCurrentNewStudent();
            if (currNewStudent == null)
            {
                return RedirectToAction("Index", "Home");
            }
            /*var ActiveLocalhelps = db.LocalHelps.Where(l => l.LTime > DateTime.Now).OrderBy(l => l.LTime).ToList();
            ViewBag.ActiveLocalhelps = ActiveLocalhelps;*/
            ViewBag.CurrNewStudent = currNewStudent;

            var flightWarning = CheckFilghtNumber(currNewStudent.Flight);
            ViewBag.flightWarning = flightWarning;

            var emailWarning = !IsValidEmail(currNewStudent.Email);
            ViewBag.emailWarning = emailWarning;

            return View(currNewStudent);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            web_db.Dispose();
            base.Dispose(disposing);
        }

        private enum ExcelFormat
        {
            NAME = 1,
            CNNAME,
            GENDER,
            MAJOR,
            EMAIL,
            PHONE,
            NEEDPICKUP,
            NEEDHOUSE,
            DAYSOFHOUSING,
            ARRIVEDATE,
            ARRIVETIME,
            ENTRYPORT,
            FLIGHT,
            NOTE,
            ASSIGNEDORG,
            ASSIGNEDGROUP,
            PICKSTATUS,
            HOUSESTATUS,


        }

        private enum ExcelFormatOfferHelp
        {
            NAME = 1,
            GENDER,
            MAJOR,
            EMAIL,
            ARRIVETIME,
            HASAPT,
            WHENAPT,
            WHEREAPT,
            WILLINGTOHELP,
            HELPNOTE,
        }


        public String SaveToExcel(List<NewStudentViewModel> studList, string filename)
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
                ExcelWorksheet xlWorkSheet = xlPackage.Workbook.Worksheets.Add("New Student List");
                xlWorkSheet.Cell(1, (int)ExcelFormat.NAME).Value = "Name(First Last)";
                xlWorkSheet.Cell(1, (int)ExcelFormat.CNNAME).Value = "中文名";
                xlWorkSheet.Cell(1, (int)ExcelFormat.GENDER).Value = "Gender";
                xlWorkSheet.Cell(1, (int)ExcelFormat.MAJOR).Value = "Major";
                xlWorkSheet.Cell(1, (int)ExcelFormat.EMAIL).Value = "Email";
                xlWorkSheet.Cell(1, (int)ExcelFormat.PHONE).Value = "Phone";
                xlWorkSheet.Cell(1, (int)ExcelFormat.NEEDPICKUP).Value = "PICK UP";
                xlWorkSheet.Cell(1, (int)ExcelFormat.NEEDHOUSE).Value = "TEMP. HOUSING";
                xlWorkSheet.Cell(1, (int)ExcelFormat.DAYSOFHOUSING).Value = "Days of HOUSING(If needed)";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ARRIVEDATE).Value = "Arrival Date";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ARRIVETIME).Value = "Time(AM/PM)";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ENTRYPORT).Value = "Port of entry";
                xlWorkSheet.Cell(1, (int)ExcelFormat.FLIGHT).Value = "Flight";
                xlWorkSheet.Cell(1, (int)ExcelFormat.NOTE).Value = "Comment";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ASSIGNEDORG).Value = "Organization";
                xlWorkSheet.Cell(1, (int)ExcelFormat.ASSIGNEDGROUP).Value = "Group";
                xlWorkSheet.Cell(1, (int)ExcelFormat.PICKSTATUS).Value = "Pickup";
                xlWorkSheet.Cell(1, (int)ExcelFormat.HOUSESTATUS).Value = "Housing";

                int row = 2;
                foreach (var item in studList)
                {
                    xlWorkSheet.Cell(row, (int)ExcelFormat.NAME).Value = item.Name ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.CNNAME).Value = item.CnName ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.GENDER).Value = item.Gender;
                    xlWorkSheet.Cell(row, (int)ExcelFormat.MAJOR).Value = item.Major ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.EMAIL).Value = item.Email ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.PHONE).Value = item.Phone ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.NEEDPICKUP).Value = item.NeedPickup ? "YES" : "NO";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.NEEDHOUSE).Value = item.NeedTempHousing ? "YES" : "NO";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.DAYSOFHOUSING).Value = item.DaysOfHousing;
                    xlWorkSheet.Cell(row, (int)ExcelFormat.ARRIVEDATE).Value = DateTime.Parse(item.ArrivalTime).ToString("MM/dd/yyyy");
                    xlWorkSheet.Cell(row, (int)ExcelFormat.ARRIVETIME).Value = DateTime.Parse(item.ArrivalTime).ToString("hh:mm tt");
                    xlWorkSheet.Cell(row, (int)ExcelFormat.ENTRYPORT).Value = Server.HtmlEncode(item.EntryPort) ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.FLIGHT).Value = Server.HtmlEncode(item.Flight) ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.NOTE).Value = Server.HtmlEncode(item.Note) ?? "";

                    xlWorkSheet.Cell(row, (int)ExcelFormat.ASSIGNEDORG).Value = Server.HtmlEncode(item.AssignedOrg) ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.ASSIGNEDGROUP).Value = Server.HtmlEncode(item.AssignedGrp) ?? "";

                    xlWorkSheet.Cell(row, (int)ExcelFormat.PICKSTATUS).Value = Server.HtmlEncode(item.PickupStatus) ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormat.HOUSESTATUS).Value = Server.HtmlEncode(item.TempHousingStatus) ?? "";

                    row++;
                }
                xlWorkSheet.Column((int)ExcelFormat.EMAIL).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.NOTE).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.PHONE).Width = 20;
                xlWorkSheet.Column((int)ExcelFormat.NAME).Width = 15;
                xlWorkSheet.Column((int)ExcelFormat.CNNAME).Width = 15;
                xlWorkSheet.Column((int)ExcelFormat.NEEDHOUSE).Width = 15;
                xlWorkSheet.Column((int)ExcelFormat.DAYSOFHOUSING).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.ARRIVEDATE).Width = 15;
                xlWorkSheet.Column((int)ExcelFormat.ARRIVETIME).Width = 15;
                xlWorkSheet.Column((int)ExcelFormat.ENTRYPORT).Width = 15;

                xlWorkSheet.Column((int)ExcelFormat.ASSIGNEDORG).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.ASSIGNEDGROUP).Width = 25;

                xlWorkSheet.Column((int)ExcelFormat.PICKSTATUS).Width = 25;
                xlWorkSheet.Column((int)ExcelFormat.HOUSESTATUS).Width = 25;

                xlPackage.Save();
            }
            return physicalPath;
        }

        public ActionResult _ExportNewStudentToExcel(List<NewStudentViewModel> studList, string filename)
        {
            string physicalPath = SaveToExcel(studList, filename);
            byte[] ByteFile = System.IO.File.ReadAllBytes(physicalPath);
            MemoryStream m = new MemoryStream(ByteFile);
            return File(m, "application/vnd.ms-excel", filename);
        }

        public ActionResult ExportExcelOfferHelp(List<NewStudentViewModel> studList, string filename)
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
                ExcelWorksheet xlWorkSheet = xlPackage.Workbook.Worksheets.Add("New Student List");
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.NAME).Value = "Name(First Last)";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.GENDER).Value = "Gender";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.MAJOR).Value = "Major";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.EMAIL).Value = "Email";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.ARRIVETIME).Value = "Arrival Time";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.HASAPT).Value = "Has Apt?";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.WHENAPT).Value = "Move In Date";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.WHEREAPT).Value = "Where is Apt?";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.WILLINGTOHELP).Value = "Willing to help?";
                xlWorkSheet.Cell(1, (int)ExcelFormatOfferHelp.HELPNOTE).Value = "Comment";

                int row = 2;
                foreach (var item in studList)
                {
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.NAME).Value = item.Name ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.GENDER).Value = item.Gender;
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.MAJOR).Value = item.Major ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.EMAIL).Value = item.Email ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.ARRIVETIME).Value = item.ArrivalTime;
                    /*xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.HASAPT).Value = item.HasApt ? "YES" : "NO";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.WHENAPT).Value = item.WhenApt ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.WHEREAPT).Value = item.WhereApt ?? "";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.WILLINGTOHELP).Value = item.WillingToHelp ? "YES" : "NO";
                    xlWorkSheet.Cell(row, (int)ExcelFormatOfferHelp.HELPNOTE).Value = Server.HtmlEncode(item.HelpNote) ?? "";*/
                    row++;
                }
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.EMAIL).Width = 25;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.GENDER).Width = 15;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.MAJOR).Width = 20;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.NAME).Width = 15;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.HASAPT).Width = 25;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.WHENAPT).Width = 25;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.WHEREAPT).Width = 25;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.WILLINGTOHELP).Width = 25;
                xlWorkSheet.Column((int)ExcelFormatOfferHelp.HELPNOTE).Width = 25;

                xlPackage.Save();
            }

            byte[] ByteFile = System.IO.File.ReadAllBytes(physicalPath);
            MemoryStream m = new MemoryStream(ByteFile);
            return File(m, "application/vnd.ms-excel", filename);
        }

        public string GetVirtualPath(string physicalPath)
        {
            if (!physicalPath.StartsWith(HttpContext.Request.PhysicalApplicationPath))
            {
                throw new InvalidOperationException("Physical path is not within the application root");
            }

            String virtualPath = "~/" + physicalPath.Substring(HttpContext.Request.PhysicalApplicationPath.Length)
                  .Replace("\\", "/");

            return virtualPath;
        }

        //
        private bool HasFile(HttpPostedFileBase file)
        {
            return (file != null && file.ContentLength > 0) ? true : false;
        }

        public bool NewStuIsExist(string Name, string Email)
        {
            var result = db.NewStudents.Where(s => s.Email == Email);
            if (result.Count() == 0)
            {
                return false;
            }
            return true;
        }

        private Volunteer GetCurrentVolunteer()
        {
            string userName = User.Identity.Name;
            var currVolunteer = db.Volunteers.Where(v => v.UserName == userName).SingleOrDefault();
            return currVolunteer;
        }

        private NewStudent GetCurrentNewStudent()
        {
            string userName = User.Identity.Name;
            var currNewStudent = db.NewStudents.Where(v => v.UserName == userName).FirstOrDefault();
            return currNewStudent;
        }

        internal void NewStudentChangeInfoEmail(NewStudent stud)
        {
            var emailSendModel = new SmtpEmail();

            emailSendModel.Bcc.Add(stud.Email);
            if ((stud.Organization != null) && (stud.Organization.OrgLeader != null))
            {
                emailSendModel.Bcc.Add(stud.Organization.OrgLeader.Email);
                if (stud.PickupVolunteer != null)
                {
                    emailSendModel.Bcc.Add(stud.PickupVolunteer.Email);
                }
                if ((stud.TempHouseVolunteer != null) && (stud.TempHouseVolunteer != stud.PickupVolunteer))
                {
                    emailSendModel.Bcc.Add(stud.TempHouseVolunteer.Email);
                }
            }
            emailSendModel.Subject = "Important Notice:" + stud.CnName + " changed his/her information";
            emailSendModel.Body = RenderPartialViewToString("_ImportantDetails", stud);
            emailSendModel.Send();
        }

        internal void sendFirstLetterToNewStudent(String emailAddr)
        {
            var emailRecord = web_db.EmailRecords.Find(1);
            if (emailRecord == null)
            {
                return;
            }
            var emailSendModel = new SmtpEmail();

            emailSendModel.Bcc.Add(emailAddr);
            emailSendModel.Subject = emailRecord.Title;
            emailSendModel.Body = Server.HtmlDecode(emailRecord.Body);
            emailSendModel.Send();
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        bool IsCharOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < 'A' || c > 'z')
                    return false;
            }
            return true;
        }

        private string CheckFilghtNumber(string flight)
        {
            if (String.IsNullOrEmpty(flight))
            {
                return "fligth cannot be empty";
            }
            if (flight.Contains("boeing") || flight.Contains("Boeing") || flight.Contains("波音") || flight.Contains("BOEING"))
            {
                return "航班号有误！波音是飞机型号，不是航班号, 航班号: 例如AA1234, UA1234 ...";
            }
            if (IsDigitsOnly(flight))
            {
                return "航班号有误！航班号应该包含航空公司名和号码，e.g. AA1234";
            }
            if (IsCharOnly(flight))
            {
                return "航班号有误！航班号应该包含航空公司名和号码， e.g. AA1234";
            }
            return null;
        }

        private bool IsValidEmail(string email)
        {
            if (!email.Contains("@"))
            {
                return false;
            }
            if (!email.Contains("."))
            {
                return false;
            }
            return true;
        }
    }
}