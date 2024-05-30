using AircraftStateCore.Models;

namespace AircraftStateCore.Services.Interfaces;

public interface IPlaneData : IPageUpdate
{
    PlaneDataStruct CurrentData { get; set; }
    List<string> Profiles { get; set; }
    Task LookUpProfile(string profile);
    Task DeleteProfile(string profile);
    Task SaveProfile(string profile);
}