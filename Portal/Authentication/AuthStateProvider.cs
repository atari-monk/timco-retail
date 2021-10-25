using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;

namespace Portal.Authentication
{
	public class AuthStateProvider : AuthenticationStateProvider
	{
		private readonly HttpClient httpClient;
		private readonly ILocalStorageService localStorage;
		private readonly IAPIHelper apiHelper;
		private readonly AuthenticationState anonymous;
		private readonly string authTokenStorageKey;

		public AuthStateProvider(
			HttpClient httpClient
			, ILocalStorageService localStorage
			, IConfiguration config
			, IAPIHelper apiHelper)
		{
			this.httpClient = httpClient;
			this.localStorage = localStorage;
			this.apiHelper = apiHelper;
			authTokenStorageKey = config[key:"authTokenStorageKey"];
			anonymous = new AuthenticationState(user: new ClaimsPrincipal(identity: new ClaimsIdentity()));
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await localStorage.GetItemAsync<string>(key: authTokenStorageKey);

			if(string.IsNullOrWhiteSpace(token))
			{
				return anonymous;
			}

			bool isAuthenticated = await NotifyUserAuthenticationAsync(token);

			if (isAuthenticated == false)
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

		public async Task<bool> NotifyUserAuthenticationAsync(string token)
		{
			bool isAuthenticatedOutput;
			Task<AuthenticationState> authState;
			try
			{
				await apiHelper.GetLoggedInUserInfo(token);
				var authenticatedUser = new ClaimsPrincipal(
				identity: new ClaimsIdentity(
					JwtParser.ParseClaimsFromJwt(token)
					, authenticationType: "jwtAuthType"));
				authState = Task.FromResult(new AuthenticationState(authenticatedUser));
				NotifyAuthenticationStateChanged(authState);
				isAuthenticatedOutput = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				await NotifyUserLogout();
				isAuthenticatedOutput = false;
			}
			return isAuthenticatedOutput;
		}

		public async Task NotifyUserLogout()
		{
			await localStorage.RemoveItemAsync(key: authTokenStorageKey);
			var authState = Task.FromResult(anonymous);
			apiHelper.LogOffUser();
			httpClient.DefaultRequestHeaders.Authorization = null;
			NotifyAuthenticationStateChanged(authState);
		}
	}
}