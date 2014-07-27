using System.Linq;
using System.Web.Mvc;
using ccbs.Models;
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using ccbs.Helpers;
using System.Globalization;

namespace ccbs.Controllers
{
    public class HomeController : BaseController
    {
        private WebModelContainer db = new WebModelContainer();

        public ActionResult Index()
        {
            var homeGalleries = db.HomeGalleries.OrderBy(h => h.Order).ToList();
            ViewBag.homeGalleries = homeGalleries;
            return View();
        }

        public ActionResult RegisterPage()
        {
            return View();
        }

        public ActionResult _onCalendarChange(string date)
        {
            if (date == "undefined")
            {
                date = DateTime.Today.ToString("MM/dd/yyyy");
            }
            DateTime selDate = DateTime.Parse(date);
            ViewBag.selDate = date;
            var al = db.Activities.OrderBy(a => a.TimeFrom).ToList();
            var selActivities = al.Where(a => a.TimeFrom.Date == selDate.Date).ToList();
            return View(selActivities);
        }

        public ActionResult test()
        {
            var homeGalleries = db.HomeGalleries.OrderBy(h => h.Order).ToList();
            ViewBag.homeGalleries = homeGalleries;

            List<DateTime> eventDateList = new List<DateTime>();
            var al = db.Activities.OrderBy(a => a.TimeFrom).ToList();

            var today = DateTime.Now;
            var thisMonth = today.AddDays(30);
            var comingEvents = al.Where(a => (a.TimeFrom > today) && (a.TimeFrom < thisMonth)).ToList();

            return View(comingEvents);
        }

        public ActionResult Language(string language, string returnUrl)
        {
            SessionHelper.Culture = new CultureInfo(language);

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

        public ActionResult Download()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult UnauthorizedError()
        {
            return View();
        }

        public ActionResult Sidebar(DateTime date)
        {
            var activities = db.Activities.Where(a => a.TimeFrom.Date == DateTime.Today).OrderBy(a => a.TimeFrom).ToList();
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
