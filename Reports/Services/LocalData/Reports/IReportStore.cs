using Reports.Models;

namespace Reports.Services.LocalData.Reports
{
	public interface IReportStore
	{
		string[] IDs { get; }

		bool Exists(string id);

		Report Get(string id);

		void Save(Report r);

		void Delete(string id);
	}
}
