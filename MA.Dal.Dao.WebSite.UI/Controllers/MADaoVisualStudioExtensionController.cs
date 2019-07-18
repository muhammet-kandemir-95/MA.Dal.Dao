using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MA.Dal.Dao.WebSite.UI.Controllers
{
    public class MADaoVisualStudioExtensionController : Controller
    {
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Install()
        {
            ViewBag.Title = "Install";
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult HowToUse()
        {
            ViewBag.Title = "How To Use";
            return View();
        }
    }
}