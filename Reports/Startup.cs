using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reports.Config;
using Reports.Dependencies.ReportStore;

namespace Reports
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.Configure<ReportStoreOptions>(Configuration.GetSection("ReportStore"));
			services.Configure<HostingOptions>(Configuration.GetSection("Hosting"));
			services.AddMvc();
			services.AddSingleton<IReportStore, ReportStore>();
			//services.AddSingleton(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseStaticFiles();
			app.UseMvc();
		}
	}
}
