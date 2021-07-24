using Caliburn.Micro;

namespace TRMDesktopUI.ViewModels
{
	public class LoginViewModel : Screen
	{
		private string userName;
		private string password;

		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				userName = value;
				NotifyOfPropertyChange(() => UserName);
				NotifyOfPropertyChange(() => CanLogIn);
			}
		}

		public string Password
		{
			get
			{
				return password;
			}

			set
			{
				password = value;
				NotifyOfPropertyChange(() => Password);
				NotifyOfPropertyChange(() => CanLogIn);
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

		public void LogIn()
		{

		}
	}
}