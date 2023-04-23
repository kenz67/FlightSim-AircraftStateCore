using System.Runtime.InteropServices;

namespace AircraftStateCore.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct TrimData
{
    public double elevatorTrim;    //settable
    public double rudderTrim;
    public double aileronTrim;
}