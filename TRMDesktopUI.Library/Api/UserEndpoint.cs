using System;
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
	}
}