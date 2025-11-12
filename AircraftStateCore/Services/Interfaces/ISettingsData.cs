using AircraftStateCore.Models;

namespace AircraftStateCore.Services.Interfaces;

public interface ISettingsData : IPageUpdate
{
	Task<Settings> ReadSettings();
	Task SaveSettings(Settings settings);

	// Settings Settings { get; set; }
	List<AvailableDataItem> GetSelectedData();
}