﻿using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SecretApiSample
{
	public class AuthOptions
	{
		public const string ISSUER = "MyAuthServer"; // издатель токена
		public const string AUDIENCE = "MyAuthClient"; // потребитель токена
		const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
		public const int LIFETIME = 20; // время жизни токена в минутах
		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
		}
    }
}
