using System.Runtime.InteropServices;

namespace AircraftStateCore.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct LocationData
{
	public double latitude;   //setttable
	public double longitude;  //settable
	public int altitude;    //settable
	public double heading;    //settable
	public double pitch;      //settable
}