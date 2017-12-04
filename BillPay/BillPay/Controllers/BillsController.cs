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
    [Authorize]
    public class BillsController : ApplicationBaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Bills
        public ActionResult Index(string sortOrder)
        {
            var bills = db.Bills.Include(b => b.User);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.CostSortParm = sortOrder == "Cost" ? "Cost_desc" : "Cost";
            switch (sortOrder)
            {
                case "Name_desc":
                    bills = bills.OrderByDescending(b => b.Name);
                    break;
                case "Date":
                    bills = bills.OrderBy(b => b.DueDate);
                    break;
                case "Date_desc":
                    bills = bills.OrderByDescending(b => b.DueDate);
                    break;
                case "Cost":
                    bills = bills.OrderBy(b => b.Cost);
                    break;
                case "Cost_desc":
                    bills = bills.OrderByDescending(b => b.Cost);
                    break;
                default:
                    bills = bills.OrderBy(b => b.Name);
                    break;
            }

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
            string[] colors = new string[] { "Red", "Blue", "Green", "Yellow", "Silver", "Black", "Teal", "Purple" };
            ViewBag.Colors = new SelectList(colors);
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

                DateTime date = new DateTime(bill.DueDate.Year, bill.DueDate.Month, bill.DueDate.Day);

                for (int i = 0; i <= 11; i++)
                {

                    bill.DueDate = date.AddMonths(i);
                    db.Bills.Add(bill);
                    db.SaveChanges();
                }
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
            string[] colors = new string[] { "Red", "Blue", "Green", "Yellow", "Silver", "Black", "Teal", "Purple" };
            ViewBag.Colors = new SelectList(colors);
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
                bill.UserID = User.Identity.GetUserId();
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
            var bills = db.Bills.Where(x => x.UserID == id && x.DueDate > from && x.DueDate < to).OrderBy(x => x.DueDate).ThenBy(x => x.Name).ToList();
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
                    id = b.BillID,
                    title = b.Name,
                    description = b.Website,
                    start = b.DueDate,
                    color = b.Color,
                    cost = b.Cost
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
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public DateTime start { get; set; }
    public string color { get; set; }
    public decimal cost { get; set; }
}
