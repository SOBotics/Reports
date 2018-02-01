using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Controllers
{
	public class ReportController : Controller
	{
		[Route("/report/{id:regex(^[[a-zA-Z0-9]]{{6}}$)}")]
		[Route("/r/{id:regex(^[[a-zA-Z0-9]]{{6}}$)}")]
		public IActionResult ViewReport(string id)
		{
			//TODO: Check report exists.

			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ServerVersion"] = ViewData["LocalCommit"].ToString().Substring(0, 5);
			ViewData["ReportID"] = id;

			return View();
		}
	}
}