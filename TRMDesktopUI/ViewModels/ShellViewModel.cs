using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
	public class ShellViewModel : 
		Conductor<object>
		, IHandle<LogOnEvent>
	{
		private readonly IEventAggregator events;
		private readonly SalesViewModel salesVM;

		public ShellViewModel(
			IEventAggregator events
			, SalesViewModel salesVM)
		{
			this.events = events;
			this.salesVM = salesVM;

			events.Subscribe(this);
			
			ActivateItem(IoC.Get<LoginViewModel>());
		}

		public void Handle(LogOnEvent message)
		{
			ActivateItem(salesVM);
		}
	}
}