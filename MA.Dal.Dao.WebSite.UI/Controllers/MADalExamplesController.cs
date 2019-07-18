using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MA.Dal.Dao.WebSite.UI.Controllers
{
    public class MADalExamplesController : Controller
    {
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult ConnectionOpenOrClose()
        {
            ViewBag.Title = "Connection Open or Close";
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult CommandExecuteNonQuery()
        {
            ViewBag.Title = "Command ExecuteNonQuery";
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult CommandExecuteReader()
        {
            ViewBag.Title = "Command ExecuteReader";
            return View();
        }

        [OutputCache(Duration = int.MaxValue)]
        public ActionResult DataAdapterFill()
        {
            ViewBag.Title = "DataAdapter Fill";
            return View();
        }
    }
}