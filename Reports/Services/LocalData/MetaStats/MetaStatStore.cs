using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Reports.Config;

namespace Reports.Services.LocalData.MetaStats
{
	public class MetaStatStore : DataStore, IMetaStatStore
	{
		public const string ApiTimesKey = "apiTimes";
		public const string DynamicTimesKey = "dynamicContentTimes";
		public const string StaticTimesKey = "staticContentTimes";

		public MetaStatStore(IOptions<DataStoreOptions> config) : base(config.Value.MetaStats)
		{

		}
	}
}
