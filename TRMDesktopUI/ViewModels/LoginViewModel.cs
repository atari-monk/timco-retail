using Caliburn.Micro;
using System;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Api;

namespace TRMDesktopUI.ViewModels
{
	public class LoginViewModel : Screen
	{
		private string userName;
		private string password;
		private string errorMessage;
		private readonly IAPIHelper apiHelper;

		public LoginViewModel(IAPIHelper apiHelper)
		{
			this.apiHelper = apiHelper;
		}

		public string UserName
		{
			get { return userName; }
			
			set
			{
				userName = value;
				NotifyOfPropertyChange(() => UserName);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public string Password
		{
			get { return password; }

			set
			{
				password = value;
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public bool IsErrorVisible
		{
			get 
			{
				bool output = false;

				if (ErrorMessage?.Length > 0 )
				{
					output = true;
				}

				return output;
			}
		}

		public string ErrorMessage
		{
			get { return errorMessage; }
			set 
			{ 
				errorMessage = value;
				NotifyOfPropertyChange(() => ErrorMessage);
				NotifyOfPropertyChange(() => IsErrorVisible);
			}
		}

		public bool CanLogIn
		{
			get
			{
				bool output = false;

				if (UserName?.Length > 0 && Password?.Length > 0)
				{
					output = true;
				}

				return output;
			}
		}

		public async Task LogIn()
		{
			try
			{
				ErrorMessage = "";
				var result = await apiHelper.Authenticate(UserName, Password);

				await apiHelper.GetLoggedInUserInfo(result.Access_Token);
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
		}
	}
}