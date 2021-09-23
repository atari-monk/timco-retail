using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
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
		private UserModel selectedUser;
		private string selectedUserName;
		private BindingList<string> userRoles = new BindingList<string>();
		private BindingList<string> availableRoles = new BindingList<string>();
		private string selectedUserRole;
		private string selectedAvailableRole;

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

		public UserModel SelectedUser
		{
			get
			{
				return selectedUser;
			}
			set
			{
				selectedUser = value;
				SelectedUserName = value.Email;
				UserRoles.Clear();
				UserRoles = new BindingList<string>(value.Roles.Select(x => x.Value).ToList());
				LoadRoles();
				NotifyOfPropertyChange(() => SelectedUser);
			}
		}

		public string SelectedUserRole
		{
			get { return selectedUserRole; }
			set 
			{ 
				selectedUserRole = value;
				NotifyOfPropertyChange(() => SelectedUserRole);
			}
		}

		public string SelectedAvailableRole
		{
			get { return selectedAvailableRole; }
			set
			{
				selectedAvailableRole = value;
				NotifyOfPropertyChange(() => SelectedAvailableRole);
			}
		}

		public string SelectedUserName
		{
			get
			{
				return selectedUserName;
			}
			set
			{
				selectedUserName = value;
				NotifyOfPropertyChange(() => SelectedUserName);
			}
		}

		public BindingList<string> UserRoles
		{
			get
			{
				return userRoles;
			}
			set
			{
				userRoles = value;
				NotifyOfPropertyChange(() => UserRoles);
			}
		}

		public BindingList<string> AvailableRoles
		{
			get
			{
				return availableRoles;
			}
			set
			{
				availableRoles = value;
				NotifyOfPropertyChange(() => AvailableRoles);
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

		private async Task LoadRoles()
		{
			var roles = await userEndpoint.GetAllRoles();
			foreach (var role in roles)
			{
				if(UserRoles.IndexOf(role.Value) < 0)
				{
					AvailableRoles.Add(role.Value);
				}
			}
		}

		public async void AddSelectedRole()
		{
			await userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

			UserRoles.Add(SelectedAvailableRole);
			AvailableRoles.Remove(SelectedAvailableRole);
		}

		public async void RemoveSelectedRole()
		{
			await userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

			AvailableRoles.Add(SelectedUserRole);
			UserRoles.Remove(SelectedUserRole);
		}
	}
}