﻿using AircraftStateCore.Helpers;

namespace AircraftStateCore.Models;

public class AvailableData
{
	public List<AvailableDataItem> Items { get; set; }

	public AvailableData()
	{
		Items =
		[
            //Radios
            new AvailableDataItem("0000", FieldText.Headers["0000"]),
			new AvailableDataItem("0000.0", FieldText.Headers["0000"], true),
			new AvailableDataItem("0001", FieldText.RadiosCom1Both),
			new AvailableDataItem("0002", FieldText.RadiosCom2Both),
			new AvailableDataItem("0003", FieldText.RadiosNav1Both),
			new AvailableDataItem("0004", FieldText.RadiosNav2Both),
			new AvailableDataItem("0005", FieldText.RadiosAdfBoth),

            //OBS
            new AvailableDataItem("0050", FieldText.Headers["0050"]),
			new AvailableDataItem("0050.0", FieldText.Headers["0050"], true),
			new AvailableDataItem("0051", FieldText.ObsObs1),
			new AvailableDataItem("0052", FieldText.ObsObs2),
			new AvailableDataItem("0053", FieldText.ObsAdfCard),

            //Location
            new AvailableDataItem("0100", FieldText.Headers["0100"]),
			new AvailableDataItem("0100.0", FieldText.Headers["0100"], true),
			new AvailableDataItem("0101", FieldText.LocationLongitude),
			new AvailableDataItem("0102", FieldText.LocationLatitude),
			new AvailableDataItem("0103", FieldText.LocationAltitude),
			new AvailableDataItem("0104", FieldText.LocationHeading),
			new AvailableDataItem("0105", FieldText.LocationPitch),
			new AvailableDataItem("0106", FieldText.GyroDrift),

            //Fuel
            new AvailableDataItem("0150", FieldText.Headers["0150"]),
			new AvailableDataItem("0150.0", FieldText.Headers["0150"], true),
			new AvailableDataItem("0151", FieldText.FuelQtyAll),
			new AvailableDataItem("0152", FieldText.FuelSelector),

            //Plane Configuration
            new AvailableDataItem("0200", FieldText.Headers["0200"]),
			new AvailableDataItem("0200.0", FieldText.Headers["0200"], true),
			new AvailableDataItem("0201", FieldText.ConfigurationFlaps),
			new AvailableDataItem("0202", FieldText.ConfigurationParkingBrake),
			new AvailableDataItem("0203", FieldText.ConfigurationElevatorTrim),
			new AvailableDataItem("0204", FieldText.ConfigurationRudderTrim),
			new AvailableDataItem("0205", FieldText.ConfigurationAileronTrim),

            //Other
            new AvailableDataItem("0250", FieldText.Headers["0250"]),
			new AvailableDataItem("0250.0", FieldText.Headers["0250"], true),
			new AvailableDataItem("0251", FieldText.OtherKolhsman),
			new AvailableDataItem("0252", FieldText.OtherHeadingBug),
			new AvailableDataItem("0253", FieldText.OtherBatteryVoltage),
			new AvailableDataItem("0254", FieldText.OtherTransponder),

            //Power
            new AvailableDataItem("0300", FieldText.Headers["0300"]),
			new AvailableDataItem("0300.0", FieldText.Headers["0300"], true),
			new AvailableDataItem("0301", FieldText.PowerMasterBattery),
			new AvailableDataItem("0302", FieldText.PowerMasterAlternator),
			new AvailableDataItem("0303", FieldText.PowerMasterAvionics),

            // Lights
            new AvailableDataItem("0350", FieldText.Headers["0350"]),
			new AvailableDataItem("0350.0", FieldText.Headers["0350"], true),
			new AvailableDataItem("0351", FieldText.LightsNav),
			new AvailableDataItem("0352", FieldText.LightsBeacon),
			new AvailableDataItem("0353", FieldText.LightsLanding),
			new AvailableDataItem("0354", FieldText.LightsTaxi),
			new AvailableDataItem("0355", FieldText.LightsStrobe),
			new AvailableDataItem("0356", FieldText.LightsPanel),
			new AvailableDataItem("0357", FieldText.LightsCabin),
			new AvailableDataItem("0358", FieldText.LightsLogo),
			new AvailableDataItem("0359", FieldText.LightsWing),
			new AvailableDataItem("0361", FieldText.LightsRecognition),
			new AvailableDataItem("0362", FieldText.LightsGlareshieldPower),
			new AvailableDataItem("0363", FieldText.LightsPanelPower),
			new AvailableDataItem("0364", FieldText.LightsCabinPower),
			new AvailableDataItem("0365", FieldText.LightsPedestalPower),

			// Payload
			new AvailableDataItem("0400", FieldText.Headers["0400"]),
			new AvailableDataItem("0400.0", FieldText.Headers["0400"], true),
			new AvailableDataItem("0401", FieldText.payLoadAll),
		];
	}
}