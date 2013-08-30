using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;

namespace ccbs.Controllers
{
    public class ManageController : Controller
    {
        //
        // GET: /Manage/

        private StudentModelContainer db = new StudentModelContainer();

        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Index()
        {
            var orgs = db.Organizations.ToList();
            ViewBag.DropDownList_Organizations = new SelectList(orgs, "Id", "Name");
            return View();
        }

        [Authorize(Roles = LWSFRoles.organizationLeader)]
        public ActionResult VoluteerGroup()
        {
            return View();
        }
    }
}
