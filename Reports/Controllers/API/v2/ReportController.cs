using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
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

		public ReportController(IReportStore rs)
		{
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

			if (report.ExpiresAt == default(DateTime))
			{
				//TODO: Should probably move this to the config file.
				report.ExpiresAt = DateTime.UtcNow.AddDays(30);
			}
			else if (report.ExpiresAt > DateTime.UtcNow.AddYears(1))
			{
				report.ExpiresAt = DateTime.UtcNow.AddYears(1);
			}
			else if (report.ExpiresAt < DateTime.UtcNow)
			{
				return BadRequest("Report expritartion date cannot be in the past.");
			}

			report.ID = GenerateId();

			reportStore.AddReport(report);

			return Json(new
			{
				ReportURL = $"http://{Request.Host}/r/{report.ID}"
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