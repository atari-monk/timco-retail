using System.Web.Http;
using TRMDataManager.Models;

namespace TRMDataManager.Controllers
{
	[Authorize]
	public class SaleController : ApiController
	{
		public void Post(SaleModel sale)
		{

		}

		//public List<ProductModel> Get()
		//{
		//	var data = new ProductData();

		//	return data.GetProducts();
		//}
	}
}