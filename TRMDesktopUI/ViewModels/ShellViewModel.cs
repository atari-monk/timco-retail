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
		private readonly SimpleContainer container;

		public ShellViewModel(
			IEventAggregator events
			, SalesViewModel salesVM
			, SimpleContainer container)
		{
			this.events = events;
			this.salesVM = salesVM;
			this.container = container;

			events.Subscribe(this);
			
			ActivateItem(container.GetInstance<LoginViewModel>());
		}

		public void Handle(LogOnEvent message)
		{
			ActivateItem(salesVM);
		}
	}
}