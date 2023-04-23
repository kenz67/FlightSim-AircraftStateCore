namespace AircraftStateCore.DAL.DatabaseContext;

public partial class AppVersionDatum
{
	public string VersionNumber { get; set; }

	public DateTime BuildDate { get; set; }

	public DateTime CreateDate { get; set; }

	public string Notes { get; set; }
}