using AircraftStateCore.DAL.Repositories.Interfaces;

namespace AircraftStateCore;

public partial class MainPage : ContentPage
{
	public MainPage(IDbInit dbInit)
	{
		InitializeComponent();

		dbInit.Init();
		//_ = new SimConnectService();
	}
}
