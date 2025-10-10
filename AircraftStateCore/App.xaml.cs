namespace AircraftStateCore
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

		protected override Window CreateWindow(IActivationState activationState)
		{
			double width = Preferences.Get("LastWindowWidth", 1200);
			double height = Preferences.Get("LastWindowHeight", 800);
			double x = Preferences.Get("LastWindowX", 100);
			double y = Preferences.Get("LastWindowY", 100);

			var window = new Window(new MainPage())
			{
				Title = "AircraftStateCore",
				Width = width,
				Height = height,
				X = x,
				Y = y
			};

			// Subscribe to Destroying event (fires when app is closing)
			window.Destroying += (s, e) =>
			{
				var w = (Window)s;
				double width = w.Width;
				double height = w.Height;
				double x = w.X;
				double y = w.Y;

				// Save to Preferences or local storage
				Preferences.Set("LastWindowWidth", width);
				Preferences.Set("LastWindowHeight", height);
				Preferences.Set("LastWindowX", x);
				Preferences.Set("LastWindowY", y);
			};

			return window;
		}
	}
}
