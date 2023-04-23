using AircraftStateCore.Models;

namespace AircraftStateCore.DAL.Repositories.Interfaces;

public interface IPlaneDataRepo
{
    Task<PlaneDataStruct> GetDataForProfile(string profile);
    Task SaveDataForProfile(string profile, PlaneDataStruct data);
    Task<List<string>> GetSavedProfiles();
    Task DeleteSavedProfile(string Profile);
}