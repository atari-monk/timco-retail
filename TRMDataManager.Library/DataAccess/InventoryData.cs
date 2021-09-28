using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class InventoryData
	{
		private readonly IConfiguration config;

		public InventoryData(IConfiguration config)
		{
			this.config = config;
		}

		public List<InventoryModel> GetInventory()
		{
			var sql = new SqlDataAccess(config);
			var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");
			return output;
		}

		public void SaveInventoryRecord(InventoryModel item)
		{
			var sql = new SqlDataAccess(config);
			sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
		}
	}
}