using AircraftStateCore.DAL.DatabaseContext;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.enums;
using Microsoft.EntityFrameworkCore;

namespace AircraftStateCore.DAL.Repositories;

public class DbInit : IDbInit
{
    private readonly AircraftStateContext _dbContext;

    public DbInit(AircraftStateContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.Migrate();
    }

    public void Init()
    {
        CopyLegacyData();
    }

    private void CopyLegacyData()
    {
        try
        {
            if (_dbContext.ApplicationSettings.Any())
            {
                var newData = new List<ProfileDatum>();
                var newSettings = new List<ApplicationSettingsDatum>();

                foreach (var data in _dbContext.PlaneData)
                {
                    newData.Add(new ProfileDatum { ProfileName = data.Plane, Data = data.Data });
                }

                if (newData.Count > 0)
                {
                    _dbContext.ProfileData.AddRange(newData);
                }

                foreach (var s in _dbContext.Settings)
                {
                    switch (s.DataKey)
                    {
                        case SettingDefinitions.AutoSave:
                        case SettingDefinitions.DataToSend:
                        case SettingDefinitions.BlockLocation:
                        case SettingDefinitions.BlockFuel:
                            newSettings.Add(new ApplicationSettingsDatum { DataKey = s.DataKey, DataValue = s.DataValue }); break;
                        case "ApplyFuel":
                            FlipValue(newSettings, s, SettingDefinitions.BlockFuel); break;
                        case "ApplyLocation":
                            FlipValue(newSettings, s, SettingDefinitions.BlockLocation); break;
                    }
                }

                _dbContext.ApplicationSettings.AddRange(newSettings);
                _dbContext.SaveChanges();
            }

            _dbContext.Database.ExecuteSql($"DELETE FROM planeData");
            _dbContext.Database.ExecuteSql($"DELETE FROM settings");
        }
        catch { }
    }

    private void FlipValue(List<ApplicationSettingsDatum> newSettings, SettingsDatum setting, string newKey)
    {
        if (_dbContext.Settings.Where(s => s.DataKey == newKey).FirstOrDefault() == null)
        {
            if (setting.DataValue.ToLower().Equals("false", StringComparison.Ordinal))
            {
                newSettings.Add(new ApplicationSettingsDatum { DataKey = newKey, DataValue = "True" });
            }
            else
            {
                newSettings.Add(new ApplicationSettingsDatum { DataKey = newKey, DataValue = "False" });
            }
        }
    }
}