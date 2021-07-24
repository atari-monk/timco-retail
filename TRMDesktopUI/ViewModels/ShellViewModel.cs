using Caliburn.Micro;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : Conductor<object>
	{
		private readonly LoginViewModel loginVM;

		public ShellViewModel(LoginViewModel loginVM)
		{
			this.loginVM = loginVM;
			ActivateItem(loginVM);
		}
	}
}