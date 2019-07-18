using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MA.Dal.Dao.WebSite.UI.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Index()
        {
            ViewBag.Title = "About";
            return View();
        }
    }
}