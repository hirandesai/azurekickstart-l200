using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AzureKickStart.Models;
using System.Diagnostics;
using System.IO;
using AzureKickStart.Common;
using System.Threading.Tasks;

namespace AzureKickStart.Controllers
{
    public class PersonsController : Controller
    {
        private readonly StorageService storageService;
        private readonly QueueService queueService;


        public PersonsController()
        {
            var storageConnectionString = new ConfigurationService().GetConnectionStringValue("StorageConnectionString");

            this.storageService = new StorageService(storageConnectionString);
            this.queueService = new QueueService(storageConnectionString);
        }

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
        public async Task<ActionResult> Create([Bind(Include = "FullName,BirthDate")] Person person, HttpPostedFileBase file)
        {
            Trace.WriteLine("POST /Person/Create");
            if (ModelState.IsValid)
            {
                Person peron = db.Persons.Add(person);
                db.SaveChanges();
                if (peron != null)
                {
                    if (file.ContentLength > 0)
                    {
                        string containerName = peron.ID.ToString();
                        string _FileName = string.Format("{0}_{1}", Guid.NewGuid(), Path.GetFileName(file.FileName));
                        string fileURL = await storageService.UploadImageToAzureBlobStorageAsync(containerName, _FileName, ConvertToBytes(file), file.ContentType);

                        person.ImageURL = fileURL;
                        ResizeImageQueueRequest resizeImageQueueRequest = new ResizeImageQueueRequest() { ContainerName = containerName, FileName = _FileName };
                        await queueService.InsertMessage("resizeimagequeue", resizeImageQueueRequest);

                        db.SaveChanges();
                    }
                }

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

        private static byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            int fileSizeInBytes = file.ContentLength;
            byte[] data = null;
            using (var br = new BinaryReader(file.InputStream))
            {
                data = br.ReadBytes(fileSizeInBytes);
            }

            return data;
        }
    }
}