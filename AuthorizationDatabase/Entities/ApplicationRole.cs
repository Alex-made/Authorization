using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationDatabase.Entities
{
	public class ApplicationRole : IdentityRole<Guid>
	{
	}
}
