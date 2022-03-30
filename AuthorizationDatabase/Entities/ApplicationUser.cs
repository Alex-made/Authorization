using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationDatabase.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		//Id, Username хранятся уже в суперклассе. Password хранится в хэше

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
