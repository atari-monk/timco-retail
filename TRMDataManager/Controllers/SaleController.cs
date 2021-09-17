using Microsoft.AspNet.Identity;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
	[Authorize]
	public class SaleController : ApiController
	{
		public void Post(SaleModel sale)
		{
			var data = new SaleData();
			var userId = RequestContext.Principal.Identity.GetUserId();
			data.SaveSale(sale, userId);
		}
	}
}