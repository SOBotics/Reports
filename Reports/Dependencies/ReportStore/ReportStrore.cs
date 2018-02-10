using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reports.Config;
using Reports.Models;

namespace Reports.Dependencies.ReportStore
{
	public class ReportStore : IReportStore, IDisposable
	{
		private readonly IOptions<ReportStoreOptions> configAccessor;
		private readonly Dictionary<string, Report> reports;
		private readonly HashSet<string> allAppNames;
		private readonly ManualResetEvent deleteLoopMre;
		private readonly object lck;
		private int totalReports;
		private bool dispose;

		public HashSet<string> AppNames
		{
			get
			{
				return allAppNames;
			}
		}

		public int Count
		{
			get
			{
				return totalReports;
			}
		}

		public Report this[string id] => reports[id];



		public ReportStore(IOptions<ReportStoreOptions> ca)
		{
			configAccessor = ca;
			reports = new Dictionary<string, Report>();
			allAppNames = new HashSet<string>();
			deleteLoopMre = new ManualResetEvent(false);
			lck = new object();

			var reportDir = ca.Value.ReportDirectory;
			if (!Directory.Exists(reportDir))
			{
				Directory.CreateDirectory(reportDir);
			}

			var files = Directory.EnumerateFiles(reportDir);
			foreach (var f in files)
			{
				var json = File.ReadAllText(f);
				var r = JsonConvert.DeserializeObject<Report>(json);

				if (!allAppNames.Contains(r.AppName))
				{
					allAppNames.Add(r.AppName);
				}

				totalReports++;

				reports.Add(r.ID, r);
			}

			Task.Run(() => DeleteReportsLoop());
		}

		~ReportStore() => Dispose();

		public void Dispose()
		{
			if (dispose) return;
			dispose = true;

			deleteLoopMre.Set();
			deleteLoopMre.Dispose();
			reports.Clear();

			GC.SuppressFinalize(this);
		}

		public bool Exists(string id)
		{
			var res = false;

			//TODO: Do we really need to lock this?
			lock (lck)
			{
				res = reports.ContainsKey(id);
			}

			return res;
		}

		public void AddReport(Report report)
		{
			if (reports.ContainsKey(report.ID))
			{
				throw new Exception("Report ID collision.");
			}

			lock (lck)
			{
				if (!allAppNames.Contains(report.AppName))
				{
					allAppNames.Add(report.AppName);
				}

				totalReports++;

				reports.Add(report.ID, report);
			}

			var json = JsonConvert.SerializeObject(report);
			var reportPath = Path.Combine(configAccessor.Value.ReportDirectory, report.ID);

			File.WriteAllText(reportPath, json);
		}



		private void DeleteReportsLoop()
		{
			while (!dispose)
			{
				var reportsToDelete = new HashSet<Report>();

				lock (lck)
				foreach (var r in reports.Values)
				{
					if (DateTime.UtcNow > r.ExpiresAt)
					{
						reportsToDelete.Add(r);
					}
				}

				foreach (var r in reportsToDelete)
				{
					lock (lck)
					{
						totalReports--;
						reports.Remove(r.ID);
						
						if (reports.Values.All(x => x.AppName != r.AppName))
						{
							allAppNames.Remove(r.AppName);
						}
					}

					var reportPath = Path.Combine(configAccessor.Value.ReportDirectory, r.ID);

					try
					{
						File.Delete(reportPath);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Failed to delete report {r.ID} ({ex.Message}).");
					}
				}

				deleteLoopMre.WaitOne(TimeSpan.FromSeconds(5));
			}
		}
	}
}
