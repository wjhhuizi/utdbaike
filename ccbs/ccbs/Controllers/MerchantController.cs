using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ccbs.Models;

namespace ccbs.Controllers
{
    public class MerchantController : Controller
    {
        private WebModelContainer db = new WebModelContainer();

        //
        // GET: /Merchant/
        [Authorize(Roles = LWSFRoles.admin)]
        public ViewResult Index()
        {
            return View(db.Merchants.ToList());
        }

        public ActionResult _MerchantList()
        {
            var merchants = db.Merchants.OrderBy(m => m.LastUpdate).ToList();
            return View(merchants);
        }

        //
        // GET: /Merchant/Details/5

        public ViewResult Details(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            return View(merchant);
        }

        //
        // GET: /Merchant/Create
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create()
        {
            Merchant merchant = new Merchant
            {
                LastUpdate = DateTime.Now,
                DueDate = DateTime.Now,
            };
            return View(merchant);
        }

        //
        // POST: /Merchant/Create

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Create(Merchant merchant)
        {
            if (ModelState.IsValid)
            {
                db.Merchants.Add(merchant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(merchant);
        }

        //
        // GET: /Merchant/Edit/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            merchant.Description = Server.HtmlDecode(merchant.Description);
            return View(merchant);
        }

        //
        // POST: /Merchant/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Edit(Merchant merchant)
        {
            if (ModelState.IsValid)
            {
                merchant.LastUpdate = DateTime.Now;
                db.Entry(merchant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(merchant);
        }

        //
        // GET: /Merchant/Delete/5
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult Delete(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            return View(merchant);
        }

        //
        // POST: /Merchant/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = LWSFRoles.admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            db.Merchants.Remove(merchant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}