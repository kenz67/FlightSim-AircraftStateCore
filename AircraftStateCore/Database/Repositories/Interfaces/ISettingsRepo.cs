using AircraftStateCore.Models;

namespace AircraftStateCore.DAL.Repositories.Interfaces;

public interface ISettingsRepo
{
	Task<Settings> GetSettings();
	Task SaveSettings(Settings settings);
	Task<bool> UpdateVersion(String VersionNumber);
}