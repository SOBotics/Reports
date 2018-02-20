using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Reports.Services.LocalData.MetaStats;

namespace Reports.Middleware
{
	public class ResponseMetaStatsLogger
	{
		private readonly RequestDelegate nextMiddleware;

		public ResponseMetaStatsLogger(RequestDelegate next)
		{
			nextMiddleware = next;
		}

		public async Task Invoke(HttpContext context, IMetaStatStore store)
		{
			var sw = new Stopwatch();
			var size = 0L;

			//TODO: We should probably just implement
			// our own stream that tracks length with every write.
			using (var ms = new MemoryStream())
			{
				var oldBody = context.Response.Body;
				context.Response.Body = ms;

				sw.Start();

				await nextMiddleware(context);

				sw.Stop();

				size = ms.Length;

				if (context.Response.StatusCode == 200)
				{
					ms.Position = 0;
					await ms.CopyToAsync(oldBody);
				}
			}

			var storeKey = "";

			if (context.Request.Method == "POST")
			{
				storeKey = MetaStatStore.ApiTimesKey;
			}
			else
			{
				var resourceExt = Path.GetExtension(context.Request.Path);

				if (!string.IsNullOrWhiteSpace(resourceExt))
				{
					storeKey = MetaStatStore.StaticTimesKey;
				}
				else
				{
					storeKey = MetaStatStore.DynamicTimesKey;
				}
			}

			size += context.Request.ContentLength ?? 0;
			size += context.Request.Headers.Sum(x => x.Key.Length + x.Value.Sum(z => z.Length));
			size += context.Response.Headers.Sum(x => x.Key.Length + x.Value.Sum(z => z.Length));

			UpdateStore(store, storeKey, sw.ElapsedMilliseconds, size);
		}


		private void UpdateStore(IMetaStatStore store, string storeKey, long time, long size)
		{
			var stats = store
				.GetData<HashSet<RequestResponseStat>>(storeKey)
				?? new HashSet<RequestResponseStat>();

			var toDelete = new HashSet<DateTime>();

			foreach (var entry in stats)
			{
				if (entry.ExecutedAt.AddDays(7) < DateTime.UtcNow)
				{
					toDelete.Add(entry.ExecutedAt);
				}
			}

			stats.RemoveWhere(x => toDelete.Contains(x.ExecutedAt));

			stats.Add(new RequestResponseStat
			{
				ExecutedAt = DateTime.UtcNow,
				Size = size,
				Time = time
			});

			store.SetData(storeKey, stats);
		}
	}
}
