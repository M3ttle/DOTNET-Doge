using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOGEOnlineGeneralEditor.Models;
using DOGEOnlineGeneralEditor.Models.POCO;
using DOGEOnlineGeneralEditor.Models.ViewModels;
using DOGEOnlineGeneralEditor.Services;

namespace DOGEOnlineGeneralEditor.Controllers
{
    public class FileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
		private GeneralService service;

		public FileController()
		{
			service = new GeneralService(null);
		}

        // GET: Files
        public ActionResult Index()
        {
            var files = db.File.Include(f => f.LanguageType).Include(f => f.Project);
            return View(files.ToList());
        }

        // GET: Files/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.File.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // GET: File/Create
        public ActionResult Create(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.LanguageTypeID = new SelectList(db.LanguageType, "ID", "Name");

            CreateFileViewModel model = new CreateFileViewModel { ProjectID = id.Value };
            return View(model);
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateFileViewModel file)
        {
            if (ModelState.IsValid)
            {
                if(service.fileExists(file.ProjectID, file.Name))
                {
                    ModelState.AddModelError("", "A file with that name already exists in this project");
                    
                }
                else
                {
                    service.addFileToDatabase(file);
                    return RedirectToAction("Index");
                }
				
            }

            ViewBag.LanguageTypeID = service.getLanguageTypes();
            return View(file);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.File.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageTypeID = new SelectList(db.LanguageType, "ID", "Name", file.LanguageTypeID);
            ViewBag.ProjectID = new SelectList(db.Project, "ID", "Name", file.ProjectID);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,ProjectID,LanguageTypeID")] File file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageTypeID = new SelectList(db.LanguageType, "ID", "Name", file.LanguageTypeID);
            ViewBag.ProjectID = new SelectList(db.Project, "ID", "Name", file.ProjectID);
            return View(file);
        }

        // GET: Files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.File.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            File file = db.File.Find(id);
            db.File.Remove(file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Save(FileViewModel model)
        {
            service.saveFile(model);
            return RedirectToAction("Editor", "Workspace", new { ID = model.ID });
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
