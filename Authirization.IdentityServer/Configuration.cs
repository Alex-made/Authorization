using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;

namespace Authirization.IdentityServer
{
	public static class Configuration
	{
		public static IEnumerable<ApiScope> GetScopes()
		{
			return new List<ApiScope>
			{
				new ApiScope("OrdersAPI")
			};
		}

		public static IEnumerable<Client> GetClients()
		{
			return new List<Client>
			{
				new Client
				{
					ClientId = "Client_id",
					ClientSecrets = new List<Secret>
					{
						new Secret("Client_secret".ToSha256())
					},
					AllowedGrantTypes = GrantTypes.ClientCredentials,
					AllowedScopes =
					{
						"OrdersAPI"
					}
				}
			};
		}

		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
			{
				new ApiResource("OrdersAPI")
			};
		}

		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
			{
				new IdentityResources.OpenId()
			};
		}

		

	}
}
