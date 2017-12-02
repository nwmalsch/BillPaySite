using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BillPay.Models;
using Microsoft.AspNet.Identity;

namespace BillPay.Controllers
{
    public class BillsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bills
        public ActionResult Index()
        {
            var bills = db.Bills.Include(b => b.User);
            return View(bills.ToList());
        }

        // GET: Bills/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        // GET: Bills/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Bills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BillID,Name,Website,Cost,DueDate,Color,UserID")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                bill.UserID = User.Identity.GetUserId();
                db.Bills.Add(bill);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", bill.UserID);
            return View(bill);
        }

        // GET: Bills/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", bill.UserID);
            return View(bill);
        }

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BillID,Name,Website,Cost,DueDate,Color,UserID")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bill).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "Id", "Email", bill.UserID);
            return View(bill);
        }

        // GET: Bills/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill bill = db.Bills.Find(id);
            if (bill == null)
            {
                return HttpNotFound();
            }
            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bill bill = db.Bills.Find(id);
            db.Bills.Remove(bill);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Calculator()
        {
            string id = User.Identity.GetUserId();
            var bills = db.Bills.Where(x => x.UserID == id).ToList();
            ViewBag.billList = null;
            ViewBag.budget = 0;
            return View();
        }

        [HttpPost]
        public ActionResult Calculator(DateTime from, DateTime to)
        {
            string id = User.Identity.GetUserId();
            ViewBag.from = from;
            ViewBag.to = to;
            var bills = db.Bills.Where(x => x.UserID == id && x.DueDate > from && x.DueDate < to).ToList();
            decimal budget = 0;

            foreach (var b in bills)
            {
                budget += b.Cost;
            }
            ViewBag.budget = budget;
            ViewBag.billList = bills;


            return View();
        }

        public ActionResult Calendar()
        {
            return View();
        }
        public JsonResult GetBills()
        {
            string id = User.Identity.GetUserId();
            var bills = db.Bills.ToList().Where(b => b.UserID == id);
            var events = new List<Events>();

            foreach (Bill b in bills)
            {
                Events e = new Events
                {
                    title = b.Name,
                    description = b.Website,
                    start = b.DueDate,
                    color = b.Color
                };
                events.Add(e);
            }
            var rows = events.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
    }
}
public class Events
{
    public string title { get; set; }
    public string description { get; set; }
    public DateTime start { get; set; }
    public string color { get; set; }
}
