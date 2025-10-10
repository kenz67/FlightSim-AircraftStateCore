using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.enums;
using AircraftStateCore.Helpers;
using AircraftStateCore.Models;
using AircraftStateCore.Services;
using AircraftStateCore.Services.Interfaces;
using Microsoft.FlightSimulator.SimConnect;
using System.Diagnostics.CodeAnalysis;
using Assert = Xunit.Assert;

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
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = []
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

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
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = []
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

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
	[Theory]
	[InlineData(true, 1)]
	[InlineData(false, 0)]
	public void SendConfigurationDataTests(bool enabled, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.ConfigurationFlaps, enabled) },
				{  new AvailableDataItem("0", FieldText.ConfigurationParkingBrake, enabled) },
				{  new AvailableDataItem("0", FieldText.ConfigurationElevatorTrim, enabled) },
				{  new AvailableDataItem("0", FieldText.ConfigurationRudderTrim, enabled) },
				{  new AvailableDataItem("0", FieldText.ConfigurationAileronTrim, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();

		var trimData = new TrimData { elevatorTrim = data.elevtorTrim, aileronTrim = data.aileronTrim, rudderTrim = data.rudderTrim };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.PARKING_BRAKE_SET, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimTrimData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), trimData), Times.Exactly(expectedTimesCalled));

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

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_UP, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_1, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_2, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_3, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_4, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FLAPS_DOWN, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Lights Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, 1)]
	[InlineData(false, 0)]
	public void SendLightDataTests(bool enabled, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LightsBeacon, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsCabin, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsLanding, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsLogo, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsNav, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsPanel, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsRecognition, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsStrobe, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsTaxi, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsWing, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsGlareshieldPower, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsPanelPower, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsCabinPower, enabled) },
				{  new AvailableDataItem("0", FieldText.LightsPedestalPower, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.BEACON_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.LANDING_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TAXI_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.STROBE_LIGHT, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.PANEL_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.RECOGNITION_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.WING_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.CABIN_LIGHT, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.LOGO_LIGHT, (uint)0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent_Ex1(It.IsAny<uint>(), EVENT_IDS.GLARESHIELD_LIGHTPOWER, GROUPID.MAX, It.IsAny<SIMCONNECT_EVENT_FLAG>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent_Ex1(It.IsAny<uint>(), EVENT_IDS.PANEL_LIGHTS_POWER_SETTING_SET, GROUPID.MAX, It.IsAny<SIMCONNECT_EVENT_FLAG>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(4 * expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent_Ex1(It.IsAny<uint>(), EVENT_IDS.CABIN_LIGHTPOWER, GROUPID.MAX, It.IsAny<SIMCONNECT_EVENT_FLAG>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2 * expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent_Ex1(It.IsAny<uint>(), EVENT_IDS.PEDESTRAL_LIGHT_POWER, GROUPID.MAX, It.IsAny<SIMCONNECT_EVENT_FLAG>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Fuel Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, false, 1)]
	[InlineData(true, true, 0)]
	[InlineData(false, false, 0)]
	public void SendFuelDataTests(bool enabled, bool blockFuel, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.FuelQtyAll, enabled) },
				{  new AvailableDataItem("0", FieldText.FuelSelector, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();

		var fuelData = new FuelData { fuelLeftMain = data.fuelLeftMain, fuelRightMain = data.fuelRightMain };
		svc.SendDataToSim(data, blockFuel, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.FUEL_SELECTOR_SET, (uint)data.fuelSelector, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneFuelData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), fuelData), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Location Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, false, 1)]
	[InlineData(true, true, 0)]
	[InlineData(false, false, 0)]
	public void SendLocationDataTests(bool enabled, bool blockLocation, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.LocationAltitude, enabled) },
				{  new AvailableDataItem("0", FieldText.LocationHeading, enabled) },
				{  new AvailableDataItem("0", FieldText.LocationLatitude, enabled) },
				{  new AvailableDataItem("0", FieldText.LocationLongitude, enabled) },
				{  new AvailableDataItem("0", FieldText.LocationPitch, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();

		var locationData = new LocationData { altitude = data.altitude, heading = data.heading, latitude = data.latitude, longitude = data.longitude, pitch = data.pitch };
		svc.SendDataToSim(data, false, blockLocation);

		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPlaneLocationData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), locationData), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Obs Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, 1)]
	[InlineData(false, 0)]
	public void SendObsDataTests(bool enabled, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.ObsAdfCard, enabled) },
				{  new AvailableDataItem("0", FieldText.ObsObs1, enabled) },
				{  new AvailableDataItem("0", FieldText.ObsObs2, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_CARD_SET, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.OBS1, (uint)2, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.OBS2, (uint)3, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Other Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, 1)]
	[InlineData(false, 0)]
	public void SendOtherDataTests(bool enabled, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{ new AvailableDataItem("0", FieldText.OtherHeadingBug, enabled) },
				{ new AvailableDataItem("0", FieldText.OtherKolhsman, enabled) },
				{ new AvailableDataItem("0", FieldText.OtherBatteryVoltage, enabled) },
				{ new AvailableDataItem("0", FieldText.OtherTransponder, enabled) },
				{ new AvailableDataItem("0", FieldText.GyroDrift, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();
		var batteryData = new BatteryVoltage { batteryVoltage = data.batteryVoltage };

		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.HEADING_BUG_SET, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.KOHLSMAN_SET, (uint)1083, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPowerData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), batteryData), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.SetDataOnSimObject(DATA_DEFINITIONS.SimPowerData, It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), batteryData), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TRANSPONDER1000INC, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TRANSPONDER100INC, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(2 * expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TRANSPONDER10INC, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(3 * expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.TRANSPONDER1INC, It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(4 * expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.HEADING_GYRO_SET, 0, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.GYRO_DRIFT_SET, (uint)1145, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Power Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, 1)]
	[InlineData(false, 0)]
	public void SendPowerDataTests(bool enabled, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.PowerMasterAlternator, enabled) },
				{  new AvailableDataItem("0", FieldText.PowerMasterAvionics, enabled) },
				{  new AvailableDataItem("0", FieldText.PowerMasterBattery, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.MASTER_ALTERNATOR, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.MASTER_BATTERY, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.AVIONICS_MASTER, (uint)1, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// Radio Data
	/////////////////////////////////////////////////////////////////////////////////////
	[Theory]
	[InlineData(true, 1)]
	[InlineData(false, 0)]
	public void SendRadioDataTests(bool enabled, int expectedTimesCalled)
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings
		{
			SelectedData = new List<AvailableDataItem>()
			{
				{  new AvailableDataItem("0", FieldText.RadiosCom1Both, enabled) },
				{  new AvailableDataItem("0", FieldText.RadiosCom2Both, enabled) },
				{  new AvailableDataItem("0", FieldText.RadiosNav1Both, enabled) },
				{  new AvailableDataItem("0", FieldText.RadiosNav2Both, enabled) },
				{  new AvailableDataItem("0", FieldText.RadiosAdfBoth, enabled) },
			}
		};

		mockSettings.Setup(s => s.ReadSettings()).ReturnsAsync(settingsData);
		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		var data = GetSampleData();
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_RADIO_SET_HZ, (uint)121500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_RADIO_SET_HZ, (uint)120500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM_STBY_RADIO_SET_HZ, (uint)121500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM2_RADIO_SET_HZ, (uint)122500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.COM2_STBY_RADIO_SET_HZ, (uint)123500000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV1_RADIO_SET_HZ, (uint)114000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV1_STBY_SET_HZ, (uint)115000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV2_RADIO_SET_HZ, (uint)116000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.NAV2_STBY_SET_HZ, (uint)117000000, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_STBY_SET, (uint)268435456, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), EVENT_IDS.ADF_STBY_SET, (uint)536870912, It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Exactly(expectedTimesCalled));
	}

	//////////////////////////////////////////////////////////////////////////
	//
	//////////////////////////////////////////////////////////////////////////
	[Fact]
	public void ConnectedTest()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings()
		{
			SelectedData = []
		};

		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		Assert.False(svc.Connected());

		mockProxy.Setup(p => p.IsConnected()).Returns(true);
		Assert.True(svc.Connected());
	}

	[Fact]
	public void DisconnectTest()
	{
		var mockProxy = new Mock<ISimConnectProxy>();
		var mockSettings = new Mock<ISettingsData>();
		var mockPlaneDataRepo = new Mock<IPlaneDataRepo>();
		var mockDbInit = new Mock<IDbInit>();

		var settingsData = new Settings()
		{
			SelectedData = []
		};

		var svc = new SimConnectService(mockProxy.Object, mockSettings.Object, mockPlaneDataRepo.Object, mockDbInit.Object);

		svc.Disconnect();
		mockProxy.Verify(p => p.Disconnect(), Times.Once());
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
			fuelLeftMain = 1,
			fuelRightMain = 2,
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
			transponder = 4660,
			gyroDriftError = 20,

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