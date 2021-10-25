using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
	public class ProductEndpoint : IProductEndpoint
	{
		private readonly IAPIHelper apiHelper;

		public ProductEndpoint(
			IAPIHelper apiHelper)
		{
			this.apiHelper = apiHelper;
		}

		public async Task<List<ProductModel>> GetAll()
		{
			using HttpResponseMessage response = await apiHelper.ApiClient.GetAsync("/api/Product");
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsAsync<List<ProductModel>>();
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}
	}
}