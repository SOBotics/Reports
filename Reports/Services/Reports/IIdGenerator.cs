namespace Reports.Services.Reports
{
	public interface IIdGenerator
	{
		string GetNewId(int idLength = 6);
	}
}
