using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DOGEOnlineGeneralEditor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About DOGE";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact DOGE";

            return View();
        }
    }
}