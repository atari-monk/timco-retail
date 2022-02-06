using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
	public class InventoryData : IInventoryData
	{
		private readonly ISqlDataAccess sql;

		public InventoryData(
			ISqlDataAccess sql)
		{
			this.sql = sql;
		}

		public List<InventoryModel> GetInventory()
		{
			var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");
			return output;
		}

		public void SaveInventoryRecord(InventoryModel item)
		{
			sql.SaveData("dbo.spInventory_Insert", item, "TRMData");
		}
	}
}