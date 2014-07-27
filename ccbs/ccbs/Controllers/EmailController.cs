using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using System.Net.Mail;
using utdbaike;


namespace ccbs.Controllers
{
    public class EmailController : Controller
    {
        private StudentModelContainer db = new StudentModelContainer();
        private WebModelContainer web_db = new WebModelContainer();

        //
        // GET: /Email/

        public ActionResult Index()
        {
            return View();
        }

        /*public ActionResult TestEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TestEmail(String emailContent)
        {
            var emailRecord = web_db.EmailRecords.Find(1);
            if (emailRecord == null)
            {
                return RedirectToAction("Index", "Home");
            }

            MailMessage email = new MailMessage();

            email.To.Add(new MailAddress("lasonxia@gmail.com"));

            email.Subject = emailRecord.Title;
            email.IsBodyHtml = true;

            email.Body = Server.HtmlDecode(emailRecord.Body);

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.EnableSsl = true;  //must be here, otherwise there will be execption
            smtpClient.Send(email);

            email.Dispose();

            return View();
        }*/

        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.emailAdmin)]
        public ActionResult SendSystemEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin + ", " + LWSFRoles.newStudentAdmin + ", " + LWSFRoles.emailAdmin)]
        public ActionResult SendSystemEmail(EmailRecord record)
        {
            var emailModel = new SmtpEmail();
            bool isSave;
            bool toNewStudents;
            bool toFacssVolunteers;
            bool toPickupVolunteers;
            bool toHousingVolunteers;
            bool toAllVolunteers;
            string str = Request["savetorecord"];
            if (str == "on")
            {
                isSave = true;
            }
            else
            {
                isSave = false;
            }

            str = Request["toAllNewStudents"];
            if (str == "on")
            {
                toNewStudents = true;
            }
            else
            {
                toNewStudents = false;
            }
            str = Request["toFacssVolunteers"];
            if (str == "on")
            {
                toFacssVolunteers = true;
            }
            else
            {
                toFacssVolunteers = false;
            }

            str = Request["toPickupVolunteers"];
            if (str == "on")
            {
                toPickupVolunteers = true;
            }
            else
            {
                toPickupVolunteers = false;
            }

            str = Request["toHousingVolunteers"];
            if (str == "on")
            {
                toHousingVolunteers = true;
            }
            else
            {
                toHousingVolunteers = false;
            }

            str = Request["toAllVolunteers"];
            if (str == "on")
            {
                toAllVolunteers = true;
            }
            else
            {
                toAllVolunteers = false;
            }

            record.LastUpdate = DateTime.Now;
            record.Body = Server.HtmlEncode(record.Body);
            if (isSave)
            {
                web_db.EmailRecords.Add(record);
                web_db.SaveChanges();
            }
            if (toNewStudents)
            {
                var slist = db.NewStudents.ToList();

                foreach (var s in slist)
                {
                    emailModel.Bcc.Add(s.Email);
                }
            }
            if (toAllVolunteers)
            {
                var vlist = db.Volunteers.ToList();
                foreach (var v in vlist)
                {
                    emailModel.Bcc.Add(v.Email);
                }
                var mlist = db.ManualAssignInfoes.ToList();
                foreach (var m in mlist)
                {
                    if (emailModel.Bcc.Contains(m.VolEmail))
                    {
                        continue;
                    }
                    emailModel.Bcc.Add(m.VolEmail);
                }
            }
            else
            {
                if (toFacssVolunteers)
                {
                    var facss = db.Organizations.Where(o => o.Name.Contains("FACSS")).FirstOrDefault();
                    var vlist = facss.Volunteers.ToList();
                    foreach (var v in vlist)
                    {
                        emailModel.Bcc.Add(v.Email);
                    }
                }
                if (toPickupVolunteers)
                {
                    var vlist = db.Volunteers.ToList().Where(v => v.PickupNewStudents != null && v.PickupNewStudents.Count > 0).ToList();
                    foreach (var v in vlist)
                    {
                        emailModel.Bcc.Add(v.Email);
                    }

                    var mlist = db.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).ToList();
                    foreach (var m in mlist)
                    {
                        if (emailModel.Bcc.Contains(m.VolEmail))
                        {
                            continue;
                        }
                        emailModel.Bcc.Add(m.VolEmail);
                    }
                }
                if (toHousingVolunteers)
                {
                    var vlist = db.Volunteers.ToList().Where(v => v.TempHouseNewStudents != null && v.TempHouseNewStudents.Count > 0).ToList();
                    foreach (var v in vlist)
                    {
                        emailModel.Bcc.Add(v.Email);
                    }

                    var mlist = db.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).ToList();
                    foreach (var m in mlist)
                    {
                        if (emailModel.Bcc.Contains(m.VolEmail))
                        {
                            continue;
                        }
                        emailModel.Bcc.Add(m.VolEmail);
                    }
                }
            }
            emailModel.Subject = record.Title;
            emailModel.Body = Server.HtmlDecode(record.Body);
            emailModel.Send();

            return Content(emailModel.Count + "emails were sent out successfully!");
        }
    }
}
