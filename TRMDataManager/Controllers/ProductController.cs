using System.Collections.Generic;
using System.Web.Http;
using TRMDataManager.Library.SqlDataAcces;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
	[Authorize(Roles = "Cashier")]
	public class ProductController : ApiController
	{
		public List<ProductModel> Get()
		{
			var data = new ProductData();

			return data.GetProducts();
		}
	}
}