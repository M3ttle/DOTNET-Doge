using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOGEOnlineGeneralEditor.Services;
using DOGEOnlineGeneralEditor.Utilities;

namespace DOGEOnlineGeneralEditor.Controllers
{
    [Authorize]
    public class WorkspaceController : Controller
    {
        private GeneralService service;

        public WorkspaceController()
        {
            service = new GeneralService(null);
        }
 
 		public ActionResult Editor(int? id)
 		{
            if(id == null)
            {
                throw new FileNotFoundException();
            }
            var model = service.GetEditorViewModel(User.Identity.Name, id.Value);

            if(service.HasAccess(User.Identity.Name, model.ProjectID) == false)
            {
                throw new UnauthorizedAccessException();
            }
            ViewBag.LanguageTypeID = service.GetLanguageTypes(model.LanguageTypeID);
            ViewBag.UserThemeID = service.GetAceThemes(model.UserThemeID);
            string decoded = Server.HtmlDecode(model.Data);
            model.Data = decoded;
            return View(model);
 		}
 
 		public ActionResult MyProjects()
 		{
            var userName = User.Identity.GetUserName();
            var model = service.GetMyProjectsByName(userName); 
            return View(model);
 		}
 
 		public ActionResult PublicProjects()
 		{
            var model = service.GetPublicProjects();
 			return View(model);
 		}
 	}
}