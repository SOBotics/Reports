using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reports.Config;
using Reports.Middleware;
using Reports.Services.LocalData.Reports;
using Reports.Services.Reports;

namespace Reports
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.Configure<DataStoreOptions>(Configuration.GetSection("DataStore"));
			services.Configure<HostingOptions>(Configuration.GetSection("Hosting"));

			services.AddMvc();
			services.AddResponseCompression();

			services.AddSingleton<IReportStore, ReportStore>();
			services.AddSingleton<ReportDeleter>();

			services.AddTransient<IIdGenerator, IdGenerator>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			// Make sure service is initialised.
			app.ApplicationServices.GetService<ReportDeleter>();

			app.UseResponseCompression();
			app.UseStaticFiles();
			app.UseReportViewCounter();
			app.UseMvc();
		}
	}
}
