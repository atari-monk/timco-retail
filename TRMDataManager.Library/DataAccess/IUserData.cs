using System.Collections.Generic;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.SqlDataAcces
{
	public interface IUserData
	{
		List<UserModel> GetUserById(string Id);
	}
}