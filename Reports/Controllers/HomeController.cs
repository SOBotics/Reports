using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Controllers
{
	public class HomeController : Controller
	{
		[Route("/")]
		[Route("/home")]
		[Route("/index")]
		public IActionResult Index()
		{
			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ServerVersion"] = ViewData["LocalCommit"].ToString().Substring(0, 5);

			return View();
		}
	}
}
