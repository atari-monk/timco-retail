using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portal.Authentication
{
	public class AuthStateProvider : AuthenticationStateProvider
	{
		private readonly HttpClient httpClient;
		private readonly ILocalStorageService localStorage;
		private readonly AuthenticationState anonymous;
		private readonly string authTokenStorageKey;

		public AuthStateProvider(
			HttpClient httpClient
			, ILocalStorageService localStorage
			, IConfiguration config)
		{
			this.httpClient = httpClient;
			this.localStorage = localStorage;
			authTokenStorageKey = config["authTokenStorageKey"];
			anonymous = new AuthenticationState(user: new ClaimsPrincipal(identity: new ClaimsIdentity()));
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await localStorage.GetItemAsync<string>(key: authTokenStorageKey);

			if(string.IsNullOrWhiteSpace(token))
			{
				return anonymous;
			}

			httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("bearer", token);

			return new AuthenticationState(
				user: new ClaimsPrincipal(
					identity: new ClaimsIdentity(
						JwtParser.ParseClaimsFromJwt(token)
						, authenticationType:"jwtAuthType"))); 
		}

		public void NotifyUserAuthentication(string token)
		{
			var authenticatedUser = new ClaimsPrincipal(
				identity: new ClaimsIdentity(
					JwtParser.ParseClaimsFromJwt(token)
					, authenticationType: "jwtAuthType"));
			var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
			NotifyAuthenticationStateChanged(authState);
		}

		public void NotifyUserLogout()
		{
			var authState = Task.FromResult(anonymous);
			NotifyAuthenticationStateChanged(authState);
		}
	}
}