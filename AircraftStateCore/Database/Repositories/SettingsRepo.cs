using AircraftStateCore.DAL.DatabaseContext;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.enums;
using AircraftStateCore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AircraftStateCore.DAL.Repositories;

public class SettingsRepo : ISettingsRepo
{
	private readonly AircraftStateContext _dbContext;

	public SettingsRepo(AircraftStateContext dbContext)
	{
		_dbContext = dbContext;
	}

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
				case SettingDefinitions.ShowApplyForm: settings.ShowApplyForm = setting.DataValue.Equals("true", StringComparison.OrdinalIgnoreCase); break;
				case SettingDefinitions.AutoSave: settings.AutoSave = setting.DataValue.Equals("true", StringComparison.OrdinalIgnoreCase); break;
				case SettingDefinitions.ShowSaveAs: settings.ShowSaveAs = setting.DataValue.Equals("true", StringComparison.OrdinalIgnoreCase); break;
				case SettingDefinitions.DataToSend: settings.SelectedData = JsonConvert.DeserializeObject<List<AvailableDataItem>>(setting.DataValue); break;
				case SettingDefinitions.Version: settings.Version = setting.DataValue; break;
			}
		}

		return settings;
	}

	public async Task SaveSettings(Settings settings)
	{
		await SetIndividualSetting(SettingDefinitions.BlockLocation, settings.BlockLocation.ToString());
		await SetIndividualSetting(SettingDefinitions.BlockFuel, settings.BlockFuel.ToString());
		await SetIndividualSetting(SettingDefinitions.AutoSave, settings.AutoSave.ToString());
		await SetIndividualSetting(SettingDefinitions.ShowSaveAs, settings.ShowSaveAs.ToString());
		await SetIndividualSetting(SettingDefinitions.DataToSend, JsonConvert.SerializeObject(settings.SelectedData));
	}

	public async Task<bool> UpdateVersion(string VersionNumber)
	{
		var existingVersion = _dbContext.ApplicationSettings.Where(s => s.DataKey.Equals(SettingDefinitions.Version)).FirstOrDefault();
		if (existingVersion == null || !VersionNoDate(VersionNumber).Equals(VersionNoDate(existingVersion.DataValue)))
		{
			await SetIndividualSetting(SettingDefinitions.Version, VersionNumber);
			return true;
		}

		return false;
	}

	private static string VersionNoDate(string VersionNumber)
	{
		var parts = VersionNumber.Split(".");
		return $"{parts[0]}.{parts[1]}";
	}

	private async Task SetIndividualSetting(string key, string value)
	{
		var dbValue = _dbContext.ApplicationSettings.Where(s => s.DataKey.Equals(key)).FirstOrDefault();
		if (dbValue == null)
		{
			var newSetting = new ApplicationSettingsDatum { DataKey = key, DataValue = value };
			_dbContext.ApplicationSettings.Add(newSetting);
			await _dbContext.SaveChangesAsync();
		}
		else if (dbValue.DataValue != value)
		{
			dbValue.DataValue = value;
			await _dbContext.SaveChangesAsync();
		}
	}
}