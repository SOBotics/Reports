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
			var v = ThisAssembly.Git.Sha.ToUpperInvariant().Substring(0, 5);

			if (ThisAssembly.Git.IsDirty)
			{
				v += "+";
			}
			
			ViewData["ServerVersion"] = v;
			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;

			return View();
		}
	}
}
