using AircraftStateCore.DAL.DatabaseContext;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AircraftStateCore.DAL.Repositories;

//TODO UT
public class PlaneDataRepo : IPlaneDataRepo
{
	private readonly AircraftStateContext _dbContext;

	public PlaneDataRepo(AircraftStateContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task DeleteSavedProfile(string profile)
	{
		var data = await _dbContext.ProfileData.Where(p => p.ProfileName.Equals(profile)).FirstOrDefaultAsync();
		_dbContext.ProfileData.Remove(data);
		await _dbContext.SaveChangesAsync();
		return;
	}

	public async Task<PlaneDataStruct> GetDataForProfile(string profile)
	{
		PlaneDataStruct data;
		var dbData = await _dbContext.ProfileData.Where(p => p.ProfileName.Equals(profile)).FirstOrDefaultAsync();
		if (dbData != null)
		{
			data = JsonConvert.DeserializeObject<PlaneDataStruct>(dbData.Data);
			data.validData = true;
		}
		else
		{
			data = new PlaneDataStruct() { validData = false };
		}

		return data;
	}

	public async Task<List<string>> GetSavedProfiles()
	{
		return await _dbContext
			.ProfileData.Select(p => p.ProfileName)
			.OrderBy(p => p)
			.ToListAsync();
	}

	public async Task SaveDataForProfile(string profile, PlaneDataStruct data)
	{
		var dbData = await _dbContext.ProfileData.Where(p => p.ProfileName.Equals(profile)).FirstOrDefaultAsync();
		if (dbData != null)
		{
			dbData.Data = JsonConvert.SerializeObject(data);
			_dbContext.ProfileData.Update(dbData);
		}
		else
		{
			dbData = new ProfileDatum
			{
				Data = JsonConvert.SerializeObject(data),
				ProfileName = profile
			};
			_dbContext.ProfileData.Add(dbData);
		}

		await _dbContext.SaveChangesAsync();
		return;
	}
}