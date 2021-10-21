using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public interface IUserData
	{
		void CreateUser(UserModel user);
		List<UserModel> GetUserById(string Id);
	}
}