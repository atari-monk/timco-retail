using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public class UserData
	{
		private readonly IConfiguration config;

		public UserData(IConfiguration config)
		{
			this.config = config;
		}

		public List<UserModel> GetUserById(string Id)
		{
			var sql = new SqlDataAccess(config);

			var p = new { Id };

			var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", p, "TRMData");

			return output;
		}
	}
}