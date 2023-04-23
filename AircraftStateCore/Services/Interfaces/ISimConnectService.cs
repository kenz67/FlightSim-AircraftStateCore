using AircraftStateCore.Models;
using Microsoft.FlightSimulator.SimConnect;

namespace AircraftStateCore.Services.Interfaces;

public interface ISimConnectService : IPageUpdate
{
    SimConnect Sim { get; }

    public PlaneDataStruct SimData { get; }
    public MasterData MasterData { get; }
    public string DisplayMessage { get; set; }

    public bool VerifyAutoSave();

    void CloseConnection();

    bool Connected();

    bool ConnectToSim(bool Reconnect = false);

    void GetSimEnvInfo();

    void SaveDataToDb(string SaveName);

    void SendDataToSim(PlaneDataStruct data, bool BlockFuel, bool BlockLocation);

    public event Func<Task> OnMessageUpdate;
}