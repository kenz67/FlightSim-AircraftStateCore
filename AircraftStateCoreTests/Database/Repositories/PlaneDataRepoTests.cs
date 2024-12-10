using AircraftStateCore.DAL.DatabaseContext;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace AircraftStateCore.DAL.Repositories.Tests;

[ExcludeFromCodeCoverage]
public class PlaneDataRepoTests
{
	private readonly IServiceProvider _serviceProvider;

	public PlaneDataRepoTests()
	{
		var services = new ServiceCollection();

		_serviceProvider = services
			.AddDbContext<AircraftStateContext>(optionsAction: options => options.UseInMemoryDatabase($"Settings-{DateTime.Now.Microsecond}"))
			.AddSingleton<IPlaneDataRepo, PlaneDataRepo>()
			.BuildServiceProvider();

		const string sampleData = @"{
			""latitude"":40.542120933291464,""longitude"":122.35228244326578,""altitude"":11,
			""heading"":308,""pitch"":0.10814145798399287,
			""com1Active"":118.9,""com1Standby"":120.35,
			""com2Active"":119.2,""com2Standby"":119.45,
			""nav1Active"":117.1,""nav1Standby"":109.9,
			""nav2Active"":111.55,""nav2Standby"":117.2,
			""adfActive"":0.89,""adfStandby"":1.4,
			""obs1"":81.0,""obs2"":12.0,""adfCard"":0.0,
			""fuelLeft"":16.063632965087891,""fuelRight"":9.2027988433837891,""fuelSelector"":2,
			""parkingBrake"":false,
			""kohlsman"":29.92,
			""headingBug"":81.0,
			""flapsIndex"":2,
			""elevtorTrim"":-0.010227162802383912,	""rudderTrim"":1.0,""aileronTrim"":2.0,
			""masterBattery"":false,""masterAlternator"":false,""masterAvionics"":false,
			""batteryVoltage"":24.794763565063477,
			""lightNav"":false,""lightBeacon"":false,""lightLanding"":false,""lightTaxi"":false,
			""lightStrobe"":false,""lightPanel"":true,""lightRecognition"":false,""lightWing"":false,
			""lightCabin"":true,""lightLogo"":false,""junk"":true,""validData"":true}";

