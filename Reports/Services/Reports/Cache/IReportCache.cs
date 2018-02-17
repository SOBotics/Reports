using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reports.Models;

namespace Reports.Services.Reports.Cache
{
	public interface IReportCache
	{
		Report GetReport(string id);
	}
}
