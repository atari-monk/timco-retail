using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class InventoryController : ControllerBase
	{
		private readonly IInventoryData inventoryData;

		public InventoryController(
			IInventoryData inventoryData)
		{
			this.inventoryData = inventoryData;
		}

		[Authorize(Roles = "Admin,Manager")]
		[HttpGet]
		public List<InventoryModel> Get()
		{
			return inventoryData.GetInventory();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public void Post(InventoryModel item)
		{
			inventoryData.SaveInventoryRecord(item);
		}
	}
}