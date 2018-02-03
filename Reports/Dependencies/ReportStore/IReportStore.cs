using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reports.Models;

namespace Reports.Dependencies.ReportStore
{
	public interface IReportStore
	{
		HashSet<string> AppNames { get; }

		int Count { get; }

		Report this[string id] { get; }

		bool Exists(string id);

		void AddReport(Report report);
	}
}
