namespace AircraftStateCore;

public partial class App : Application
{
	public App(MainPage mainPage)
	{
		//MainPage = new MainPage();
		MainPage = mainPage;
	}

	protected override Window CreateWindow(IActivationState activationState)
	{
		var window = base.CreateWindow(activationState);

		const int newWidth = 1200;
		const int newHeight = 800;

		window.Width = newWidth;
		window.Height = newHeight;

		return window;
	}
}
