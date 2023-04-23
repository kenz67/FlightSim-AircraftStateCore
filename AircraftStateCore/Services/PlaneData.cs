using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Models;
using AircraftStateCore.Services.Interfaces;

namespace AircraftStateCore.Services;

public class PlaneData : IPlaneData
{
    public event Func<Task> OnChangeAsync;

    public PlaneDataStruct CurrentData { get; set; }
    public List<string> Profiles { get; set; }

    private readonly IPlaneDataRepo _planeData;

    public PlaneData(IPlaneDataRepo planeData)
    {
        _planeData = planeData;
        CurrentData = new PlaneDataStruct();

        string appDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\AircraftState";
        if (!Directory.Exists(appDir))
        {
            Directory.CreateDirectory(appDir);
        }

        Profiles = Task.Run(() => _planeData.GetSavedProfiles()).Result;
    }

    public async Task LookUpProfile(string profile)
    {
        CurrentData = await _planeData.GetDataForProfile(profile);
        await OnChangeAsync();
    }

    public async Task DeleteProfile(string profile)
    {
        await _planeData.DeleteSavedProfile(profile);
    }

    public async Task SaveProfile(string profile)
    {
        await _planeData.SaveDataForProfile(profile, CurrentData);
        Profiles.Add(profile);
        Profiles.Sort();
    }
}
