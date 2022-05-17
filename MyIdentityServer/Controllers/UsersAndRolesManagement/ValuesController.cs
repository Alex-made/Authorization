using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyIdentityServer.Controllers.UsersAndRolesManagement
{
	//Локальный контроллер, куда можно ходить аутентифицировавшись по схеме Cookies
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

		[Authorize(Roles = "Admin")]
		[Route("getrole")]
		public IActionResult GetRole()
		{
			return Ok("Ваша роль: администратор");
		}
	}
}
