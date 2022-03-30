using System;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationVkontakte.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		//Id, Username хранятся уже в суперклассе. Password хранится в хэше

		public ApplicationUser()
		{
			
		}

		public ApplicationUser(string userName) : base(userName)
		{
			
		}

		public string FirstName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}
	}
}
