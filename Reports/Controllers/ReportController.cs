using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Reports.Dependencies.ReportStore;

namespace Reports.Controllers
{
	public class ReportController : Controller
	{
		private readonly IReportStore reports;

		public ReportController(IReportStore reportStore)
		{
			reports = reportStore;
		}

		[Route("/report/{id:regex(^[[a-zA-Z0-9]]{{6}}$)}")]
		[Route("/r/{id:regex(^[[a-zA-Z0-9]]{{6}}$)}")]
		public IActionResult ViewReport(string id)
		{
			if (!reports.Exists(id))
			{
				return NotFound();
			}

			ViewData["LocalCommit"] = ThisAssembly.Git.Sha;
			ViewData["ServerVersion"] = ViewData["LocalCommit"].ToString().Substring(0, 5);
			var sortableFields = new Dictionary<string, string>();

			var r = reports[id];
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
				//r.Fields[0].Where(x =>
				//{
				//	var value = x.Value;
				//	if (x.Type == Models.Type.Answers && value.ToUpperInvariant().StartsWith("A"))
				//	{
				//		value = value.Remove(0, 1);
				//	}
				//	return float.TryParse(value, out var res);
				//}).ToDictionary(x => x.ID, x => x.Name);

			return View(r);
		}
	}
}