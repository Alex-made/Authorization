﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentit_JWT.Models
{
	public class User
	{
		public string DisplayName
		{
			get;
			set;
		}

		public string Token
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public byte[] Image
		{
			get;
			set;
		}
	}
}
