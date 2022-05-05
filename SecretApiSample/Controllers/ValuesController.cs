using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecretApiSample.Controllers
{
	//Контроллер, куда можно ходить, получив токен через AccountController на сервисе выдачи токенов
	[ApiController]
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		[Authorize]
		[Route("getlogin")]
		public IActionResult GetLogin()
		{
			return Ok($"Ваш логин: {User.Identity.Name}");
		}

		[Authorize(Roles = "admin")]
		[Route("getrole")]
		public IActionResult GetRole()
		{
			return Ok("Ваша роль: администратор");
		}
	}
}
