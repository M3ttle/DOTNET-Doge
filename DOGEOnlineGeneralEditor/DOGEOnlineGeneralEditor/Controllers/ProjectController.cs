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
using DOGEOnlineGeneralEditor.Utilities;

namespace DOGEOnlineGeneralEditor.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private GeneralService service;
        public ProjectController()
        {
            service = new GeneralService(null);
        }

        // GET: Projects
        public ActionResult Index()
        {
            var projects = db.Project.Include(p => p.LanguageType);
            return View(projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectViewModel model = service.getProjectViewModelByID(id.Value);
            if(model == null)
            {
                throw new ProjectNotFoundException();
            }
            if(service.hasAccess(User.Identity.Name, id.Value))
            {
                return View(model);
            }
            throw new UnauthorizedAccessToProjectException();
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.LanguageTypeID = service.getLanguageTypes();
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,IsPublic,LanguageTypeID")] ProjectViewModel model)
        {
            var userName = User.Identity.Name;
            
            if (ModelState.IsValid)
            {
                if (service.projectExists(userName, model.Name))
                {
                    //Error duplicate project name
                    ModelState.AddModelError("", "You already have a project with that name");
                }
                else
                { 
                    model.Owner = userName;
                    service.addProjectToDatabase(model);
                    service.addUserToProject(userName, model);
                    return RedirectToAction("MyProjects", "Workspace",null);
                }
            }
            ViewBag.LanguageTypeID = service.getLanguageTypes(model.LanguageTypeID);
            return View(model);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageTypeID = new SelectList(db.LanguageType, "ID", "Name", project.LanguageTypeID);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,OwnerID,IsPublic,LanguageTypeID")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageTypeID = new SelectList(db.LanguageType, "ID", "Name", project.LanguageTypeID);
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectViewModel project = service.getProjectViewModelByID(id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (project.Owner != User.Identity.Name)
            {
                // User is not projectect owner = not allowed to delete
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectViewModel project = service.getProjectViewModelByID(id);
            if (project.Owner != User.Identity.Name)
            {
                // User is not projectect owner = not allowed to delete
            }
            service.removeProject(id);
            return RedirectToAction("Index");
        }
        // Get: 
        [HttpGet]
        public ActionResult AddUserToProject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(service.getCollaboratorViewModel(id.Value));
        }
        // Post:
        [HttpPost]
        public ActionResult AddUserToProject(FormCollection formCollection)
        {
            int userID;
            int projectID;
            int.TryParse(formCollection["UserID"], out userID);
            int.TryParse(formCollection["ProjectID"], out projectID);
            if (service.addUserToProject(userID, projectID))
            {
                ViewBag.Success = "User was added to the project";
                return RedirectToAction("Details/" + projectID.ToString());
            }
            // Some error happened if we got here
            return View(service.getCollaboratorViewModel(projectID));
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
