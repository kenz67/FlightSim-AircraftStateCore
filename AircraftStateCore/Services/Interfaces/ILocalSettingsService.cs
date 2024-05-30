using AircraftStateCore.Models;

namespace AircraftStateCore.Services.Interfaces
{
	public interface ILocalSettingsService
	{
		LocalSettings Settings { get; set; }

		void SaveSettings();
	}
}