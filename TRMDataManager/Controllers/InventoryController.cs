using System.Collections.Generic;
using System.Web.Http;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Controllers
{
	[Authorize]
	public class InventoryController : ApiController
	{
		[Authorize(Roles = "Admin,Manager")]
		public List<InventoryModel> Get()
		{
			var data = new InventoryData();
			return data.GetInventory();
		}

		[Authorize(Roles = "Admin")]
		public void Post(InventoryModel item)
		{
			var data = new InventoryData();
			data.SaveInventoryRecord(item);
		}
	}
}