using System;
using System.Collections.Generic;
using System.Linq;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class SaleData
	{
		public void SaveSale(SaleModel saleInfo, string cashierId)
		{
			//TODO: Make this SOLID/DRY/Better
			//Start filling in the sale details models we will save to database
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

				//Get the information about this product
				var productInfo = products.GetProductById(item.ProductId);

				if(productInfo == null)
				{
					throw new Exception($"The product Id of {item.ProductId} could not be found in database.");
				}
				
				//Fill in the available information
				detail.PurchasePrice = productInfo.RetailPrice * detail.Quantity;

				if(productInfo.IsTaxable)
				{
					detail.Tax = (detail.PurchasePrice * taxRate);
				}

				details.Add(detail);
			}

			//Create the Same model
			var sale = new SaleDBModel()
			{
				SubTotal = details.Sum(x => x.PurchasePrice)
				, Tax = details.Sum(x => x.Tax)
				, CashierId = cashierId
			};

			sale.Total = sale.SubTotal + sale.Tax;

			//Save the same model

			var sql = new SqlDataAccess();
			sql.SaveData("dbo.spSale_Insert", sale, "TRMData");

			//Get the ID from the sale model
			sale.Id = sql.LoadData<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }, "TRMData").FirstOrDefault();

			//Finish filling in the sale detail models
			foreach (var item in details)
			{
				item.SaleId = sale.Id;
				//Save the same detail models
				sql.SaveData("dbo.spSaleDetail_Insert", item, "TRMData");
			}
		}
	}
}