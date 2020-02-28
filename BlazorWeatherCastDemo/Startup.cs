using BlazorContacts.Application;
using BlazorContacts.Persistence;
using BlazorWeatherCastDemo.Data;
using BlazorWeatherCastDemo.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Net.Http;
using System.Reflection;

namespace BlazorWeatherCastDemo
{
	public class Startup
	{
		private readonly IConfiguration _Configuration;
		private readonly ILoggerFactory _LoggerFactory;
		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_Configuration = configuration;
			_LoggerFactory = loggerFactory;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			// ******
			// BLAZOR COOKIE Auth Code (begin)
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = _ => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
			services.AddAuthentication(
				CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie();

			// Database Context Registration
			services.AddDbContext<IBlazorContactsDBContext, BlazorContactsDBContext>(opts =>
			{
				opts.UseSqlServer(_Configuration.GetConnectionString("BlazorContactsDatabase"));

				if (_Configuration.GetValue<bool>("DbContextLogging"))
					opts.UseLoggerFactory(_LoggerFactory);
			});

			services
				.AddControllersWithViews()
				.AddNewtonsoftJson((opts) => {

					opts.SerializerSettings.Formatting = Formatting.Indented;
					//TODO: Add DecimalRoundingJsonConverter
					//opts.SerializerSettings.Converters.Add(new DecimalRoundingJsonConverter())
					opts.SerializerSettings.Converters.Add(new StringEnumConverter());
					opts.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
					opts.SerializerSettings.NullValueHandling = NullValueHandling.Include;
					opts.SerializerSettings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
					opts.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
					opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

					opts.SerializerSettings.Culture = CultureInfo.InvariantCulture;
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddFluentValidation(config => config.RegisterValidatorsFromAssembly(typeof(IBlazorContactsDBContext).GetTypeInfo().Assembly));

			services.Configure<ForwardedHeadersOptions>(opts =>
			{
				opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
				//opts.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
				opts.ForwardedForHeaderName = "X-Cluster-Client-Ip";
			});

			// https://stackoverflow.com/questions/51145243/how-do-i-customize-asp-net-core-model-binding-errors
			services.Configure<ApiBehaviorOptions>(o => o.InvalidModelStateResponseFactory = actionContext => new BadRequestObjectResult(actionContext.ModelState));

			//https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-2.2
			services.AddHttpContextAccessor();

			//services.AddSignalR()
			services.LoadCorsDefaultPolicy(_Configuration);

			// Register MassTransit
			services.InitializeMassTransit(_Configuration);

			// BLAZOR COOKIE Auth Code (end)
			// ******
			services.AddRazorPages();
			services.AddServerSideBlazor();
			services.AddSingleton<WeatherForecastService>();

			// ******
			// BLAZOR COOKIE Auth Code (begin)
			// From: https://github.com/aspnet/Blazor/issues/1554
			// HttpContextAccessor
			services.AddHttpContextAccessor();
			services.AddScoped<HttpContextAccessor>();
			services.AddHttpClient();
			services.AddScoped<HttpClient>();
			// BLAZOR COOKIE Auth Code (end)
			// ******
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseForwardedHeaders();

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			// ******
			// BLAZOR COOKIE Auth Code (begin)
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseCookiePolicy();
			app.UseAuthentication();
			// BLAZOR COOKIE Auth Code (end)r
			// ******

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");

				// Please Read about SignalR security https://docs.microsoft.com/en-us/aspnet/core/signalr/security?view=aspnetcore-3.1
				// Default policy is loaded on LoadCorsDefaultPolicy in ConfigureServices
				app.UseCors(Constants.DefaultCorsPolicyName);

				app.LoadWebSocketsWithCors(_Configuration);
			});
		}
	}
}
