using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;
using utdbaike;
using System.Threading;

namespace ccbs.Controllers
{
    [Authorize(Roles = LWSFRoles.admin)]
    public class SmtpEmailController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /SmtpEmail/

        private bool VerifyAccount(EmailAccount acct)
        {
            acct.VerifyMessage = SmtpEmail.ValidateSmtpEmailAccount(acct);
            acct.Verified = (acct.VerifyMessage == "success");
            acct.LastVerifyDate = DateTime.Now;
            return acct.Verified;
        }

        public ViewResult Index()
        {
            var accountList = db.EmailAccounts;
            var now = DateTime.Now;
            foreach (var acct in accountList)
            {
                var nextVerifyDate = acct.LastVerifyDate.AddDays(SmtpEmail.VERIFY_INTERVAL);
                if (nextVerifyDate < now)
                {
                    VerifyAccount(acct);
                }
            }
            db.SaveChanges();
            return View(accountList.ToList());
        }

        public ActionResult Verify(int id)
        {
            EmailAccount acct = db.EmailAccounts.Find(id);

            VerifyAccount(acct);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /SmtpEmail/Details/5

        public ViewResult Details(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            return View(emailaccount);
        }

        public ActionResult AutoAddAccount()
        {
            for (int i = 1; i <= 10; i++)
            {
                string username = "utdbaike" + ((i<10) ? ("0"+i.ToString()) : i.ToString());
                var acct = new EmailAccount
                {
                    Name = username,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Username = username,
                    Password = "T0PSECRET!",
                    From = username + "@gmail.com",
                    SmtpDailyLimit = 400,
                    SmtpPerTimeLimit = 100,
                    Verified = false,
                    VerifyMessage = "Not Verified",
                    LastVerifyDate = new DateTime(1990, 1, 1),
                };
                db.EmailAccounts.Add(acct);
            }
            db.SaveChanges();
            return Content("finished");
        }

        //
        // GET: /SmtpEmail/Create

        public ActionResult Create()
        {
            var account = new EmailAccount
            {
                Verified = false,
                VerifyMessage = "Not Verified",
                LastVerifyDate = new DateTime(1990, 1, 1),
            };
            return View(account);
        }

        //
        // POST: /SmtpEmail/Create

        [HttpPost]
        public ActionResult Create(EmailAccount emailaccount)
        {
            if (ModelState.IsValid)
            {
                db.EmailAccounts.Add(emailaccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(emailaccount);
        }

        //
        // GET: /SmtpEmail/Edit/5

        public ActionResult Edit(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            return View(emailaccount);
        }

        //
        // POST: /SmtpEmail/Edit/5

        [HttpPost]
        public ActionResult Edit(EmailAccount emailaccount)
        {
            if (ModelState.IsValid)
            {
                emailaccount.LastVerifyDate = new DateTime(1990, 1, 1);
                db.Entry(emailaccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(emailaccount);
        }

        //
        // GET: /SmtpEmail/Delete/5

        public ActionResult Delete(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            return View(emailaccount);
        }

        //
        // POST: /SmtpEmail/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            EmailAccount emailaccount = db.EmailAccounts.Find(id);
            db.EmailAccounts.Remove(emailaccount);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DoSmokeTest()
        {
            SmokeTest();
            return Content("SMTP Email Smoke Test Finished");
        }

        private void SmokeTest()
        {
            int count = 10;
            List<string> bcc = new List<string>();
            bcc.Add("xlshezn@hotmail.com");
            bcc.Add("longsheng.xia@hotmail.com");
            bcc.Add("lasonxia@gmail.com");
            bcc.Add("yuanzhengfacss@gmail.com");
            bcc.Add("yuanzheng.utd@gmail.com");

            while (count-- > 0)
            {
                var emailClient = new SmtpEmail();
                emailClient.Subject = "This is a test";
                emailClient.Body = "This is a test";
                emailClient.Bcc = bcc;
                emailClient.Send();
                Thread.Sleep(1000);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}