using AircraftStateCore.DAL.DatabaseContext;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.enums;
using AircraftStateCore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AircraftStateCore.DAL.Repositories;

public class SettingsRepo(AircraftStateContext dbContext) : ISettingsRepo
{
	private readonly AircraftStateContext _dbContext = dbContext;

	public async Task<Settings> GetSettings()
	{
		var settings = new Settings();
		var dbSettings = await _dbContext.ApplicationSettings.ToListAsync();

		foreach (var setting in dbSettings)
		{
			switch (setting.DataKey)
			{
				case SettingDefinitions.BlockLocation: settings.BlockLocation = setting.DataValue.Equals("true", StringComparison.OrdinalIgnoreCase); break;
				case SettingDefinitions.BlockFuel: settings.BlockFuel = setting.DataValue.Equals("true", StringComparison.OrdinalIgnoreCase); break;
				case SettingDefinitions.AutoSave: settings.AutoSave = setting.DataValue.Equals("true", StringComparison.OrdinalIgnoreCase); break;
				case SettingDefinitions.DataToSend: settings.SelectedData = JsonConvert.DeserializeObject<List<AvailableDataItem>>(setting.DataValue); break;
				case SettingDefinitions.Version: settings.Version = setting.DataValue; break;
			}
		}

		return settings;
	}

	public async Task SaveSettings(Settings settings)
	{
		try
		{
			_dbContext.SaveChanges();
			await SetIndividualSetting(SettingDefinitions.BlockLocation, settings.BlockLocation.ToString());
			await SetIndividualSetting(SettingDefinitions.BlockFuel, settings.BlockFuel.ToString());
			await SetIndividualSetting(SettingDefinitions.AutoSave, settings.AutoSave.ToString());
			await SetIndividualSetting(SettingDefinitions.DataToSend, JsonConvert.SerializeObject(settings.SelectedData));
		}
		catch
		{ }
	}

	public async Task<bool> UpdateVersion(string VersionNumber)
	{
		var existingVersion = _dbContext.ApplicationSettings.Where(s => s.DataKey.Equals(SettingDefinitions.Version)).FirstOrDefault();
		var retValue = !VersionNoDate(VersionNumber).Equals(VersionNoDate(existingVersion?.DataValue ?? "no version"));
		if (existingVersion == null || !VersionNumber.Equals(existingVersion.DataValue))
		{
			await SetIndividualSetting(SettingDefinitions.Version, VersionNumber);
		}

		return retValue;
	}

	private static string VersionNoDate(string VersionNumber)
	{
		try
		{
			var parts = VersionNumber.Split(".");
			return $"{parts[0]}.{parts[1]}";
		}
		catch
		{
			return String.Empty;
		}
	}

	private async Task SetIndividualSetting(string key, string value)
	{
		var dbValue = _dbContext.ApplicationSettings.FirstOrDefault(s => s.DataKey.Equals(key));
		if (dbValue == null)
		{
			var newSetting = new ApplicationSettingsDatum { DataKey = key, DataValue = value };
			_dbContext.ApplicationSettings.Add(newSetting);
			await _dbContext.SaveChangesAsync();
		}
		else if (dbValue.DataValue != value)
		{
			_dbContext.Attach(dbValue);
			dbValue.DataValue = value;
			//_dbContext.ApplicationSettings.Update(dbValue);
			await _dbContext.SaveChangesAsync();
		}
	}
}