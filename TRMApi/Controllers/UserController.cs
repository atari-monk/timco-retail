﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMApi.Data;
using TRMApi.Models;
using TRMDataManager.Library.Models;
using TRMDataManager.Library.SqlDataAcces;

namespace TRMApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserController : ControllerBase
	{
		private readonly ApplicationDbContext context;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IUserData userData;
		private readonly ILogger<UserController> logger;

		public UserController(
			ApplicationDbContext context
			, UserManager<IdentityUser> userManager
			, IUserData userData
			, ILogger<UserController> logger)
		{
			this.context = context;
			this.userManager = userManager;
			this.userData = userData;
			this.logger = logger;
		}

		[HttpGet]
		public UserModel GetById()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			return userData.GetUserById(userId).First();
		}

		public record UserRegistrationModel(
			string FirstName
			, string LastName
			, string EmailAddress
			, string Password);

		[HttpPost]
		[Route("Register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register(UserRegistrationModel user)
		{
			if (ModelState.IsValid)
			{
				var existingUser = await userManager.FindByEmailAsync(user.EmailAddress);
				if (existingUser is null)
				{
					IdentityUser newUser = new()
					{
						Email = user.EmailAddress
						, EmailConfirmed = true
						, UserName = user.EmailAddress
					};

					IdentityResult result = await userManager.CreateAsync(newUser, user.Password);

					if (result.Succeeded)
					{
						existingUser = await userManager.FindByEmailAsync(user.EmailAddress);

						if (existingUser is null)
						{
							return BadRequest();
						}

						UserModel u = new()
						{
							Id = existingUser.Id
							, FirstName = user.FirstName
							, LastName = user.LastName
							, EmailAddress = user.EmailAddress
						};
						userData.CreateUser(u);
						return Ok();
					}
				}
			}

			return BadRequest();
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("Admin/GetAllUsers")]
		public List<ApplicationUserModel> GetAllUsers()
		{
			var output = new List<ApplicationUserModel>();

			var users = context.Users.ToList();
			var userRoles = from ur in context.UserRoles
											join r in context.Roles on ur.RoleId equals r.Id
											select new {
												ur.UserId, ur.RoleId, r.Name
											};

			foreach (var user in users)
			{
				var u = new ApplicationUserModel
				{
					Id = user.Id
					, Email = user.Email
				};

				u.Roles = userRoles.Where(x => x.UserId == u.Id).ToDictionary(k => k.RoleId, v => v.Name);

				output.Add(u);
			}

			return output;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		[Route("Admin/GetAllRoles")]
		public Dictionary<string, string> GetAllRoles()
		{
			var roles = context.Roles.ToDictionary(x => x.Id, x => x.Name);

			return roles;
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("Admin/AddRole")]
		public async Task AddRole(UserRolePairModel pairing)
		{
			var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var user = await userManager.FindByIdAsync(pairing.UserId);

			logger.LogInformation("Admin {Admin} adds user {User} to role {Role}"
				, loggedInUserId
				, user.Id
				, pairing.RoleName);

			await userManager.AddToRoleAsync(user, pairing.RoleName);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		[Route("Admin/RemoveRole")]
		public async Task RemoveRole(UserRolePairModel pairing)
		{
			var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var user = await userManager.FindByIdAsync(pairing.UserId);

			logger.LogInformation("Admin {Admin} removes user {User} from role {Role}"
			, loggedInUserId
			, user.Id
			, pairing.RoleName);

			await userManager.RemoveFromRoleAsync(user, pairing.RoleName);
		}
	}
}