using System;
using ZeroFormatter;

namespace Reports.Services.LocalData.MetaStats
{
	[ZeroFormattable]
	public class RequestResponseStat
	{
		[Index(0)]
		public virtual DateTime ExecutedAt { get; set; }

		[Index(1)]
		public virtual long Size { get; set; }

		[Index(2)]
		public virtual long Time { get; set; }
	}
}
