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
		private const string storeKey = "responseStats";
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

			var responseType = "";

			if (context.Request.Method == "POST")
			{
				responseType = MetaStatStore.ApiStatsKey;
			}
			else
			{
				var resourceExt = Path.GetExtension(context.Request.Path);

				if (!string.IsNullOrWhiteSpace(resourceExt))
				{
					responseType = MetaStatStore.StaticFileStatsKey;
				}
				else
				{
					responseType = MetaStatStore.DynamicViewStatsKey;
				}
			}

			size += context.Request.ContentLength ?? 0;
			size += context.Request.Headers.Sum(x => x.Key.Length + x.Value.Sum(z => z.Length));
			size += context.Response.Headers.Sum(x => x.Key.Length + x.Value.Sum(z => z.Length));

			UpdateStore(store, responseType, sw.ElapsedMilliseconds, size);
		}


		private void UpdateStore(IMetaStatStore store, string responseType, long time, long size)
		{
			var stats = store
				.GetData<Dictionary<string, HashSet<RequestResponseStat>>>(storeKey)
				?? new Dictionary<string, HashSet<RequestResponseStat>>
				{
					[MetaStatStore.ApiStatsKey] = new HashSet<RequestResponseStat>(),
					[MetaStatStore.StaticFileStatsKey] = new HashSet<RequestResponseStat>(),
					[MetaStatStore.DynamicViewStatsKey] = new HashSet<RequestResponseStat>()
				};

			var toDelete = new HashSet<DateTime>();

			foreach (var entry in stats[responseType])
			{
				if (entry.ExecutedAt.AddDays(7) < DateTime.UtcNow)
				{
					toDelete.Add(entry.ExecutedAt);
				}
			}

			stats[responseType].RemoveWhere(x => toDelete.Contains(x.ExecutedAt));

			stats[responseType].Add(new RequestResponseStat
			{
				ExecutedAt = DateTime.UtcNow,
				Size = size,
				Time = time
			});

			store.SetData(storeKey, stats);
		}
	}
}
