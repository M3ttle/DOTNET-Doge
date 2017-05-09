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
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(service.HasAccess(User.Identity.Name, id.Value) == false)
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
        public ActionResult Create(CreateFileViewModel file)
        {
            if (ModelState.IsValid)
            {
                if(service.HasAccess(User.Identity.Name, file.ProjectID) == false)
                {
                    throw new UnauthorizedAccessException();
                }
                else if(service.FileExists(file.ProjectID, file.Name))
                {
                    ModelState.AddModelError("", "A file with that name already exists in this project");
                }
                else
                {
                    service.AddFileToDatabase(file);
                    return RedirectToAction("Details", "Project", new { ID = file.ProjectID});
                }
            }

            ViewBag.LanguageTypeID = service.GetLanguageTypes(file.LanguageTypeID);
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
                        string rawInput = sr.ReadToEnd();
                        string encoded = Server.HtmlEncode(rawInput);
                        file.Data = encoded;
                    }

                    if (service.FileExists(file.ProjectID, file.PostedFile.FileName))
                    {
                        ModelState.AddModelError("", "A file with that name already exists in this project");
                    }
                    else
                    {
                        service.AddFileToDatabase(file);
                        TempData["Success"] = string.Format("{0} has been added to the project.", file.PostedFile.FileName);
                        return RedirectToAction("Details", "Project", new { ID = file.ProjectID });
                    }
                }
            }
            ViewBag.LanguageTypeID = service.GetLanguageTypes(file.LanguageTypeID);
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

            if(service.FileExists(fileID.Value))
            {
                int projectID = service.GetFileProjectID(fileID.Value);
                service.RemoveFile(fileID.Value);
                return RedirectToAction("Details", "Project", new { ID = projectID});
            }
            //File does not exist exception!
            throw new Exception();
        }

        // POST: File/Save/2
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EditorViewModel model)
        {
            string encoded = Server.HtmlEncode(model.Data);
            model.Data = encoded;
            if (ModelState.IsValid)
            {
                if(service.FileExists(model.ProjectID, model.Name))
                {
                    // File Exists!!
                }
                else
                {
                    service.SaveFile(model);
                    int userID = service.GetUserIDByName(User.Identity.Name);
                    if (model.UserThemeID != service.GetUserTheme(userID))
                    {
                        service.UpdateUserTheme(userID, model.UserThemeID);
                    }
                }
            }

            return RedirectToAction("Editor", "Workspace", new { ID = model.ID });
        }
    }
}
