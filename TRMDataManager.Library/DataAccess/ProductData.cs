﻿using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public class ProductData : IProductData
	{
		private readonly ISqlDataAccess sql;

		public ProductData(ISqlDataAccess sql)
		{
			this.sql = sql;
		}

		public List<ProductModel> GetProducts()
		{
			var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "TRMData");

			return output;
		}

		public ProductModel GetProductById(int productId)
		{
			var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

			return output;
		}
	}
}