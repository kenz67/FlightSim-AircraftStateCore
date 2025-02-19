using System.Runtime.InteropServices;

namespace AircraftStateCore.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct PayloadData
{
	public double payload0; //settable
	public double payload1; //settable
	public double payload2; //settable
	public double payload3; //settable
	public double payload4; //settable
	public double payload5; //settable
	public double payload6; //settable
	public double payload7; //settable
	public double payload8; //settable
	public double payload9; //settable
	public double payload10; //settable
	public double payload11; //settable
	public double payload12; //settable
	public double payload13; //settable
	public double payload14; //settable
}