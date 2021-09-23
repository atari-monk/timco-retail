using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : 
		Conductor<object>
		, IHandle<LogOnEvent>
	{
		private readonly IEventAggregator events;
		private readonly SalesViewModel salesVM;
		private readonly ILoggedInUserModel user;
		private readonly IAPIHelper helper;

		public bool IsLoggedIn
		{
			get
			{
				bool output = false;

				if (string.IsNullOrWhiteSpace(user.Token) == false)
				{
					output = true;
				}

				return output;
			}
		}

		public ShellViewModel(
			IEventAggregator events
			, SalesViewModel salesVM
			, ILoggedInUserModel user
			, IAPIHelper helper)
		{
			this.events = events;
			this.salesVM = salesVM;
			this.user = user;
			this.helper = helper;

			events.Subscribe(this);
			
			ActivateItem(IoC.Get<LoginViewModel>());
		}

		public void Handle(LogOnEvent message)
		{
			ActivateItem(salesVM);
			NotifyOfPropertyChange(() => IsLoggedIn);
		}

		public void ExitApplication()
		{
			TryClose();
		}

		public void UserManagement()
		{
			ActivateItem(IoC.Get<UserDisplayViewModel>());
		}

		public void LogOut()
		{
			user.ResetUserModel();
			helper.LogOffUser();
			ActivateItem(IoC.Get<LoginViewModel>());
			NotifyOfPropertyChange(() => IsLoggedIn);
		}
	}
}