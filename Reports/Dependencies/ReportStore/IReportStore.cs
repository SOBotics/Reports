using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Reports.Models;

namespace Reports.Dependencies.ReportStore
{
	public interface IReportStore
	{
		Report this[string id] { get; }

		bool Exists(string id);

		void AddReport(Report report);
	}
}
