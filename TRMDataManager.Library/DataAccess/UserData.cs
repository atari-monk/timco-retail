using System.Collections.Generic;
using TRMDataManager.Library.Internal.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public class UserData : IUserData
	{
		private readonly ISqlDataAccess sql;

		public UserData(ISqlDataAccess sql)
		{
			this.sql = sql;
		}

		public List<UserModel> GetUserById(string Id)
		{
			return sql.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "TRMData");
		}
	}
}