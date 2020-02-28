using Autofac;
using BlazorContacts.Application.Infrastructure.MediatR;
using BlazorContacts.Application.Interfaces.Notifications;
using BlazorContacts.MessageQueues.Infrastructure;
using MediatR;
namespace BlazorWeatherCastDemo.Extensions
{
	public static class ContainerExtensions
	{
		public static ContainerBuilder RegisterServices(this ContainerBuilder builder)
		{
			// PLEASE FIRST READ https://autofaccn.readthedocs.io/en/latest/lifetime/working-with-scopes.html
			// also to help your lifecycle migration read this https://devblogs.microsoft.com/cesardelatorre/comparing-asp-net-core-ioc-service-life-times-and-autofac-ioc-instance-scopes/

			// MediatR behaviors : https://github.com/jbogard/MediatR/wiki/Behaviors
			builder.RegisterGeneric(typeof(RequestLogCommandQueryBehavior<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerDependency();
			builder.RegisterGeneric(typeof(RequestValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>)).InstancePerDependency();
			builder.RegisterGeneric(typeof(MassTransitDispatcher<>)).As(typeof(IMessageDispatcher<>)).SingleInstance();

			return builder;
		}
	}
}
