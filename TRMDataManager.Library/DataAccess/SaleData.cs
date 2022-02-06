using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public class SaleData : ISaleData
	{
		private readonly IProductData productData;
		private readonly ISqlDataAccess sql;
		private readonly IConfiguration configuration;

		public SaleData(
			IProductData productData
			, ISqlDataAccess sql
			, IConfiguration configuration)
		{
			this.productData = productData;
			this.sql = sql;
			this.configuration = configuration;
		}

		public decimal GetTaxRate()
		{
			string rateText = configuration.GetValue<string>("TaxRate");

			var isValidTaxRate = decimal.TryParse(rateText, out decimal output);

			if (isValidTaxRate == false)
			{
				throw new ConfigurationErrorsException("The tax rate is not set properly");
			}

			output = output / 100;
			return output;
		}

		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO: Make this SOLID/DRY/Better
			var details = new List<SaleDetailDBModel>();
			var taxRate = GetTaxRate();

			foreach (var item in saleInfo.SaleDetails)
			{
				var detail = new SaleDetailDBModel
				{
					ProductId = item.ProductId
					,
					Quantity = item.Quantity
				};

				var productInfo = productData.GetProductById(item.ProductId);

				if (productInfo == null)
				{
					throw new Exception($"The product Id of {item.ProductId} could not be found in database.");
				}

				detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

				if (productInfo.IsTaxable)
				{
					detail.Tax = (detail.PurchasePrice * taxRate);
				}

				details.Add(detail);
			}

			var sale = new SaleDBModel()
			{
				SubTotal = details.Sum(x => x.PurchasePrice)
				,
				Tax = details.Sum(x => x.Tax)
				,
				CashierId = cashierId
			};

			sale.Total = sale.SubTotal + sale.Tax;

			try
			{
				sql.StartTransaction("TRMData");

				sql.SaveDataInTransaction("dbo.spSale_Insert", sale);

				sale.Id = sql.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

				foreach (var item in details)
				{
					item.SaleId = sale.Id;
					sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
				}

				sql.CommitTransaction();
			}
			catch
			{
				sql.RollbackTransaction();
				throw;
			}
		}

		public List<SaleReportModel> GetSaleReport()
		{
			var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");
			return output;
		}
	}
}