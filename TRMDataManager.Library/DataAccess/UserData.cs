﻿using System.Collections.Generic;
using TRMDataManager.Library.DataAccess;
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

		public void CreateUser(UserModel user)
		{
			sql.SaveData("dbo.spUser_Insert", new { user.Id, user.FirstName, user.LastName, user.EmailAddress }, "TRMData");
		}
	}
}