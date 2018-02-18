using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reports.Services.LocalData.Reports
{
	public class ReportDeleter : IDisposable
	{
		private ManualResetEvent waitMre;
		private IReportStore store;
		private bool dispose;



		public ReportDeleter(IReportStore reportStore)
		{
			store = reportStore;
			waitMre = new ManualResetEvent(false);
			Task.Run(() => DeleterLoop());
		}

		~ReportDeleter()
		{
			Dispose();
		}



		public void Dispose()
		{
			if (dispose) return;
			dispose = true;

			waitMre.Set();
			waitMre.Dispose();
			store = null;

			GC.SuppressFinalize(this);
		}


		private void DeleterLoop()
		{
			var waitTime = TimeSpan.FromMinutes(5);

			while (!dispose)
			{
				var toDelete = new HashSet<string>();

				foreach (var id in store.IDs)
				{
					var r = store.Get(id);

					if (DateTime.UtcNow > r.ExpiresAt)
					{
						toDelete.Add(id);
					}
				}

				foreach (var id in toDelete)
				{
					store.Delete(id);
				}

				waitMre.WaitOne(waitTime);
			}
		}
	}
}
