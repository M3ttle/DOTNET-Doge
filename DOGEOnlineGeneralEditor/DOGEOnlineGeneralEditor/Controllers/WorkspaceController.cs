﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOGEOnlineGeneralEditor.Controllers
{
    public class WorkspaceController : Controller
    {
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
 			return View();
 		}
 
 		public ActionResult PublicProjects()
 		{
 			return View();
 		}
 	}
}