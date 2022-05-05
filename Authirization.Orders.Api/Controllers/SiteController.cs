using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Authirization.Orders.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SiteController : Controller
	{
		private readonly ILogger<SiteController> _logger;

		public SiteController(ILogger<SiteController> logger)
		{
			_logger = logger;
		}

		[HttpGet("Index")]
		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		[HttpGet("GetSecrets")]
		public string GetSecrets()
		{
			return "Secret string from Orders API";
		}
	}
}
