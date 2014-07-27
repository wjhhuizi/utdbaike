using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using ccbs.Models;

namespace utdbaike
{
    public class SmtpEmail
    { // all emails will be sent by bcc
        public int Count { get; private set; }
        public List<string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public int FailedCount { get; private set; }
        public string ErrorAccounts { get; private set; }

        public SmtpEmail()
        {
            this.Count = 0;
            this.FailedCount = 0;
            this.Bcc = new List<string>();
        }

        public bool ErrorExist()
        {
            return (FailedCount > 0);
        }

        public void Send()
        {
            var plan = getPlan();
            var web_db = new WebModelContainer();
            foreach (var p in plan)
            {
                var acct = web_db.EmailAccounts.Find(p.acct.Id);
                var ret = send(p);
                if ("success" != ret)
                {
                    // acct.Verified = false;
                    this.FailedCount += p.msg.Bcc.Count;
                    this.ErrorAccounts += ", " + p.acct.Name + ":" + ret + ": " + this.FailedCount.ToString();
                }

                if (acct != null)
                {
                    var dailyCount = acct.DailyCounts.Where(d => d.WhichDate == DateTime.Today).SingleOrDefault();
                    if (dailyCount == null)
                    {
                        dailyCount = new DailyCount
                        {
                            WhichDate = DateTime.Today,
                            Count = 0,
                            EmailAccount = acct,
                            EmailAccountId = acct.Id,
                        };
                        web_db.DailyCounts.Add(dailyCount);
                    }
                    dailyCount.Count += p.msg.Bcc.Count;
                }
                this.Count += p.msg.Bcc.Count();
            }
            web_db.SaveChanges();
            web_db.Dispose();
        }

        private static string send(Pair p)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient
                    {
                        Host = p.acct.Host,
                        Port = p.acct.Port,
                        Credentials = new System.Net.NetworkCredential(p.acct.Username, p.acct.Password),
                    };

                smtpClient.EnableSsl = true;  //must be here, otherwise there will be execption
                smtpClient.Send(p.msg);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "success";
        }

        private List<Pair> getPlan()
        {
            var plan = new List<Pair>();

            var web_db = new WebModelContainer();
            var emailAccounts = web_db.EmailAccounts.ToList();

            int pos = 0;
            int total = Bcc.Count;
            foreach (var acct in emailAccounts)
            {
                if (!acct.Verified)
                {
                    continue;
                }
                var dailyCount = acct.DailyCounts.Where(d => d.WhichDate == DateTime.Today).SingleOrDefault();
                if (dailyCount == null)
                {
                    dailyCount = new DailyCount
                        {
                            WhichDate = DateTime.Today,
                            Count = 0,
                            EmailAccount = acct,
                            EmailAccountId = acct.Id,
                        };
                    web_db.DailyCounts.Add(dailyCount);
                    web_db.SaveChanges();
                }
                if (dailyCount.Count >= acct.SmtpDailyLimit)
                {
                    continue;
                }

                var msg = new MailMessage();
                int perTimeCount = 0;
                while ((pos < total) && (dailyCount.Count < acct.SmtpDailyLimit) && (perTimeCount < acct.SmtpPerTimeLimit))
                {
                    msg.Bcc.Add(Bcc.ElementAt(pos));
                    pos++;
                    dailyCount.Count++;
                    perTimeCount++;
                }

                msg.Subject = Subject;
                msg.Body = Body;
                msg.IsBodyHtml = true;
                msg.From = new MailAddress(acct.From);
                plan.Add(new Pair(acct, msg));

                if (pos == total)
                {
                    break;
                }
            }
            web_db.Dispose();
            return plan;
        }

        public const int VERIFY_INTERVAL = 2; // 2 days

        public static string ValidateSmtpEmailAccount(EmailAccount account)
        {
            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress(account.From);
            message.From = fromAddress;
            message.Subject = "SMTP Email Account verification";
            message.IsBodyHtml = true;
            message.Body = "This is to test email account: " + account.From;
            message.To.Add("xlshezn@hotmail.com");
            var pair = new Pair(account, message);

            var ret = send(pair);
            return ret;
        }

        private class Pair
        {
            public EmailAccount acct { get; set; }
            public MailMessage msg { get; set; }

            public Pair(EmailAccount a, MailMessage m)
            {
                acct = a;
                msg = m;
            }
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