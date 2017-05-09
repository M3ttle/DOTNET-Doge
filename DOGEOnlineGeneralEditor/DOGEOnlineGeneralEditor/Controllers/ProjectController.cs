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
        private GeneralService service;
        public ProjectController()
        {
            service = new GeneralService(null);
        }


        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectViewModel model = service.GetProjectViewModelByID(id.Value);
            if (model == null)
            {
                throw new ProjectNotFoundException();
            }
            if (service.HasAccess(User.Identity.Name, id.Value))
            {
                return View(model);
            }
            throw new UnauthorizedAccessToProjectException();
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.LanguageTypeID = service.GetLanguageTypes();
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
                if (service.ProjectExists(userName, model.Name))
                {
                    //Error duplicate project name
                    ModelState.AddModelError("", "You already have a project with that name");
                }
                else
                {
                    model.Owner = userName;
                    service.AddProjectToDatabase(model);
                    service.AddUserToProject(userName, model);
                    TempData["Success"] = string.Format("{0} has been created", model.Name);
                    return RedirectToAction("MyProjects", "Workspace", null);
                }
            }
            ViewBag.LanguageTypeID = service.GetLanguageTypes(model.LanguageTypeID);
            return View(model);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectViewModel project = service.GetProjectViewModelByID(id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageTypeID = service.GetLanguageTypes(project.LanguageTypeID);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectID,Name,Owner,IsPublic,LanguageTypeID")] ProjectViewModel project)
        {
            if (ModelState.IsValid)
            {
                ProjectViewModel oldProjectViewModel = service.GetProjectViewModelByID(project.ProjectID);
                if (service.ProjectExists(User.Identity.Name, project.Name)
                        && oldProjectViewModel.Name != project.Name)
                {
                    ModelState.AddModelError("", "You already have a project with that name");
                }
                else
                {
                    service.EditProject(project);
                    TempData["Success"] = string.Format("{0} was successfully edited.", project.Name);
                    return RedirectToAction("Details", "Project", new { ID = project.ProjectID });
                }

            }
            ViewBag.LanguageTypeID = service.GetLanguageTypes(project.LanguageTypeID);
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectViewModel project = service.GetProjectViewModelByID(id.Value);
            if (project == null)
            {
                return HttpNotFound();
            }
            if (project.Owner != User.Identity.Name)
            {
                // User is not projectect owner = not allowed to delete
                ModelState.AddModelError("", "You cannot delete a project you do now own");
                return RedirectToAction("MyProjects", "Workspace", null);
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectViewModel project = service.GetProjectViewModelByID(id);
            if (project.Owner != User.Identity.Name)
            {
                // User is not projectect owner = not allowed to delete
                ModelState.AddModelError("", "You cannot delete a project you do now own");
            }
            else
            {
                service.RemoveProject(id);
                TempData["Success"] = string.Format("The project has been deleted");
            }
            return RedirectToAction("MyProjects", "Workspace", null);
        }
        // Get: 
        [HttpGet]
        public ActionResult AddUserToProject(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(service.GetCollaboratorViewModel(User.Identity.Name, id.Value));
        }
        // Post:
        [HttpPost]
        public ActionResult AddUserToProject(FormCollection formCollection)
        {
            int userID;
            int projectID;
            int.TryParse(formCollection["UserID"], out userID);
            int.TryParse(formCollection["ProjectID"], out projectID);
            if (service.AddUserToProject(userID, projectID))
            {
                string collaboratorName = service.GetUserNameByID(userID);
                TempData["Success"] = string.Format("{0} was added to the project", collaboratorName);
                return RedirectToAction("AddUserToProject", new { ID = projectID });
            }
            // Some error happened if we got here
            return View(service.GetCollaboratorViewModel(User.Identity.Name, projectID));
        }

        [HttpPost]
        public ActionResult LeaveProject(FormCollection formCollection)
        {
            int userID = service.GetUserIDByName(User.Identity.Name);
            int projectID;
            if (int.TryParse(formCollection["ProjectID"], out projectID))
            {
                if (service.RemoveUserProject(userID, projectID))
                {
                    TempData["Success"] = "You have left the project";
                    return RedirectToAction("MyProjects", "Workspace");
                }
            }
            // Some error happened if we got here
            return RedirectToAction("MyProjects", "Workspace");
        }
    }
}
