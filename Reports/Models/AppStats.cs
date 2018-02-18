namespace Reports.Models
{
	public class AppStats
	{
		public string Name { get; set; }

		public string AppURL { get; set; }

		public int TotalViews { get; set; }

		public int LiveReports { get; set; }

		public double ReportsPerDay { get; set; }

		public double ViewsPerReport { get; set; }
	}
}
