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
                return View("ProjectNotFoundError");
            }
            ProjectViewModel model = service.GetProjectViewModel(id.Value);
            if (model == null)
            {
                throw new CustomProjectNotFoundException();
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
                    service.CreateProject(model);
                    service.AddUserToProject(userName, model);
                    TempData["Success"] = string.Format("{0} has been created", model.Name);
                    int theUserID = service.GetUserID(User.Identity.Name);
                    int projectID = service.GetProjectID(theUserID, model.Name);
                    return RedirectToAction("Details", "Project", new { id = projectID });
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
                return View("Error");
            }
            ProjectViewModel project = service.GetProjectViewModel(id.Value);
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
        public ActionResult Edit([Bind(Include = "ID,Name,Owner,IsPublic,LanguageTypeID")] ProjectViewModel project)
        {
            if (ModelState.IsValid)
            {
                ProjectViewModel oldProjectViewModel = service.GetProjectViewModel(project.ID);
                if (service.ProjectExists(User.Identity.Name, project.Name)
                        && oldProjectViewModel.Name != project.Name)
                {
                    ModelState.AddModelError("", "You already have a project with that name");
                }
                else
                {
                    service.UpdateProject(project);
                    TempData["Success"] = string.Format("{0} was successfully edited.", project.Name);
                    return RedirectToAction("Details", "Project", new { ID = project.ID });
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
                return View("Error");
            }
            ProjectViewModel project = service.GetProjectViewModel(id.Value);
            if (project == null)
            {
                return View("ProjectNotFoundError");
            }
            if (project.Owner != User.Identity.Name)
            {
                // User is not projectect owner = not allowed to delete
                ModelState.AddModelError("", "You cannot delete a project you do not own");
                return RedirectToAction("MyProjects", "Workspace", null);
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectViewModel project = service.GetProjectViewModel(id);
            if (project.Owner != User.Identity.Name)
            {
                // User is not projectect owner = not allowed to delete
                throw new UnauthorizedAccessToProjectException();
            }
            else
            {
                service.RemoveProject(id);
            }
            return RedirectToAction("MyProjects", "Workspace", null);
        }
        // Get: 
        [HttpGet]
        public ActionResult AddUserToProject(int? id)
        {
            if (id == null)
            {
                return View("ProjectNotFoundError");
            }
            if (service.HasAccess(User.Identity.Name, id.Value) == false)
            {
                throw new UnauthorizedAccessToProjectException();
            }
            return View(service.GetCollaboratorViewModel(User.Identity.Name, id.Value));
        }
        // Post
        [HttpPost]
        public ActionResult AddUserToProject(FormCollection formCollection)
        {
            int.TryParse(formCollection["UserID"], out int userID);
            int.TryParse(formCollection["ProjectID"], out int projectID);
            if (service.AddUserToProject(userID, projectID))
            {
                string collaboratorName = service.GetUserName(userID);
                TempData["Success"] = string.Format("{0} was added to the project", collaboratorName);
                return RedirectToAction("AddUserToProject", new { ID = projectID });
            }
            // Some error happened if we got here
            return View(service.GetCollaboratorViewModel(User.Identity.Name, projectID));
        }

        [HttpPost]
        public ActionResult LeaveProject(FormCollection formCollection)
        {
            int userID = service.GetUserID(User.Identity.Name);

            if (int.TryParse(formCollection["ProjectID"], out int projectID))
            {
                if (service.RemoveUserProject(userID, projectID))
                {
                    return RedirectToAction("MyProjects", "Workspace");
                }
            }
            // Some error happened if we got here
            return RedirectToAction("MyProjects", "Workspace");
        }
    }
}
