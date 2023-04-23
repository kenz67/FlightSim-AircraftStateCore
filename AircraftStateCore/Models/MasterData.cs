using System.Runtime.InteropServices;

namespace AircraftStateCore.Models;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct MasterData
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string title;
}