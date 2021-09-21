using System;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public class SaleData
	{
		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO: Make this SOLID/DRY/Better
			var details = new List<SaleDetailDBModel>();
			var products = new ProductData();
			var taxRate = ConfigHelper.GetTaxRate()/100;

			foreach (var item in saleInfo.SaleDetails)
			{
				var detail = new SaleDetailDBModel
				{
					ProductId = item.ProductId
					, Quantity = item.Quantity
				};
				
				var productInfo = products.GetProductById(item.ProductId);

				if(productInfo == null)
				{
					throw new Exception($"The product Id of {item.ProductId} could not be found in database.");
				}
				
				detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

				if(productInfo.IsTaxable)
				{
					detail.Tax = (detail.PurchasePrice * taxRate);
				}

				details.Add(detail);
			}

			var sale = new SaleDBModel()
			{
				SubTotal = details.Sum(x => x.PurchasePrice)
				, Tax = details.Sum(x => x.Tax)
				, CashierId = cashierId
			};

			sale.Total = sale.SubTotal + sale.Tax;

			using (var sql = new SqlDataAccess())
			{
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
		}

		public List<SaleReportModel> GetSaleReport()
		{
			var sql = new SqlDataAccess();
			var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");
			return output;
		}
	}
}