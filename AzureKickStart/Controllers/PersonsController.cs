using AzureKickStart.Models;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AzureKickStart.Controllers
{
    public class PersonsController : Controller
    {
        private MyDatabaseContext db = new MyDatabaseContext();

        // GET: Persons
        public ActionResult Index()
        {            
            Trace.WriteLine("GET /Person/Index");
            return View(db.Persons.ToList());
        }

        // GET: Persons/Details/5
        public ActionResult Details(int? id)
        {
            Trace.WriteLine("GET /Person/Details/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: persons/Create
        public ActionResult Create()
        {
            Trace.WriteLine("GET /Person/Create");
            return View(new Person { BirthDate = DateTime.Now.AddYears(-20) });
        }

        // POST: persons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FullName,BirthDate")] Person person)
        {
            Trace.WriteLine("POST /Person/Create");
            if (ModelState.IsValid)
            {
                db.Persons.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: persons/Edit/5
        public ActionResult Edit(int? id)
        {
            Trace.WriteLine("GET /Person/Edit/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: persons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,FullName,BirthDate")] Person person)
        {
            Trace.WriteLine("POST /Person/Edit/" + person.ID);
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: persons/Delete/5
        public ActionResult Delete(int? id)
        {
            Trace.WriteLine("GET /Person/Delete/" + id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.Persons.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: persons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trace.WriteLine("POST /Person/Delete/" + id);
            Person person = db.Persons.Find(id);
            db.Persons.Remove(person);
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
    }
}
