﻿using System.Collections.Generic;
using Reports.Models;

namespace Reports.Services.Reports.Accessor
{
	public interface IReportAccessor
	{
		HashSet<string> AppNames { get; }

		int Count { get; }

		Report this[string id] { get; }

		bool Exists(string id);

		void AddReport(Report report);
	}
}
