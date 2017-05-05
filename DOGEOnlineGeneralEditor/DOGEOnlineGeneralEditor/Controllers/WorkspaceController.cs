using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DOGEOnlineGeneralEditor.Services;

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
        // GET: Workspace
        public ActionResult Index()
        {
            return View();
        }
 
 		public ActionResult Editor()
 		{
 			return View();
 		}
 
 		public ActionResult MyProjects()
 		{
            var userName = User.Identity.GetUserName();
            var model = service.getMyProjectsByName(userName); 
            return View(model);
 		}
 
 		public ActionResult PublicProjects()
 		{
 			return View();
 		}
 	}
}