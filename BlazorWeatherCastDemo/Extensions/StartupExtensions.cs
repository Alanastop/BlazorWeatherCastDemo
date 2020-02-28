using BlazorWeatherCastDemo.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BlazorWeatherCastDemo.Extensions
{
	public static class Constants
	{
		public const string DefaultCorsPolicyName = "DefaultCorsPolicy";
	}

	public static class ServiceExtensions
	{
		public static IServiceCollection LoadCorsDefaultPolicy(this IServiceCollection services, IConfiguration configuration)
		{
			var corsConfig = configuration.GetSection("CorsOptions").Get<CorsOptions>();

			services.AddCors(opts =>
			{
				opts.AddPolicy(Constants.DefaultCorsPolicyName,
					p => p.AllowAnyHeader()
					.WithOrigins(corsConfig.BasicCors.Origins.ToArray())
					.WithMethods(corsConfig.BasicCors.AllowedMethods.ToArray())
					.AllowCredentials());
			});

			return services;
		}

		public static IServiceCollection InitializeMassTransit(this IServiceCollection services, IConfiguration configuration)
		{
			var massConfig = configuration.GetSection("MassTransitOptions").Get<MassTransitOptions>();

			services.AddMassTransit(svcCnfg =>
			{
				svcCnfg.AddBus(_ => Bus.Factory.CreateUsingRabbitMq(cfg =>
				{
					var host = cfg.Host(new Uri(massConfig.Endpoint), hostConfigurator =>
					{
						hostConfigurator.Username(massConfig.Username);
						hostConfigurator.Password(massConfig.Password);
					});

					cfg.ConfigureJsonSerializer(serializerSettings =>
					{
						//settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
						//settings.Converters = new List<JsonConverter> { new TimespanJsonConverter() };
						serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;

						return serializerSettings;
					});


					//cfg.ReceiveEndpoint(host, $"{massConfig.QueueNamePrefix}:{Environment.MachineName}", endpoint =>
					//{
					//	endpoint.AutoDelete = massConfig.AutoDelete;
					//	endpoint.UseConcurrencyLimit(1);

					//	if (Debugger.IsAttached)
					//	{
					//		endpoint.Handler<SampleMessage>(context =>
					//			{
					//				Debug.WriteLine($"Received message for {context.Message.UserId} : {context.Message.Comments}");
					//				return Task.CompletedTask;
					//			});
					//	}
					//});

				}));
			});

			return services;
		}

	}

	public static class AppBuilderExtensions
	{
		public static IApplicationBuilder LoadWebSocketsWithCors(this IApplicationBuilder builder, IConfiguration configuration)
		{
			var corsConfig = configuration.GetSection("CorsOptions").Get<CorsOptions>();

			var socketOpts = new WebSocketOptions();

			foreach (var allowedOrigin in corsConfig.WebSocketCors.AllowedOrigins)
			{
				socketOpts.AllowedOrigins.Add(allowedOrigin);
			}

			builder.UseWebSockets(socketOpts);
			return builder;
		}
	}
}
