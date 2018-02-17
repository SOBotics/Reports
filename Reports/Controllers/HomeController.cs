using Microsoft.AspNetCore.Mvc;
using Reports.Services.Reports.Accessor;

namespace Reports.Controllers
{
	[Route("/[action]")]
	public class HomeController : Controller
	{
		private readonly IReportAccessor reports;

		public HomeController(IReportAccessor reportStore)
		{
			reports = reportStore;
		}

		[Route("/")]
		[Route("/home")]
		public IActionResult Index()
		{
			ViewData["Sha"] = ThisAssembly.Git.Sha;

			return View();
		}

		public IActionResult Stats()
		{
			ViewData["Sha"] = ThisAssembly.Git.Sha;

			return View();
		}
	}
}
