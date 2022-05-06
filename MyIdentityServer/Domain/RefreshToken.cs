using System;

namespace MyIdentityServer.Domain
{

	public class RefreshToken
	{
		/// <summary>
		/// Initializes new instance of <see cref="RefreshToken"/>.
		/// </summary>
		/// <param name="userId">The user identifier.</param>
		/// <param name="token">The refresh token.</param>
		public RefreshToken(string userId, string token)
		{
			UserId = userId;
			Token = token;
			Id = Guid.NewGuid().ToString();
		}

		public string Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets user primary key.
		/// </summary>
		public string UserId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets refresh token.
		/// </summary>
		public string Token
		{
			get;
			set;
		}
	}
}