namespace Reports.Models
{
	public class ServerStats
	{
		public int TotalReqs
		{
			get
			{
				return ApiReqCount + StaticReqCount + DynamicReqCount;
			}
		}

		public double AvgerageResponseTimeMS
		{
			get
			{
				return (MedianApiTime + MedianDynamicTime + MedianStaticTime) / 3;
			}
		}

		public long TotalDataProcessed
		{
			get
			{
				return ApiBytesProcessed + StaticBytesProcessed + DynamicBytesProcessed;
			}
		}

		public int ApiReqCount { get; set; }

		public int StaticReqCount { get; set; }

		public int DynamicReqCount { get; set; }

		public double MedianApiTime { get; set; }

		public double MedianStaticTime { get; set; }

		public double MedianDynamicTime { get; set; }

		public long ApiBytesProcessed { get; set; }

		public long StaticBytesProcessed { get; set; }

		public long DynamicBytesProcessed { get; set; }
	}
}
