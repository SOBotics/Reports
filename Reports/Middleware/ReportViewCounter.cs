using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Reports.Services.LocalData.Reports;

namespace Reports.Middleware
{
	public class ReportViewCounter
	{
		private readonly string[] reportBasePaths;
		private readonly RequestDelegate nextMiddleware;

		public ReportViewCounter(RequestDelegate next)
		{
			nextMiddleware = next;
			reportBasePaths = new[] { "r", "report" };
		}

		public Task Invoke(HttpContext context, IReportStore reports)
		{
			var req = context.Request;
			var pathSplit = req.Path.ToString().Split("/");

			if (req.Method == "GET" && reportBasePaths.Contains(pathSplit[1]))
			{
				Task.Run(() =>
				{
					var id = pathSplit[2];
					var r = reports.Get(id);

					r.Views++;

					reports.Save(r);
				});
			}

			return nextMiddleware(context);
		}
	}
}
