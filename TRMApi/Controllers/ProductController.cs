using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		private readonly IProductData productData;

		public ProductController(IProductData productData)
		{
			this.productData = productData;
		}

		[HttpGet]
		public List<ProductModel> Get()
		{
			return productData.GetProducts();
		}
	}
}