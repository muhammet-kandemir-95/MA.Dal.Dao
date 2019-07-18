using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MA.Dal.Dao.WebSite.UI.Controllers
{
    public class MADaoExamplesController : Controller
    {
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult LinqSelect()
        {
            ViewBag.Title = "Linq Select";
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult LinqWhere()
        {
            ViewBag.Title = "Linq Where";
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult LinqJoin()
        {
            ViewBag.Title = "Linq Join";
            return View();
        }
    }
}