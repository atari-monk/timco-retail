using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Portal.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portal.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly HttpClient client;
		private readonly AuthenticationStateProvider authStateProvider;
		private readonly ILocalStorageService localStorage;
		private readonly IConfiguration config;
		private readonly string authTokenStorageKey;

		public AuthenticationService(
			HttpClient client
			, AuthenticationStateProvider authStateProvider
			, ILocalStorageService localStorage
			, IConfiguration config)
		{
			this.client = client;
			this.authStateProvider = authStateProvider;
			this.localStorage = localStorage;
			this.config = config;
			authTokenStorageKey = config["authTokenStorageKey"];
		}

		public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
		{
			var data = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>(key:"grant_type", value:"password")
				, new KeyValuePair<string,string>("username", userForAuthentication.Email)
				, new KeyValuePair<string,string>("password", userForAuthentication.Password)
			});

			var authResult = await client.PostAsync(
				config["api"] + config["tokenEndpoint"]
				, data);
			var authContent = await authResult.Content.ReadAsStringAsync();

			if (authResult.IsSuccessStatusCode == false)
			{
				return null;
			}

			var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
				authContent
				, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

			await localStorage.SetItemAsync(key: authTokenStorageKey, result.Access_Token);

			await ((AuthStateProvider)authStateProvider).NotifyUserAuthenticationAsync(result.Access_Token);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
				scheme: "bearer", result.Access_Token);

			return result;
		}

		public async Task Logout()
		{
			await ((AuthStateProvider)authStateProvider).NotifyUserLogout();
		}
	}
}