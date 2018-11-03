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

        private void FormatField(Field f)
        {
            f.ID = "FID" + f.ID;
            f.Name = f.Name.Trim();
            f.Value = f.Value?.Trim();
            f.Fields = f.Fields ?? new SubField[0];

            switch (f.Type)
            {
                case Models.Type.Date:
                    {
                        var dt = DateTime.Parse(f.Value);
                        var epoch = dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

                        f.Value = epoch.ToString();
                    }
                    break;
                case Models.Type.Fields:
                    foreach (var sf in f.Fields)
                    {
                        sf.ID = "FID" + sf.ID;
                        sf.Name = sf.Name.Trim();
                        sf.Value = sf.Value.Trim();

                        if (sf.Type == SubType.Date)
                        {
                            var dt = DateTime.Parse(sf.Value);
                            var epoch = dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

                            sf.Value = epoch.ToString();
                        }
                    }
                    break;
            }


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

		    foreach (var fields in report.Fields)
		    {
		        foreach (var field in fields)
		        {
		            FormatField(field);
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
