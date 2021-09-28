using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
		private readonly IConfiguration config;

		public InventoryController(IConfiguration config)
		{
			this.config = config;
		}

		[Authorize(Roles = "Admin,Manager")]
		[HttpGet]
		public List<InventoryModel> Get()
		{
			var data = new InventoryData(config);
			return data.GetInventory();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public void Post(InventoryModel item)
		{
			var data = new InventoryData(config);
			data.SaveInventoryRecord(item);
		}
	}
}