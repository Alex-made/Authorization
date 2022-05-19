using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyIdentityServer.ViewModels;

namespace MyIdentityServer.Controllers.UsersAndRolesManagement
{
	[Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

		public IActionResult Create() => View();

		[HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
				IdentityUser user = new IdentityUser { Email = model.Email, UserName = model.Email};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
			var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new EditUserViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;

					var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

		public async Task<IActionResult> ResetPassword(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			var model = new ResetPasswordViewModel { Id = user.Id, Email = user.Email };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.Id);
				if (user != null)
				{
					string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
					var passwordChangeResult = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

					if (passwordChangeResult.Succeeded)
					{
						return RedirectToAction("Index");
					}
					else
					{
						foreach (var error in passwordChangeResult.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Пользователь не найден");
				}
			}
			return View(model);
		}

		public async Task<IActionResult> ChangePassword(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByIdAsync(model.Id);
				if (user != null)
				{
					IdentityResult result =
						await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Index");
					}
					else
					{
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Пользователь не найден");
				}
			}
			return View(model);
		}
    }
}