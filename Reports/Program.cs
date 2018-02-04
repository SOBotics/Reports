using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Reports
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseConfiguration(
					new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("hosting.json", false)
					.Build()
				)
				.ConfigureAppConfiguration((hostContext, config) =>
				{
					config.Sources.Clear();
					config.AddJsonFile("config.json", false, true);
				})
				.Build();
	}
}
