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
using System.IO;

namespace DOGEOnlineGeneralEditor.Controllers
{
    [Authorize]
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

        // GET: File/Create
        public ActionResult Create(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.LanguageTypeID = service.getLanguageTypes();

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
                    return RedirectToAction("Details", "Project", new { ID = file.ProjectID});
                }
            }

            ViewBag.LanguageTypeID = service.getLanguageTypes(file.LanguageTypeID);
            return View(file);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromFile(CreateFileFromFileViewModel file)
        {
            if(ModelState.IsValid)
            {
                if (file.PostedFile != null)
                {
                    using (StreamReader sr = new StreamReader(file.PostedFile.InputStream))
                    {
                        file.Data = sr.ReadToEnd();
                    }

                    if (service.fileExists(file.ProjectID, file.PostedFile.FileName))
                    {
                        ModelState.AddModelError("", "A file with that name already exists in this project");
                    }
                    else
                    {
                        service.addFileToDatabase(file);
                        return RedirectToAction("Index");
                    }
                }
            }
            ViewBag.LanguageTypeID = service.getLanguageTypes(file.LanguageTypeID);
            return View("Create");
        }

        // POST: File/Delete/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? fileID)
        {
            if(fileID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(service.fileExists(fileID.Value))
            {
                service.removeFile(fileID.Value);
                return RedirectToAction("Index");
            }
            //File does not exist exception!
            throw new Exception();
        }

        // POST: File/Save/2
        [HttpPost]
        public ActionResult Save(FileViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(service.fileExists(model.ProjectID, model.Name))
                {
                    ModelState.AddModelError("", "A file with that name already exists in this project");
                }
                else
                {
                    service.saveFile(model);
                }
            }
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
