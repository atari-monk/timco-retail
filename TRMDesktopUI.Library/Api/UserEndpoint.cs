﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
	public class UserEndpoint : IUserEndpoint
	{
		private readonly IAPIHelper apiHelper;

		public UserEndpoint(IAPIHelper apiHelper)
		{
			this.apiHelper = apiHelper;
		}

		public async Task<List<UserModel>> GetAll()
		{
			using (var response = await apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllUsers"))
			{
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsAsync<List<UserModel>>();
					return result;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public async Task CreateUser(CreateUserModel model)
		{
			var data = new { 
				model.FirstName
				, model.LastName
				, model.EmailAddress
				, model.Password };

			using (var response = await apiHelper.ApiClient.PostAsJsonAsync("/api/User/Register", data))
			{
				if (response.IsSuccessStatusCode == false)
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public async Task<Dictionary<string,string>> GetAllRoles()
		{
			using (var response = await apiHelper.ApiClient.GetAsync("/api/User/Admin/GetAllRoles"))
			{
				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsAsync<Dictionary<string,string>>();
					return result;
				}
				else
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public async Task AddUserToRole(string userId, string roleName)
		{
			var data = new { userId, roleName };

			using (var response = await apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/AddRole", data))
			{
				if (response.IsSuccessStatusCode == false)
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}

		public async Task RemoveUserFromRole(string userId, string roleName)
		{
			var data = new { userId, roleName };

			using (var response = await apiHelper.ApiClient.PostAsJsonAsync("/api/User/Admin/RemoveRole", data))
			{
				if (response.IsSuccessStatusCode == false)
				{
					throw new Exception(response.ReasonPhrase);
				}
			}
		}
	}
}