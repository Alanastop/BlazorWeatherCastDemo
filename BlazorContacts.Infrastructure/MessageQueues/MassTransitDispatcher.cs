using BlazorContacts.Application.Interfaces.Notifications;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace BlazorContacts.MessageQueues.Infrastructure
{
	public class MassTransitDispatcher<TMessage> : IMessageDispatcher<TMessage>, IDisposable
	{
		private readonly IBusControl _messageBus;

		public MassTransitDispatcher(IBusControl messageBus)
		{
			_messageBus = messageBus;
			_messageBus.Start();
		}

		public Task Dispatch(TMessage message)
		{
			return _messageBus.Publish(message);
		}

		public void Dispose()
		{
			_messageBus.Stop();
		}
	}
}
