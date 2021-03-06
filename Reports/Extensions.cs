﻿using System;

namespace Reports
{
	public static class Extensions
	{
		private const string numStyle = "{0:#,##0.##}";

		public static string Prettify(this int num, int? round = null) => Prettify((double)num, round);

		public static string Prettify(this double num, int? round = null)
		{
			if (round != null)
			{
				num = Math.Round(num, round.Value);
			}

			return string.Format(numStyle, num);
		}

		public static string ToReadableByteCount(this long num)
		{
			var bytes = num * 1.0;
			var unit = "B";

			if (bytes > 1024)
			{
				unit = "KiB";
				bytes /= 1024;
			}
			if (bytes > 1024)
			{
				unit = "MiB";
				bytes /= 1024;
			}
			if (bytes > 1024)
			{
				unit = "GiB";
				bytes /= 1024;
			}
			if (bytes > 1024)
			{
				unit = "TiB";
				bytes /= 1024;
			}

			bytes = Math.Round(bytes);

			return $"{bytes.Prettify()}{unit}";
		}
	}
}