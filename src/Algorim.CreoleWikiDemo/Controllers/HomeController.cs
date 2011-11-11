using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Algorim.CreoleWiki;
using Algorim.CreoleWikiDemo.Models;

namespace Algorim.CreoleWikiDemo.Controllers
{
    public class HomeController : Controller
    {
		public ActionResult Index()
		{
			var model = new HomeModel()
			{
				Markup = @"= Creole markup =

Enter your **wiki** markup //here//"
			};
			model.Wiki = new CreoleParser().Parse(model.Markup);

			return View(model);
		}
    }
}
