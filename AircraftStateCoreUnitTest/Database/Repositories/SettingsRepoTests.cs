using AircraftStateCore.DAL.DatabaseContext;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.enums;
using AircraftStateCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace AircraftStateCore.DAL.Repositories.Tests;

[ExcludeFromCodeCoverage]
public class SettingsRepoTests
{
	private readonly IServiceProvider _serviceProvider;

	public SettingsRepoTests()
	{
		var services = new ServiceCollection();

		_serviceProvider = services
			.AddDbContext<AircraftStateContext>(optionsAction: options => options.UseInMemoryDatabase("Settings"))
			.AddSingleton<ISettingsRepo, SettingsRepo>()
			.BuildServiceProvider();

		var context = _serviceProvider.GetRequiredService<AircraftStateContext>();
		context.RemoveRange(context.ApplicationSettings);
		context.ApplicationSettings.Add(new ApplicationSettingsDatum { DataKey = SettingDefinitions.BlockLocation, DataValue = "True" });
		context.ApplicationSettings.Add(new ApplicationSettingsDatum { DataKey = SettingDefinitions.BlockFuel, DataValue = "False" });
		context.ApplicationSettings.Add(new ApplicationSettingsDatum { DataKey = SettingDefinitions.AutoSave, DataValue = "False" });
		context.ApplicationSettings.Add(new ApplicationSettingsDatum { DataKey = SettingDefinitions.ShowSaveAs, DataValue = "True" });
		context.ApplicationSettings.Add(new ApplicationSettingsDatum
		{
			DataKey = SettingDefinitions.DataToSend,
			DataValue =
			@"[{""txt"":""Off"",""value"":""0000"",""enabled"":false}
				,{""txt"":""On"",""value"":""0000.0"",""enabled"":true}]"
		});
		context.ApplicationSettings.Add(new ApplicationSettingsDatum { DataKey = SettingDefinitions.Version, DataValue = "1.2.3" });
		context.SaveChanges();
	}

	[Fact()]
	public async Task GetSettingsTest()
	{
		var repo = _serviceProvider.GetRequiredService<ISettingsRepo>();
		var settings = await repo.GetSettings();

		Assert.True(settings.BlockLocation);
		Assert.False(settings.BlockFuel);
		Assert.False(settings.AutoSave);
		Assert.True(settings.ShowSaveAs);
		Assert.Equal("1.2.3", settings.Version);
		Assert.Equal(2, settings.SelectedData.Count);
		Assert.Contains(settings.SelectedData, s => s.txt.Equals("Off") && !s.enabled);
		Assert.Contains(settings.SelectedData, s => s.txt.Equals("On") && s.enabled);
	}

	[Fact()]
	public async Task SaveSettingsTest()
	{
		var repo = _serviceProvider.GetRequiredService<ISettingsRepo>();
		var newSettings = new Settings
		{
			AutoSave = true,
			ShowApplyForm = true,
			ShowSaveAs = false,
			BlockFuel = true,
			BlockLocation = true,
			SelectedData = new List<AvailableDataItem>
			{
				{  new AvailableDataItem (string.Empty, "On", false) },
				{  new AvailableDataItem (string.Empty, "Off", true)  },
			},
			Version = "x.y.z",
		};

		await repo.SaveSettings(newSettings);
		var settings = await repo.GetSettings();
		Assert.True(settings.BlockLocation);
		Assert.True(settings.BlockFuel);
		Assert.True(settings.AutoSave);
		Assert.False(settings.ShowSaveAs);
		Assert.Equal("1.2.3", settings.Version);
		Assert.Equal(2, settings.SelectedData.Count);
		Assert.Contains(settings.SelectedData, s => s.txt.Equals("Off") && s.enabled);
		Assert.Contains(settings.SelectedData, s => s.txt.Equals("On") && !s.enabled);
	}

	[Fact()]
	public async Task SaveMissingSettingsTest()
	{
		var context = _serviceProvider.GetRequiredService<AircraftStateContext>();
		var repo = _serviceProvider.GetRequiredService<ISettingsRepo>();
		var newSettings = new Settings
		{
			AutoSave = true,
			ShowSaveAs = false,
			BlockFuel = true,
			BlockLocation = true,
			SelectedData = new List<AvailableDataItem>
			{
				{  new AvailableDataItem (string.Empty, "On", false) },
				{  new AvailableDataItem (string.Empty, "Off", true)  },
			},
		};

		context.RemoveRange(context.ApplicationSettings);
		await context.SaveChangesAsync();

		await repo.SaveSettings(newSettings);
		var settings = await repo.GetSettings();
		Assert.True(settings.BlockLocation);
		Assert.True(settings.BlockFuel);
		Assert.True(settings.AutoSave);
		Assert.False(settings.ShowSaveAs);
		Assert.Equal(2, settings.SelectedData.Count);
		Assert.Contains(settings.SelectedData, s => s.txt.Equals("Off") && s.enabled);
		Assert.Contains(settings.SelectedData, s => s.txt.Equals("On") && !s.enabled);
	}

	[Fact()]
	public async Task UpdateVersionTest()
	{
		var repo = _serviceProvider.GetRequiredService<ISettingsRepo>();
		Assert.True(await repo.UpdateVersion("x.y.z"));
		Assert.False(await repo.UpdateVersion("x.y.z"));

		var settings = await repo.GetSettings();
		Assert.Equal("x.y.z", settings.Version);
	}
}