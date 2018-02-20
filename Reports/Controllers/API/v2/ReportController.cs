using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Reports.Config;
using Reports.Models;
using Reports.Services.LocalData.Reports;
using Reports.Services.Reports;

namespace Reports.Controllers.API.V2
{
	[Route("api/v2/[controller]/[action]")]
	public class ReportController : Controller
	{
		private readonly IReportStore reportStore;
		private readonly IIdGenerator idGenerator;
		private readonly IOptionsSnapshot<HostingOptions> configAccessor;

		public ReportController(IOptionsSnapshot<HostingOptions> ca, IReportStore rs, IIdGenerator idGen)
		{
			configAccessor = ca;
			reportStore = rs;
			idGenerator = idGen;
		}

		[HttpPost]
		public IActionResult Create([FromBody]Report report)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			
			if (report.ExpiresAt == default(DateTime))
			{
				report.ExpiresAt = DateTime.UtcNow.AddDays(30);
			}

			report.ID = idGenerator.GetNewId();
			report.AppName = report.AppName.Trim();
			report.AppURL = report.AppURL?.Trim();
			report.CreatedAt = DateTime.UtcNow;

			for (var i = 0; i < report.Fields.Length; i++)
			for (var j = 0; j < report.Fields[i].Length; j++)
			{
				var f = report.Fields[i][j];

				report.Fields[i][j].ID = "FID" + f.ID;
				report.Fields[i][j].Name = f.Name.Trim();
				report.Fields[i][j].Value = f.Value.Trim();

				if (f.Type == Models.Type.Date)
				{
					var dt = DateTime.Parse(f.Value);
					var epoch = dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

					report.Fields[i][j].Value = epoch.ToString();
				}
			}

			reportStore.Save(report);

			var protocol = "http";

			if (configAccessor.Value.TlsSupported)
			{
				protocol += "s";
			}

			return Json(new
			{
#if RELEASE
				ReportURL = $"{protocol}://{configAccessor.Value.FQD}/r/{report.ID}"
#else
				ReportURL = $"{protocol}://{Request.Host}/r/{report.ID}"
#endif
			});
		}
	}
}
