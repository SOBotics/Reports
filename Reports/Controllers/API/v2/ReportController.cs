using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reports.Models;

namespace Reports.Controllers.API.V2
{
	[Route("api/v2/[controller]/[action]")]
	public class ReportController : Controller
	{
		[HttpPost]
		public IActionResult Create([FromBody]ReportModel report)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			//TODO: Temp
			return Content("Success");
		}
	}
}