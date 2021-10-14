﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
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

		public AuthStateProvider(
			HttpClient httpClient
			, ILocalStorageService localStorage)
		{
			this.httpClient = httpClient;
			this.localStorage = localStorage;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await localStorage.GetItemAsync<string>(key: "authToken");

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