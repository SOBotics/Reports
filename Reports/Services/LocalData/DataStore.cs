using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LZ4;
using ZeroFormatter;

namespace Reports.Services.LocalData
{
	public class DataStore : IDataStore
	{
		private readonly Dictionary<string, object> lck;
		private readonly string dir;

		public string[] IDs => lck.Keys.ToArray();



		public DataStore(string dataDir)
		{
			if (dataDir == null)
			{
				throw new ArgumentNullException(nameof(dataDir));
			}

			if (!Directory.Exists(dataDir))
			{
				Directory.CreateDirectory(dataDir);
				lck = new Dictionary<string, object>();
			}
			else
			{
				lck = Directory
					.EnumerateFiles(dataDir)
					.Select(Path.GetFileNameWithoutExtension)
					.ToDictionary(k => k, v => new object());
			}

			dir = dataDir;
		}



		public bool Exists(string id) => lck.ContainsKey(id);

		public T GetData<T>(string id)
		{
			if (!lck.ContainsKey(id))
			{
				return default(T);
			}

			var path = GetPath(id);

			byte[] compressedBytes;

			lock (lck[id])
			{
				// Lock object might have been removed from the
				// dictionary before we gained access to it.
				if (!lck.ContainsKey(id))
				{
					return default(T);
				}

				compressedBytes = File.ReadAllBytes(path);
			}

			var uncompressedBytes = LZ4Codec.Unwrap(compressedBytes);
			var obj = ZeroFormatterSerializer.Deserialize<T>(uncompressedBytes);

			return obj;
		}

		public void SetData<T>(string id, T data)
		{
			var path = GetPath(id);
			var uncompressedBytes = ZeroFormatterSerializer.Serialize(data);
			var compressedBytes = LZ4Codec.Wrap(uncompressedBytes, 0, uncompressedBytes.Length);

			if (lck.ContainsKey(id))
			{
				lock (lck[id])
				{
					File.WriteAllBytes(path, compressedBytes);
				}
			}
			else
			{
				lock (lck)
				{
					lck[id] = new object();
					File.WriteAllBytes(path, compressedBytes);
				}
			}
		}

		public void Delete(string id)
		{
			var file = GetPath(id);

			lock (lck[id])
			{
				File.Delete(file);

				lck.Remove(id);
			}
		}



		private string GetPath(string id) => Path.Combine(dir, id + ".lz4");
	}
}