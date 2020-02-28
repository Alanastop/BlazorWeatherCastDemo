using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlazorContacts.Persistence.Infrastracture
{
	public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext> where TContext : DbContext
	{
		private const string ConnectionStringName = "BlazorContactsDatabase";
		private const string AspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";
		protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);

		public TContext CreateDbContext(string[] args)
		{
			string basePath;
#if DEBUG
			basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}BlazorWeatherCastDemo", Path.DirectorySeparatorChar);
#else
			basePath = Directory.GetCurrentDirectory();
#endif

			return Create(basePath, Environment.GetEnvironmentVariable(AspNetCoreEnvironment));
		}


		private TContext Create(string basePath, string environmentName)
		{
			var config = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json")
				.AddJsonFile("appsettings.Local.json", true)
				.AddJsonFile($"appsettings.{environmentName}.json", true)
				.AddEnvironmentVariables()
				.Build();

			var connectionString = config.GetConnectionString(ConnectionStringName);
			return Create(connectionString);
		}

		private TContext Create(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentException($"Connection string '{ConnectionStringName}' is null or empty.", nameof(connectionString));

#if DEBUG
			Debug.WriteLine($"DesignTimeDbContextFactoryBase.Create(string): Connection string: {connectionString}");
#endif
			var optionsBuilder = new DbContextOptionsBuilder<TContext>();

			optionsBuilder.UseSqlServer(connectionString);

			return CreateNewInstance(optionsBuilder.Options);
		}
	}
}

