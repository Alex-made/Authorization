using System.ComponentModel.DataAnnotations;

namespace MyIdentityServer.ViewModels
{
	public class LoginUserViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Запомнить?")]
		public bool RememberMe { get; set; }

		public string ReturnUrl { get; set; }
    }
}