using Microsoft.Extensions.Options;
using Reports.Config;
using Reports.Models;

namespace Reports.Services.LocalData.Reports
{
	public class ReportStore : DataStore, IReportStore
	{
		public ReportStore(IOptions<DataStoreOptions> config) : base(config.Value.Reports)
		{
			
		}

		public Report Get(string id) => GetData<Report>(id);

		public void Save(Report r) => SetData(r.ID, r);
	}
}
