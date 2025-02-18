using AircraftStateCore.Models;
using System.Reflection;

namespace AircraftStateCore.Helpers;

public static class Formatter
{
	public static string GetLatitude(double source)
	{
		string result;

		int iPart = (int)source;

		if (iPart < 0)
		{
			result = "S";
		}
		else
		{
			result = "N";
		}

		var x = (source - Math.Truncate(source)) * 60;

		result += $"{Math.Abs(iPart)}{(char)176} {Math.Abs(x):N2}'";

		return result;
	}

	public static string GetPercent(double source) => Math.Round(source, 0).ToString();
	public static string GetPercent100(double source) => Math.Round(100 * source, 0).ToString();

	public static string GetLongitude(double source)
	{
		string result;

		int iPart = (int)source;

		if (iPart < 0)
		{
			result = "W";
		}
		else
		{
			result = "E";
		}

		var x = (source - Math.Truncate(source)) * 60;

		result += $"{Math.Abs(iPart)}{(char)176} {Math.Abs(x):N2}'";

		return result;
	}

	public static string PitchLabel(PlaneDataStruct planeData) =>
		Math.Round(planeData.pitch, 3) >= 0 ? "Nose Up" : "Nose Down";
	public static string ElevatorTrimLabel(PlaneDataStruct planeData) =>
		Math.Round(planeData.elevtorTrim, 3) >= 0 ? "Nose Up" : "Nose Down";
	public static string RudderTrimLabel(PlaneDataStruct planeData) =>
			Math.Round(planeData.rudderTrim, 3) >= 0 ? "% Rgt" : "% Lft";

	public static string GyroDriftLabel(PlaneDataStruct planeData) =>
		Math.Round(planeData.gyroDriftError, 3) >= 0 ? "° Rgt" : "° Lft";

	public static string AileronTrimLabel(PlaneDataStruct planeData) =>
				Math.Round(planeData.rudderTrim, 3) >= 0 ? "% Rgt" : "% Lft";

	public static string GetOnOff(bool val) => val ? "On" : "Off";
	public static string Decimal0Formatter(double source) => Math.Round(source, 0).ToString();
	public static string Decimal2Formatter(double source) => Math.Round(source, 2).ToString("N2");
	public static string Decimal2AbsFormatter(double source) => Math.Abs(Math.Round(source, 2)).ToString("N2");
	public static string Decimal3AbsFormatter(double source) => Math.Abs(Math.Round(source, 3)).ToString("N3");
	public static string DecimalAdfFormatter(double source) => Math.Abs(1000 * Math.Round(source, 3)).ToString();
	public static string GyroDriftFormatter(double source) => Math.Round(source * 57.2958, 1).ToString();

	public static string FuelSelectorFormatter(int source)
	{
		return source switch
		{
			0 => "Off",
			1 => "All",
			2 => "Left",
			3 => "Right",
			5 => "Right auxiliary",
			6 => "Center",
			7 => "Center2",
			8 => "Center3",
			9 => "External1",
			10 => "External2",
			11 => "Right tip",
			12 => "Left tip",
			13 => "Crossfeed",
			14 => "Crossfeed left to right",
			15 => "Crossfeed right to left",
			16 => "Both",
			17 => "External",
			18 => "Isolate",
			19 => "Left main",
			20 => "Right main",
			_ => source.ToString(),
		};
	}

	/*

	*/
	public static string GetBuildNumber()
	{
		//var date = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
		var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		//var x = Assembly.GetExecutingAssembly().GetCustomAttributes();		
		//return $"{version.Replace(".0.0", String.Empty)}.{date.Replace("1.1.", String.Empty)}";
		return version;
	}

	public static string GetTransponder(uint value)
	{
		int digit1 = (int)((value >> 12) & 0x0F);
		int digit2 = (int)((value >> 8) & 0x0F);
		int digit3 = (int)((value >> 4) & 0x0F);
		int digit4 = (int)(value & 0x0F);

		return $"{digit1}{digit2}{digit3}{digit4}";
	}
}
