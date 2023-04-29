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

		var data = getSampleData();

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

		//Send with all turned off
		var data = getSampleData();

		svc.SimData = new PlaneDataStruct { elevtorTrim = 10 };
		svc.SendDataToSim(data, false, false);

		mockProxy.Verify(p => p.TransmitClientEvent(It.IsAny<uint>(), It.IsAny<Enum>(), It.IsAny<uint>(),
			It.IsAny<Enum>(), It.IsAny<SIMCONNECT_EVENT_FLAG>()), Times.Never);

		var trimData = new TrimData { elevatorTrim = 10, aileronTrim = 0, rudderTrim = 0 };
		mockProxy.Verify(p => p.SetDataOnSimObject(It.IsAny<Enum>(), It.IsAny<uint>(), It.IsAny<SIMCONNECT_DATA_SET_FLAG>(), trimData), Times.Once);
	}

	private PlaneDataStruct getSampleData()
	{
		return new PlaneDataStruct
		{
			//Plane Configuration Data
			flapsIndex = 1,
			parkingBrake = true,
			elevtorTrim = 1,
			rudderTrim = -2,
			aileronTrim = 3

			//Fuel Data

			//Lights Data

			//Location Data

			//OBS Data

			//Other Data

			//Power Data

			//Radio Data
		};
	}
}