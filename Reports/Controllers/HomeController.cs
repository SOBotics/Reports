using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Models;
using Reports.Services.LocalData.MetaStats;
using Reports.Services.LocalData.Reports;

namespace Reports.Controllers
{
	[Route("/[action]")]
	public class HomeController : Controller
	{
		private readonly IReportStore reportStore;
		private readonly IMetaStatStore metaStatStore;



		public HomeController(IReportStore rs, IMetaStatStore mss)
		{
			reportStore = rs;
			metaStatStore = mss;
		}

		[Route("/")]
		[Route("/home")]
		public IActionResult Index()
		{
			ViewData["sha"] = ThisAssembly.Git.Sha;

			return View();
		}

		public IActionResult Stats()
		{
			ViewData["appStats"] = GetAppStats();
			ViewData["serverStats"] = GetServerStats();
			ViewData["sha"] = ThisAssembly.Git.Sha;

			return View();
		}

		private Dictionary<string, ResponseTypeStats> GetServerStats()
		{
			var data = metaStatStore.GetData<Dictionary<string, HashSet<RequestResponseStat>>>("responseStats");
			var stats = new Dictionary<string, ResponseTypeStats>();
			var responseTypes = new[]
			{
				MetaStatStore.ApiStatsKey,
				MetaStatStore.StaticFileStatsKey,
				MetaStatStore.DynamicViewStatsKey
			};

			foreach (var type in responseTypes)
			{
				if (data?.ContainsKey(type) ?? false)
				{
					stats[type] = new ResponseTypeStats
					{
						Requests = data[type].Count,
						BytesTransferred = data[type].Sum(x => x.Size),
						MedianResponseTime = GetMedianTime(data[type])
					};
				}
			}

			return stats;
		}

		private double GetMedianTime(HashSet<RequestResponseStat> stats)
		{
			if (stats?.Count == 0)
			{
				return 0;
			}

			if (stats.Count % 2 == 1)
			{
				return stats.OrderByDescending(x => x.Time).ElementAt(stats.Count / 2).Time;
			}
			else
			{
				var sorted = stats.OrderByDescending(x => x.Time);
				var len = stats.Count;

				var a = sorted.ElementAt((len / 2) - 1).Time;
				var b = sorted.ElementAt(len / 2).Time;

				return a + b / 2.0;
			}
		}

		private Dictionary<string, AppStats> GetAppStats()
		{
			var appStats = new Dictionary<string, AppStats>();
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

				if (!appStats.ContainsKey(r.AppName))
				{
					appStats[r.AppName] = new AppStats
					{
						Name = r.AppName,
						AppURL = r.AppURL
					};
				}

				appStats[r.AppName].LiveReports++;
				appStats[r.AppName].TotalViews += r.Views;
			}

			// Calculate views/report and reports/day
			foreach (var app in appStats.Keys)
			{
				if (appStats[app].TotalViews > 0)
				{
					var vpr = appStats[app].TotalViews * 1.0 / appStats[app].LiveReports;

					appStats[app].ViewsPerReport = vpr;
				}
				var days = reports[app].GroupBy(x => x);
				appStats[app].ReportsPerDay = days.Average(x => x.Count());
			}

			return appStats;
		}
	}
}
