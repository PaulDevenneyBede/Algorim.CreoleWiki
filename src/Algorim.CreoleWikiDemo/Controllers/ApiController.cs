using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Algorim.CreoleWiki;

namespace Algorim.CreoleWikiDemo.Controllers
{
    public class ApiController : Controller
    {
        public ActionResult Wiki(string markup = "")
        {
			var html = new CreoleParser().Parse(markup);

            return Json(html, JsonRequestBehavior.AllowGet);
        }
    }
}
