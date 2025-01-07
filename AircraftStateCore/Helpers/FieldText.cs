namespace AircraftStateCore.Helpers;

public static class FieldText
{
	public const string RadiosCom1 = "Com 1";
	public const string RadiosCom2 = "Com 2";
	public const string RadiosNav1 = "Nav 1";
	public const string RadiosNav2 = "Nav 2";
	public const string RadiosAdf = "Adf";

	public const string RadiosCom1Both = "Com 1 (Active/Standby)";
	public const string RadiosCom2Both = "Com 2 (Active/Standby)";
	public const string RadiosNav1Both = "Nav 1 (Active/Standby)";
	public const string RadiosNav2Both = "Nav 2 (Active/Standby)";
	public const string RadiosAdfBoth = "ADF (Active/Standby)";

	public const string ObsObs1 = "OBS 1";
	public const string ObsObs2 = "OBS 2";
	public const string ObsAdfCard = "ADF Card";

	public const string LocationLongitude = "Longitude";
	public const string LocationLatitude = "Latitude";
	public const string LocationAltitude = "Altitude";
	public const string LocationHeading = "Heading";
	public const string LocationPitch = "Pitch";
	public const string GyroDrift = "Gyro Drift Error";

	public const string FuelLeft = "Left";
	public const string FuelRight = "Right";

	public const string FuelQtyBoth = "Qty (left and right)";
	public const string FuelSelector = "Fuel Selector";

	public const string ConfigurationFlaps = "Flaps";
	public const string ConfigurationParkingBrake = "Parking Brake";
	public const string ConfigurationElevatorTrim = "Elevator Trim";
	public const string ConfigurationRudderTrim = "Rudder Trim";
	public const string ConfigurationAileronTrim = "Aileron Trim";

	public const string OtherTransponder = "Transponder";
	public const string OtherKolhsman = "Kolhsman";
	public const string OtherHeadingBug = "Heading Bug";
	public const string OtherBatteryVoltage = "Battery Voltage";

	public const string PowerMasterBattery = "Master Battery";
	public const string PowerMasterAlternator = "Master Alternator";
	public const string PowerMasterAvionics = "Master Avionics";

	public const string LightsNav = "Nav";
	public const string LightsBeacon = "Beacon";
	public const string LightsLanding = "Landing";
	public const string LightsTaxi = "Taxi";
	public const string LightsStrobe = "Strobe";
	public const string LightsPanel = "Panel";
	public const string LightsCabin = "Cabin";
	public const string LightsLogo = "Logo";
	public const string LightsWing = "Wing";
	public const string LightsRecognition = "Recognition";
	public const string LightsGlareshieldPower = "Glare Shield %";
	public const string LightsPanelPower = "Panel %";
	public const string LightsCabinPower = "Cabin %";
	public const string LightsPedestalPower = "Pedestal %";

	public const string radiosHeader = "Radios";
	public const string obsHeader = "OBS";
	public const string locationHeader = "Location";
	public const string configHeader = "Plane Configuration";
	public const string fuelHeader = "Fuel";
	public const string otherHeader = "Other";
	public const string lightsHeader = "Lights";
	public const string powerHeader = "Power";

	public static readonly Dictionary<string, string> Headers = new()
	{
		{ "0000", radiosHeader },
		{ "0050", obsHeader },
		{ "0100", locationHeader },
		{ "0150", fuelHeader },
		{ "0200", configHeader },
		{ "0250", otherHeader },
		{ "0300", powerHeader },
		{ "0350", lightsHeader }
	};
}
