namespace AircraftStateCore.Services;

public static class DbCommon
{
	public static readonly string DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
	public static readonly string DbName = $"{DataPath}\\AircraftState\\AircraftState.sqlite";
}