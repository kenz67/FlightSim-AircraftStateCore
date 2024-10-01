using AircraftStateCore.Services.Interfaces;

namespace AircraftStateCore
{
	public partial class App : Application
	{
		readonly ILocalSettingsService _localSettingsService;
		public App(ILocalSettingsService localSettings)
		{
			_localSettingsService = localSettings;
			InitializeComponent();

			MainPage = new MainPage();
		}

		protected override Window CreateWindow(IActivationState activationState)
		{
			var window = base.CreateWindow(activationState);

			window.Width = _localSettingsService.Settings.WindowWidth;
			window.Height = _localSettingsService.Settings.WindowHeight;
			window.X = _localSettingsService.Settings.X;
			window.Y = _localSettingsService.Settings.Y;

			//window.SizeChanged += OnSizeChanged;
			window.Destroying += OnClose;
			return window;
		}

		//private void OnSizeChanged(object sender, EventArgs e)
		//{
		//	_localSettingsService.Settings.WindowHeight = (sender as Window).Height;
		//	_localSettingsService.Settings.WindowWidth = (sender as Window).Width;
		//}

		private void OnClose(object sender, EventArgs e)
		{
			var win = sender as Window;
			_localSettingsService.Settings.WindowHeight = win.Height;
			_localSettingsService.Settings.WindowWidth = win.Width;
			_localSettingsService.Settings.X = win.X;
			_localSettingsService.Settings.Y = win.Y;

			_localSettingsService.SaveSettings();
		}
	}
}
