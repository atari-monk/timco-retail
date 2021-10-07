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
		private readonly ISaleData saleData;

		public SaleController(ISaleData saleData)
		{
			this.saleData = saleData;
		}

		[Authorize(Roles = "Cashier")]
		[HttpPost]
		public void Post(SaleModel sale)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			saleData.SaveSale(sale, userId);
		}

		[Authorize(Roles = "Admin,Manager")]
		[Route("GetSalesReport")]
		[HttpGet]
		public List<SaleReportModel> GetSalesReport()
		{
			return saleData.GetSaleReport();
		}

		[AllowAnonymous]
		[Route("GetTaxRate")]
		[HttpGet]
		public decimal GetTaxRate()
		{
			return saleData.GetTaxRate();
		}
	}
}