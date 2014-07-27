using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.IO;
using OfficeOpenXml;

namespace ccbs.Controllers
{
    public class FacssDepartmentController : BaseController
    {
        private StudentModelContainer db = new StudentModelContainer();

        //
        // GET: /FacssDepartment/

        public ViewResult Index()
        {
            ViewBag.currentStudent = GetCurrentNewStudent();
            return View(db.FacssDepartments.ToList());
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult RegistrationDetails(int id)
        {
            FacssDepartment facssdepartment = db.FacssDepartments.Find(id);
            return View(facssdepartment);
        }

        //
        // GET: /FacssDepartment/Details/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ViewResult Details(int id)
        {
            FacssDepartment facssdepartment = db.FacssDepartments.Find(id);
            return View(facssdepartment);
        }

        //
        // GET: /FacssDepartment/Create
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FacssDepartment/Create

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Create(FacssDepartment facssdepartment)
        {
            if (ModelState.IsValid)
            {
                db.FacssDepartments.Add(facssdepartment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            facssdepartment.Introduction = Server.HtmlDecode(facssdepartment.Introduction);
            facssdepartment.ApplyNotification = Server.HtmlDecode(facssdepartment.ApplyNotification);
            return View(facssdepartment);
        }

        //
        // GET: /FacssDepartment/Edit/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Edit(int id)
        {
            FacssDepartment facssdepartment = db.FacssDepartments.Find(id);
            facssdepartment.Introduction = Server.HtmlDecode(facssdepartment.Introduction);
            facssdepartment.ApplyNotification = Server.HtmlDecode(facssdepartment.ApplyNotification);
            return View(facssdepartment);
        }

        //
        // POST: /FacssDepartment/Edit/5

        [HttpPost]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Edit(FacssDepartment facssdepartment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facssdepartment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            facssdepartment.Introduction = Server.HtmlDecode(facssdepartment.Introduction);
            facssdepartment.ApplyNotification = Server.HtmlDecode(facssdepartment.ApplyNotification);
            return View(facssdepartment);
        }

        [Authorize(Roles = LWSFRoles.newStudent)]
        public ActionResult ApplyForDepartment(int Did, int Sid, string returnUrl)
        {
            var s = db.NewStudents.Find(Sid);
            var department = db.FacssDepartments.Find(Did);
            if (department != null && s != null)
            {
                department.AppliedNewStudents.Add(s);
                db.SaveChanges();
            }
            return Json(new
            {
                Success = true,
                returnUrl = returnUrl,
                PartialViewHtml = RenderPartialViewToString("_ApplyNotification", department),
            });
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.newStudent)]
        public ActionResult CancelApplication(int Did, int Sid, string returnUrl)
        {
            var s = db.NewStudents.Find(Sid);
            var department = db.FacssDepartments.Find(Did);
            if (department != null && s != null)
            {
                department.AppliedNewStudents.Remove(s);
                db.SaveChanges();
            }
            return Redirect(returnUrl);
        }

        //
        // GET: /FacssDepartment/Delete/5
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult Delete(int id)
        {
            FacssDepartment facssdepartment = db.FacssDepartments.Find(id);
            return View(facssdepartment);
        }

        //
        // POST: /FacssDepartment/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult DeleteConfirmed(int id)
        {
            FacssDepartment facssdepartment = db.FacssDepartments.Find(id);
            db.FacssDepartments.Remove(facssdepartment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private enum LocalHelpRegistration
        {
            NAME = 1,
            GENDER,
            MAJOR,
            EMAIL,
            PHONE,
        }

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin)]
        public ActionResult ExportExcelAppliedStudents(int id)
        {
            var department = db.FacssDepartments.Find(id);
            string filename = department.Name + "报名名单" + ".xlsx";

            var physicalPath = Path.Combine(Server.MapPath("~/Download/"), filename);
            FileInfo file = new FileInfo(physicalPath);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(physicalPath);
            }

            using (ExcelPackage xlPackage = new ExcelPackage(file))
            {
                ExcelWorksheet xlWorkSheet = xlPackage.Workbook.Worksheets.Add(department.Name);
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.NAME).Value = "Name";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.GENDER).Value = "Gender";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.MAJOR).Value = "Major";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.EMAIL).Value = "Email";
                xlWorkSheet.Cell(1, (int)LocalHelpRegistration.PHONE).Value = "Phone";

                int row = 2;
                foreach (var item in department.AppliedNewStudents)
                {
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.NAME).Value = item.CnName ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.GENDER).Value = SystemGender.ToStringGender(item.Gender);
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.MAJOR).Value = item.Major ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.EMAIL).Value = item.Email ?? "";
                    xlWorkSheet.Cell(row, (int)LocalHelpRegistration.PHONE).Value = item.Phone;
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}