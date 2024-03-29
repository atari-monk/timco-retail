﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TRMApi.Data;

namespace TRMApi.Controllers
{
	public class TokenController : Controller
	{
		private readonly ApplicationDbContext context;
		private readonly UserManager<IdentityUser> userManager;
		private readonly IConfiguration configuration;

		public TokenController(
			ApplicationDbContext context
			, UserManager<IdentityUser> userManager
			, IConfiguration configuration)
		{
			this.context = context;
			this.userManager = userManager;
			this.configuration = configuration;
		}

		[Route("/token")]
		[HttpPost]
		public async Task<IActionResult> Create(
			string username
			, string password
			, string grant_type)
		{
			if (await IsValidUsernameAndPassword(username, password))
			{
				return new ObjectResult(await GenerateToken(username));
			}
			else
			{
				return BadRequest();
			}
		}

		private async Task<bool> IsValidUsernameAndPassword(string username, string password)
		{
			var user = await userManager.FindByEmailAsync(username);
			return await userManager.CheckPasswordAsync(user, password);
		}

		public async Task<dynamic> GenerateToken(string username)
		{
			var user = await userManager.FindByEmailAsync(username);
			var roles = from ur in context.UserRoles
									join r in context.Roles on ur.RoleId equals r.Id
									where ur.UserId == user.Id
									select new {
										ur.UserId, ur.RoleId, r.Name
									};
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, username)
				, new Claim(ClaimTypes.NameIdentifier, user.Id)
				, new Claim(JwtRegisteredClaimNames.Nbf,
					new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString())
				, new Claim(JwtRegisteredClaimNames.Exp,
					new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
			};

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role.Name));
			}

			var key = configuration.GetValue<string>("Secrets:SecurityKey");

			var token = new JwtSecurityToken(
				new JwtHeader(
					new SigningCredentials(
						new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
							, SecurityAlgorithms.HmacSha256))
				, new JwtPayload(claims));

			var output = new {
				Access_Token = new JwtSecurityTokenHandler().WriteToken(token)
				,
				UserName = username
			};
			return output;
		}
	}
}