		var context = _serviceProvider.GetRequiredService<AircraftStateContext>();
		context.RemoveRange(context.PlaneData);
		context.ProfileData.Add(new ProfileDatum { Data = sampleData, ProfileName = "test profile" });
		context.ProfileData.Add(new ProfileDatum { Data = string.Empty, ProfileName = "test profile 2" });
		context.SaveChanges();
	}

	[Fact()]
	public async Task DeleteSavedProfileTest()
	{
		var context = _serviceProvider.GetRequiredService<AircraftStateContext>();
		var repo = _serviceProvider.GetRequiredService<IPlaneDataRepo>();

		await repo.DeleteSavedProfile("not exist");
		Assert.Equal(2, context.ProfileData.ToList().Count);

		await repo.DeleteSavedProfile("test profile");
		Assert.Single(context.ProfileData.ToList());
	}

	[Fact()]
	public async Task GetDataForProfileTest()
	{
		var repo = _serviceProvider.GetRequiredService<IPlaneDataRepo>();
		var data = await repo.GetDataForProfile("test profile");

		Assert.Equal(40.542120933291464, data.latitude);
		Assert.Equal(122.35228244326578, data.longitude);
		Assert.Equal(11, data.altitude);
		Assert.Equal(308, data.heading);
		Assert.Equal(0.10814145798399287, data.pitch);

		Assert.Equal(118.9, data.com1Active);
		Assert.Equal(120.35, data.com1Standby);
		Assert.Equal(119.2, data.com2Active);
		Assert.Equal(119.45, data.com2Standby);
		Assert.Equal(117.1, data.nav1Active);
		Assert.Equal(109.9, data.nav1Standby);
		Assert.Equal(111.55, data.nav2Active);
		Assert.Equal(117.2, data.nav2Standby);
		Assert.Equal(0.89, data.adfActive);
		Assert.Equal(1.4, data.adfStandby);

		Assert.Equal(81.0, data.obs1);
		Assert.Equal(12.0, data.obs2);
		Assert.Equal(0, data.adfCard);

		Assert.Equal(16.063632965087891, data.fuelLeft);
		Assert.Equal(9.2027988433837891, data.fuelRight);
		Assert.Equal(2, data.fuelSelector);

		Assert.False(data.parkingBrake);
		Assert.Equal(29.92, data.kohlsman);
		Assert.Equal(81, data.headingBug);

		Assert.Equal(2, data.flapsIndex);
		Assert.Equal(-0.010227162802383912, data.elevtorTrim);
		Assert.Equal(1, data.rudderTrim);
		Assert.Equal(2, data.aileronTrim);

		Assert.False(data.masterBattery);
		Assert.False(data.masterAlternator);
		Assert.False(data.masterAvionics);

		Assert.Equal(24.794763565063477, data.batteryVoltage);

		Assert.False(data.lightNav);
		Assert.False(data.lightBeacon);
		Assert.False(data.lightLanding);
		Assert.False(data.lightTaxi);
		Assert.False(data.lightStrobe);
		Assert.True(data.lightPanel);
		Assert.False(data.lightRecognition);
		Assert.False(data.lightWing);
		Assert.True(data.lightCabin);
		Assert.False(data.lightWing);
	}

	[Fact()]
	public async Task GetDataForBadProfileTest()
	{
		var repo = _serviceProvider.GetRequiredService<IPlaneDataRepo>();
		var data = await repo.GetDataForProfile("junk profile");

		Assert.Equal(0, data.latitude);
		Assert.Equal(0, data.longitude);
		Assert.Equal(0, data.altitude);
		Assert.Equal(0, data.heading);
		Assert.Equal(0, data.pitch);

		Assert.Equal(0, data.com1Active);
		Assert.Equal(0, data.com1Standby);
		Assert.Equal(0, data.com2Active);
		Assert.Equal(0, data.com2Standby);
		Assert.Equal(0, data.nav1Active);
		Assert.Equal(0, data.nav1Standby);
		Assert.Equal(0, data.nav2Active);
		Assert.Equal(0, data.nav2Standby);
		Assert.Equal(0, data.adfActive);
		Assert.Equal(0, data.adfStandby);

		Assert.Equal(0, data.obs1);
		Assert.Equal(0, data.obs2);
		Assert.Equal(0, data.adfCard);

		Assert.Equal(0, data.fuelLeft);
		Assert.Equal(0, data.fuelRight);
		Assert.Equal(0, data.fuelSelector);

		Assert.False(data.parkingBrake);
		Assert.Equal(0, data.kohlsman);
		Assert.Equal(0, data.headingBug);

		Assert.Equal(0, data.flapsIndex);
		Assert.Equal(0, data.elevtorTrim);
		Assert.Equal(0, data.rudderTrim);
		Assert.Equal(0, data.aileronTrim);

		Assert.False(data.masterBattery);
		Assert.False(data.masterAlternator);
		Assert.False(data.masterAvionics);

		Assert.Equal(0, data.batteryVoltage);

		Assert.False(data.lightNav);
		Assert.False(data.lightBeacon);
		Assert.False(data.lightLanding);
		Assert.False(data.lightTaxi);
		Assert.False(data.lightStrobe);
		Assert.False(data.lightPanel);
		Assert.False(data.lightRecognition);
		Assert.False(data.lightWing);
		Assert.False(data.lightCabin);
		Assert.False(data.lightWing);
	}

	[Fact()]
	public async Task GetSavedProfilesTest()
	{
		var repo = _serviceProvider.GetRequiredService<IPlaneDataRepo>();
		var profiles = await repo.GetSavedProfiles();

		Assert.Equal(2, profiles.Count);
		Assert.Contains("test profile", profiles);
		Assert.Contains("test profile 2", profiles);
	}

	[Fact()]
	public void SaveDataForProfileTest()
	{
		var repo = _serviceProvider.GetRequiredService<IPlaneDataRepo>();
		var data = new PlaneDataStruct
		{
			adfActive = 1,
			adfCard = 2,
			adfStandby = 3,
			aileronTrim = 4,
			altitude = 5,
			batteryVoltage = 5.5,
			com1Active = 6,
			com1Standby = 7,
			com2Active = 8,
			com2Standby = 9,
			elevtorTrim = 10,
			fuelLeft = 11,
			fuelRight = 12,
			flapsIndex = 13,
			fuelSelector = 14,
			heading = 15,
			headingBug = 16,
			kohlsman = 17,
			latitude = 18,
			lightBeacon = true,
			lightCabin = false,
			lightLanding = true,
			lightLogo = false,
			lightNav = true,
			lightPanel = false,
			lightRecognition = true,
			lightStrobe = false,
			lightTaxi = true,
			lightWing = false,
			longitude = 19,
			masterBattery = true,
			masterAlternator = false,
			masterAvionics = true,
			nav1Active = 20,
			nav1Standby = 21,
			nav2Active = 22,
			nav2Standby = 23,
			obs1 = 24,
			obs2 = 25,
			parkingBrake = false,
			pitch = 26,
			rudderTrim = 27,
		};

		repo.SaveDataForProfile("new profile", data);

		var context = _serviceProvider.GetRequiredService<AircraftStateContext>();
		var newProfile = context.ProfileData.ToList().Find(f => f.ProfileName.Equals("new profile")) as ProfileDatum;

		Assert.NotNull(newProfile);
		var readData = JsonConvert.DeserializeObject<PlaneDataStruct>(newProfile.Data);

		Assert.Equal(readData, data);
	}
}