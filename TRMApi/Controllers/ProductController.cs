using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TRMDataManager.Library.Models;
using TRMDataManager.Library.SqlDataAcces;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = "Cashier")]
	public class ProductController : ControllerBase
	{
		private readonly IConfiguration config;

		public ProductController(IConfiguration config)
		{
			this.config = config;
		}

		[HttpGet]
		public List<ProductModel> Get()
		{
			var data = new ProductData(config);

			return data.GetProducts();
		}
	}
}