using Microsoft.AspNetCore.Mvc;
using Reports.Services.ReportStore;

namespace Reports.Controllers
{
	public class HomeController : Controller
	{
		private readonly IReportStore reports;

		public HomeController(IReportStore reportStore)
		{
			reports = reportStore;
		}

		[Route("/")]
		[Route("/home")]
		[Route("/index")]
		public IActionResult Index()
		{
			var reportCount = $"{reports.Count.ToString("N0")} report{(reports.Count == 1 ? "" : "s")}";
			var appCount = $"{reports.AppNames.Count} app{(reports.AppNames.Count == 1 ? "" : "s")}";

			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ServerVersion"] = ViewData["LocalCommit"].ToString().Substring(0, 5);
			ViewData["ReportCount"] = reportCount;
			ViewData["AppCount"] = appCount;

			Response.Headers["ETag"] = "";

			return View();
		}
	}
}
