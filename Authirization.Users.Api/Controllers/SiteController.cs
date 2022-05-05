using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Authirization.Users.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class SiteController : Controller
	{
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<SiteController> _logger;

		public SiteController(IHttpClientFactory clientFactory, ILogger<SiteController> logger)
		{
			_clientFactory = clientFactory;
			_logger = logger;
		}

		[HttpGet("Index")]
		public IActionResult Index()
		{
			return View();
		}

		
		[HttpGet("GetOrders")]
		public async Task<IActionResult> GetOrders()
		{
			var authClient = _clientFactory.CreateClient();
			//// возьмет инфу из well-known сссылки
			var discoveryDocument = await authClient.GetDiscoveryDocumentAsync("https://localhost:10001");
			
			//запрос к клиенту, созданному в Clients на Identity
			var tokenResponce = await authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
			{
				Address = discoveryDocument.TokenEndpoint,
				ClientId = "Client_id",
				ClientSecret = "Client_secret",//.ToSha256()
				Scope = "OrdersAPI"
			});

			var ordersClient = _clientFactory.CreateClient();
			ordersClient.SetBearerToken(tokenResponce.AccessToken);
			var responce = await ordersClient.GetAsync("http://localhost:5000/Site/GetSecrets");

			if (!responce.IsSuccessStatusCode)
			{
				ViewBag.Message = responce.StatusCode;
			}
			else
			{
				ViewBag.Message = await responce.Content.ReadAsStringAsync();
			}

			
			return View();
		}
	}
}
