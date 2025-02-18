using System.Runtime.InteropServices;

namespace AircraftStateCore.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct FuelData
{
	public double fuelCenter;    //settable
	public double fuelCenter2;    //settable
	public double fuelCenter3;    //settable
	public double fuelExternal1;    //settable
	public double fuelExternal2;    //settable
	public double fuelLeftAux;    //settable
	public double fuelLeftMain;    //settable
	public double fuelLeftTip;    //settable
	public double fuelRightAux;    //settable
	public double fuelRightMain;    //settable
	public double fuelRightTip;    //settable
}