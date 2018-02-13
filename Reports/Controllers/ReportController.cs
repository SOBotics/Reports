using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Dependencies.ReportStore;

namespace Reports.Controllers
{
	public class ReportController : Controller
	{
		private const string reportIDPattern = "^[[a-zA-Z0-9]]{{6}}$";
		private readonly IReportStore reports;

		public ReportController(IReportStore reportStore)
		{
			reports = reportStore;
		}

		[Route("/report/{id:regex(" + reportIDPattern + ")}")]
		[Route("/r/{id:regex(" + reportIDPattern + ")}")]
		public IActionResult ViewReport(string id)
		{
			if (!reports.Exists(id))
			{
				return NotFound();
			}

			var r = reports[id];
			var sortableFields = new Dictionary<string, string>();

			foreach (var field in r.Fields[0])
			{
				if (r.Fields.All(x =>
				{
					var f = x.Single(z => z.ID == field.ID);
					var value = f.Value;
					if (f.Type == Models.Type.Answers && value.ToUpperInvariant().StartsWith("A"))
					{
						value = value.Remove(0, 1);
					}
					return float.TryParse(value, out var res);
				}))
				{
					sortableFields[field.ID] = field.Name;
				}
			}

			ViewData["SortableFields"] = sortableFields;
			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ServerVersion"] = ViewData["LocalCommit"].ToString().Substring(0, 5);

			return View(r);
		}
	}
}