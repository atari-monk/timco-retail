using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using TRMApi.Models;

namespace TRMApi.Controllers
{
	public class HomeController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<IdentityUser> userManager;

		public HomeController(
			RoleManager<IdentityRole> roleManager
			, UserManager<IdentityUser> userManager)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Privacy()
		{
			string[] roles = { "Admin", "Manager", "Cashier" };

			foreach (var role in roles)
			{
				var roleExist = await roleManager.RoleExistsAsync(role);

				if (roleExist == false)
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			var user = await userManager.FindByEmailAsync("kmazanek@gmail.com");

			if (user != null)
			{
				await userManager.AddToRoleAsync(user, "Admin");
				await userManager.AddToRoleAsync(user, "Cashier");
			}

			await Task.Run(() => { });

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}