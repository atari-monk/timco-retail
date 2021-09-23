using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
	public class UserDisplayViewModel : Screen
	{
		private readonly StatusInfoViewModel status;
		private readonly IWindowManager windowManager;
		private readonly IUserEndpoint userEndpoint;
		private BindingList<UserModel> users;

		public BindingList<UserModel> Users
		{
			get
			{
				return users;
			}
			set
			{
				users = value;
				NotifyOfPropertyChange(() => Users);
			}
		}

		public UserDisplayViewModel(
			StatusInfoViewModel status
			, IWindowManager windowManager
			, IUserEndpoint userEndpoint)
		{
			this.status = status;
			this.windowManager = windowManager;
			this.userEndpoint = userEndpoint;
		}

		protected override async void OnViewLoaded(object view)
		{
			base.OnViewLoaded(view);
			try
			{
				await LoadUsers();
			}
			catch (Exception ex)
			{
				dynamic settings = new ExpandoObject();
				settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				settings.ResizeMode = ResizeMode.NoResize;
				settings.Title = "System Error";

				if (ex.Message == "Unauthorized")
				{
					status.UpdateMessage(
						"Unauthorized Access"
						, "You dont have permission to interact with the Sales Form.");
					windowManager.ShowDialog(status, null, settings);
				}
				else
				{
					status.UpdateMessage(
						"FatalException"
						, ex.Message);
					windowManager.ShowDialog(status, null, settings);
				}

				TryClose();
			}
		}

		private async Task LoadUsers()
		{
			var userList = await userEndpoint.GetAll();
			Users = new BindingList<UserModel>(userList);
		}
	}
}