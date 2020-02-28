using System.Threading.Tasks;

namespace BlazorContacts.Application.Interfaces.Notifications
{
	public interface IMessageDispatcher<TMessage>
	{
		Task Dispatch(TMessage message);
	}
}
