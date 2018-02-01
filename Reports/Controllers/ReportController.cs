using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Controllers
{
    public class ReportController : Controller
    {
		[Route("/Report")]
        public IActionResult Index()
        {
            return View();
        }
    }
}