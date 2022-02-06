using System;
using System.Net.Http;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
	public class SaleEndpoint : ISaleEndpoint
	{
		private readonly IAPIHelper apiHelper;

		public SaleEndpoint(
			IAPIHelper apiHelper)
		{
			this.apiHelper = apiHelper;
		}

		public async Task PostSale(SaleModel sale)
		{
			using HttpResponseMessage response = await apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale);
			if (response.IsSuccessStatusCode)
			{
			}
			else
			{
				throw new Exception(response.ReasonPhrase);
			}
		}
	}
}