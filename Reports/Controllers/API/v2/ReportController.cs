using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Reports.Config;
using Reports.Dependencies.ReportStore;
using Reports.Models;

namespace Reports.Controllers.API.V2
{
	[Route("api/v2/[controller]/[action]")]
	public class ReportController : Controller
	{
		// Collision chance using the following ID generation config: 1 in 56,800,235,584 (62^6)
		private const string validIdChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const int idLength = 6;

		private readonly RandomNumberGenerator rng;
		private readonly IReportStore reportStore;
		private readonly IOptionsSnapshot<HostingOptions> configAccessor;

		public ReportController(IOptionsSnapshot<HostingOptions> ca, IReportStore rs)
		{
			configAccessor = ca;
			reportStore = rs;
			rng = RandomNumberGenerator.Create();
		}

		[HttpPost]
		public IActionResult Create([FromBody]Report report)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			report.ID = GenerateId();
			report.AppName = report.AppName.Trim();
			report.AppURL = report.AppURL.Trim();

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

			reportStore.AddReport(report);

			return Json(new
			{
#if RELEASE
				ReportURL = $"http://{configAccessor.Value.FQD}/r/{report.ID}"
#else
				ReportURL = $"http://{Request.Host}/r/{report.ID}"
#endif
			});
		}

		private string GenerateId()
		{
			var id = "";

			for (var i = 0; i < idLength; i++)
			{
				var b = new byte[4];

				rng.GetBytes(b);

				var bInt = Math.Abs(BitConverter.ToInt32(b, 0));
				var charIndex = bInt % validIdChars.Length;

				id += validIdChars[charIndex];
			}

			return id;
		}
	}
}