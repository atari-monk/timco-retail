using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using TRMDataManager.Library.Models;
using TRMDataManager.Library.SqlDataAcces;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SaleController : ControllerBase
	{
		[Authorize(Roles = "Cashier")]
		public void Post(SaleModel sale)
		{
			var data = new SaleData();
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			data.SaveSale(sale, userId);
		}

		[Authorize(Roles = "Admin,Manager")]
		[Route("GetSalesReport")]
		public List<SaleReportModel> GetSalesReport()
		{
			var data = new SaleData();
			return data.GetSaleReport();
		}
	}
}