using AircraftStateCore.Models;
using AircraftStateCore.Services.Interfaces;
using Newtonsoft.Json;

namespace AircraftStateCore.Services
{
	public class LocalSettingsService : ILocalSettingsService
	{
		public LocalSettings Settings { get; set; }

		public LocalSettingsService()
		{
			try
			{
				using StreamReader reader = new("appSettings.json");
				string json = reader.ReadToEnd();
				Settings = JsonConvert.DeserializeObject<LocalSettings>(json);
			}
			catch
			{
				Settings = new();
			}
		}

		public void SaveSettings()
		{
			try
			{
				using StreamWriter writer = new("appSettings.json");
				string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
				writer.Write(json);
			}
			catch { }
		}
	}
}
