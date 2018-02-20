using Microsoft.AspNetCore.Builder;

namespace Reports.Middleware
{
	public static class Extensions
	{
		public static IApplicationBuilder UseReportViewCounter(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ReportViewCounter>();
		}

		public static IApplicationBuilder UseResponseMetaStatsLogger(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ResponseMetaStatsLogger>();
		}
	}
}
