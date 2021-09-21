using Microsoft.AspNet.Identity;
using System.Web.Http;
using TRMDataManager.Library.SqlDataAcces;
using TRMDataManager.Library.Models;
using System.Collections.Generic;

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

		[Route("GetSalesReport")]
		public List<SaleReportModel> GetSalesReport()
		{
			var data = new SaleData();
			return data.GetSaleReport();
		}
	}
}