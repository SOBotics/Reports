﻿using System;
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

		private ServerStats GetServerStats()
		{
			//TODO: Refactor this crap.
			var apiStats = metaStatStore.GetData<HashSet<RequestResponseStat>>(MetaStatStore.ApiTimesKey);
			var staticStats = metaStatStore.GetData<HashSet<RequestResponseStat>>(MetaStatStore.StaticTimesKey);
			var dynamicStats = metaStatStore.GetData<HashSet<RequestResponseStat>>(MetaStatStore.DynamicTimesKey);

			return new ServerStats
			{
				ApiReqCount = apiStats?.Count ?? 0,
				StaticReqCount = staticStats?.Count ?? 0,
				DynamicReqCount = dynamicStats?.Count ?? 0,
				MedianApiTime = GetMedianTime(apiStats),
				MedianStaticTime = GetMedianTime(staticStats),
				MedianDynamicTime = GetMedianTime(dynamicStats),
				ApiBytesProcessed = apiStats?.Sum(x => x.Size) ?? 0,
				StaticBytesProcessed = staticStats?.Sum(x => x.Size) ?? 0,
				DynamicBytesProcessed = dynamicStats?.Sum(x => x.Size) ?? 0
			};
		}

		private double GetMedianTime(HashSet<RequestResponseStat> stats)
		{
			if (stats == null)
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

					appStats[app].ViewsPerReport = Math.Round(vpr, 1);
				}
				var days = reports[app].GroupBy(x => x);
				appStats[app].ReportsPerDay = days.Average(x => x.Count());
			}

			return appStats;
		}
	}
}
