using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorContacts.Application.Infrastructure.MediatR
{
	public class RequestLogCommandQueryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
	{
		private readonly ILogger _logger;

		public RequestLogCommandQueryBehavior(ILogger<TRequest> logger)
		{
			_logger = logger;
		}

		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var name = typeof(TRequest).Name;
			var data = JsonConvert.SerializeObject(request, Formatting.Indented,
										new JsonSerializerSettings
										{
											ReferenceLoopHandling = ReferenceLoopHandling.Ignore
										});

			_logger.LogInformation($"Application Request for {name} with data : {data}");

			return next();
		}
	}
}
