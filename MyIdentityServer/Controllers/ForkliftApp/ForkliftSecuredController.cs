using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyIdentityServer.Controllers.ForkliftApp
{
	//Локальный контроллер, куда можно ходить, получив токен через AccountController
	[ApiController]
	[Route("api/[controller]")]
	public class ForkliftSecuredController : Controller
	{
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[Route("getlogin")]
		public IActionResult GetLogin()
		{
			return Ok($"Ваш логин: {User.Identity.Name}");
		}

		[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[Route("getrole")]
		public IActionResult GetRole()
		{
			return Ok("Ваша роль: администратор");
		}
	}
}
