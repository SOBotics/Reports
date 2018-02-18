using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Services.LocalData.Reports;

namespace Reports.Controllers
{
	[Route("/[action]")]
	public class HomeController : Controller
	{
		private readonly IReportStore reportStore;



		public HomeController(IReportStore rs)
		{
			reportStore = rs;
		}



		[Route("/")]
		[Route("/home")]
		public IActionResult Index()
		{
			ViewData["Sha"] = ThisAssembly.Git.Sha;

			return View();
		}

		public IActionResult Stats()
		{
			var stats = new Dictionary<string, AppStats>();
			var reports = new Dictionary<string, List<DateTime>>();

			// Calculate total views and total reports
			foreach (var id in reportStore.IDs)
			{
				var r = reportStore.Get(id);

				if (!reports.ContainsKey(r.AppName))
				{
					reports[r.AppName] = new List<DateTime>();
				}

				reports[r.AppName].Add(r.CreatedAt.Date);

				if (!stats.ContainsKey(r.AppName))
				{
					stats[r.AppName] = new AppStats
					{
						Name = r.AppName,
						AppURL = r.AppURL
					};
				}

				stats[r.AppName].LiveReports++;
				stats[r.AppName].TotalViews += r.Views;
			}

			// Calculate views/report and reports/day
			foreach (var app in stats.Keys)
			{
				if (stats[app].TotalViews > 0)
				{
					var vpr = stats[app].TotalViews * 1.0 / stats[app].LiveReports;

					stats[app].ViewsPerReport = Math.Round(vpr, 1);
				}
				var days = reports[app].GroupBy(x => x);
				stats[app].ReportsPerDay = days.Average(x => x.Count());
			}

			ViewData["Sha"] = ThisAssembly.Git.Sha;

			return View(stats);
		}
	}
}
