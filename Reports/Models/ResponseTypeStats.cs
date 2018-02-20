using System;

namespace Reports.Models
{
	public class ResponseTypeStats
	{
		public int Requests { get; set; }

		public double MedianResponseTime { get; set; }

		public string ResponseTimePretty
		{
			get
			{
				if (Requests > 0 && MedianResponseTime < 0.5)
				{
					return "<1";
				}

				return Math.Round(MedianResponseTime).Prettify();
			}
		}

		public long BytesTransferred { get; set; }
	}
}
