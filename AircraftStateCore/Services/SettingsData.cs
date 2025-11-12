using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Models;
using AircraftStateCore.Services.Interfaces;

namespace AircraftStateCore.Services;

public class SettingsData(ISettingsRepo settingsRepo) : ISettingsData
{
	public Settings Settings { get; set; }
	private readonly ISettingsRepo _settingsRepo = settingsRepo;

	public event Func<Task> OnChangeAsync;

	public async Task<Settings> ReadSettings()
	{
		Settings = await _settingsRepo.GetSettings();

		if (Settings.SelectedData == null)
		{
			Settings.SelectedData = new AvailableData().Items;
		}
		else   //fill in possible new values
		{
			var all = new AvailableData();
			var missing = all.Items.Where(i => !Settings.SelectedData.Any(i2 => i2.txt == i.txt)).ToList();
			Settings.SelectedData.AddRange(missing);
		}

		return Settings;
	}

	public async Task UpdatePage()
	{
		await OnChangeAsync();
	}

	public List<AvailableDataItem> GetSelectedData()
	{
		return Settings.SelectedData.Where(s => s.enabled).ToList();
	}

	public async Task SaveSettings(Settings settings)
	{
		await _settingsRepo.SaveSettings(Settings);
	}
}
