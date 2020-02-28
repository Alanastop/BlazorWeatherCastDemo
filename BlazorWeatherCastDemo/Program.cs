using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlazorContacts.Application;
using BlazorContacts.Persistence;
using BlazorWeatherCastDemo.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using BlazorContacts.Application.Settings;
using MediatR;
using AutoMapper;
using BlazorContacts.Automapper.Common;

namespace BlazorWeatherCastDemo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// NLog: setup the logger first to catch all errors
			var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var configPath = Path.Combine(binPath, "nlog.config");
			var logger = NLogBuilder.ConfigureNLog(configPath).GetCurrentClassLogger();

			var host = CreateHostBuilder(args).Build();

			using (var scope = host.Services.CreateScope())
			{

				try
				{
					logger.Info($"Service starting...");

					using (var context = (BlazorContactsDBContext)scope.ServiceProvider.GetService<IBlazorContactsDBContext>())
					{
						var dbCreator = context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
						var migrator = context.GetService<IMigrator>();

						var pendingMigrations = context.Database.GetPendingMigrations();

						if (!dbCreator.Exists())
						{
							dbCreator.Create();

							//Initial migration with schema
							if (pendingMigrations.Any())
							{
								var initialSchemaMigration = pendingMigrations.First();
								migrator.Migrate(initialSchemaMigration);
								pendingMigrations = pendingMigrations.Except(new[] { initialSchemaMigration });
							}

							BlazorContactsDbInitializer.Seed(context);
						}

						if (pendingMigrations.Any())
						{
							foreach (var migration in pendingMigrations)
							{
								migrator.Migrate(migration);
							}
						}
					}

					host.Run();
				}
				catch (Exception ex)
				{
					logger.Error(ex, "An error occurred while starting the application.");
					throw;
				}
				finally
				{
					logger.Info($"Service stopped!");
					NLog.LogManager.Shutdown();
				}
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseServiceProviderFactory(new AutofacServiceProviderFactory()) //https://mderriey.com/2018/08/02/autofac-integration-in-asp-net-core-generic-hosts/
				.ConfigureContainer<ContainerBuilder>(builder =>
				{
					builder.RegisterServices(); // Custom DI using Autofac
				})
				.UseContentRoot(Directory.GetCurrentDirectory())
					.ConfigureAppConfiguration((hostingContext, config) =>
					{
						var env = hostingContext.HostingEnvironment;
						config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
								.AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true)
								.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
						config.AddEnvironmentVariables();
					})
				.ConfigureLogging(logging =>
				{
					logging.ClearProviders();
					logging.AddNLog();
					logging.SetMinimumLevel(LogLevel.Trace);
				})
				.ConfigureServices((builder, services) => {

					//services.Configure<CorsOptions>(_Configuration.GetSection("CorsOptions"));
					// Automapper
					services.AddAutoMapper(new Assembly[] { typeof(AutoMapperProfile).GetTypeInfo().Assembly });

					// MediatR :: Load from application layer where the mocked dbcontext exists
					// plus automatic validation using FluentValidation
					services.AddMediatR(new Assembly[] { typeof(BlazorContactsDBContext).GetTypeInfo().Assembly });

					// Database Context Registration
					services.AddDbContext<IBlazorContactsDBContext, BlazorContactsDBContext>(opts =>
					{
						opts.UseSqlServer(builder.Configuration.GetConnectionString("Database"));

						//if (_Configuration.GetValue<bool>("DbContextLogging"))
						//	opts.UseLoggerFactory(_LoggerFactory);
					});

					services.AddSingleton<ObjectCache>(s => MemoryCache.Default);
					services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
				})
				.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
	}
}
