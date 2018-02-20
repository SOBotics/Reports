using Microsoft.Extensions.Options;
using Reports.Config;

namespace Reports.Services.LocalData.MetaStats
{
	public class MetaStatStore : DataStore, IMetaStatStore
	{
		public const string ApiStatsKey = "API";
		public const string StaticFileStatsKey = "Static Files";
		public const string DynamicViewStatsKey = "Dynamic Views";

		public MetaStatStore(IOptions<DataStoreOptions> config) : base(config.Value.MetaStats)
		{

		}
	}
}
