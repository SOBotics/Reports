using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Reports.Dependencies.ReportStore;

namespace Reports.Controllers
{
	public class ReportController : Controller
	{
		private readonly IReportStore reports;

		public ReportController(IReportStore reportStore)
		{
			reports = reportStore;
		}

		[Route("/report/{id:regex(^[[a-zA-Z0-9]]{{6}}$)}")]
		[Route("/r/{id:regex(^[[a-zA-Z0-9]]{{6}}$)}")]
		public IActionResult ViewReport(string id)
		{
			if (!reports.Exists(id))
			{
				return NotFound();
			}

			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ServerVersion"] = ViewData["LocalCommit"].ToString().Substring(0, 5);

			return View(reports[id]);
		}
	}
}