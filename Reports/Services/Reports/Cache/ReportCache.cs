using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reports.Models;

namespace Reports.Services.Reports.Cache
{
	public class ReportCache : IReportCache
	{
		private readonly ConcurrentDictionary<string, Report> cache;

		public ReportCache()
		{
			cache = new ConcurrentDictionary<string, Report>();


		}

		public Report GetReport(string id)
		{
			if (cache.ContainsKey(id))
			{
				return cache[id];
			}

			return null;
		}
	}
}
