using AircraftStateCore.DAL.Repositories;
using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Database;
using AircraftStateCore.Services;
using AircraftStateCore.Services.Interfaces;
using MudBlazor.Services;

namespace AircraftStateCore
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif
			builder.Services
				.AddMudServices()
				.AddBootstrapBlazor()
				//.AddSingleton<MainPage>()
				.AddSqlite<AircraftStateContext>($"Data Source={DbCommon.DbName}")
				.AddSingleton<IDbInit, DbInit>()
				.AddSingleton<IPlaneDataRepo, PlaneDataRepo>()
				.AddSingleton<ISettingsRepo, SettingsRepo>()
				.AddSingleton<IPlaneData, PlaneData>()
				.AddSingleton<ISettingsData, SettingsData>()
				.AddSingleton<ISimConnectProxy, SimConnectProxy>()
				.AddSingleton<ISimConnectService, SimConnectService>()
				;

			return builder.Build();
		}
	}
}
