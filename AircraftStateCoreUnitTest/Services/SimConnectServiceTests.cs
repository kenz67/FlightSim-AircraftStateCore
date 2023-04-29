using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.enums;
using AircraftStateCore.Helpers;
using AircraftStateCore.Models;
using AircraftStateCore.Services;
using AircraftStateCore.Services.Interfaces;
using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics.CodeAnalysis;

namespace AircraftStateCoreUnitTest.Services;

[ExcludeFromCodeCoverage]
public class SimConnectServiceTests
{
	[Fact]
	public void TestVerifyAutoSave()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var expectFalse1 = svc.VerifyAutoSave();

		svc.SendDataToSim(new PlaneDataStruct(), true, true);
		svc.masterBatteryOn = true;
		var expectFalse2 = svc.VerifyAutoSave();

		settingsData.AutoSave = true;
		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);

		var expectTrue = svc.VerifyAutoSave();

		Assert.False(expectFalse1, "All Settings off, should be false");
		Assert.False(expectFalse2, "Send To Sim on, others off, should be false");
		Assert.True(expectTrue);
	}

	[Fact]
	public void SaveDataToDbTest()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = new PlaneDataStruct
		{
			com1Active = 124.85,
			com1Standby = 124.85
		};
		svc.SimData = data;

		svc.SaveDataToDb("Test1");

		data.latitude = 10;
		svc.SimData = data;
		svc.SaveDataToDb("Test2");

		data.longitude = 20;
		svc.SimData = data;
		svc.SaveDataToDb("Test3");

		data.com1Active = 155.0;
		svc.SimData = data;
		svc.SaveDataToDb("Test4");

		mockPlaneDataRepo.Verify(p => p.SaveDataForProfile("Test1", It.IsAny<PlaneDataStruct>()), Times.Never);
		mockPlaneDataRepo.Verify(p => p.SaveDataForProfile("Test2", It.IsAny<PlaneDataStruct>()), Times.Never);
		mockPlaneDataRepo.Verify(p => p.SaveDataForProfile("Test3", It.IsAny<PlaneDataStruct>()), Times.Never);
		mockPlaneDataRepo.Verify(p => p.SaveDataForProfile("Test4", data), Times.Once);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Configuration Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendConfigurationDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.ConfigurationFlaps, true) },
				{  new AvailableDataItem("0", FieldText.ConfigurationParkingBrake, true) },
				{  new AvailableDataItem("0", FieldText.ConfigurationElevatorTrim, true) },
				{  new AvailableDataItem("0", FieldText.ConfigurationRudderTrim, true) },
				{  new AvailableDataItem("0", FieldText.ConfigurationAileronTrim, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();

		var trimData = new TrimData { elevatorTrim = data.elevtorTrim, aileronTrim = data.aileronTrim, rudderTrim = data.rudderTrim };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.PARKING_BRAKE_SET, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimTrimData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), trimData), Times.Once);

		data.flapsIndex = 0;
		svc.SendDataToSim(data, false, false);

		data.flapsIndex = 2;
		svc.SendDataToSim(data, false, false);

		data.flapsIndex = 3;
		svc.SendDataToSim(data, false, false);

		data.flapsIndex = 4;
		svc.SendDataToSim(data, false, false);

		data.flapsIndex = 5;
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_UP, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_1, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_2, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_3, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_4, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_DOWN, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
	}

	[Fact]
	public void SendConfigurationDataNegativeTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.ConfigurationFlaps, false) },
				{  new AvailableDataItem("0", FieldText.ConfigurationParkingBrake, false) },
				{  new AvailableDataItem("0", FieldText.ConfigurationElevatorTrim, false) },
				{  new AvailableDataItem("0", FieldText.ConfigurationRudderTrim, false) },
				{  new AvailableDataItem("0", FieldText.ConfigurationAileronTrim, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();

		svc.SimData = new PlaneDataStruct { elevtorTrim = 10 };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<uint>(),
			It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);

		var trimData = new TrimData { elevatorTrim = 10, aileronTrim = 0, rudderTrim = 0 };
		mockProxy.Verify(p => p.SetDataOnSimObject(It.IsAny<Enum>(), It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), trimData), Times.Once);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Lights Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendLightDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LightsBeacon, true) },
				{  new AvailableDataItem("0", FieldText.LightsCabin, true) },
				{  new AvailableDataItem("0", FieldText.LightsLanding, true) },
				{  new AvailableDataItem("0", FieldText.LightsLogo, true) },
				{  new AvailableDataItem("0", FieldText.LightsNav, true) },
				{  new AvailableDataItem("0", FieldText.LightsPanel, true) },
				{  new AvailableDataItem("0", FieldText.LightsRecognition, true) },
				{  new AvailableDataItem("0", FieldText.LightsStrobe, true) },
				{  new AvailableDataItem("0", FieldText.LightsTaxi, true) },
				{  new AvailableDataItem("0", FieldText.LightsWing, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.BEACON_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.LANDING_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TAXI_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.STROBE_LIGHT, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.PANEL_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.RECOGNITION_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.WING_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.CABIN_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.LOGO_LIGHT, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
	}

	[Fact]
	public void SendLightDataNegativeTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LightsBeacon, false) },
				{  new AvailableDataItem("0", FieldText.LightsCabin, false) },
				{  new AvailableDataItem("0", FieldText.LightsLanding, false) },
				{  new AvailableDataItem("0", FieldText.LightsLogo, false) },
				{  new AvailableDataItem("0", FieldText.LightsNav, false) },
				{  new AvailableDataItem("0", FieldText.LightsPanel, false) },
				{  new AvailableDataItem("0", FieldText.LightsRecognition, false) },
				{  new AvailableDataItem("0", FieldText.LightsStrobe, false) },
				{  new AvailableDataItem("0", FieldText.LightsTaxi, false) },
				{  new AvailableDataItem("0", FieldText.LightsWing, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.BEACON_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.LANDING_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TAXI_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.STROBE_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.PANEL_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.RECOGNITION_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.WING_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.CABIN_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.LOGO_LIGHT, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Fuel Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendFuelDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.FuelQtyBoth, true) },
				{  new AvailableDataItem("0", FieldText.FuelSelector, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();

		var fuelData = new FuelData { fuelLeft = data.fuelLeft, fuelRight = data.fuelRight };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FUEL_SELECTOR_SET, (uint)data.fuelSelector, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneFuelData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), fuelData), Times.Once);
	}

	[Fact]
	public void SendFuelDataNegativeTests1()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.FuelQtyBoth, true) },
				{  new AvailableDataItem("0", FieldText.FuelSelector, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();

		svc.SimData = new PlaneDataStruct { fuelLeft = 10 };
		svc.SendDataToSim(data, true, true);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<uint>(),
			It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);

		var fuelData = new FuelData { fuelLeft = 10, fuelRight = 20 };
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneFuelData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), fuelData), Times.Never);
	}

	[Fact]
	public void SendFuelDataNegativeTests2()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.FuelQtyBoth, false) },
				{  new AvailableDataItem("0", FieldText.FuelSelector, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		//Send with all turned off
		var data = GetSampleData();

		svc.SimData = new PlaneDataStruct { fuelLeft = 30, fuelRight = 40 };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<uint>(),
			It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);

		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneFuelData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), It.IsAny<FuelData>()), Times.Never);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Fuel Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendLocationDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LocationAltitude, true) },
				{  new AvailableDataItem("0", FieldText.LocationHeading, true) },
				{  new AvailableDataItem("0", FieldText.LocationLatitude, true) },
				{  new AvailableDataItem("0", FieldText.LocationLongitude, true) },
				{  new AvailableDataItem("0", FieldText.LocationPitch, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();

		var locationData = new LocationData { altitude = data.altitude, heading = data.heading, latitude = data.latitude, longitude = data.longitude, pitch = data.pitch };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneLocationData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), locationData), Times.Once);
	}

	[Fact]
	public void SendLocationDataNegativeTests1()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LocationAltitude, true) },
				{  new AvailableDataItem("0", FieldText.LocationHeading, true) },
				{  new AvailableDataItem("0", FieldText.LocationLatitude, true) },
				{  new AvailableDataItem("0", FieldText.LocationLongitude, true) },
				{  new AvailableDataItem("0", FieldText.LocationPitch, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();

		svc.SimData = new PlaneDataStruct { longitude = 10 };
		svc.SendDataToSim(data, true, true);

		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneLocationData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), It.IsAny<LocationData>()), Times.Never);
	}

	[Fact]
	public void SendLocationDataNegativeTests2()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LocationAltitude, false) },
				{  new AvailableDataItem("0", FieldText.LocationHeading, false) },
				{  new AvailableDataItem("0", FieldText.LocationLatitude, false) },
				{  new AvailableDataItem("0", FieldText.LocationLongitude, false) },
				{  new AvailableDataItem("0", FieldText.LocationPitch, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		//Send with all turned off
		var data = GetSampleData();

		svc.SimData = new PlaneDataStruct { altitude = 10, heading = 20, latitude = 30, longitude = 40, pitch = 50 };
		svc.SendDataToSim(data, false, false);

		var locationData = new LocationData { altitude = 10, heading = 20, latitude = 30, longitude = 40, pitch = 50 };
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneLocationData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), locationData), Times.Once);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Obs Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendLObsDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.ObsAdfCard, true) },
				{  new AvailableDataItem("0", FieldText.ObsObs1, true) },
				{  new AvailableDataItem("0", FieldText.ObsObs2, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_CARD_SET, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.OBS1, (uint)2, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.OBS2, (uint)3, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
	}

	[Fact]
	public void SendObsDataNegativeTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.ObsAdfCard, false) },
				{  new AvailableDataItem("0", FieldText.ObsObs1, false) },
				{  new AvailableDataItem("0", FieldText.ObsObs2, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_CARD_SET, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.OBS1, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.OBS2, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Obs Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendLOtherDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.OtherHeadingBug, true) },
				{  new AvailableDataItem("0", FieldText.OtherKolhsman, true) },
				{  new AvailableDataItem("0", FieldText.OtherBatteryVoltage, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		var batteryData = new BatteryVoltage { batteryVoltage = data.batteryVoltage };

		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.HEADING_BUG_SET, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.KOHLSMAN_SET, (uint)1083, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPowerData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), batteryData), Times.Once);
	}

	[Fact]
	public void SendOtherDataNegativeTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.OtherHeadingBug, false) },
				{  new AvailableDataItem("0", FieldText.OtherKolhsman, false) },
				{  new AvailableDataItem("0", FieldText.OtherBatteryVoltage, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.HEADING_BUG_SET, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.KOHLSMAN_SET, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPowerData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), It.IsAny<BatteryVoltage>()), Times.Never);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Power Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendPowerDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.PowerMasterAlternator, true) },
				{  new AvailableDataItem("0", FieldText.PowerMasterAvionics, true) },
				{  new AvailableDataItem("0", FieldText.PowerMasterBattery, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.MASTER_ALTERNATOR, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.MASTER_BATTERY, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.AVIONICS_MASTER, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
	}

	[Fact]
	public void SendPowerDataNegativeTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.PowerMasterAlternator, false) },
				{  new AvailableDataItem("0", FieldText.PowerMasterAvionics, false) },
				{  new AvailableDataItem("0", FieldText.PowerMasterBattery, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.MASTER_ALTERNATOR, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.MASTER_BATTERY, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.AVIONICS_MASTER, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Lights Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Fact]
	public void SendLRadioDataTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.RadiosCom1Both, true) },
				{  new AvailableDataItem("0", FieldText.RadiosCom2Both, true) },
				{  new AvailableDataItem("0", FieldText.RadiosNav1Both, true) },
				{  new AvailableDataItem("0", FieldText.RadiosNav2Both, true) },
				{  new AvailableDataItem("0", FieldText.RadiosAdfBoth, true) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_RADIO_SET_HZ, (uint)121500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_RADIO_SET_HZ, (uint)120500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_STBY_RADIO_SET_HZ, (uint)121500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM2_RADIO_SET_HZ, (uint)122500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM2_STBY_RADIO_SET_HZ, (uint)123500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV1_RADIO_SET_HZ, (uint)114000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV1_STBY_SET_HZ, (uint)115000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV2_RADIO_SET_HZ, (uint)116000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV2_STBY_SET_HZ, (uint)117000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_STBY_SET, (uint)268435456, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_STBY_SET, (uint)536870912, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Once);
	}

	[Fact]
	public void SendRadioDataNegativeTests()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.RadiosCom1Both, false) },
				{  new AvailableDataItem("0", FieldText.RadiosCom2Both, false) },
				{  new AvailableDataItem("0", FieldText.RadiosNav1Both, false) },
				{  new AvailableDataItem("0", FieldText.RadiosNav2Both, false) },
				{  new AvailableDataItem("0", FieldText.RadiosAdfBoth, false) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_RADIO_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_STBY_RADIO_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM2_RADIO_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM2_STBY_RADIO_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV1_RADIO_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV1_STBY_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV2_RADIO_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV2_STBY_SET_HZ, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_STBY_SET, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_STBY_SET, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);
	}

	//////////////////////////////////////////////////////////////////////////
	// Setup Data
	//////////////////////////////////////////////////////////////////////////
	private static PlaneDataStruct GetSampleData()
	{
		return new PlaneDataStruct
		{
			//Plane Configuration Data
			flapsIndex = 1,
			parkingBrake = true,
			elevtorTrim = 1,
			rudderTrim = -2,
			aileronTrim = 3,

			//Fuel Data
			fuelLeft = 1,
			fuelRight = 2,
			fuelSelector = 3,

			//Lights Data
			lightBeacon = true,
			lightCabin = true,
			lightLanding = true,
			lightLogo = false,
			lightPanel = true,
			lightNav = true,
			lightRecognition = true,
			lightStrobe = false,
			lightTaxi = true,
			lightWing = true,

			//Location Data
			altitude = 1,
			heading = 2,
			latitude = 3,
			longitude = 4,
			pitch = 5,

			//OBS Data
			adfCard = 1,
			obs1 = 2,
			obs2 = 3,

			//Other Data
			headingBug = 1,
			kohlsman = 2,
			batteryVoltage = 3,

			//Power Data
			masterAlternator = true,
			masterBattery = true,
			masterAvionics = true,

			//Radio Data
			com1Active = 120.5,
			com1Standby = 121.5,
			com2Active = 122.5,
			com2Standby = 123.5,
			adfActive = 201,
			adfStandby = 202,
			nav1Active = 114.0,
			nav1Standby = 115.0,
			nav2Active = 116.0,
			nav2Standby = 117.0
		};
	}
}