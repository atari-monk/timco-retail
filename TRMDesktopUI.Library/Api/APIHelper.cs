using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
	public class APIHelper : IAPIHelper
	{
		private HttpClient apiClient;
		private readonly ILoggedInUserModel loggedInUser;
		private readonly IConfiguration config;

		public APIHelper(
			ILoggedInUserModel loggedInUser
			, IConfiguration config)
		{
			this.loggedInUser = loggedInUser;
			this.config = config;
			InitializeClient();
		}

		public HttpClient ApiClient
		{
			get {
				return apiClient;
			}
		}

		private void InitializeClient()
		{
			string api = config.GetValue<string>("api");

			apiClient = new HttpClient
			{
				BaseAddress = new Uri(api)
			};
			apiClient.DefaultRequestHeaders.Accept.Clear();
			apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<AuthenticatedUser> Authenticate(string username, string password)
		{
			var data = new FormUrlEncodedContent(
				new[]
				{
					new KeyValuePair<string,string>("grant_type", "password")
					, new KeyValuePair<string,string>("username", username)
					, new KeyValuePair<string,string>("password", password)
				});

			using HttpResponseMessage response = await apiClient.PostAsync("/Token", data);
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
				return result;
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}

		public void LogOffUser()
		{
			apiClient.DefaultRequestHeaders.Clear();
		}

		public async Task GetLoggedInUserInfo(string token)
		{
			apiClient.DefaultRequestHeaders.Clear();
			apiClient.DefaultRequestHeaders.Accept.Clear();
			apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer { token }");

			using HttpResponseMessage response = await apiClient.GetAsync("/api/User");
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
				loggedInUser.CreatedDate = result.CreatedDate;
				loggedInUser.EmailAddress = result.EmailAddress;
				loggedInUser.FirstName = result.FirstName;
				loggedInUser.LastName = result.LastName;
				loggedInUser.Id = result.Id;
				loggedInUser.Token = token;
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}
	}
}