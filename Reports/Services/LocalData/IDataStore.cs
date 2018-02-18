namespace Reports.Services.LocalData
{
	public interface IDataStore
	{
		string[] IDs { get; }

		bool Exists(string id);

		T GetData<T>(string id);

		void SetData<T>(string id, T data);

		void Delete(string id);
	}
}
