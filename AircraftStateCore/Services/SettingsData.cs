using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Models;
using AircraftStateCore.Services.Interfaces;
using BootstrapBlazor.Components;

namespace AircraftStateCore.Services;

public class SettingsData : ISettingsData
{
	public Settings Settings { get; set; }
	private readonly ISettingsRepo _settingsRepo;

	public event Func<Task> OnChangeAsync;

	public SettingsData(ISettingsRepo settingsRepo)
	{
		_settingsRepo = settingsRepo;
	}

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
		//TODO - Is this needed?
		await OnChangeAsync();
	}

	public List<AvailableDataItem> GetSelectedData()
	{
		return Settings.SelectedData.Where(s => s.enabled).ToList();
	}

	public async Task SaveSettings(List<SelectedItem> SelectedItems)
	{
		foreach (var item in Settings.SelectedData)
		{
			item.enabled = SelectedItems.Exists(s => s.Value.Equals(item.value));
		}

		await _settingsRepo.SaveSettings(Settings);
	}
}
