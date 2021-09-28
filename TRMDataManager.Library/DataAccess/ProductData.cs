using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public class ProductData
	{
		private readonly IConfiguration config;

		public ProductData(IConfiguration config)
		{
			this.config = config;
		}

		public List<ProductModel> GetProducts()
		{
			var sql = new SqlDataAccess(config);

			var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "TRMData");

			return output;
		}

		public ProductModel GetProductById(int productId)
		{
			var sql = new SqlDataAccess(config);

			var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

			return output;
		}
	}
}