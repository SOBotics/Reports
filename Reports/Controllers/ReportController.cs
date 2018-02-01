﻿using System;
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
			var v = ThisAssembly.Git.Sha.ToUpperInvariant().Substring(0, 5);

			if (ThisAssembly.Git.IsDirty)
			{
				v += "+";
			}

			ViewData["ServerVersion"] = v;
			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ReportOwner"] = "OpenReports";
			ViewData["ReportID"] = id;

			return View();
		}
	}
}