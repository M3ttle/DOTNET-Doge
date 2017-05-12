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
using DOGEOnlineGeneralEditor.Utilities;

namespace DOGEOnlineGeneralEditor.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        private GeneralService service;

        public FileController()
        {
            service = new GeneralService(null);
        }


        // GET: File/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                throw new CustomProjectNotFoundException();
            }
            if (service.HasAccess(User.Identity.Name, id.Value) == false)
            {
                throw new UnauthorizedAccessToProjectException();
            }

            ViewBag.LanguageTypeID = service.GetLanguageTypes();

            CreateFileViewModel model = new CreateFileViewModel { ProjectID = id.Value };
            return View(model);
        }

        // POST: File/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name, ProjectID, LanguageTypeID")] CreateFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (service.HasAccess(User.Identity.Name, model.ProjectID) == false)
                {
                    throw new UnauthorizedAccessException();
                }
                else if (service.FileExists(model.ProjectID, model.Name))
                {
                    ModelState.AddModelError("Name", "A file with that name already exists in this project");
                }
                else
                {
                    service.CreateFile(model);
                    return RedirectToAction("Details", "Project", new { ID = model.ProjectID});
                }
            }

            ViewBag.LanguageTypeID = service.GetLanguageTypes(model.LanguageTypeID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFromFile([Bind(Include = "Name, ProjectID, LanguageTypeID, postedFile")]CreateFileFromFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (StreamReader sr = new StreamReader(model.PostedFile.InputStream))
                {
                    string rawInput = sr.ReadToEnd();
                    string encoded = Server.HtmlEncode(rawInput);
                    model.Data = encoded;
                }

                if (service.FileExists(model.ProjectID, model.PostedFile.FileName))
                {
                    ModelState.AddModelError("LanguageTypeID", "A file with that name already exists in this project");
                }
                else if (!service.MIMETypeIsValid(model.PostedFile.ContentType))
                {
                    ModelState.AddModelError("LanguageTypeID", "I'm sorry that extension is not supported");
                }
                else
                {
                    service.CreateFile(model);
                    TempData["Success"] = string.Format("{0} has been added to the project.", model.PostedFile.FileName);
                    return RedirectToAction("Details", "Project", new { ID = model.ProjectID });
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select a file.");
            }

            ViewBag.LanguageTypeID = service.GetLanguageTypes(model.LanguageTypeID);
            CreateFileViewModel newModel = new CreateFileViewModel { ProjectID = model.ProjectID};
            return View("Create", newModel);
        }

        // POST: File/Delete/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                if (service.FileExists(id.Value))
                {
                    int projectID = service.GetProjectIDOfFile(id.Value);

                    if (service.HasAccess(User.Identity.Name, projectID) == false)
                    {
                        throw new UnauthorizedAccessException();
                    }

                    service.RemoveFile(id.Value);
                    return RedirectToAction("Details", "Project", new { ID = projectID});
                }
            }
            throw new CustomFileNotFoundException();
        }

        // POST: File/Save/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EditorViewModel model)
        {
            string encoded = Server.HtmlEncode(model.Data);
            model.Data = encoded;
            if (ModelState.IsValid)
            {
                if (service.FileExists(model.ProjectID, model.Name, model.ID))
                {
                    return Json(new { success = false, responseText = "A file with that name already exists in this project" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    service.SaveFile(model);
                    int userID = service.GetUserID(User.Identity.Name);
                    if (model.UserThemeID != service.GetUserTheme(userID))
                    {
                        service.UpdateUserTheme(userID, model.UserThemeID);
                    }
                    return Json(new { success = true, responseText = "Saved" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, responseText = "The filename must be between 3-30 characters long" }, JsonRequestBehavior.AllowGet);
        }
    }
}
